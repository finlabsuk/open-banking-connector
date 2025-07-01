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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

internal class
    DomesticPaymentConsentOperations :
    IObjectCreate<DomesticPaymentConsentRequest, DomesticPaymentConsentCreateResponse,
        ConsentCreateParams>,
    IObjectRead<DomesticPaymentConsentCreateResponse, ConsentReadParams>,
    IReadFundsConfirmationContext<DomesticPaymentConsentFundsConfirmationResponse, ConsentBaseReadParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly ClientAccessTokenGet _clientAccessTokenGet;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;

    private readonly ConsentCommon<DomesticPaymentConsentPersisted,
        DomesticPaymentConsentRequest,
        DomesticPaymentConsentCreateResponse,
        PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
        PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> _consentCommon;

    private readonly IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> _consentEntityMethods;
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly DomesticPaymentConsentCommon _domesticPaymentConsentCommon;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;
    private readonly ITimeProvider _timeProvider;

    public DomesticPaymentConsentOperations(
        IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> consentEntityMethods,
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
        DomesticPaymentConsentCommon domesticPaymentConsentCommon)
    {
        _consentEntityMethods = consentEntityMethods;
        _bankProfileService = bankProfileService;
        _consentAccessTokenGet = consentAccessTokenGet;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _clientAccessTokenGet = clientAccessTokenGet;
        _domesticPaymentConsentCommon = domesticPaymentConsentCommon;
        _mapper = mapper;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _consentCommon =
            new ConsentCommon<DomesticPaymentConsentPersisted, DomesticPaymentConsentRequest,
                DomesticPaymentConsentCreateResponse,
                PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                bankRegistrationMethods,
                instrumentationClient);
    }

    private string ClientCredentialsGrantScope => "payments";

    private string RelativePathBeforeId => "/domestic-payment-consents";

    public async
        Task<(DomesticPaymentConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
            )>
        CreateAsync(DomesticPaymentConsentRequest request, ConsentCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Determine entity ID
        var entityId = Guid.NewGuid();

        // Create new or use existing external API object
        PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5? externalApiResponse;
        ExternalApiResponseInfo? externalApiResponseInfo;
        string externalApiId;
        bool pispUseV4;
        if (request.ExternalApiObject is null)
        {
            // Load BankRegistration and related
            (BankRegistrationEntity bankRegistration, string tokenEndpoint,
                    SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);
            pispUseV4 = bankRegistration.PispUseV4;

            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            PaymentInitiationApi paymentInitiationApi = bankProfile.GetRequiredPaymentInitiationApi(pispUseV4);
            string bankFinancialId = bankProfile.PaymentInitiationApiSettings.FinancialId ?? bankProfile.FinancialId;
            ClientCredentialsGrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour =
                bankProfile.CustomBehaviour?.ClientCredentialsGrantPost;
            DomesticPaymentConsentCustomBehaviour? readWritePostCustomBehaviour =
                bankProfile.CustomBehaviour?.DomesticPaymentConsent;

            // Get IApiClient
            IApiClient apiClient =
                (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

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
            var externalApiUrl = new Uri(paymentInitiationApi.BaseUrl + RelativePathBeforeId);
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4 externalApiRequest = request.ExternalApiRequest ??
                throw new InvalidOperationException(
                    "ExternalApiRequest specified as null so not possible to create external API request.");
            externalApiRequest = bankProfile.PaymentInitiationApiSettings
                .DomesticPaymentConsentExternalApiRequestAdjustments(externalApiRequest);
            if (externalApiRequest.Risk.ContractPresentInidicator is not null)
            {
                throw new ArgumentException(
                    "ExternalApiRequest contains mis-spelt field Risk/ContractPresentInidicator.");
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
            string? xFapiInteractionId;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            switch (paymentInitiationApi.ApiVersion)
            {
                case PaymentInitiationApiVersion.Version3p1p11:
                    PaymentInitiationModelsV3p1p11.OBWriteDomesticConsent4 externalApiRequestV3 =
                        PaymentInitiationModelsPublic.Mappings.MapFromOBWriteDomesticConsent4(externalApiRequest);
                    var apiRequestsV3 =
                        new ApiRequests<PaymentInitiationModelsV3p1p11.OBWriteDomesticConsent4,
                            PaymentInitiationModelsV3p1p11.OBWriteDomesticConsentResponse5,
                            PaymentInitiationModelsV3p1p11.OBWriteDomesticConsent4,
                            PaymentInitiationModelsV3p1p11.OBWriteDomesticConsentResponse5>(
                            new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                            new PaymentInitiationPostRequestProcessor<
                                PaymentInitiationModelsV3p1p11.OBWriteDomesticConsent4>(
                                bankFinancialId,
                                ccGrantAccessToken,
                                _instrumentationClient,
                                softwareStatement,
                                obSealKey));
                    (PaymentInitiationModelsV3p1p11.OBWriteDomesticConsentResponse5 externalApiResponseV3,
                            xFapiInteractionId, newNonErrorMessages) =
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
                        PaymentInitiationModelsPublic.Mappings.MapToOBWriteDomesticConsentResponse5(
                            externalApiResponseV3);
                    break;
                case PaymentInitiationApiVersion.VersionPublic:
                    var apiRequests =
                        new ApiRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                            PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                            new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                            new PaymentInitiationPostRequestProcessor<
                                PaymentInitiationModelsPublic.OBWriteDomesticConsent4>(
                                bankFinancialId,
                                ccGrantAccessToken,
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
                        $"PISP API version {paymentInitiationApi.ApiVersion} not supported.");
            }
            nonErrorMessages.AddRange(newNonErrorMessages);
            externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
            externalApiId = externalApiResponse.Data.ConsentId;
            if (!pispUseV4)
            {
                externalApiResponse.Risk.AdjustAfterReceiveFromBank();
            }

            // Transform links
            if (externalApiResponse.Links is not null)
            {
                string? transformedLinkUrlWithoutQuery = createParams.PublicRequestUrlWithoutQuery is { } x
                    ? $"{x}/{entityId}"
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
        }
        else
        {
            externalApiResponse = null;
            externalApiResponseInfo = null;
            externalApiId = request.ExternalApiObject.ExternalApiId;
            pispUseV4 = false;
        }

        // Create persisted entity and return response
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var persistedConsent = new DomesticPaymentConsentPersisted(
            entityId,
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            null,
            null,
            null,
            utcNow,
            request.CreatedBy,
            request.ExternalApiUserId,
            utcNow,
            request.CreatedBy,
            request.BankRegistrationId,
            externalApiId,
            pispUseV4);

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
            new DomesticPaymentConsentCreateResponse
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
        Task<(DomesticPaymentConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
            )>
        ReadAsync(ConsentReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticPaymentConsent and related
        (DomesticPaymentConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(readParams.Id, false);
        string externalApiConsentId = persistedConsent.ExternalApiId;
        bool pispUseV4 = persistedConsent.CreatedWithV4;

        bool excludeExternalApiOperation =
            readParams.ExcludeExternalApiOperation;
        PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5? externalApiResponse;
        ExternalApiResponseInfo? externalApiResponseInfo;
        if (!excludeExternalApiOperation)
        {
            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            PaymentInitiationApi paymentInitiationApi = bankProfile.GetRequiredPaymentInitiationApi(pispUseV4);
            string bankFinancialId = bankProfile.PaymentInitiationApiSettings.FinancialId ?? bankProfile.FinancialId;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
            DomesticPaymentConsentCustomBehaviour? readWriteGetCustomBehaviour =
                customBehaviour?.DomesticPaymentConsent;

            // Get IApiClient
            IApiClient apiClient =
                (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

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
            var externalApiUrl = new Uri(
                paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      GET {PispBaseUrl}{{RelativePathBeforeId}}/{ConsentId}
                      """,
                BankProfile = bankProfile.BankProfileEnum
            };
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            string? xFapiInteractionId;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            switch (paymentInitiationApi.ApiVersion)
            {
                case PaymentInitiationApiVersion.Version3p1p11:
                    var apiRequestsV3 =
                        new ApiRequests<PaymentInitiationModelsV3p1p11.OBWriteDomesticConsent4,
                            PaymentInitiationModelsV3p1p11.OBWriteDomesticConsentResponse5,
                            PaymentInitiationModelsV3p1p11.OBWriteDomesticConsent4,
                            PaymentInitiationModelsV3p1p11.OBWriteDomesticConsentResponse5>(
                            new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                            new PaymentInitiationPostRequestProcessor<
                                PaymentInitiationModelsV3p1p11.OBWriteDomesticConsent4>(
                                bankFinancialId,
                                ccGrantAccessToken,
                                _instrumentationClient,
                                softwareStatement,
                                obSealKey));
                    (PaymentInitiationModelsV3p1p11.OBWriteDomesticConsentResponse5 externalApiResponseV3,
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
                        PaymentInitiationModelsPublic.Mappings.MapToOBWriteDomesticConsentResponse5(
                            externalApiResponseV3);
                    break;
                case PaymentInitiationApiVersion.VersionPublic:
                    var apiRequests =
                        new ApiRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                            PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                            new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                            new PaymentInitiationPostRequestProcessor<
                                PaymentInitiationModelsPublic.OBWriteDomesticConsent4>(
                                bankFinancialId,
                                ccGrantAccessToken,
                                _instrumentationClient,
                                softwareStatement,
                                obSealKey));
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
            externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
            if (!pispUseV4)
            {
                externalApiResponse.Risk.AdjustAfterReceiveFromBank();
            }

            // Transform links 
            if (externalApiResponse.Links is not null)
            {
                string? transformedLinkUrlWithoutQuery = readParams.PublicRequestUrlWithoutQuery;
                Uri expectedLinkUrlWithoutQuery = externalApiUrl;
                var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
                    LinksUrlOperations.GetMethodExpectedLinkUrls(
                        expectedLinkUrlWithoutQuery,
                        readWriteGetCustomBehaviour),
                    transformedLinkUrlWithoutQuery,
                    readWriteGetCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
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
        }
        else
        {
            externalApiResponse = null;
            externalApiResponseInfo = null;
        }

        // Create response
        var response =
            new DomesticPaymentConsentCreateResponse
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

    public async
        Task<DomesticPaymentConsentFundsConfirmationResponse> ReadFundsConfirmationAsync(
            ConsentBaseReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticPaymentConsent and related
        (DomesticPaymentConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(readParams.Id, true);
        string externalApiConsentId = persistedConsent.ExternalApiId;
        bool pispUseV4 = persistedConsent.CreatedWithV4;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        PaymentInitiationApi paymentInitiationApi = bankProfile.GetRequiredPaymentInitiationApi(pispUseV4);
        bool supportsSca = bankProfile.SupportsSca;
        string bankFinancialId = bankProfile.PaymentInitiationApiSettings.FinancialId ?? bankProfile.FinancialId;
        string issuerUrl = bankProfile.IssuerUrl;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;
        DomesticPaymentConsentCustomBehaviour? readWriteGetCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticPaymentConsent;
        RefreshTokenGrantPostCustomBehaviour? domesticPaymentConsentRefreshTokenGrantPostCustomBehaviour =
            bankProfile.CustomBehaviour?.DomesticPaymentConsentRefreshTokenGrantPost;
        JwksGetCustomBehaviour? jwksGetCustomBehaviour = bankProfile.CustomBehaviour?.JwksGet;
        ConsentAuthGetCustomBehaviour? domesticPaymentConsentAuthGetCustomBehaviour = bankProfile.CustomBehaviour
            ?.DomesticPaymentConsentAuthGet;

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
                externalApiSecret,
                bankRegistration.TokenEndpoint,
                bankProfile.UseOpenIdConnect,
                apiClient,
                obSealKey,
                supportsSca,
                bankProfile.BankProfileEnum,
                idTokenSubClaimType,
                domesticPaymentConsentRefreshTokenGrantPostCustomBehaviour,
                jwksGetCustomBehaviour,
                readParams.ModifiedBy);

        // Read object from external API
        var externalApiUrl = new Uri(
            paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}" + "/funds-confirmation");
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  GET {PispBaseUrl}{{RelativePathBeforeId}}/{ConsentId}/funds-confirmation
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 externalApiResponse;
        string? xFapiInteractionId;
        IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
        switch (paymentInitiationApi.ApiVersion)
        {
            case PaymentInitiationApiVersion.Version3p1p11:
                var apiRequestsV3 =
                    new ApiGetRequests<PaymentInitiationModelsV3p1p11.OBWriteFundsConfirmationResponse1,
                        PaymentInitiationModelsV3p1p11.OBWriteFundsConfirmationResponse1>(
                        new ApiGetRequestProcessor(bankFinancialId, accessToken));
                (PaymentInitiationModelsV3p1p11.OBWriteFundsConfirmationResponse1 externalApiResponseV3,
                        xFapiInteractionId, newNonErrorMessages) =
                    await apiRequestsV3.GetAsync(
                        externalApiUrl,
                        readParams.ExtraHeaders,
                        tppReportingRequestInfo,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);
                externalApiResponse =
                    PaymentInitiationModelsPublic.Mappings.MapToOBWriteFundsConfirmationResponse1(
                        externalApiResponseV3);
                break;
            case PaymentInitiationApiVersion.VersionPublic:
                var apiRequests =
                    new ApiGetRequests<PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1,
                        PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1>(
                        new ApiGetRequestProcessor(bankFinancialId, accessToken));
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
                LinksUrlOperations.GetMethodExpectedLinkUrls(expectedLinkUrlWithoutQuery, readWriteGetCustomBehaviour),
                transformedLinkUrlWithoutQuery,
                readWriteGetCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
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
        var response =
            new DomesticPaymentConsentFundsConfirmationResponse
            {
                ExternalApiResponse = externalApiResponse,
                ExternalApiResponseInfo = externalApiResponseInfo
            };

        return response;
    }
}
