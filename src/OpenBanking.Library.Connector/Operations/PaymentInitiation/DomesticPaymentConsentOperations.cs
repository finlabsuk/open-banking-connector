// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
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
using Newtonsoft.Json;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

internal class
    DomesticPaymentConsentOperations :
        IObjectCreate<DomesticPaymentConsentRequest, DomesticPaymentConsentCreateResponse,
            ConsentCreateParams>,
        IObjectRead<DomesticPaymentConsentReadResponse, ConsentReadParams>,
        IObjectReadFundsConfirmation<DomesticPaymentConsentReadFundsConfirmationResponse, ConsentBaseReadParams>
{
    private readonly IDbReadOnlyEntityMethods<PaymentInitiationApiEntity> _apiEntityMethods;
    private readonly AuthContextAccessTokenGet _authContextAccessTokenGet;

    private readonly IBankProfileService _bankProfileService;

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
        IDbReadOnlyEntityMethods<PaymentInitiationApiEntity> apiEntityMethods,
        IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods,
        IBankProfileService bankProfileService,
        AuthContextAccessTokenGet authContextAccessTokenGet)
    {
        _apiEntityMethods = apiEntityMethods;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _authContextAccessTokenGet = authContextAccessTokenGet;
        _entityMethods = entityMethods;
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
        Task<(DomesticPaymentConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(DomesticPaymentConsentRequest request, ConsentCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Resolve bank profile
        BankProfile? bankProfile = null;
        if (request.BankProfile is not null)
        {
            bankProfile = _bankProfileService.GetBankProfile(request.BankProfile.Value);
        }

        // Determine entity ID
        var entityId = Guid.NewGuid();

        // Create new or use existing external API object
        PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5? externalApiResponse;
        string externalApiId;
        if (request.ExternalApiObject is null)
        {
            // Load BankRegistration and related
            (BankRegistration bankRegistration, string bankFinancialId, string tokenEndpoint,
                    ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);

            // Load PaymentInitiationApi
            PaymentInitiationApiEntity paymentInitiationApi =
                await GetPaymentInitiationApi(request.PaymentInitiationApiId, bankRegistration.BankId);

            // Get client credentials grant access token
            string ccGrantAccessToken =
                (await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    tokenEndpoint,
                    null,
                    processedSoftwareStatementProfile.ApiClient,
                    _instrumentationClient))
                .AccessToken;

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
            externalApiResponse.Links.Self = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Self);
            externalApiResponse.Links.First = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.First);
            externalApiResponse.Links.Prev = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Prev);
            externalApiResponse.Links.Next = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Next);
            externalApiResponse.Links.Last = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Last);
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
            request.PaymentInitiationApiId);

        AccessToken? accessToken = request.ExternalApiObject?.AccessToken;
        if (accessToken is not null)
        {
            persistedConsent.UpdateAccessToken(
                accessToken.Token,
                accessToken.ExpiresIn,
                accessToken.RefreshToken,
                utcNow,
                request.CreatedBy);
        }

        // Save entity
        await _entityMethods.AddAsync(persistedConsent);

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        // Create response (may involve additional processing based on entity)
        var response =
            new DomesticPaymentConsentCreateResponse(
                persistedConsent.Id,
                persistedConsent.Created,
                persistedConsent.CreatedBy,
                persistedConsent.Reference,
                null,
                persistedConsent.BankRegistrationId,
                persistedConsent.PaymentInitiationApiId,
                persistedConsent.ExternalApiId,
                externalApiResponse);

        return (response, nonErrorMessages);
    }

    public async
        Task<(DomesticPaymentConsentReadResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ConsentReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticPaymentConsent and related
        (DomesticPaymentConsentPersisted persistedObject, string externalApiConsentId,
                PaymentInitiationApiEntity paymentInitiationApi, BankRegistration bankRegistration,
                string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(readParams.Id);

        // Get client credentials grant access token
        string ccGrantAccessToken =
            (await _grantPost.PostClientCredentialsGrantAsync(
                ClientCredentialsGrantScope,
                processedSoftwareStatementProfile,
                bankRegistration,
                bankRegistration.BankNavigation.TokenEndpoint,
                null,
                processedSoftwareStatementProfile.ApiClient,
                _instrumentationClient))
            .AccessToken;

        // Read object from external API
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        IApiGetRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> apiRequests =
            ApiRequests(
                paymentInitiationApi.ApiVersion,
                bankFinancialId,
                ccGrantAccessToken,
                processedSoftwareStatementProfile);
        var externalApiUrl = new Uri(paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
        (PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 externalApiResponse,
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
        externalApiResponse.Links.Self = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Self);
        externalApiResponse.Links.First = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.First);
        externalApiResponse.Links.Prev = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Prev);
        externalApiResponse.Links.Next = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Next);
        externalApiResponse.Links.Last = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Last);

        // Create response
        var response =
            new DomesticPaymentConsentReadResponse(
                persistedObject.Id,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.Reference,
                null,
                persistedObject.BankRegistrationId,
                persistedObject.PaymentInitiationApiId,
                persistedObject.ExternalApiId,
                externalApiResponse);

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
        (DomesticPaymentConsentPersisted persistedObject, string bankApiId,
                PaymentInitiationApiEntity paymentInitiationApi, BankRegistration bankRegistration,
                string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(readParams.Id);

        // Get access token
        string bankIssuerUrl =
            persistedObject.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                ?.DomesticPaymentConsentAuthGet
                ?.AudClaim ??
            bankRegistration.BankNavigation.IssuerUrl;
        string accessToken =
            await _authContextAccessTokenGet.GetAccessTokenAndUpdateConsent(
                persistedObject,
                bankIssuerUrl,
                "openid payments",
                bankRegistration,
                persistedObject.BankRegistrationNavigation.BankNavigation.TokenEndpoint,
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
            paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{bankApiId}" + "/funds-confirmation");
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
            new DomesticPaymentConsentReadFundsConfirmationResponse(
                persistedObject.Id,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.Reference,
                null,
                persistedObject.BankRegistrationId,
                persistedObject.PaymentInitiationApiId,
                persistedObject.ExternalApiId,
                externalApiResponse);

        return (response, nonErrorMessages);
    }

    private async Task<PaymentInitiationApiEntity> GetPaymentInitiationApi(
        Guid paymentInitiationApiId,
        Guid bankId)
    {
        PaymentInitiationApiEntity paymentInitiationApi =
            await _apiEntityMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == paymentInitiationApiId) ??
            throw new KeyNotFoundException(
                $"No record found for PaymentInitiationApi {paymentInitiationApiId} specified by request.");

        if (paymentInitiationApi.BankId != bankId)
        {
            throw new ArgumentException(
                "Specified PaymentInitiationApi and BankRegistration objects do not share same BankId.");
        }

        return paymentInitiationApi;
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
