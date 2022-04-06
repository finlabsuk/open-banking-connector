// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    /// <summary>
    ///     Delete operations on entities (objects stored in external (i.e. bank) database and local database).
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal abstract class
        EntityDelete<TEntity> :
            LocalEntityDelete<TEntity>
        where TEntity : class, IEntity, new()
    {
        protected EntityDelete(
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

        protected override async
            Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiDelete(
                DeleteRequestInfo requestInfo)
        {
            (TEntity persistedObject, IApiClient apiClient, Uri uri, IDeleteRequestProcessor deleteRequestProcessor,
                    List<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
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
