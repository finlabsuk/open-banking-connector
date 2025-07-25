// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

/// <summary>
///     DomesticPayment operations.
/// </summary>
internal class DomesticPayment :
    IDomesticPaymentContext<DomesticPaymentRequest, DomesticPaymentResponse, DomesticPaymentPaymentDetailsResponse,
        ConsentExternalCreateParams, ConsentExternalEntityReadParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly ClientAccessTokenGet _clientAccessTokenGet;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;
    private readonly DomesticPaymentConsentCommon _domesticPaymentConsentCommon;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;
    private readonly ITimeProvider _timeProvider;

    public DomesticPayment(
        IDbEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        ITimeProvider timeProvider,
        ConsentAccessTokenGet consentAccessTokenGet,
        IBankProfileService bankProfileService,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods,
        ClientAccessTokenGet clientAccessTokenGet,
        DomesticPaymentConsentCommon domesticPaymentConsentCommon)
    {
        _instrumentationClient = instrumentationClient;
        _mapper = mapper;
        _timeProvider = timeProvider;
        _consentAccessTokenGet = consentAccessTokenGet;
        _bankProfileService = bankProfileService;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _clientAccessTokenGet = clientAccessTokenGet;
        _domesticPaymentConsentCommon = domesticPaymentConsentCommon;
    }

    private string RelativePathBeforeId => "/domestic-payments";

    public async
        Task<DomesticPaymentPaymentDetailsResponse> ReadPaymentDetailsAsync(
            ConsentExternalEntityReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticPaymentConsent and related
        (DomesticPaymentConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(readParams.ConsentId, false);
        string externalApiId = readParams.ExternalApiId;
        bool pispUseV4 = persistedConsent.CreatedWithV4;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        PaymentInitiationApi paymentInitiationApi = bankProfile.GetRequiredPaymentInitiationApi(pispUseV4);
        string bankFinancialId = bankProfile.PaymentInitiationApiSettings.FinancialId ?? bankProfile.FinancialId;
        DomesticPaymentCustomBehaviour? domesticPaymentGetCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticPayment;
        ClientCredentialsGrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour =
            bankProfile.CustomBehaviour?.ClientCredentialsGrantPost;

        // Determine whether bank endpoint should be used
        if (!bankProfile.PaymentInitiationApiSettings.UseDomesticPaymentGetPaymentDetailsEndpoint)
        {
            throw new InvalidOperationException("Bank profile does not specify support for this endpoint.");
        }

        // Get IApiClient
        IApiClient apiClient = bankRegistration.UseSimulatedBank
            ? bankProfile.ReplayApiClient
            : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Get client credentials grant access token
        var scope = "payments";
        string ccGrantAccessToken =
            await _clientAccessTokenGet.GetAccessToken(
                scope,
                obSealKey,
                bankRegistration,
                externalApiSecret,
                clientCredentialsGrantPostCustomBehaviour,
                apiClient,
                bankProfile.BankProfileEnum);

        // Read object from external API
        var externalApiUrl = new Uri(
            paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{externalApiId}/payment-details");
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  GET {PispBaseUrl}{{RelativePathBeforeId}}/{DomesticPaymentId}/payment-details
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        PaymentInitiationModelsPublic.OBWritePaymentDetailsResponse1 externalApiResponse;
        string? xFapiInteractionId;
        IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
        switch (paymentInitiationApi.ApiVersion)
        {
            case PaymentInitiationApiVersion.Version3p1p11:
                var apiRequestsV3 =
                    new ApiGetRequests<PaymentInitiationModelsV3p1p11.OBWritePaymentDetailsResponse1,
                        PaymentInitiationModelsV3p1p11.OBWritePaymentDetailsResponse1>(
                        new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken));
                (PaymentInitiationModelsV3p1p11.OBWritePaymentDetailsResponse1 externalApiResponseV3,
                        xFapiInteractionId,
                        newNonErrorMessages) =
                    await apiRequestsV3.GetAsync(
                        externalApiUrl,
                        readParams.ExtraHeaders,
                        tppReportingRequestInfo,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);
                externalApiResponse =
                    PaymentInitiationModelsPublic.Mappings.MapToOBWritePaymentDetailsResponse1(externalApiResponseV3);
                break;
            case PaymentInitiationApiVersion.VersionPublic:
                var apiRequests =
                    new ApiGetRequests<PaymentInitiationModelsPublic.OBWritePaymentDetailsResponse1,
                        PaymentInitiationModelsPublic.OBWritePaymentDetailsResponse1>(
                        new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken));
                (externalApiResponse, xFapiInteractionId,
                        newNonErrorMessages) =
                    await apiRequests.GetAsync(
                        externalApiUrl,
                        readParams.ExtraHeaders,
                        tppReportingRequestInfo,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"PISP API version {paymentInitiationApi.ApiVersion} not supported.");
        }
        nonErrorMessages.AddRange(newNonErrorMessages);
        var externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };

        // Transform links 
        if (externalApiResponse.Links is not null)
        {
            string? transformedLinkUrlWithoutQuery = readParams.PublicRequestUrlWithoutQuery;
            Uri expectedLinkUrlWithoutQuery = externalApiUrl;
            var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
                LinksUrlOperations.GetMethodExpectedLinkUrls(
                    expectedLinkUrlWithoutQuery,
                    domesticPaymentGetCustomBehaviour),
                transformedLinkUrlWithoutQuery,
                domesticPaymentGetCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
                false);
            externalApiResponse.Links.Self =
                linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Self);
            if (externalApiResponse.Links.First is not null)
            {
                externalApiResponse.Links.First =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.First);
            }
            if (externalApiResponse.Links.Prev is not null)
            {
                externalApiResponse.Links.Prev =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Prev);
            }
            if (externalApiResponse.Links.Next is not null)
            {
                externalApiResponse.Links.Next =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Next);
            }
            if (externalApiResponse.Links.Last is not null)
            {
                externalApiResponse.Links.Last =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Last);
            }
        }

        // Create response
        var response = new DomesticPaymentPaymentDetailsResponse
        {
            ExternalApiResponse = externalApiResponse,
            ExternalApiResponseInfo = externalApiResponseInfo
        };
        return response;
    }

    public async
        Task<DomesticPaymentResponse> CreateAsync(
            DomesticPaymentRequest request,
            ConsentExternalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Validate request data and convert to messages
        ValidationResult validationResult = await request.ValidateAsync();
        if (validationResult.Errors.Any(failure => failure.Severity == Severity.Error))
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Load DomesticPaymentConsent and related
        (DomesticPaymentConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(request.DomesticPaymentConsentId, true);
        string externalApiConsentId = persistedConsent.ExternalApiId;
        bool pispUseV4 = persistedConsent.CreatedWithV4;

        // Validate consent ID
        if (string.IsNullOrEmpty(request.ExternalApiRequest.Data.ConsentId))
        {
            request.ExternalApiRequest.Data.ConsentId = externalApiConsentId;
        }
        else if (request.ExternalApiRequest.Data.ConsentId != externalApiConsentId)
        {
            throw new ArgumentException(
                $"ExternalApiRequest contains consent ID that differs from {externalApiConsentId} " +
                "inferred from specified DomesticPaymentConsent ID.");
        }

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        PaymentInitiationApi paymentInitiationApi = bankProfile.GetRequiredPaymentInitiationApi(pispUseV4);
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        string bankFinancialId = bankProfile.PaymentInitiationApiSettings.FinancialId ?? bankProfile.FinancialId;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;
        ConsentAuthGetCustomBehaviour? domesticPaymentConsentAuthGetCustomBehaviour = bankProfile.CustomBehaviour
            ?.DomesticPaymentConsentAuthGet;
        DomesticPaymentCustomBehaviour? readWritePostCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticPayment;
        RefreshTokenGrantPostCustomBehaviour? domesticPaymentConsentRefreshTokenGrantPostCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticPaymentConsentRefreshTokenGrantPost;
        JwksGetCustomBehaviour? jwksGetCustomBehaviour = bankProfile.CustomBehaviour?.JwksGet;

        // Get IApiClient
        IApiClient apiClient = bankRegistration.UseSimulatedBank
            ? bankProfile.ReplayApiClient
            : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Get access token
        string bankTokenIssuerClaim = domesticPaymentConsentAuthGetCustomBehaviour
            ?.AudClaim ?? issuerUrl; // Get bank token issuer ("iss") claim
        string accessToken =
            await _consentAccessTokenGet.GetAccessTokenAndUpdateConsent(
                persistedConsent,
                bankTokenIssuerClaim,
                "payments",
                bankRegistration,
                _domesticPaymentConsentCommon.GetAccessToken,
                _domesticPaymentConsentCommon.GetRefreshToken,
                _domesticPaymentConsentCommon.AddNewAccessToken,
                _domesticPaymentConsentCommon.AddNewRefreshToken,
                externalApiSecret,
                bankProfile.UseOpenIdConnect,
                apiClient,
                obSealKey,
                supportsSca,
                bankProfile.BankProfileEnum,
                idTokenSubClaimType,
                domesticPaymentConsentRefreshTokenGrantPostCustomBehaviour,
                jwksGetCustomBehaviour,
                request.ModifiedBy);

        // Create new object at external API
        var externalApiUrl = new Uri(paymentInitiationApi.BaseUrl + RelativePathBeforeId);
        PaymentInitiationModelsPublic.OBWriteDomestic2 externalApiRequest = request.ExternalApiRequest;
        externalApiRequest = bankProfile.PaymentInitiationApiSettings
            .DomesticPaymentExternalApiRequestAdjustments(externalApiRequest);
        if (externalApiRequest.Risk.ContractPresentInidicator is not null)
        {
            throw new ArgumentException("ExternalApiRequest contains mis-spelt field Risk/ContractPresentInidicator.");
        }
        if (!pispUseV4)
        {
            bool preferMisspeltContractPresentIndicator =
                readWritePostCustomBehaviour?.PreferMisspeltContractPresentIndicator ?? false;
            externalApiRequest.Risk.AdjustBeforeSendToBank(preferMisspeltContractPresentIndicator);
        }
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  POST {PispBaseUrl}{{RelativePathBeforeId}}
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };
        JsonSerializerSettings? requestJsonSerializerSettings = null;
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        PaymentInitiationModelsPublic.OBWriteDomesticResponse5 externalApiResponse;
        string? xFapiInteractionId;
        IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
        switch (paymentInitiationApi.ApiVersion)
        {
            case PaymentInitiationApiVersion.Version3p1p11:
                PaymentInitiationModelsV3p1p11.OBWriteDomestic2 externalApiRequestV3 =
                    PaymentInitiationModelsPublic.Mappings.MapFromOBWriteDomestic2(externalApiRequest);
                var apiRequestsV3 =
                    new ApiRequests<PaymentInitiationModelsV3p1p11.OBWriteDomestic2,
                        PaymentInitiationModelsV3p1p11.OBWriteDomesticResponse5,
                        PaymentInitiationModelsV3p1p11.OBWriteDomestic2,
                        PaymentInitiationModelsV3p1p11.OBWriteDomesticResponse5>(
                        new ApiGetRequestProcessor(bankFinancialId, accessToken),
                        new PaymentInitiationPostRequestProcessor<PaymentInitiationModelsV3p1p11.OBWriteDomestic2>(
                            bankFinancialId,
                            accessToken,
                            _instrumentationClient,
                            softwareStatement,
                            obSealKey));
                (PaymentInitiationModelsV3p1p11.OBWriteDomesticResponse5 externalApiResponseV3, xFapiInteractionId,
                        newNonErrorMessages) =
                    await apiRequestsV3.PostAsync(
                        externalApiUrl,
                        createParams.ExtraHeaders,
                        externalApiRequestV3,
                        tppReportingRequestInfo,
                        requestJsonSerializerSettings,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);
                externalApiResponse =
                    PaymentInitiationModelsPublic.Mappings.MapToOBWriteDomesticResponse5(externalApiResponseV3);
                break;
            case PaymentInitiationApiVersion.VersionPublic:
                var apiRequests =
                    new ApiRequests<PaymentInitiationModelsPublic.OBWriteDomestic2,
                        PaymentInitiationModelsPublic.OBWriteDomesticResponse5,
                        PaymentInitiationModelsPublic.OBWriteDomestic2,
                        PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(
                        new ApiGetRequestProcessor(bankFinancialId, accessToken),
                        new PaymentInitiationPostRequestProcessor<PaymentInitiationModelsPublic.OBWriteDomestic2>(
                            bankFinancialId,
                            accessToken,
                            _instrumentationClient,
                            softwareStatement,
                            obSealKey));
                (externalApiResponse, xFapiInteractionId, newNonErrorMessages) =
                    await apiRequests.PostAsync(
                        externalApiUrl,
                        createParams.ExtraHeaders,
                        externalApiRequest,
                        tppReportingRequestInfo,
                        requestJsonSerializerSettings,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"Payment Initiation API version {paymentInitiationApi.ApiVersion} not supported.");
        }
        nonErrorMessages.AddRange(newNonErrorMessages);
        var externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
        string externalApiId = externalApiResponse.Data.DomesticPaymentId;

        // Transform links
        if (externalApiResponse.Links is not null)
        {
            string? transformedLinkUrlWithoutQuery = createParams.PublicRequestUrlWithoutQuery is { } x
                ? $"{x}/{externalApiId}"
                : null;
            var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
                LinksUrlOperations.PostMethodExpectedLinkUrls(
                    externalApiUrl,
                    externalApiId,
                    readWritePostCustomBehaviour),
                transformedLinkUrlWithoutQuery,
                readWritePostCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
                false);
            externalApiResponse.Links.Self =
                linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Self);
            if (externalApiResponse.Links.First is not null)
            {
                externalApiResponse.Links.First =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.First);
            }
            if (externalApiResponse.Links.Prev is not null)
            {
                externalApiResponse.Links.Prev =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Prev);
            }
            if (externalApiResponse.Links.Next is not null)
            {
                externalApiResponse.Links.Next =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Next);
            }
            if (externalApiResponse.Links.Last is not null)
            {
                externalApiResponse.Links.Last =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Last);
            }
        }

        // Create response
        var response = new DomesticPaymentResponse
        {
            ExternalApiResponse = externalApiResponse,
            ExternalApiResponseInfo = externalApiResponseInfo
        };
        return response;
    }

    public async
        Task<DomesticPaymentResponse> ReadAsync(ConsentExternalEntityReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticPaymentConsent and related
        (DomesticPaymentConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(readParams.ConsentId, false);
        string externalApiId = readParams.ExternalApiId;
        bool pispUseV4 = persistedConsent.CreatedWithV4;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        PaymentInitiationApi paymentInitiationApi = bankProfile.GetRequiredPaymentInitiationApi(pispUseV4);
        string bankFinancialId = bankProfile.PaymentInitiationApiSettings.FinancialId ?? bankProfile.FinancialId;
        DomesticPaymentCustomBehaviour? domesticPaymentGetCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticPayment;
        ClientCredentialsGrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour =
            bankProfile.CustomBehaviour?.ClientCredentialsGrantPost;

        // Get IApiClient
        IApiClient apiClient = bankRegistration.UseSimulatedBank
            ? bankProfile.ReplayApiClient
            : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Get client credentials grant access token
        var scope = "payments";
        string ccGrantAccessToken =
            await _clientAccessTokenGet.GetAccessToken(
                scope,
                obSealKey,
                bankRegistration,
                externalApiSecret,
                clientCredentialsGrantPostCustomBehaviour,
                apiClient,
                bankProfile.BankProfileEnum);

        // Read object from external API
        var externalApiUrl = new Uri(paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{externalApiId}");
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  GET {PispBaseUrl}{{RelativePathBeforeId}}/{DomesticPaymentId}
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        PaymentInitiationModelsPublic.OBWriteDomesticResponse5 externalApiResponse;
        string? xFapiInteractionId;
        IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
        switch (paymentInitiationApi.ApiVersion)
        {
            case PaymentInitiationApiVersion.Version3p1p11:
                var apiRequestsV3 =
                    new ApiRequests<PaymentInitiationModelsV3p1p11.OBWriteDomestic2,
                        PaymentInitiationModelsV3p1p11.OBWriteDomesticResponse5,
                        PaymentInitiationModelsV3p1p11.OBWriteDomestic2,
                        PaymentInitiationModelsV3p1p11.OBWriteDomesticResponse5>(
                        new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                        new PaymentInitiationPostRequestProcessor<PaymentInitiationModelsV3p1p11.OBWriteDomestic2>(
                            bankFinancialId,
                            ccGrantAccessToken,
                            _instrumentationClient,
                            softwareStatement,
                            obSealKey));
                (PaymentInitiationModelsV3p1p11.OBWriteDomesticResponse5 externalApiResponseV3, xFapiInteractionId,
                        newNonErrorMessages) =
                    await apiRequestsV3.GetAsync(
                        externalApiUrl,
                        readParams.ExtraHeaders,
                        tppReportingRequestInfo,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);
                externalApiResponse =
                    PaymentInitiationModelsPublic.Mappings.MapToOBWriteDomesticResponse5(externalApiResponseV3);
                break;
            case PaymentInitiationApiVersion.VersionPublic:
                var apiRequests =
                    new ApiRequests<PaymentInitiationModelsPublic.OBWriteDomestic2,
                        PaymentInitiationModelsPublic.OBWriteDomesticResponse5,
                        PaymentInitiationModelsPublic.OBWriteDomestic2,
                        PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(
                        new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                        new PaymentInitiationPostRequestProcessor<PaymentInitiationModelsPublic.OBWriteDomestic2>(
                            bankFinancialId,
                            ccGrantAccessToken,
                            _instrumentationClient,
                            softwareStatement,
                            obSealKey));
                (externalApiResponse, xFapiInteractionId, newNonErrorMessages) =
                    await apiRequests.GetAsync(
                        externalApiUrl,
                        readParams.ExtraHeaders,
                        tppReportingRequestInfo,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"Payment Initiation API version {paymentInitiationApi.ApiVersion} not supported.");
        }
        nonErrorMessages.AddRange(newNonErrorMessages);
        var externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };

        // Transform links 
        if (externalApiResponse.Links is not null)
        {
            string? transformedLinkUrlWithoutQuery = readParams.PublicRequestUrlWithoutQuery;
            Uri expectedLinkUrlWithoutQuery = externalApiUrl;
            var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
                LinksUrlOperations.GetMethodExpectedLinkUrls(
                    expectedLinkUrlWithoutQuery,
                    domesticPaymentGetCustomBehaviour),
                transformedLinkUrlWithoutQuery,
                domesticPaymentGetCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
                false);
            externalApiResponse.Links.Self =
                linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Self);
            if (externalApiResponse.Links.First is not null)
            {
                externalApiResponse.Links.First =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.First);
            }
            if (externalApiResponse.Links.Prev is not null)
            {
                externalApiResponse.Links.Prev =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Prev);
            }
            if (externalApiResponse.Links.Next is not null)
            {
                externalApiResponse.Links.Next =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Next);
            }
            if (externalApiResponse.Links.Last is not null)
            {
                externalApiResponse.Links.Last =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Last);
            }
        }

        // Create response
        var response = new DomesticPaymentResponse
        {
            ExternalApiResponse = externalApiResponse,
            ExternalApiResponseInfo = externalApiResponseInfo
        };
        return response;
    }
}
