// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class BankRegistrationDelete : EntityDelete<BankRegistration>
    {
        public BankRegistrationDelete(
            IDbReadWriteEntityMethods<BankRegistration> entityMethods,
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
            Task<(BankRegistration persistedObject, IApiClient apiClient, Uri uri, IDeleteRequestProcessor
                deleteRequestProcessor, List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiDeleteData(
                Guid id,
                bool useRegistrationAccessToken)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            BankRegistration persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Bank Registration with ID {id}.");

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    persistedObject.SoftwareStatementProfileId,
                    persistedObject.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            string bankApiId = persistedObject.ExternalApiId;

            var uri = new Uri(persistedObject.RegistrationEndpoint + $"/{bankApiId}");

            // Get appropriate token
            string accessToken = useRegistrationAccessToken
                ? persistedObject.RegistrationAccessToken ??
                  throw new InvalidOperationException("No registration access token available")
                : (await PostTokenRequest.PostClientCredentialsGrantAsync(
                    null,
                    processedSoftwareStatementProfile,
                    persistedObject,
                    null,
                    apiClient,
                    _instrumentationClient))
                .AccessToken;

            IDeleteRequestProcessor deleteRequestProcessor =
                new ApiDeleteRequestProcessor(accessToken, null);

            return (persistedObject, apiClient, uri, deleteRequestProcessor, nonErrorMessages);
        }
    }
}
