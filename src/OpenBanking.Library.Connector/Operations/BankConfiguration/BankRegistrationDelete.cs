// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class BankRegistrationDelete : BaseDelete<BankRegistration, BankRegistrationDeleteParams>
    {
        private readonly IBankProfileService _bankProfileService;
        private readonly IGrantPost _grantPost;

        public BankRegistrationDelete(
            IDbReadWriteEntityMethods<BankRegistration> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IBankProfileService bankProfileService,
            IGrantPost grantPost) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _bankProfileService = bankProfileService;
            _grantPost = grantPost;
        }

        protected override async
            Task<(BankRegistration persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiDelete(BankRegistrationDeleteParams deleteParams)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            BankProfile? bankProfile = deleteParams.BankProfileEnum is not null
                ? _bankProfileService.GetBankProfile(deleteParams.BankProfileEnum.Value)
                : null;

            bool includeExternalApiOperationValue =
                deleteParams.IncludeExternalApiOperation ??
                bankProfile?.BankConfigurationApiSettings.UseRegistrationDeleteEndpoint ??
                throw new ArgumentNullException(
                    null,
                    "includeExternalApiOperation specified as null and cannot be obtained using specified BankProfile (also null).");

            // Load object
            BankRegistration entity =
                await _entityMethods
                    .DbSet
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == deleteParams.Id) ??
                throw new KeyNotFoundException($"No record found for Bank Registration with ID {deleteParams.Id}.");
            CustomBehaviourClass? customBehaviour = entity.BankNavigation.CustomBehaviour;

            if (includeExternalApiOperationValue)
            {
                string registrationEndpoint =
                    entity.BankNavigation.RegistrationEndpoint ??
                    throw new InvalidOperationException(
                        $"Bank with ID {entity.BankId} does not have a registration endpoint configured.");

                bool useRegistrationAccessTokenValue =
                    deleteParams.UseRegistrationAccessToken ??
                    bankProfile?.BankConfigurationApiSettings.UseRegistrationAccessToken ??
                    throw new ArgumentNullException(
                        null,
                        "useRegistrationAccessToken specified as null and cannot be obtained using specified BankProfile (also null).");

                // Get API client
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                    await _softwareStatementProfileRepo.GetAsync(
                        entity.SoftwareStatementProfileId,
                        entity.SoftwareStatementProfileOverride);
                IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

                // Get URI
                string bankApiId = entity.ExternalApiObject.ExternalApiId;
                var apiRequestUrl = new Uri(registrationEndpoint.TrimEnd('/') + $"/{bankApiId}");

                // Get appropriate token
                string accessToken;
                if (useRegistrationAccessTokenValue)
                {
                    accessToken = entity.ExternalApiObject.RegistrationAccessToken ??
                                  throw new InvalidOperationException("No registration access token available");
                }
                else
                {
                    string? scope = customBehaviour?.BankRegistrationPut?.CustomTokenScope;
                    accessToken = (await _grantPost.PostClientCredentialsGrantAsync(
                            scope,
                            processedSoftwareStatementProfile,
                            entity,
                            entity.BankNavigation.TokenEndpoint,
                            null,
                            apiClient,
                            _instrumentationClient))
                        .AccessToken;
                }

                // Delete at API
                IDeleteRequestProcessor deleteRequestProcessor =
                    new BankRegistrationDeleteRequestProcessor(accessToken);
                await deleteRequestProcessor.DeleteAsync(apiRequestUrl, apiClient);
            }

            return (entity, nonErrorMessages);
        }
    }
}
