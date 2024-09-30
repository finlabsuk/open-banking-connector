// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
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

internal class
    DomesticVrpConsentOperations :
    IObjectCreate<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse, ConsentCreateParams>,
    IObjectRead<DomesticVrpConsentCreateResponse, ConsentReadParams>,
    ICreateVrpConsentFundsConfirmationContext<DomesticVrpConsentFundsConfirmationRequest,
        DomesticVrpConsentFundsConfirmationResponse>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly ClientAccessTokenGet _clientAccessTokenGet;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;

    private readonly ConsentCommon<DomesticVrpConsentPersisted,
        DomesticVrpConsentRequest,
        DomesticVrpConsentCreateResponse,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> _consentCommon;

    private readonly IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> _consentEntityMethods;
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly DomesticVrpConsentCommon _domesticVrpConsentCommon;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;
    private readonly ITimeProvider _timeProvider;

    public DomesticVrpConsentOperations(
        IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IBankProfileService bankProfileService,
        ConsentAccessTokenGet consentAccessTokenGet,
        IDbReadOnlyEntityMethods<BankRegistrationEntity> bankRegistrationMethods,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods,
        ClientAccessTokenGet clientAccessTokenGet,
        DomesticVrpConsentCommon domesticVrpConsentCommon)
    {
        _consentEntityMethods = entityMethods;
        _bankProfileService = bankProfileService;
        _consentAccessTokenGet = consentAccessTokenGet;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _clientAccessTokenGet = clientAccessTokenGet;
        _domesticVrpConsentCommon = domesticVrpConsentCommon;
        _mapper = mapper;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _consentCommon =
            new ConsentCommon<DomesticVrpConsentPersisted, DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>(
                bankRegistrationMethods,
                instrumentationClient);
    }

    private string ClientCredentialsGrantScope => "payments";

    private string RelativePathBeforeId => "/domestic-vrp-consents";

    public async
        Task<DomesticVrpConsentFundsConfirmationResponse> CreateFundsConfirmationAsync(
            VrpConsentFundsConfirmationCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Validate request data and convert to messages
        ValidationResult validationResult = await createParams.Request.ValidateAsync();
        if (validationResult.Errors.Any(failure => failure.Severity == Severity.Error))
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Load DomesticVrpConsent and related
        (DomesticVrpConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(createParams.ConsentId, true);
        string externalApiConsentId = persistedConsent.ExternalApiId;

        // Validate consent ID
        if (string.IsNullOrEmpty(createParams.Request.ExternalApiRequest.Data.ConsentId))
        {
            createParams.Request.ExternalApiRequest.Data.ConsentId = externalApiConsentId;
        }
        else if (createParams.Request.ExternalApiRequest.Data.ConsentId != externalApiConsentId)
        {
            throw new ArgumentException(
                $"ExternalApiRequest contains consent ID that differs from {externalApiConsentId} " +
                "inferred from URL path.");
        }

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        bool supportsSca = bankProfile.SupportsSca;
        string bankFinancialId =
            bankProfile.VariableRecurringPaymentsApiSettings.FinancialId ?? bankProfile.FinancialId;
        string issuerUrl = bankProfile.IssuerUrl;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;
        RefreshTokenGrantPostCustomBehaviour? domesticVrpConsentRefreshTokenGrantPostCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticVrpConsentRefreshTokenGrantPost;
        JwksGetCustomBehaviour? jwksGetCustomBehaviour = bankProfile.CustomBehaviour?.JwksGet;
        ConsentAuthGetCustomBehaviour? domesticVrpConsentAuthGetCustomBehaviour = bankProfile.CustomBehaviour
            ?.DomesticVrpConsentAuthGet;
        DomesticVrpConsentCustomBehaviour? domesticVrpConsentPostFundsConfirmationCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticVrpConsent;

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
                createParams.Request.ModifiedBy);

        // Read object from external API
        JsonSerializerSettings? requestJsonSerializerSettings = null;
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest,
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse> apiRequests =
            ApiRequestsFundsConfirmation(
                variableRecurringPaymentsApi.ApiVersion,
                bankFinancialId,
                accessToken,
                softwareStatement,
                obSealKey);
        var externalApiUrl = new Uri(
            variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}" +
            "/funds-confirmation");
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  POST {VrpBaseUrl}{{RelativePathBeforeId}}/{ConsentId}/funds-confirmation
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };

        (VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse externalApiResponse,
                string? xFapiInteractionId,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.PostAsync(
                externalApiUrl,
                createParams.ExtraHeaders,
                createParams.Request.ExternalApiRequest,
                tppReportingRequestInfo,
                requestJsonSerializerSettings,
                responseJsonSerializerSettings,
                apiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);
        var externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };

        // No link URLs to transform

        // Create response
        var response =
            new DomesticVrpConsentFundsConfirmationResponse
            {
                ExternalApiResponse = externalApiResponse,
                ExternalApiResponseInfo = externalApiResponseInfo
            };

        return response;
    }

    public async
        Task<(DomesticVrpConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(DomesticVrpConsentRequest request, ConsentCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Determine entity ID
        var entityId = Guid.NewGuid();

        // Create new or use existing external API object
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? externalApiResponse;
        ExternalApiResponseInfo? externalApiResponseInfo;
        string externalApiId;
        if (request.ExternalApiObject is null)
        {
            // Load BankRegistration and related
            (BankRegistrationEntity bankRegistration, string tokenEndpoint,
                    SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);

            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            VariableRecurringPaymentsApi variableRecurringPaymentsApi =
                bankProfile.GetRequiredVariableRecurringPaymentsApi();
            string bankFinancialId =
                bankProfile.VariableRecurringPaymentsApiSettings.FinancialId ?? bankProfile.FinancialId;
            ClientCredentialsGrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour =
                bankProfile.CustomBehaviour?.ClientCredentialsGrantPost;
            DomesticVrpConsentCustomBehaviour? domesticVrpConsentPostCustomBehaviour =
                bankProfile.CustomBehaviour?.DomesticVrpConsent;

            // Get IApiClient
            IApiClient apiClient = bankRegistration.UseSimulatedBank
                ? bankProfile.ReplayApiClient
                : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

            // Get OBSeal key
            OBSealKey obSealKey =
                (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

            // Get client credentials grant access token
            string ccGrantAccessToken =
                await _clientAccessTokenGet.GetAccessToken(
                    ClientCredentialsGrantScope,
                    obSealKey,
                    bankRegistration,
                    externalApiSecret,
                    clientCredentialsGrantPostCustomBehaviour,
                    apiClient,
                    bankProfile.BankProfileEnum);

            // Create new object at external API
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> apiRequests =
                ApiRequests(
                    variableRecurringPaymentsApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    softwareStatement,
                    obSealKey);
            var externalApiUrl = new Uri(variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId);
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest externalApiRequest =
                request.ExternalApiRequest ??
                throw new InvalidOperationException(
                    "ExternalApiRequest specified as null so not possible to create external API request.");
            externalApiRequest = bankProfile.VariableRecurringPaymentsApiSettings
                .DomesticVrpConsentExternalApiRequestAdjustments(externalApiRequest);
            bool preferMisspeltContractPresentIndicator =
                domesticVrpConsentPostCustomBehaviour?.PreferMisspeltContractPresentIndicator ?? false;
            externalApiRequest.Risk.AdjustBeforeSendToBank(preferMisspeltContractPresentIndicator);
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      POST {VrpBaseUrl}{{RelativePathBeforeId}}
                      """,
                BankProfile = bankProfile.BankProfileEnum
            };
            (externalApiResponse, string? xFapiInteractionId,
                    IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.PostAsync(
                    externalApiUrl,
                    createParams.ExtraHeaders,
                    externalApiRequest,
                    tppReportingRequestInfo,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);
            externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
            externalApiId = externalApiResponse.Data.ConsentId;
            externalApiResponse.Risk.AdjustAfterReceiveFromBank();

            // Transform links
            string? transformedLinkUrlWithoutQuery = createParams.PublicRequestUrlWithoutQuery is { } x
                ? $"{x}/{entityId}"
                : null;
            var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
                LinksUrlOperations.PostMethodExpectedLinkUrls(
                    externalApiUrl,
                    externalApiId,
                    domesticVrpConsentPostCustomBehaviour),
                transformedLinkUrlWithoutQuery,
                domesticVrpConsentPostCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
                false);
            externalApiResponse.Links.Self = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Self);
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
        else
        {
            externalApiResponse = null;
            externalApiResponseInfo = null;
            externalApiId = request.ExternalApiObject.ExternalApiId;
        }

        // Create persisted entity and return response
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var persistedConsent = new DomesticVrpConsentPersisted(
            entityId,
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
            null,
            null,
            null,
            utcNow,
            request.CreatedBy,
            request.ExternalApiUserId,
            utcNow,
            request.CreatedBy,
            request.BankRegistrationId,
            externalApiId);

        AuthContextRequest? authContext = request.ExternalApiObject?.AuthContext;
        if (authContext is not null)
        {
            persistedConsent.UpdateAuthContext(
                authContext.State,
                authContext.Nonce,
                authContext.CodeVerifier,
                utcNow,
                authContext.ModifiedBy);
        }

        // Save entity
        await _consentEntityMethods.AddAsync(persistedConsent);

        // Create response (may involve additional processing based on entity)
        var response =
            new DomesticVrpConsentCreateResponse
            {
                Id = persistedConsent.Id,
                Created = persistedConsent.Created,
                CreatedBy = persistedConsent.CreatedBy,
                Reference = persistedConsent.Reference,
                BankRegistrationId = persistedConsent.BankRegistrationId,
                ExternalApiId = persistedConsent.ExternalApiId,
                ExternalApiUserId = persistedConsent.ExternalApiUserId,
                AuthContextModified = persistedConsent.AuthContextModified,
                AuthContextModifiedBy = persistedConsent.AuthContextModifiedBy,
                ExternalApiResponse = externalApiResponse,
                ExternalApiResponseInfo = externalApiResponseInfo
            };

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    public async
        Task<(DomesticVrpConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ConsentReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticVrpConsent and related
        (DomesticVrpConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(readParams.Id, false);
        string externalApiConsentId = persistedConsent.ExternalApiId;

        bool excludeExternalApiOperation =
            readParams.ExcludeExternalApiOperation;
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? externalApiResponse;
        ExternalApiResponseInfo? externalApiResponseInfo;
        if (!excludeExternalApiOperation)
        {
            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            VariableRecurringPaymentsApi variableRecurringPaymentsApi =
                bankProfile.GetRequiredVariableRecurringPaymentsApi();
            string bankFinancialId =
                bankProfile.VariableRecurringPaymentsApiSettings.FinancialId ?? bankProfile.FinancialId;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

            // Get IApiClient
            IApiClient apiClient = bankRegistration.UseSimulatedBank
                ? bankProfile.ReplayApiClient
                : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

            // Get OBSeal key
            OBSealKey obSealKey =
                (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

            // Get client credentials grant access token
            string ccGrantAccessToken =
                await _clientAccessTokenGet.GetAccessToken(
                    ClientCredentialsGrantScope,
                    obSealKey,
                    bankRegistration,
                    externalApiSecret,
                    customBehaviour?.ClientCredentialsGrantPost,
                    apiClient,
                    bankProfile.BankProfileEnum);

            // Read object from external API
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> apiRequests =
                ApiRequests(
                    variableRecurringPaymentsApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    softwareStatement,
                    obSealKey);
            var externalApiUrl = new Uri(
                variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      GET {VrpBaseUrl}{{RelativePathBeforeId}}/{ConsentId}
                      """,
                BankProfile = bankProfile.BankProfileEnum
            };
            (externalApiResponse, string? xFapiInteractionId,
                    IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.GetAsync(
                    externalApiUrl,
                    readParams.ExtraHeaders,
                    tppReportingRequestInfo,
                    responseJsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);
            externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
            externalApiResponse.Risk.AdjustAfterReceiveFromBank();

            // Transform links 
            DomesticVrpConsentCustomBehaviour? readWriteGetCustomBehaviour =
                customBehaviour?.DomesticVrpConsent;
            string? transformedLinkUrlWithoutQuery = readParams.PublicRequestUrlWithoutQuery;
            Uri expectedLinkUrlWithoutQuery = externalApiUrl;
            var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
                LinksUrlOperations.GetMethodExpectedLinkUrls(expectedLinkUrlWithoutQuery, readWriteGetCustomBehaviour),
                transformedLinkUrlWithoutQuery,
                readWriteGetCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
                false);
            externalApiResponse.Links.Self = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Self);
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
        else
        {
            externalApiResponse = null;
            externalApiResponseInfo = null;
        }

        // Create response
        var response =
            new DomesticVrpConsentCreateResponse
            {
                Id = persistedConsent.Id,
                Created = persistedConsent.Created,
                CreatedBy = persistedConsent.CreatedBy,
                Reference = persistedConsent.Reference,
                BankRegistrationId = persistedConsent.BankRegistrationId,
                ExternalApiId = persistedConsent.ExternalApiId,
                ExternalApiUserId = persistedConsent.ExternalApiUserId,
                AuthContextModified = persistedConsent.AuthContextModified,
                AuthContextModifiedBy = persistedConsent.AuthContextModifiedBy,
                ExternalApiResponse = externalApiResponse,
                ExternalApiResponseInfo = externalApiResponseInfo
            };

        return (response, nonErrorMessages);
    }

    private IApiRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> ApiRequests(
        VariableRecurringPaymentsApiVersion variableRecurringPaymentsApiVersion,
        string bankFinancialId,
        string accessToken,
        SoftwareStatementEntity softwareStatement,
        OBSealKey obSealKey) =>
        variableRecurringPaymentsApiVersion switch
        {
            VariableRecurringPaymentsApiVersion.VersionPublic => new ApiRequests<
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken),
                new PaymentInitiationPostRequestProcessor<
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient,
                    softwareStatement,
                    obSealKey)),
            _ => throw new ArgumentOutOfRangeException(
                $"VRP API version {variableRecurringPaymentsApiVersion} not supported.")
        };

    private IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest,
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>
        ApiRequestsFundsConfirmation(
            VariableRecurringPaymentsApiVersion variableRecurringPaymentsApiVersion,
            string bankFinancialId,
            string accessToken,
            SoftwareStatementEntity softwareStatement,
            OBSealKey obSealKey) =>
        variableRecurringPaymentsApiVersion switch
        {
            VariableRecurringPaymentsApiVersion.VersionPublic => new ApiPostRequests<
                VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest,
                VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse,
                VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest,
                VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>(
                new PaymentInitiationPostRequestProcessor<
                    VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient,
                    softwareStatement,
                    obSealKey)),
            _ => throw new ArgumentOutOfRangeException(
                $"VRP API version {variableRecurringPaymentsApiVersion} not supported.")
        };
}
