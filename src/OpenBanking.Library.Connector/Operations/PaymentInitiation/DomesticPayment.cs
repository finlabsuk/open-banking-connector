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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    /// <summary>
    ///     DomesticPayment create and read operations.
    /// </summary>
    internal class DomesticPayment :
        IExternalCreate<DomesticPaymentRequest, DomesticPaymentResponse>,
        IExternalRead<DomesticPaymentResponse>
    {
        private readonly ConsentAccessTokenGet _consentAccessTokenGet;
        private readonly DomesticPaymentConsentCommon _domesticPaymentConsentCommon;
        private readonly IGrantPost _grantPost;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IApiVariantMapper _mapper;
        private readonly ITimeProvider _timeProvider;

        public DomesticPayment(
            IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
            IInstrumentationClient instrumentationClient,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IApiVariantMapper mapper,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IGrantPost grantPost,
            ConsentAccessTokenGet consentAccessTokenGet)
        {
            _instrumentationClient = instrumentationClient;
            _mapper = mapper;
            _timeProvider = timeProvider;
            _grantPost = grantPost;
            _consentAccessTokenGet = consentAccessTokenGet;
            _domesticPaymentConsentCommon = new DomesticPaymentConsentCommon(
                entityMethods,
                instrumentationClient,
                softwareStatementProfileRepo);
        }

        private string ClientCredentialsGrantScope => "payments";

        private string RelativePathBeforeId => "/domestic-payments";

        public async
            Task<(DomesticPaymentResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            CreateAsync(DomesticPaymentRequest request, Guid consentId, string? createdBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load DomesticPaymentConsent and related
            (DomesticPaymentConsentPersisted persistedConsent, string externalApiConsentId,
                    PaymentInitiationApiEntity paymentInitiationApi, BankRegistration bankRegistration,
                    string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
                await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(consentId);

            // Get access token
            string bankIssuerUrl =
                persistedConsent.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                    ?.DomesticPaymentConsentAuthGet
                    ?.AudClaim ??
                bankRegistration.BankNavigation.IssuerUrl;
            string accessToken =
                await _consentAccessTokenGet.GetAccessTokenAndUpdateConsent(
                    persistedConsent,
                    bankIssuerUrl,
                    "openid payments",
                    bankRegistration,
                    persistedConsent.BankRegistrationNavigation.BankNavigation.TokenEndpoint,
                    createdBy);

            // Create external object at bank API
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiPostRequests<PaymentInitiationModelsPublic.OBWriteDomestic2,
                PaymentInitiationModelsPublic.OBWriteDomesticResponse5> apiRequests =
                ApiRequests(
                    paymentInitiationApi.ApiVersion,
                    bankFinancialId,
                    accessToken,
                    processedSoftwareStatementProfile);
            var externalApiUrl = new Uri(paymentInitiationApi.BaseUrl + RelativePathBeforeId);
            PaymentInitiationModelsPublic.OBWriteDomestic2 externalApiRequest = request.ExternalApiRequest;
            if (string.IsNullOrEmpty(request.ExternalApiRequest.Data.ConsentId))
            {
                externalApiRequest.Data.ConsentId = externalApiConsentId;
            }
            else if (externalApiRequest.Data.ConsentId != externalApiConsentId)
            {
                throw new ArgumentException(
                    $"ExternalApiRequest contains consent ID that differs from {externalApiConsentId} " +
                    "inferred from specified DomesticPaymentConsent.");
            }

            (PaymentInitiationModelsPublic.OBWriteDomesticResponse5 externalApiResponse,
                    IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.PostAsync(
                    externalApiUrl,
                    externalApiRequest,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response
            var response = new DomesticPaymentResponse(externalApiResponse);
            return (response, nonErrorMessages);
        }

        public async
            Task<(DomesticPaymentResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ReadAsync(string externalId, Guid consentId, string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load DomesticPaymentConsent and related
            (DomesticPaymentConsentPersisted persistedConsent, string _,
                    PaymentInitiationApiEntity paymentInitiationApi, BankRegistration bankRegistration,
                    string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
                await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(consentId);

            // Get client credentials grant access token
            string ccGrantAccessToken =
                (await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    persistedConsent.BankRegistrationNavigation.BankNavigation.TokenEndpoint,
                    null,
                    processedSoftwareStatementProfile.ApiClient,
                    _instrumentationClient))
                .AccessToken;

            // Read object from external API
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiGetRequests<PaymentInitiationModelsPublic.OBWriteDomesticResponse5> apiRequests =
                ApiRequests(
                    paymentInitiationApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            var externalApiUrl = new Uri(paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{externalId}");
            (PaymentInitiationModelsPublic.OBWriteDomesticResponse5 externalApiResponse,
                    IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.GetAsync(
                    externalApiUrl,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response
            var response = new DomesticPaymentResponse(externalApiResponse);
            return (response, nonErrorMessages);
        }

        private
            IApiRequests<PaymentInitiationModelsPublic.OBWriteDomestic2,
                PaymentInitiationModelsPublic.OBWriteDomesticResponse5> ApiRequests(
                PaymentInitiationApiVersion paymentInitiationApiVersion,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =>
            paymentInitiationApiVersion switch
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
                        _instrumentationClient,
                        paymentInitiationApiVersion <
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
                        _instrumentationClient,
                        paymentInitiationApiVersion <
                        PaymentInitiationApiVersion.Version3p1p4,
                        processedSoftwareStatementProfile)),
                _ => throw new ArgumentOutOfRangeException(
                    $"Payment Initiation API version {paymentInitiationApiVersion} not supported.")
            };
    }
}
