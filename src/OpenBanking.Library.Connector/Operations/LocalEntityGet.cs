// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    /// <summary>
    ///     Read operations on local entities (objects stored in local database only).
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    internal abstract class GetBase<TEntity, TPublicResponse> :
        IObjectRead<TPublicResponse>
        where TEntity : class, IEntity,
        new()
    {
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        protected readonly IDbReadWriteEntityMethods<TEntity> _entityMethods;
        protected readonly IInstrumentationClient _instrumentationClient;
        protected readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        protected readonly ITimeProvider _timeProvider;


        public GetBase(
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
            ReadAsync(
                Guid id,
                string? modifiedBy = null,
                string? apiResponseWriteFile = null,
                string? apiResponseOverrideFile = null)
        {
            var requestInfo = new GetRequestInfo(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // GET from bank API
            (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await ApiGet(requestInfo);
            nonErrorMessages.AddRange(newNonErrorMessages);

            return (response, nonErrorMessages);
        }


        /// <summary>
        ///     Empty function as by definition POST local does not include POST to bank API.
        /// </summary>
        /// <returns></returns>
        protected abstract
            Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGet(GetRequestInfo requestInfo);

        public class GetRequestInfo
        {
            public GetRequestInfo(
                Guid id,
                string? modifiedBy,
                string? apiResponseWriteFile,
                string? apiResponseOverrideFile)
            {
                Id = id;
                ModifiedBy = modifiedBy;
                ApiResponseWriteFile = apiResponseWriteFile;
                ApiResponseOverrideFile = apiResponseOverrideFile;
            }

            public Guid Id { get; }
            public string? ModifiedBy { get; }
            public string? ApiResponseWriteFile { get; }
            public string? ApiResponseOverrideFile { get; }
        }
    }

    internal class
        LocalEntityGet<TEntity, TPublicQuery, TPublicResponse> : GetBase<TEntity, TPublicResponse>,
            IObjectReadLocal<TPublicQuery, TPublicResponse>
        where TEntity : class, ISupportsFluentLocalEntityGet<TPublicResponse>, IEntity, new()
    {
        public LocalEntityGet(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient) { }

        public async
            Task<(IQueryable<TPublicResponse> response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
                )>
            ReadAsync(Expression<Func<TPublicQuery, bool>> predicate)
        {
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Convert expression tree type
            ParameterExpression entityInput = Expression.Parameter(typeof(TEntity), "entity");
            InvocationExpression main = Expression.Invoke(predicate, entityInput);
            Expression<Func<TEntity, bool>> predicateWithUpdatedType =
                Expression.Lambda<Func<TEntity, bool>>(main, entityInput);

            // Run query
            IQueryable<TEntity> resultEntity = await _entityMethods.GetNoTrackingAsync(predicateWithUpdatedType);

            // Process results
            IQueryable<TPublicResponse> resultResponse = resultEntity.Select(b => b.PublicGetLocalResponse);

            // Return success response (thrown exceptions produce error response)
            return (resultResponse, nonErrorMessages);
        }

        protected override async
            Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGet(
                GetRequestInfo requestInfo)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Create persisted entity
            TEntity persistedObject =
                await _entityMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == requestInfo.Id) ??
                throw new KeyNotFoundException($"No record found for entity with ID {requestInfo.Id}.");

            // Create response
            TPublicResponse response = persistedObject.PublicGetLocalResponse;

            return (response, nonErrorMessages);
        }
    }
}
