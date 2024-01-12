// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
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
using Newtonsoft.Json;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

internal class
    DomesticPaymentConsentOperations :
    IObjectCreate<DomesticPaymentConsentRequest, DomesticPaymentConsentCreateResponse,
        ConsentCreateParams>,
    IObjectRead<DomesticPaymentConsentCreateResponse, ConsentReadParams>,
    IObjectReadFundsConfirmation<DomesticPaymentConsentReadFundsConfirmationResponse, ConsentBaseReadParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;

    private readonly ConsentCommon<DomesticPaymentConsentPersisted,
        DomesticPaymentConsentRequest,
        DomesticPaymentConsentCreateResponse,
        PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
        PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> _consentCommon;

    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly DomesticPaymentConsentCommon _domesticPaymentConsentCommon;
    private readonly IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> _entityMethods;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ITimeProvider _timeProvider;

    public DomesticPaymentConsentOperations(
        IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IGrantPost grantPost,
        IBankProfileService bankProfileService,
        ConsentAccessTokenGet consentAccessTokenGet,
        IDbReadOnlyEntityMethods<BankRegistrationEntity> bankRegistrationMethods)
    {
        _entityMethods = entityMethods;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _consentAccessTokenGet = consentAccessTokenGet;
        _domesticPaymentConsentCommon = new DomesticPaymentConsentCommon(
            entityMethods,
            instrumentationClient,
            softwareStatementProfileRepo);
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
                instrumentationClient,
                softwareStatementProfileRepo);
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
        string externalApiId;
        if (request.ExternalApiObject is null)
        {
            // Load BankRegistration and related
            (BankRegistrationEntity bankRegistration, string tokenEndpoint,
                    ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);

            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            PaymentInitiationApi paymentInitiationApi = bankProfile.GetRequiredPaymentInitiationApi();
            TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
                bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
            bool supportsSca = bankProfile.SupportsSca;
            string bankFinancialId = bankProfile.FinancialId;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

            // Get client credentials grant access token
            string ccGrantAccessToken =
                await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile.OBSealKey,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    bankRegistration.ExternalApiId,
                    bankRegistration.ExternalApiSecret,
                    bankRegistration.Id.ToString(),
                    null,
                    customBehaviour?.ClientCredentialsGrantPost,
                    processedSoftwareStatementProfile.ApiClient);

            // Create new object at external API
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiPostRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> apiRequests =
                ApiRequests(
                    paymentInitiationApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            var externalApiUrl = new Uri(paymentInitiationApi.BaseUrl + RelativePathBeforeId);
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4 externalApiRequest =
                DomesticPaymentConsentPublicMethods.ResolveExternalApiRequest(
                    request.ExternalApiRequest,
                    request.TemplateRequest,
                    bankProfile);
            (externalApiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.PostAsync(
                    externalApiUrl,
                    externalApiRequest,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Transform links
            externalApiId = externalApiResponse.Data.ConsentId;
            var apiGetRequestUrl = new Uri(externalApiUrl + $"/{externalApiId}");
            string? publicGetRequestUrlWithoutQuery = createParams.PublicRequestUrlWithoutQuery switch
            {
                { } x => x + $"/{entityId}",
                null => null
            };
            var validQueryParameters = new List<string>();
            var linksUrlOperations = new LinksUrlOperations(
                apiGetRequestUrl,
                publicGetRequestUrlWithoutQuery,
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
        var persistedConsent = new DomesticPaymentConsentPersisted(
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
        await _entityMethods.AddAsync(persistedConsent);

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
                ExternalApiResponse = externalApiResponse
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
        (DomesticPaymentConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration, _, _,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(readParams.Id, false);

        bool includeExternalApiOperation =
            readParams.IncludeExternalApiOperation;
        PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5? externalApiResponse;
        if (includeExternalApiOperation)
        {
            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            PaymentInitiationApi paymentInitiationApi = bankProfile.GetRequiredPaymentInitiationApi();
            TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
                bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
            bool supportsSca = bankProfile.SupportsSca;
            string bankFinancialId = bankProfile.FinancialId;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

            // Get client credentials grant access token
            string ccGrantAccessToken =
                await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile.OBSealKey,
                    tokenEndpointAuthMethod,
                    bankRegistration.TokenEndpoint,
                    bankRegistration.ExternalApiId,
                    bankRegistration.ExternalApiSecret,
                    bankRegistration.Id.ToString(),
                    null,
                    customBehaviour?.ClientCredentialsGrantPost,
                    processedSoftwareStatementProfile.ApiClient);

            // Read object from external API
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiGetRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> apiRequests =
                ApiRequests(
                    paymentInitiationApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            string externalApiConsentId = persistedConsent.ExternalApiId;
            var externalApiUrl = new Uri(
                paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
            (externalApiResponse,
                    IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.GetAsync(
                    externalApiUrl,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
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
                ExternalApiResponse = externalApiResponse
            };

        return (response, nonErrorMessages);
    }

    public async
        Task<(DomesticPaymentConsentReadFundsConfirmationResponse response,
            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadFundsConfirmationAsync(
            ConsentBaseReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticPaymentConsent and related
        (DomesticPaymentConsentPersisted persistedObject, BankRegistrationEntity bankRegistration,
                DomesticPaymentConsentAccessToken? storedAccessToken,
                DomesticPaymentConsentRefreshToken? storedRefreshToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(readParams.Id, true);
        string externalApiConsentId = persistedObject.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        PaymentInitiationApi paymentInitiationApi = bankProfile.GetRequiredPaymentInitiationApi();
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        bool supportsSca = bankProfile.SupportsSca;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        string bankFinancialId = bankProfile.FinancialId;
        string issuerUrl = bankProfile.IssuerUrl;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;

        // Get access token
        string bankTokenIssuerClaim = DomesticPaymentConsentCommon.GetBankTokenIssuerClaim(
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
                supportsSca,
                idTokenSubClaimType,
                customBehaviour?.RefreshTokenGrantPost,
                customBehaviour?.JwksGet,
                readParams.ModifiedBy);

        // Read object from external API
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        IApiGetRequests<PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1> apiRequests =
            ApiRequestsFundsConfirmation(
                paymentInitiationApi.ApiVersion,
                bankFinancialId,
                accessToken,
                processedSoftwareStatementProfile);
        var externalApiUrl = new Uri(
            paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}" + "/funds-confirmation");
        (PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 externalApiResponse,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.GetAsync(
                externalApiUrl,
                responseJsonSerializerSettings,
                processedSoftwareStatementProfile.ApiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Create response
        var response =
            new DomesticPaymentConsentReadFundsConfirmationResponse
            {
                Id = persistedObject.Id,
                Created = persistedObject.Created,
                CreatedBy = persistedObject.CreatedBy,
                Reference = persistedObject.Reference,
                BankRegistrationId = persistedObject.BankRegistrationId,
                ExternalApiId = persistedObject.ExternalApiId,
                ExternalApiUserId = persistedObject.ExternalApiUserId,
                AuthContextModified = persistedObject.AuthContextModified,
                AuthContextModifiedBy = persistedObject.AuthContextModifiedBy,
                ExternalApiResponse = externalApiResponse
            };

        return (response, nonErrorMessages);
    }

    private IApiRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
        PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> ApiRequests(
        PaymentInitiationApiVersion paymentInitiationApiVersion,
        string bankFinancialId,
        string accessToken,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =>
        paymentInitiationApiVersion switch
        {
            PaymentInitiationApiVersion.Version3p1p4 => new ApiRequests<
                PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4,
                PaymentInitiationModelsV3p1p4.OBWriteDomesticConsentResponse4>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken),
                new PaymentInitiationPostRequestProcessor<
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient,
                    paymentInitiationApiVersion < PaymentInitiationApiVersion.Version3p1p4,
                    processedSoftwareStatementProfile)),
            PaymentInitiationApiVersion.Version3p1p6 => new ApiRequests<
                PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken),
                new PaymentInitiationPostRequestProcessor<
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient,
                    paymentInitiationApiVersion < PaymentInitiationApiVersion.Version3p1p4,
                    processedSoftwareStatementProfile)),
            _ => throw new ArgumentOutOfRangeException($"PISP API version {paymentInitiationApiVersion} not supported.")
        };

    private IApiGetRequests<PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1>
        ApiRequestsFundsConfirmation(
            PaymentInitiationApiVersion paymentInitiationApiVersion,
            string bankFinancialId,
            string accessToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =>
        paymentInitiationApiVersion switch
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
            _ => throw new ArgumentOutOfRangeException($"PISP API version {paymentInitiationApiVersion} not supported.")
        };
}
