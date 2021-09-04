// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal abstract class
        EntityDelete<TEntity> :
            LocalEntityDelete<TEntity>
        where TEntity : class, ISupportsFluentDeleteLocal<TEntity>, new()
    {
        protected EntityDelete(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IReadOnlyRepository<ProcessedSoftwareStatementProfile> softwareStatementProfileRepo) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo) { }

        protected override async
            Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiDelete(
                DeleteRequestInfo requestInfo)
        {
            var (persistedObject, apiClient, uri, deleteRequestProcessor, nonErrorMessages) =
                await ApiDeleteData(requestInfo.Id, requestInfo.UseRegistrationAccessToken);

            // Delete at API
            await deleteRequestProcessor.DeleteAsync(uri, apiClient);

            return (persistedObject, nonErrorMessages);
        }

        protected abstract Task<(
            TEntity persistedObject,
            IApiClient apiClient,
            Uri uri,
            IDeleteRequestProcessor deleteRequestProcessor,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiDeleteData(
            Guid id,
            bool useRegistrationAccessToken);
    }
}
