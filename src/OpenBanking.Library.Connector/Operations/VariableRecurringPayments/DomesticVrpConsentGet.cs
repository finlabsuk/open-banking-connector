// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    internal class
        DomesticVrpConsentGet : ConsentRead<DomesticVrpConsentPersisted, DomesticVrpConsentResponse,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse, ConsentReadParams>
    {
        public DomesticVrpConsentGet(
            IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IGrantPost grantPost) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper,
            grantPost) { }

        protected string RelativePathBeforeId => "/domestic-vrp-consents";

        protected override string ClientCredentialsGrantScope => "payments";


        protected override IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>
            ApiRequests(
                BankApiSet2 bankApiSet,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            bankApiSet.VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiRequests<
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient,
                        false,
                        processedSoftwareStatementProfile)),
                null => throw new NullReferenceException("No VRP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"VRP API version {bankApiSet.VariableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion} not supported.")
            };

        protected override async Task<(Uri endpointUrl, DomesticVrpConsentPersisted persistedObject, BankApiSet2
            bankApiInformation, BankRegistration bankRegistration, string bankFinancialId, string? accessToken,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetRequestData(Guid id, string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            DomesticVrpConsentPersisted persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.DomesticVrpConsentAuthContextsNavigation)
                    .Include(o => o.VariableRecurringPaymentsApiNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {id}.");
            VariableRecurringPaymentsApiEntity variableRecurringPaymentsApi =
                persistedObject.VariableRecurringPaymentsApiNavigation;
            var bankApiSet2 = new BankApiSet2
            {
                VariableRecurringPaymentsApi = new VariableRecurringPaymentsApi
                {
                    VariableRecurringPaymentsApiVersion = variableRecurringPaymentsApi.ApiVersion,
                    BaseUrl = variableRecurringPaymentsApi.BaseUrl
                }
            };
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

            string bankApiId = persistedObject.ExternalApiId;

            // Determine endpoint URL
            string baseUrl = variableRecurringPaymentsApi.BaseUrl;
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}");

            return (endpointUrl, persistedObject, bankApiSet2, bankRegistration, bankFinancialId, null,
                nonErrorMessages);
        }

        protected override DomesticVrpConsentResponse GetPublicResponse(
            DomesticVrpConsentPersisted persistedObject,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse apiResponse,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery)
        {
            // TODO: Update links

            return new DomesticVrpConsentResponse(
                persistedObject.Id,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.Reference,
                persistedObject.BankRegistrationId,
                persistedObject.VariableRecurringPaymentsApiId,
                persistedObject.ExternalApiId,
                apiResponse);
        }
    }
}
