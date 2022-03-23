// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    /// <summary>
    ///     Create operations on local entities (objects stored in local database only).
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    internal class LocalEntityPost<TEntity, TPublicRequest, TPublicResponse> :
        IObjectPost<TPublicRequest, TPublicResponse>
        where TEntity : class, IEntity, ISupportsFluentLocalEntityPost<TPublicRequest, TPublicResponse, TEntity>,
        new()
        where TPublicRequest : Base
    {
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        protected readonly IDbReadWriteEntityMethods<TEntity> _entityMethods;
        protected readonly IInstrumentationClient _instrumentationClient;
        protected readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        protected readonly ITimeProvider _timeProvider;


        public LocalEntityPost(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient)
        {
            _entityMethods = entityMethods;
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
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // POST to bank API and create entity
            (TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await ApiPost(requestInfo);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Save entity
            await _entityMethods.AddAsync(persistedObject);

            // Create response (may involve additional processing based on entity)
            TPublicResponse response1 = await CreateResponse(persistedObject);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();
            TPublicResponse response = response1;

            return (response, nonErrorMessages);
        }

        protected virtual Task<TPublicResponse> CreateResponse(TEntity persistedObject)
        {
            TPublicResponse response = persistedObject.PublicPostResponse;

            return Task.FromResult(response);
        }

        /// <summary>
        ///     Empty function as by definition POST local does not include POST to bank API.
        /// </summary>
        /// <returns></returns>
        protected virtual Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPost(PostRequestInfo requestInfo)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Create persisted entity
            var persistedObject = new TEntity();
            persistedObject.Initialise(
                requestInfo.Request,
                requestInfo.ModifiedBy,
                _timeProvider);

            persistedObject.Create(
                requestInfo.Request,
                requestInfo.ModifiedBy,
                _timeProvider);

            return Task
                .FromResult<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>(
                    (persistedObject, nonErrorMessages));
        }

        public class PostRequestInfo
        {
            public PostRequestInfo(
                TPublicRequest request,
                string? modifiedBy,
                string? apiRequestWriteFile,
                string? apiResponseWriteFile,
                string? apiResponseOverrideFile)
            {
                Request = request;
                ModifiedBy = modifiedBy;
                ApiRequestWriteFile = apiRequestWriteFile;
                ApiResponseWriteFile = apiResponseWriteFile;
                ApiResponseOverrideFile = apiResponseOverrideFile;
            }

            public TPublicRequest Request { get; }
            public string? ModifiedBy { get; }
            public string? ApiRequestWriteFile { get; }
            public string? ApiResponseWriteFile { get; }
            public string? ApiResponseOverrideFile { get; }
        }
    }
}
