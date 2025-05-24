// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.VariableRecurringPayments;
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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

/// <summary>
///     Domestic VRP operations.
/// </summary>
internal class DomesticVrp :
    IDomesticVrpContext<DomesticVrpRequest, DomesticVrpResponse, DomesticVrpPaymentDetailsResponse,
        ConsentExternalCreateParams,
        ConsentExternalEntityReadParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly ClientAccessTokenGet _clientAccessTokenGet;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;
    private readonly DomesticVrpConsentCommon _domesticVrpConsentCommon;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;
    private readonly ITimeProvider _timeProvider;

    public DomesticVrp(
        IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        ConsentAccessTokenGet consentAccessTokenGet,
        IBankProfileService bankProfileService,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods,
        ClientAccessTokenGet clientAccessTokenGet,
        DomesticVrpConsentCommon domesticVrpConsentCommon)
    {
        _instrumentationClient = instrumentationClient;
        _mapper = mapper;
        _timeProvider = timeProvider;
        _consentAccessTokenGet = consentAccessTokenGet;
        _bankProfileService = bankProfileService;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _clientAccessTokenGet = clientAccessTokenGet;
        _domesticVrpConsentCommon = domesticVrpConsentCommon;
    }

    private string RelativePathBeforeId => "/domestic-vrps";

    public async Task<DomesticVrpResponse> CreateAsync(
        DomesticVrpRequest request,
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

        // Load DomesticVrpConsent and related
        (DomesticVrpConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(request.DomesticVrpConsentId, true);
        string externalApiConsentId = persistedConsent.ExternalApiId;

        // Validate consent ID
        if (string.IsNullOrEmpty(request.ExternalApiRequest.Data.ConsentId))
        {
            request.ExternalApiRequest.Data.ConsentId = externalApiConsentId;
        }
        else if (request.ExternalApiRequest.Data.ConsentId != externalApiConsentId)
        {
            throw new ArgumentException(
                $"ExternalApiRequest contains consent ID that differs from {externalApiConsentId} " +
                "inferred from specified DomesticVrpConsent ID.");
        }

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        string bankFinancialId =
            bankProfile.VariableRecurringPaymentsApiSettings.FinancialId ?? bankProfile.FinancialId;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;
        ConsentAuthGetCustomBehaviour? domesticVrpConsentAuthGetCustomBehaviour = bankProfile.CustomBehaviour
            ?.DomesticVrpConsentAuthGet;
        DomesticVrpCustomBehaviour? domesticVrpPostCustomBehaviour = bankProfile.CustomBehaviour?.DomesticVrp;
        RefreshTokenGrantPostCustomBehaviour? domesticVrpConsentRefreshTokenGrantPostCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticVrpConsentRefreshTokenGrantPost;
        JwksGetCustomBehaviour? jwksGetCustomBehaviour = bankProfile.CustomBehaviour?.JwksGet;

        // Get IApiClient
        IApiClient apiClient = bankRegistration.UseSimulatedBank
            ? bankProfile.ReplayApiClient
            : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Get access token
        string bankTokenIssuerClaim = domesticVrpConsentAuthGetCustomBehaviour
            ?.AudClaim ?? issuerUrl; // Get bank token issuer ("iss") claim
        string accessToken =
            await _consentAccessTokenGet.GetAccessTokenAndUpdateConsent(
                persistedConsent,
                bankTokenIssuerClaim,
                "payments",
                bankRegistration,
                _domesticVrpConsentCommon.GetAccessToken,
                _domesticVrpConsentCommon.GetRefreshToken,
                externalApiSecret,
                bankRegistration.TokenEndpoint,
                bankProfile.UseOpenIdConnect,
                apiClient,
                obSealKey,
                supportsSca,
                bankProfile.BankProfileEnum,
                idTokenSubClaimType,
                domesticVrpConsentRefreshTokenGrantPostCustomBehaviour,
                jwksGetCustomBehaviour,
                request.ModifiedBy);

        // Create new object at external API
        var externalApiUrl = new Uri(variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId);
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest externalApiRequest = request.ExternalApiRequest;
        externalApiRequest = bankProfile.VariableRecurringPaymentsApiSettings
            .DomesticVrpExternalApiRequestAdjustments(externalApiRequest);
        bool preferMisspeltContractPresentIndicator =
            domesticVrpPostCustomBehaviour?.PreferMisspeltContractPresentIndicator ?? false;
        externalApiRequest.Risk.AdjustBeforeSendToBank(preferMisspeltContractPresentIndicator);
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  POST {VrpBaseUrl}{{RelativePathBeforeId}}
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };
        JsonSerializerSettings? requestJsonSerializerSettings = null;
        JsonSerializerSettings responseJsonSerializerSettings = ApiClient.GetDefaultJsonSerializerSettings;
        DomesticVrpRefundConverterOptions? refundResponseJsonConverter =
            domesticVrpPostCustomBehaviour?.RefundResponseJsonConverter;
        if (refundResponseJsonConverter is not null)
        {
            var optionsDict = new Dictionary<JsonConverterLabel, int>
            {
                [JsonConverterLabel.DomesticVrpRefund] = (int) refundResponseJsonConverter
            };
            responseJsonSerializerSettings.Context =
#pragma warning disable SYSLIB0050 // see https://github.com/JamesNK/Newtonsoft.Json/issues/2953
                new StreamingContext(
                    StreamingContextStates.All,
                    optionsDict);
#pragma warning restore SYSLIB0050
        }
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse externalApiResponse;
        string? xFapiInteractionId;
        IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
        switch (variableRecurringPaymentsApi.ApiVersion)
        {
            case VariableRecurringPaymentsApiVersion.Version3p1p11:
                VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPRequest externalApiRequestV3 =
                    VariableRecurringPaymentsModelsPublic.Mappings.MapFromOBDomesticVRPRequest(externalApiRequest);
                var apiRequestsV3 =
                    new ApiRequests<VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPRequest,
                        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPResponse,
                        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPRequest,
                        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPResponse>(
                        new ApiGetRequestProcessor(bankFinancialId, accessToken),
                        new PaymentInitiationPostRequestProcessor<
                            VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPRequest>(
                            bankFinancialId,
                            accessToken,
                            _instrumentationClient,
                            softwareStatement,
                            obSealKey));
                (VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPResponse externalApiResponseV3, xFapiInteractionId,
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
                    VariableRecurringPaymentsModelsPublic.Mappings.MapToOBDomesticVRPResponse(externalApiResponseV3);

                break;
            case VariableRecurringPaymentsApiVersion.VersionPublic:
                var apiRequests =
                    new ApiRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse,
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>(
                        new ApiGetRequestProcessor(bankFinancialId, accessToken),
                        new PaymentInitiationPostRequestProcessor<
                            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest>(
                            bankFinancialId,
                            accessToken,
                            _instrumentationClient,
                            softwareStatement,
                            obSealKey));
                (externalApiResponse, xFapiInteractionId,
                        newNonErrorMessages) =
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
                    $"Variable Recurring Payments API version {variableRecurringPaymentsApi.ApiVersion} not supported.");
        }
        nonErrorMessages.AddRange(newNonErrorMessages);
        var externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
        string externalApiId = externalApiResponse.Data.DomesticVRPId;
        externalApiResponse.Risk.AdjustAfterReceiveFromBank();

        // Transform links
        string? transformedLinkUrlWithoutQuery = createParams.PublicRequestUrlWithoutQuery is { } x
            ? $"{x}/{externalApiId}"
            : null;
        var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
            LinksUrlOperations.PostMethodExpectedLinkUrls(
                externalApiUrl,
                externalApiId,
                domesticVrpPostCustomBehaviour),
            transformedLinkUrlWithoutQuery,
            domesticVrpPostCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
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

        // Create response
        var response = new DomesticVrpResponse
        {
            ExternalApiResponse = externalApiResponse,
            ExternalApiResponseInfo = externalApiResponseInfo
        };
        return response;
    }

    public async Task<DomesticVrpResponse> ReadAsync(ConsentExternalEntityReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticVrpConsent and related
        (_, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(readParams.ConsentId, false);
        string externalApiId = readParams.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        string bankFinancialId =
            bankProfile.VariableRecurringPaymentsApiSettings.FinancialId ?? bankProfile.FinancialId;
        DomesticVrpCustomBehaviour? domesticVrpGetCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticVrp;
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
        var externalApiUrl =
            new Uri(
                variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId +
                $"/{externalApiId}");
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  GET {VrpBaseUrl}{{RelativePathBeforeId}}/{DomesticVrpId}
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };
        JsonSerializerSettings? responseJsonSerializerSettings = ApiClient.GetDefaultJsonSerializerSettings;
        DomesticVrpRefundConverterOptions? refundResponseJsonConverter =
            domesticVrpGetCustomBehaviour?.RefundResponseJsonConverter;
        if (refundResponseJsonConverter is not null)
        {
            var optionsDict = new Dictionary<JsonConverterLabel, int>
            {
                [JsonConverterLabel.DomesticVrpRefund] = (int) refundResponseJsonConverter
            };
#pragma warning disable SYSLIB0050 // see https://github.com/JamesNK/Newtonsoft.Json/issues/2953
            responseJsonSerializerSettings.Context =
                new StreamingContext(
                    StreamingContextStates.All,
#pragma warning restore SYSLIB0050
                    optionsDict);
        }
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse externalApiResponse;
        string? xFapiInteractionId;
        IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
        switch (variableRecurringPaymentsApi.ApiVersion)
        {
            case VariableRecurringPaymentsApiVersion.Version3p1p11:
                var apiRequestsV3 =
                    new ApiRequests<VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPRequest,
                        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPResponse,
                        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPRequest,
                        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPResponse>(
                        new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                        new PaymentInitiationPostRequestProcessor<
                            VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPRequest>(
                            bankFinancialId,
                            ccGrantAccessToken,
                            _instrumentationClient,
                            softwareStatement,
                            obSealKey));
                (VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPResponse externalApiResponseV3, xFapiInteractionId,
                        newNonErrorMessages) =
                    await apiRequestsV3.GetAsync(
                        externalApiUrl,
                        readParams.ExtraHeaders,
                        tppReportingRequestInfo,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);
                externalApiResponse =
                    VariableRecurringPaymentsModelsPublic.Mappings.MapToOBDomesticVRPResponse(externalApiResponseV3);
                break;
            case VariableRecurringPaymentsApiVersion.VersionPublic:
                var apiRequests =
                    new ApiRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse,
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>(
                        new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                        new PaymentInitiationPostRequestProcessor<
                            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest>(
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
                    $"Variable Recurring Payments API version {variableRecurringPaymentsApi.ApiVersion} not supported.");
        }
        nonErrorMessages.AddRange(newNonErrorMessages);
        var externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
        externalApiResponse.Risk.AdjustAfterReceiveFromBank();

        // Transform links 
        string? transformedLinkUrlWithoutQuery = readParams.PublicRequestUrlWithoutQuery;
        Uri expectedLinkUrlWithoutQuery = externalApiUrl;
        var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
            LinksUrlOperations.GetMethodExpectedLinkUrls(expectedLinkUrlWithoutQuery, domesticVrpGetCustomBehaviour),
            transformedLinkUrlWithoutQuery,
            domesticVrpGetCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
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

        // Create response
        var response = new DomesticVrpResponse
        {
            ExternalApiResponse = externalApiResponse,
            ExternalApiResponseInfo = externalApiResponseInfo
        };
        return response;
    }

    public async Task<DomesticVrpPaymentDetailsResponse> ReadPaymentDetailsAsync(
        ConsentExternalEntityReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticVrpConsent and related
        (_, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(readParams.ConsentId, false);
        string externalApiId = readParams.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        string bankFinancialId =
            bankProfile.VariableRecurringPaymentsApiSettings.FinancialId ?? bankProfile.FinancialId;
        DomesticVrpCustomBehaviour? domesticVrpGetCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticVrp;
        ClientCredentialsGrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour =
            bankProfile.CustomBehaviour?.ClientCredentialsGrantPost;

        // Determine whether bank endpoint should be used
        if (!bankProfile.VariableRecurringPaymentsApiSettings.UseDomesticVrpGetPaymentDetailsEndpoint)
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
        var externalApiUrl =
            new Uri(
                variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId +
                $"/{externalApiId}/payment-details");
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  GET {VrpBaseUrl}{{RelativePathBeforeId}}/{DomesticVrpId}/payment-details
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPDetails externalApiResponse;
        string? xFapiInteractionId;
        IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
        switch (variableRecurringPaymentsApi.ApiVersion)
        {
            case VariableRecurringPaymentsApiVersion.Version3p1p11:
                var apiRequestsV3 =
                    new ApiGetRequests<VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPDetails,
                        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPDetails>(
                        new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken));
                (VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPDetails externalApiResponseV3, xFapiInteractionId,
                        newNonErrorMessages) =
                    await apiRequestsV3.GetAsync(
                        externalApiUrl,
                        readParams.ExtraHeaders,
                        tppReportingRequestInfo,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);
                externalApiResponse =
                    VariableRecurringPaymentsModelsPublic.Mappings.MapToOBDomesticVRPDetails(externalApiResponseV3);
                break;
            case VariableRecurringPaymentsApiVersion.VersionPublic:
                var apiRequests =
                    new ApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPDetails,
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPDetails>(
                        new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken));
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
                    $"Variable Recurring Payments API version {variableRecurringPaymentsApi.ApiVersion} not supported.");
        }
        nonErrorMessages.AddRange(newNonErrorMessages);
        var externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };

        // No links to transform

        // Create response
        var response = new DomesticVrpPaymentDetailsResponse
        {
            ExternalApiResponse = externalApiResponse,
            ExternalApiResponseInfo = externalApiResponseInfo
        };
        return response;
    }
}
