// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class
        DomesticPaymentConsentGet : ReadWriteGet<DomesticPaymentConsentPersisted, DomesticPaymentConsentResponse,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>
    {
        public DomesticPaymentConsentGet(
            IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper) { }

        protected override string RelativePathBeforeId => "/domestic-payment-consents";

        protected override string ClientCredentialsGrantScope => "payments";


        protected override IApiGetRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> ApiRequests(
            BankApiSet2 bankApiSet,
            string bankFinancialId,
            string accessToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient) =>
            bankApiSet.PaymentInitiationApi?.PaymentInitiationApiVersion switch
            {
                PaymentInitiationApiVersion.Version3p1p4 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4,
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticConsentResponse4>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient,
                        bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion <
                        PaymentInitiationApiVersion.Version3p1p4,
                        processedSoftwareStatementProfile)),
                PaymentInitiationApiVersion.Version3p1p6 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsPublic.OBWriteDomesticConsent4>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient,
                        bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion <
                        PaymentInitiationApiVersion.Version3p1p4,
                        processedSoftwareStatementProfile)),
                null => throw new NullReferenceException("No PISP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"PISP API version {bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion} not supported.")
            };

        protected override async Task<(
            string bankApiId,
            Uri endpointUrl,
            DomesticPaymentConsentPersisted persistedObject,
            BankApiSet2 bankApiInformation,
            BankRegistration bankRegistration,
            string bankFinancialId,
            string? accessToken,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetRequestData(Guid id, string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            DomesticPaymentConsentPersisted persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.DomesticPaymentConsentAuthContextsNavigation)
                    .Include(o => o.PaymentInitiationApiNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {id}.");
            PaymentInitiationApiEntity paymentInitiationApi =
                persistedObject.PaymentInitiationApiNavigation;
            var bankApiSet2 = new BankApiSet2
            {
                PaymentInitiationApi = new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = paymentInitiationApi.ApiVersion,
                    BaseUrl = paymentInitiationApi.BaseUrl
                }
            };
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

            string bankApiId = persistedObject.ExternalApiId;

            // Determine endpoint URL
            string baseUrl = paymentInitiationApi.BaseUrl;
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            return (bankApiId, endpointUrl, persistedObject, bankApiSet2, bankRegistration, bankFinancialId, null,
                nonErrorMessages);
        }

        protected override DomesticPaymentConsentResponse GetReadResponse(
            DomesticPaymentConsentPersisted persistedObject,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 apiResponse)
        {
            return new DomesticPaymentConsentResponse(
                persistedObject.Id,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.Reference,
                persistedObject.BankRegistrationId,
                persistedObject.PaymentInitiationApiId,
                persistedObject.ExternalApiId,
                apiResponse);
        }
    }
}
