// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    /// <summary>
    ///     Create operations on local entities (objects stored in local database only).
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    internal abstract class PostBase<TPublicRequest, TPublicResponse> :
        IObjectCreate<TPublicRequest, TPublicResponse>
        where TPublicRequest : Base
    {
        protected readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        protected readonly IInstrumentationClient _instrumentationClient;
        protected readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        protected readonly ITimeProvider _timeProvider;


        public PostBase(
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient)
        {
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _instrumentationClient = instrumentationClient;
        }

        public async Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            CreateAsync(
                TPublicRequest request,
                string? createdBy = null,
                string? apiRequestWriteFile = null,
                string? apiResponseWriteFile = null,
                string? apiResponseOverrideFile = null)
        {
            var requestInfo = new PostRequestInfo(
                request,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // POST to bank API and create entity
            (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await ApiPost(requestInfo);
            nonErrorMessages.AddRange(newNonErrorMessages);

            return (response, nonErrorMessages);
        }

        /// <summary>
        ///     Empty function as by definition POST local does not include POST to bank API.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            ApiPost(PostRequestInfo requestInfo);


        public class PostRequestInfo
        {
            public PostRequestInfo(
                TPublicRequest request,
                string? apiRequestWriteFile,
                string? apiResponseWriteFile,
                string? apiResponseOverrideFile)
            {
                Request = request;
                ApiRequestWriteFile = apiRequestWriteFile;
                ApiResponseWriteFile = apiResponseWriteFile;
                ApiResponseOverrideFile = apiResponseOverrideFile;
            }

            public TPublicRequest Request { get; }
            public string? ApiRequestWriteFile { get; }
            public string? ApiResponseWriteFile { get; }
            public string? ApiResponseOverrideFile { get; }
        }
    }

    internal abstract class
        LocalEntityPost<TEntity, TPublicRequest, TPublicResponse> : PostBase<TPublicRequest,
            TPublicResponse>
        where TEntity : class, IEntity
        where TPublicRequest : Base
    {
        protected readonly IDbReadWriteEntityMethods<TEntity> _entityMethods;

        public LocalEntityPost(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient) : base(
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _entityMethods = entityMethods;
        }

        protected override async
            Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiPost(
                PostRequestInfo requestInfo)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Add entity
            TPublicResponse response = await AddEntity(
                requestInfo.Request,
                _timeProvider);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();

            return (response, nonErrorMessages);
        }

        protected abstract Task<TPublicResponse> AddEntity(
            TPublicRequest request,
            ITimeProvider timeProvider);
    }
}
