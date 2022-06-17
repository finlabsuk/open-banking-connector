// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
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
        DomesticPaymentConsentPost : ConsentPost<DomesticPaymentConsentPersisted,
            DomesticPaymentConsent,
            DomesticPaymentConsentResponse,
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>
    {
        private readonly IDbReadOnlyEntityMethods<PaymentInitiationApiEntity> _bankApiSetMethods;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationMethods;

        public DomesticPaymentConsentPost(
            IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IGrantPost grantPost,
            IDbReadOnlyEntityMethods<PaymentInitiationApiEntity> bankApiSetMethods,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper,
            grantPost)
        {
            _bankApiSetMethods = bankApiSetMethods;
            _bankRegistrationMethods = bankRegistrationMethods;
        }

        protected override string ClientCredentialsGrantScope => "payments";

        protected override async Task<DomesticPaymentConsentResponse> AddEntity(
            DomesticPaymentConsent request,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5? apiResponse,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery,
            ITimeProvider timeProvider)
        {
            // TODO: Transform links
            
            string externalApiId =
                request.ExternalApiObject is null
                    ? apiResponse!.Data.ConsentId
                    : request.ExternalApiObject.ExternalApiId;

            DateTimeOffset utcNow = timeProvider.GetUtcNow();
            var persistedObject = new DomesticPaymentConsentPersisted(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy,
                null,
                0,
                utcNow,
                request.CreatedBy,
                null,
                request.BankRegistrationId,
                externalApiId,
                null,
                utcNow,
                request.CreatedBy,
                request.PaymentInitiationApiId);
            AccessToken? accessToken = request.ExternalApiObject?.AccessToken;
            if (accessToken is not null)
            {
                persistedObject.UpdateAccessToken(
                    accessToken.Token,
                    accessToken.ExpiresIn,
                    accessToken.RefreshToken,
                    utcNow,
                    request.CreatedBy);
            }

            // Save entity
            await _entityMethods.AddAsync(persistedObject);

            // Create response (may involve additional processing based on entity)
            var response = new DomesticPaymentConsentResponse(
                persistedObject.Id,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.Reference,
                persistedObject.BankRegistrationId,
                persistedObject.PaymentInitiationApiId,
                persistedObject.ExternalApiId,
                apiResponse);

            return response;
        }

        protected override
            IApiPostRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> ApiRequests(
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

        protected override async
            Task<(
                PaymentInitiationModelsPublic.OBWriteDomesticConsent4 apiRequest,
                Uri endpointUrl,
                BankApiSet2 bankApiInformation,
                BankRegistration bankRegistration,
                string bankFinancialId,
                List<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)> ApiPostRequestData(DomesticPaymentConsent request)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data and checks
            Guid bankRegistrationId = request.BankRegistrationId;
            BankRegistration bankRegistration =
                await _bankRegistrationMethods
                    .DbSetNoTracking
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == bankRegistrationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            Guid paymentInitiationApiId = request.PaymentInitiationApiId;
            PaymentInitiationApiEntity paymentInitiationApiEntity =
                await _bankApiSetMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == paymentInitiationApiId) ??
                throw new KeyNotFoundException(
                    $"No record found for PaymentInitiationApi {paymentInitiationApiId} specified by request.");
            var bankApiSet2 = new BankApiSet2
            {
                PaymentInitiationApi = new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = paymentInitiationApiEntity.ApiVersion,
                    BaseUrl = paymentInitiationApiEntity.BaseUrl
                }
            };

            if (paymentInitiationApiEntity.BankId != bankRegistration.BankId)
            {
                throw new ArgumentException(
                    "Specified PaymentInitiationApi and BankRegistration objects do not share same BankId.");
            }

            // Create request
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4 apiRequest = request.ExternalApiRequest;

            // Determine endpoint URL
            string baseUrl = paymentInitiationApiEntity.BaseUrl;
            var endpointUrl = new Uri(baseUrl + "/domestic-payment-consents");

            return (apiRequest, endpointUrl, bankApiSet2, bankRegistration, bankFinancialId, nonErrorMessages);
        }
    }
}
