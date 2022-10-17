// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class DomesticPayment : DomesticPaymentConsentExternalObject<DomesticPaymentRequest,
        DomesticPaymentResponse,
        PaymentInitiationModelsPublic.OBWriteDomestic2, PaymentInitiationModelsPublic.OBWriteDomesticResponse5>
    {
        public DomesticPayment(
            IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
            IInstrumentationClient instrumentationClient,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IApiVariantMapper mapper,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IGrantPost grantPost,
            AuthContextAccessTokenGet authContextAccessTokenGet) : base(
            entityMethods,
            instrumentationClient,
            softwareStatementProfileRepo,
            mapper,
            dbSaveChangesMethod,
            timeProvider,
            grantPost,
            authContextAccessTokenGet) { }

        protected override string ClientCredentialsGrantScope => "payments";

        protected override PaymentInitiationModelsPublic.OBWriteDomestic2
            GetApiRequest(DomesticPaymentRequest request, string externalApiConsentId)
        {
            PaymentInitiationModelsPublic.OBWriteDomestic2 apiRequest = request.ExternalApiRequest;
            if (request.ExternalApiRequest.Data.ConsentId is null)
            {
                apiRequest.Data.ConsentId = externalApiConsentId;
            }
            else if (apiRequest.Data.ConsentId != externalApiConsentId)
            {
                throw new ArgumentException(
                    $"ExternalApiRequest contains consent ID that differs from {externalApiConsentId}" +
                    " inferred from specified DomesticPaymentConsent.");
            }

            return apiRequest;
        }

        protected override Uri RetrieveGetUrl(string baseUrl, string externalId) =>
            new(baseUrl + "/domestic-payments" + $"/{externalId}");

        protected override Uri RetrievePostUrl(string baseUrl) => new(baseUrl + "/domestic-payments");

        protected override DomesticPaymentResponse PublicGetResponse(
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5 apiResponse) => new(apiResponse);

        protected override IApiGetRequests<PaymentInitiationModelsPublic.OBWriteDomesticResponse5> ApiGetRequests(
            PaymentInitiationApi paymentInitiationApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient) =>
            paymentInitiationApi.PaymentInitiationApiVersion switch
            {
                PaymentInitiationApiVersion.Version3p1p4 => new ApiGetRequests<
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5,
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticResponse4>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                PaymentInitiationApiVersion.Version3p1p6 => new ApiGetRequests<
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5,
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"Payment Initiation API version {paymentInitiationApi.PaymentInitiationApiVersion} not supported.")
            };

        protected override
            IApiPostRequests<PaymentInitiationModelsPublic.OBWriteDomestic2,
                PaymentInitiationModelsPublic.OBWriteDomesticResponse5> ApiPostRequests(
                PaymentInitiationApi paymentInitiationApi,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            paymentInitiationApi.PaymentInitiationApiVersion switch
            {
                PaymentInitiationApiVersion.Version3p1p4 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomestic2,
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5,
                    PaymentInitiationModelsV3p1p4.OBWriteDomestic2,
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticResponse4>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsV3p1p4.OBWriteDomestic2>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient,
                        paymentInitiationApi.PaymentInitiationApiVersion <
                        PaymentInitiationApiVersion.Version3p1p4,
                        processedSoftwareStatementProfile)),
                PaymentInitiationApiVersion.Version3p1p6 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomestic2,
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5,
                    PaymentInitiationModelsPublic.OBWriteDomestic2,
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsPublic.OBWriteDomestic2>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient,
                        paymentInitiationApi.PaymentInitiationApiVersion <
                        PaymentInitiationApiVersion.Version3p1p4,
                        processedSoftwareStatementProfile)),
                _ => throw new ArgumentOutOfRangeException(
                    $"Payment Initiation API version {paymentInitiationApi.PaymentInitiationApiVersion} not supported.")
            };
    }
}
