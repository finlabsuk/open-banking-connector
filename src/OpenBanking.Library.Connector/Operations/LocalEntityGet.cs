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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class LocalEntityGet<TEntity, TPublicQuery, TPublicResponse> :
        IObjectGet<TPublicQuery, TPublicResponse>
        where TEntity : class, ISupportsFluentLocalEntityGet<TPublicResponse>, IEntity,
        new()
    {
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        protected readonly IDbReadWriteEntityMethods<TEntity> _entityMethods;
        protected readonly IInstrumentationClient _instrumentationClient;
        protected readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        protected readonly ITimeProvider _timeProvider;


        public LocalEntityGet(
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
            GetAsync(
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
            (TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await ApiGet(requestInfo);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response (may involve additional processing based on entity)
            TPublicResponse response1 = await CreateResponse(persistedObject);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();
            TPublicResponse response = response1;

            return (response, nonErrorMessages);
        }

        public async
            Task<(IQueryable<TPublicResponse> response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
                )>
            GetAsync(Expression<Func<TPublicQuery, bool>> predicate)
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
            IQueryable<TPublicResponse> resultResponse = resultEntity.Select(b => b.PublicGetResponse);

            // Return success response (thrown exceptions produce error response)
            return (resultResponse, nonErrorMessages);
        }


        protected virtual Task<TPublicResponse> CreateResponse(TEntity persistedObject)
        {
            TPublicResponse response = persistedObject.PublicGetResponse;

            return Task.FromResult(response);
        }

        /// <summary>
        ///     Empty function as by definition POST local does not include POST to bank API.
        /// </summary>
        /// <returns></returns>
        protected virtual async
            Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGet(GetRequestInfo requestInfo)
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

            return (persistedObject, nonErrorMessages);
        }

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
}
