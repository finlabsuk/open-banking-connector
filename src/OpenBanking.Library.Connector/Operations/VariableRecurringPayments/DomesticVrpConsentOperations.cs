// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

internal class
    DomesticVrpConsentOperations :
    IObjectCreate<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse, ConsentCreateParams>,
    IObjectRead<DomesticVrpConsentCreateResponse, ConsentReadParams>,
    IVrpConsentFundsConfirmationCreate<DomesticVrpConsentFundsConfirmationRequest,
        DomesticVrpConsentFundsConfirmationResponse>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;

    private readonly ConsentCommon<DomesticVrpConsentPersisted,
        DomesticVrpConsentRequest,
        DomesticVrpConsentCreateResponse,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> _consentCommon;

    private readonly IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> _consentEntityMethods;

    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly DomesticVrpConsentCommon _domesticVrpConsentCommon;
    private readonly IGrantPost _grantPost;
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
        IGrantPost grantPost,
        IBankProfileService bankProfileService,
        ConsentAccessTokenGet consentAccessTokenGet,
        IDbReadOnlyEntityMethods<BankRegistrationEntity> bankRegistrationMethods,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods)
    {
        _consentEntityMethods = entityMethods;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _consentAccessTokenGet = consentAccessTokenGet;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _domesticVrpConsentCommon = new DomesticVrpConsentCommon(
            entityMethods,
            instrumentationClient);
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
        string externalApiId;
        if (request.ExternalApiObject is null)
        {
            // Load BankRegistration and related
            (BankRegistrationEntity bankRegistration, string tokenEndpoint,
                    SoftwareStatementEntity softwareStatement) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);

            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            VariableRecurringPaymentsApi variableRecurringPaymentsApi =
                bankProfile.GetRequiredVariableRecurringPaymentsApi();
            TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
                bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
            string bankFinancialId = bankProfile.FinancialId;

            // Get IApiClient
            IApiClient apiClient = bankRegistration.UseSimulatedBank
                ? bankProfile.ReplayApiClient
                : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

            // Get OBSeal key
            OBSealKey obSealKey =
                (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

            // Get client credentials grant access token
            string ccGrantAccessToken =
                await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    obSealKey,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    bankRegistration.ExternalApiId,
                    bankRegistration.ExternalApiSecret,
                    bankRegistration.Id.ToString(),
                    null,
                    customBehaviour?.ClientCredentialsGrantPost,
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
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      POST {VrpBaseUrl}{{RelativePathBeforeId}}
                      """,
                BankProfile = bankProfile.BankProfileEnum
            };
            (externalApiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.PostAsync(
                    externalApiUrl,
                    request.ExternalApiRequest,
                    tppReportingRequestInfo,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);
            externalApiId = externalApiResponse.Data.ConsentId;

            // Transform links
            var expectedLinkUrlWithoutQuery = new Uri(externalApiUrl + $"/{externalApiId}");
            string? transformedLinkUrlWithoutQuery = createParams.PublicRequestUrlWithoutQuery switch
            {
                { } x => x + $"/{entityId}",
                null => null
            };
            var validQueryParameters = new List<string>();
            var linksUrlOperations = new LinksUrlOperations(
                expectedLinkUrlWithoutQuery,
                transformedLinkUrlWithoutQuery,
                true,
                validQueryParameters);
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
            request.BankRegistrationId,
            externalApiId,
            null,
            null,
            utcNow,
            request.CreatedBy,
            request.ExternalApiUserId,
            utcNow,
            request.CreatedBy);

        AuthContextRequest? authContext = request.ExternalApiObject?.AuthContext;
        if (authContext is not null)
        {
            persistedConsent.UpdateAuthContext(
                authContext.State,
                authContext.Nonce,
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
                ExternalApiResponse = externalApiResponse
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
        (DomesticVrpConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration, _, _,
                SoftwareStatementEntity softwareStatement) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(readParams.Id, false);
        string externalApiConsentId = persistedConsent.ExternalApiId;

        bool includeExternalApiOperation =
            readParams.IncludeExternalApiOperation;
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? externalApiResponse;
        if (includeExternalApiOperation)
        {
            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            VariableRecurringPaymentsApi variableRecurringPaymentsApi =
                bankProfile.GetRequiredVariableRecurringPaymentsApi();
            TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
                bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
            string bankFinancialId = bankProfile.FinancialId;
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
                await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    obSealKey,
                    tokenEndpointAuthMethod,
                    bankRegistration.TokenEndpoint,
                    bankRegistration.ExternalApiId,
                    bankRegistration.ExternalApiSecret,
                    bankRegistration.Id.ToString(),
                    null,
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
            (externalApiResponse,
                    IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.GetAsync(
                    externalApiUrl,
                    tppReportingRequestInfo,
                    responseJsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Transform links 
            var validQueryParameters = new List<string>();
            var linksUrlOperations = new LinksUrlOperations(
                externalApiUrl,
                readParams.PublicRequestUrlWithoutQuery,
                true,
                validQueryParameters);
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
                ExternalApiResponse = externalApiResponse
            };

        return (response, nonErrorMessages);
    }

    public async
        Task<(DomesticVrpConsentFundsConfirmationResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)> CreateFundsConfirmationAsync(
            DomesticVrpConsentFundsConfirmationRequest request,
            VrpConsentFundsConfirmationCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticVrpConsent and related
        (DomesticVrpConsentPersisted persistedObject, BankRegistrationEntity bankRegistration,
                DomesticVrpConsentAccessToken? storedAccessToken, DomesticVrpConsentRefreshToken? storedRefreshToken,
                SoftwareStatementEntity softwareStatement) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(createParams.Id, true);
        string externalApiConsentId = persistedObject.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        bool supportsSca = bankProfile.SupportsSca;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        string bankFinancialId = bankProfile.FinancialId;
        string issuerUrl = bankProfile.IssuerUrl;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;

        // Get IApiClient
        IApiClient apiClient = bankRegistration.UseSimulatedBank
            ? bankProfile.ReplayApiClient
            : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Get access token
        string bankTokenIssuerClaim = DomesticVrpConsentCommon.GetBankTokenIssuerClaim(
            customBehaviour,
            issuerUrl); // Get bank token issuer ("iss") claim
        string accessToken =
            await _consentAccessTokenGet.GetAccessTokenAndUpdateConsent(
                persistedObject,
                bankTokenIssuerClaim,
                "openid payments",
                bankRegistration,
                storedAccessToken,
                storedRefreshToken,
                tokenEndpointAuthMethod,
                persistedObject.BankRegistrationNavigation.TokenEndpoint,
                apiClient,
                obSealKey,
                supportsSca,
                bankProfile.BankProfileEnum,
                idTokenSubClaimType,
                customBehaviour?.RefreshTokenGrantPost,
                customBehaviour?.JwksGet,
                request.ModifiedBy);

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
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                $$"""
                  POST {VrpBaseUrl}{{RelativePathBeforeId}}/{ConsentId}/funds-confirmation
                  """,
            BankProfile = bankProfile.BankProfileEnum
        };


        (VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse externalApiResponse,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.PostAsync(
                externalApiUrl,
                request.ExternalApiRequest,
                tppReportingRequestInfo,
                requestJsonSerializerSettings,
                responseJsonSerializerSettings,
                apiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Create response
        var response =
            new DomesticVrpConsentFundsConfirmationResponse { ExternalApiResponse = externalApiResponse };

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
