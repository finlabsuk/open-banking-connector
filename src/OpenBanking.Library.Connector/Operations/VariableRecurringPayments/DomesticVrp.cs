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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
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
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;
    private readonly DomesticVrpConsentCommon _domesticVrpConsentCommon;
    private readonly IGrantPost _grantPost;
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
        IGrantPost grantPost,
        ConsentAccessTokenGet consentAccessTokenGet,
        IBankProfileService bankProfileService,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods)
    {
        _instrumentationClient = instrumentationClient;
        _mapper = mapper;
        _timeProvider = timeProvider;
        _grantPost = grantPost;
        _consentAccessTokenGet = consentAccessTokenGet;
        _bankProfileService = bankProfileService;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _domesticVrpConsentCommon = new DomesticVrpConsentCommon(
            entityMethods,
            instrumentationClient);
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
                DomesticVrpConsentAccessToken? storedAccessToken, DomesticVrpConsentRefreshToken? storedRefreshToken,
                SoftwareStatementEntity softwareStatement) =
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
                "inferred from specified DomesticVrpConsentId.");
        }

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        string bankFinancialId = bankProfile.FinancialId;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;
        ConsentAuthGetCustomBehaviour? domesticVrpConsentAuthGetCustomBehaviour = bankProfile.CustomBehaviour
            ?.DomesticVrpConsentAuthGet;
        DomesticVrpPostCustomBehaviour? domesticVrpPostCustomBehaviour = bankProfile.CustomBehaviour?.DomesticVrpPost;
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
                "openid payments",
                bankRegistration,
                storedAccessToken,
                storedRefreshToken,
                tokenEndpointAuthMethod,
                bankRegistration.TokenEndpoint,
                apiClient,
                obSealKey,
                supportsSca,
                bankProfile.BankProfileEnum,
                idTokenSubClaimType,
                domesticVrpConsentRefreshTokenGrantPostCustomBehaviour,
                jwksGetCustomBehaviour,
                request.ModifiedBy);

        // Create new object at external API
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
                new StreamingContext(
                    StreamingContextStates.All,
                    optionsDict);
        }

        IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> apiRequests =
            ApiRequests(
                variableRecurringPaymentsApi.ApiVersion,
                bankFinancialId,
                accessToken,
                softwareStatement,
                obSealKey);
        var externalApiUrl = new Uri(variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId);
        if (string.IsNullOrEmpty(request.ExternalApiRequest.Data.ConsentId))
        {
            request.ExternalApiRequest.Data.ConsentId = externalApiConsentId;
        }
        else if (request.ExternalApiRequest.Data.ConsentId != externalApiConsentId)
        {
            throw new ArgumentException(
                $"ExternalApiRequest contains consent ID that differs from {externalApiConsentId} " +
                "inferred from specified DomesticVrpConsent.");
        }
        bool omitVrpType = domesticVrpPostCustomBehaviour?.OmitVrpType ?? false;
        if (omitVrpType)
        {
            request.ExternalApiRequest.Data.VRPType = null;
        }
        bool omitPsuInteractionType = domesticVrpPostCustomBehaviour?.OmitPsuInteractionType ?? false;
        if (omitPsuInteractionType)
        {
            request.ExternalApiRequest.Data.PSUInteractionType = null;
        }
        bool preferMisspeltContractPresentIndicator =
            domesticVrpPostCustomBehaviour?.PreferMisspeltContractPresentIndicator ?? false;
        request.ExternalApiRequest.Risk.AdjustBeforeSendToBank(preferMisspeltContractPresentIndicator);
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  POST {VrpBaseUrl}{{RelativePathBeforeId}}
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };
        (VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse externalApiResponse, string? xFapiInteractionId,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.PostAsync(
                externalApiUrl,
                createParams.ExtraHeaders,
                request.ExternalApiRequest,
                tppReportingRequestInfo,
                requestJsonSerializerSettings,
                responseJsonSerializerSettings,
                apiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);
        var externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
        string externalApiId = externalApiResponse.Data.DomesticVRPId;
        externalApiResponse.Risk.AdjustAfterReceiveFromBank();

        // Transform links
        string? transformedLinkUrlWithoutQuery = createParams.PublicRequestUrlWithoutQuery is { } x
            ? $"{x}/{externalApiId}"
            : null;
        Uri expectedLinkUrlWithoutQuery = LinksUrlOperations.GetExpectedLinkUrlWithoutQuery(
            domesticVrpPostCustomBehaviour,
            externalApiUrl,
            externalApiId);
        var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
            expectedLinkUrlWithoutQuery,
            transformedLinkUrlWithoutQuery,
            domesticVrpPostCustomBehaviour,
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
        (_, BankRegistrationEntity bankRegistration, _, _,
                SoftwareStatementEntity softwareStatement) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(readParams.ConsentId, false);
        string externalApiId = readParams.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        string bankFinancialId = bankProfile.FinancialId;
        DomesticVrpGetCustomBehaviour? domesticVrpGetCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticVrpGet;
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
        bool addOpenIdScope = domesticVrpGetCustomBehaviour?.AddOpenIdScope ?? false;
        string ccGrantAccessToken =
            await _grantPost.PostClientCredentialsGrantAsync(
                GetClientCredentialsGrantScope(addOpenIdScope),
                obSealKey,
                tokenEndpointAuthMethod,
                bankRegistration.TokenEndpoint,
                bankRegistration.ExternalApiId,
                bankRegistration.ExternalApiSecret,
                bankRegistration.Id.ToString(),
                null,
                clientCredentialsGrantPostCustomBehaviour,
                apiClient,
                bankProfile.BankProfileEnum);

        // Read object from external API
        JsonSerializerSettings? responseJsonSerializerSettings = ApiClient.GetDefaultJsonSerializerSettings;
        DomesticVrpRefundConverterOptions? refundResponseJsonConverter =
            domesticVrpGetCustomBehaviour?.RefundResponseJsonConverter;
        if (refundResponseJsonConverter is not null)
        {
            var optionsDict = new Dictionary<JsonConverterLabel, int>
            {
                [JsonConverterLabel.DomesticVrpRefund] = (int) refundResponseJsonConverter
            };
            responseJsonSerializerSettings.Context =
                new StreamingContext(
                    StreamingContextStates.All,
                    optionsDict);
        }

        IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> apiRequests =
            ApiRequests(
                variableRecurringPaymentsApi.ApiVersion,
                bankFinancialId,
                ccGrantAccessToken,
                softwareStatement,
                obSealKey);
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
        (VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse externalApiResponse, string? xFapiInteractionId,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.GetAsync(
                externalApiUrl,
                readParams.ExtraHeaders,
                tppReportingRequestInfo,
                responseJsonSerializerSettings,
                apiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);
        var externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
        externalApiResponse.Risk.AdjustAfterReceiveFromBank();

        // Transform links 
        string? transformedLinkUrlWithoutQuery = readParams.PublicRequestUrlWithoutQuery;
        Uri expectedLinkUrlWithoutQuery = externalApiUrl;
        var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
            expectedLinkUrlWithoutQuery,
            transformedLinkUrlWithoutQuery,
            domesticVrpGetCustomBehaviour,
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
        (_, BankRegistrationEntity bankRegistration, _, _,
                SoftwareStatementEntity softwareStatement) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(readParams.ConsentId, false);
        string externalApiId = readParams.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        string bankFinancialId = bankProfile.FinancialId;
        DomesticVrpGetCustomBehaviour? domesticVrpGetCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticVrpGetPaymentDetails;
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
        bool addOpenIdScope = domesticVrpGetCustomBehaviour?.AddOpenIdScope ?? false;
        string ccGrantAccessToken =
            await _grantPost.PostClientCredentialsGrantAsync(
                GetClientCredentialsGrantScope(addOpenIdScope),
                obSealKey,
                tokenEndpointAuthMethod,
                bankRegistration.TokenEndpoint,
                bankRegistration.ExternalApiId,
                bankRegistration.ExternalApiSecret,
                bankRegistration.Id.ToString(),
                null,
                clientCredentialsGrantPostCustomBehaviour,
                apiClient,
                bankProfile.BankProfileEnum);

        // Read object from external API
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPDetails> apiRequests =
            ApiRequestsPaymentDetails(
                variableRecurringPaymentsApi.ApiVersion,
                bankFinancialId,
                ccGrantAccessToken);
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
        (VariableRecurringPaymentsModelsPublic.OBDomesticVRPDetails externalApiResponse, string? xFapiInteractionId,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.GetAsync(
                externalApiUrl,
                readParams.ExtraHeaders,
                tppReportingRequestInfo,
                responseJsonSerializerSettings,
                apiClient,
                _mapper);
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

    private string GetClientCredentialsGrantScope(bool addOpenIdScope) =>
        addOpenIdScope ? "openid payments" : "payments";

    private
        IApiRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> ApiRequests(
            VariableRecurringPaymentsApiVersion variableRecurringPaymentsApiVersion,
            string bankFinancialId,
            string accessToken,
            SoftwareStatementEntity softwareStatement,
            OBSealKey obSealKey) =>
        variableRecurringPaymentsApiVersion switch
        {
            VariableRecurringPaymentsApiVersion.VersionPublic => new ApiRequests<
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
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
                    obSealKey)),
            _ => throw new ArgumentOutOfRangeException(
                $"Variable Recurring Payments API version {variableRecurringPaymentsApiVersion} not supported.")
        };

    private
        IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPDetails> ApiRequestsPaymentDetails(
            VariableRecurringPaymentsApiVersion variableRecurringPaymentsApiVersion,
            string bankFinancialId,
            string accessToken) =>
        variableRecurringPaymentsApiVersion switch
        {
            VariableRecurringPaymentsApiVersion.VersionPublic => new ApiGetRequests<
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPDetails,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPDetails>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken)),
            _ => throw new ArgumentOutOfRangeException(
                $"Variable Recurring Payments API version {variableRecurringPaymentsApiVersion} not supported.")
        };
}
