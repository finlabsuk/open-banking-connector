// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    internal class DomesticVrpConsentDelete : BaseDelete<DomesticVrpConsent, ConsentDeleteParams>
    {
        protected readonly IGrantPost _grantPost;

        public DomesticVrpConsentDelete(
            IDbReadWriteEntityMethods<DomesticVrpConsent> entityMethods,
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
            _grantPost = grantPost;
        }

        protected string RelativePathBeforeId => "/domestic-vrp-consents";

        protected string RelativePathAfterId => "";

        protected override async
            Task<(DomesticVrpConsent persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiDelete(ConsentDeleteParams deleteParams)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            DomesticVrpConsent persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.DomesticVrpConsentAuthContextsNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == deleteParams.Id) ??
                throw new KeyNotFoundException($"No record found for Domestic Vrp Consent with ID {deleteParams.Id}.");

            if (deleteParams.IncludeExternalApiOperation)
            {
                VariableRecurringPaymentsApiEntity variableRecurringPaymentsApi =
                    persistedObject.VariableRecurringPaymentsApiNavigation;
                BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
                string bankApiId = persistedObject.ExternalApiId;
                string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

                // Get software statement profile
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                    await _softwareStatementProfileRepo.GetAsync(
                        bankRegistration.SoftwareStatementProfileId,
                        bankRegistration.SoftwareStatementProfileOverride);
                IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

                // Determine endpoint URL
                string baseUrl = variableRecurringPaymentsApi.BaseUrl;
                var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

                // Get client credentials grant token
                ClientCredentialsGrantResponse tokenEndpointResponse =
                    await _grantPost.PostClientCredentialsGrantAsync(
                        "payments",
                        processedSoftwareStatementProfile,
                        bankRegistration,
                        persistedObject.BankRegistrationNavigation.BankNavigation.TokenEndpoint,
                        null,
                        apiClient,
                        _instrumentationClient);
                IDeleteRequestProcessor deleteRequestProcessor =
                    new ApiDeleteRequestProcessor(tokenEndpointResponse.AccessToken, bankFinancialId);

                // Delete at API
                await deleteRequestProcessor.DeleteAsync(endpointUrl, apiClient);
            }

            return (persistedObject, nonErrorMessages);
        }
    }
}
