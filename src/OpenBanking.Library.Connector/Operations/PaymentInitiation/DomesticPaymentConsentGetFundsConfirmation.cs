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
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class
        DomesticPaymentConsentGetFundsConfirmation : ConsentRead<DomesticPaymentConsentPersisted,
            DomesticPaymentConsentReadFundsConfirmationResponse,
            PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1, ConsentBaseReadParams>
    {
        private readonly AuthContextAccessTokenGet _authContextAccessTokenGet;

        public DomesticPaymentConsentGetFundsConfirmation(
            IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IGrantPost grantPost,
            AuthContextAccessTokenGet authContextAccessTokenGet) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper,
            grantPost)
        {
            _authContextAccessTokenGet = authContextAccessTokenGet;
        }

        protected string RelativePathBeforeId => "/domestic-payment-consents";
        protected string RelativePathAfterId => "/funds-confirmation";

        protected override string ClientCredentialsGrantScope => "payments";


        protected override IApiGetRequests<PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1> ApiRequests(
            BankApiSet2 bankApiSet,
            string bankFinancialId,
            string accessToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient)
            => bankApiSet.PaymentInitiationApi?.PaymentInitiationApiVersion switch
            {
                PaymentInitiationApiVersion.Version3p1p4 => new ApiGetRequests<
                    PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1,
                    PaymentInitiationModelsV3p1p4.OBWriteFundsConfirmationResponse1>(
                    new ApiGetRequestProcessor(
                        bankFinancialId,
                        accessToken)),
                PaymentInitiationApiVersion.Version3p1p6 => new ApiGetRequests<
                    PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1,
                    PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1>(
                    new ApiGetRequestProcessor(
                        bankFinancialId,
                        accessToken)),
                null => throw new NullReferenceException("No PISP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"PISP API version {bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion} not supported.")
            };

        protected override async Task<(Uri endpointUrl, DomesticPaymentConsentPersisted persistedObject, BankApiSet2
            bankApiInformation, BankRegistration bankRegistration, string bankFinancialId, string? accessToken,
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
            string bankApiId = persistedObject.ExternalApiId;
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

            // Get access token
            string? requestObjectAudClaim =
                persistedObject.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                    ?.DomesticPaymentConsentAuthGet
                    ?.AudClaim;
            string bankIssuerUrl =
                requestObjectAudClaim ??
                bankRegistration.BankNavigation.IssuerUrl ??
                throw new Exception("Cannot determine issuer URL for bank");
            string accessToken =
                await _authContextAccessTokenGet.GetAccessTokenAndUpdateConsent(
                    persistedObject,
                    bankIssuerUrl,
                    "openid payments",
                    bankRegistration,
                    modifiedBy);

            // Determine endpoint URL
            string baseUrl = paymentInitiationApi.BaseUrl;
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            return (endpointUrl, persistedObject, bankApiSet2, bankRegistration, bankFinancialId,
                accessToken, nonErrorMessages);
        }

        protected override DomesticPaymentConsentReadFundsConfirmationResponse GetPublicResponse(
            DomesticPaymentConsentPersisted persistedObject,
            PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 apiResponse,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery)
        {
            // TODO: Update links

            return new DomesticPaymentConsentReadFundsConfirmationResponse(
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
