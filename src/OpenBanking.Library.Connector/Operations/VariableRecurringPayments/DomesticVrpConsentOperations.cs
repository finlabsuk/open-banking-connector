// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

internal class
    DomesticVrpConsentOperations :
        IObjectCreate<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse, ConsentCreateParams>,
        IObjectRead<DomesticVrpConsentReadResponse, ConsentReadParams>,
        IObjectReadFundsConfirmation<DomesticVrpConsentReadFundsConfirmationResponse,
            ConsentBaseReadParams>
{
    private readonly IDbReadOnlyEntityMethods<VariableRecurringPaymentsApiEntity> _apiEntityMethods;
    private readonly AuthContextAccessTokenGet _authContextAccessTokenGet;

    private readonly IBankProfileService _bankProfileService;

    private readonly ConsentCommon<DomesticVrpConsentPersisted,
        DomesticVrpConsentRequest,
        DomesticVrpConsentCreateResponse,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> _consentCommon;

    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly DomesticVrpConsentCommon _domesticVrpConsentCommon;
    private readonly IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> _entityMethods;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ITimeProvider _timeProvider;

    public DomesticVrpConsentOperations(
        IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IGrantPost grantPost,
        IDbReadOnlyEntityMethods<VariableRecurringPaymentsApiEntity> apiEntityMethods,
        IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods,
        IBankProfileService bankProfileService,
        AuthContextAccessTokenGet authContextAccessTokenGet)
    {
        _apiEntityMethods = apiEntityMethods;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _authContextAccessTokenGet = authContextAccessTokenGet;
        _entityMethods = entityMethods;
        _domesticVrpConsentCommon = new DomesticVrpConsentCommon(
            entityMethods,
            instrumentationClient,
            softwareStatementProfileRepo);
        _mapper = mapper;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _consentCommon =
            new ConsentCommon<DomesticVrpConsentPersisted, DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>(
                bankRegistrationMethods,
                instrumentationClient,
                softwareStatementProfileRepo);
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

        // Resolve bank profile
        BankProfile? bankProfile = null;
        if (request.BankProfile is not null)
        {
            bankProfile = _bankProfileService.GetBankProfile(request.BankProfile.Value);
        }

        // Determine entity ID
        var entityId = Guid.NewGuid();

        // Create new or use existing external API object
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? externalApiResponse;
        string externalApiId;
        if (request.ExternalApiObject is null)
        {
            // Load BankRegistration and related
            (BankRegistration bankRegistration, string bankFinancialId, string tokenEndpoint,
                    ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);

            // Load VariableRecurringPaymentsApi
            VariableRecurringPaymentsApiEntity variableRecurringPaymentsApi =
                await GetVariableRecurringPaymentsApi(
                    request.VariableRecurringPaymentsApiId,
                    bankRegistration.BankId);

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
            IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> apiRequests =
                ApiRequests(
                    variableRecurringPaymentsApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            var externalApiUrl = new Uri(variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId);
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest externalApiRequest =
                request.ExternalApiRequest;
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
        var persistedObject = new DomesticVrpConsentPersisted(
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
            request.CreatedBy,
            request.VariableRecurringPaymentsApiId);

        AccessToken? accessToken = request.ExternalApiObject?.AccessToken;
        if (accessToken is not null)
        {
            persistedObject.UpdateAccessToken(
                accessToken.Token,
                accessToken.ExpiresIn,
                accessToken.RefreshToken,
                utcNow,
                request.CreatedBy);
        }

        // Save entity
        await _entityMethods.AddAsync(persistedObject);

        // Create response (may involve additional processing based on entity)
        var response =
            new DomesticVrpConsentCreateResponse(
                persistedObject.Id,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.Reference,
                null,
                persistedObject.BankRegistrationId,
                persistedObject.ExternalApiId,
                persistedObject.ExternalApiUserId,
                persistedObject.VariableRecurringPaymentsApiId,
                externalApiResponse);

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    public async
        Task<(DomesticVrpConsentReadResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ConsentReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticVrpConsent and related
        (DomesticVrpConsentPersisted persistedConsent, string externalApiConsentId,
                VariableRecurringPaymentsApiEntity variableRecurringPaymentsApi, BankRegistration bankRegistration,
                string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(readParams.Id);

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
        IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> apiRequests =
            ApiRequests(
                variableRecurringPaymentsApi.ApiVersion,
                bankFinancialId,
                ccGrantAccessToken,
                processedSoftwareStatementProfile);
        var externalApiUrl = new Uri(
            variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
        (VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse externalApiResponse,
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
            new DomesticVrpConsentReadResponse(
                persistedConsent.Id,
                persistedConsent.Created,
                persistedConsent.CreatedBy,
                persistedConsent.Reference,
                null,
                persistedConsent.BankRegistrationId,
                persistedConsent.ExternalApiId,
                persistedConsent.ExternalApiUserId,
                persistedConsent.VariableRecurringPaymentsApiId,
                externalApiResponse);

        return (response, nonErrorMessages);
    }

    public async
        Task<(DomesticVrpConsentReadFundsConfirmationResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)> ReadFundsConfirmationAsync(ConsentBaseReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticVrpConsent and related
        (DomesticVrpConsentPersisted persistedObject, string bankApiId,
                VariableRecurringPaymentsApiEntity variableRecurringPaymentsApi, BankRegistration bankRegistration,
                string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(readParams.Id);

        // Get access token
        string bankIssuerUrl =
            persistedObject.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                ?.DomesticVrpConsentAuthGet
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
        IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse> apiRequests =
            ApiRequestsFundsConfirmation(
                variableRecurringPaymentsApi.ApiVersion,
                bankFinancialId,
                accessToken,
                processedSoftwareStatementProfile);
        var externalApiUrl = new Uri(
            variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId + $"/{bankApiId}" + "/funds-confirmation");
        (VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse externalApiResponse,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.GetAsync(
                externalApiUrl,
                responseJsonSerializerSettings,
                processedSoftwareStatementProfile.ApiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Create response
        var response =
            new DomesticVrpConsentReadFundsConfirmationResponse(
                persistedObject.Id,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.Reference,
                null,
                persistedObject.BankRegistrationId,
                persistedObject.ExternalApiId,
                persistedObject.ExternalApiUserId,
                persistedObject.VariableRecurringPaymentsApiId,
                externalApiResponse);

        return (response, nonErrorMessages);
    }

    private async Task<VariableRecurringPaymentsApiEntity> GetVariableRecurringPaymentsApi(
        Guid variableRecurringPaymentsApiId,
        Guid bankId)
    {
        VariableRecurringPaymentsApiEntity variableRecurringPaymentsApiEntity =
            await _apiEntityMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == variableRecurringPaymentsApiId) ??
            throw new KeyNotFoundException(
                $"No record found for VariableRecurringPaymentsApi {variableRecurringPaymentsApiId} specified by request.");

        if (variableRecurringPaymentsApiEntity.BankId != bankId)
        {
            throw new ArgumentException(
                "Specified VariableRecurringPaymentsApi and BankRegistration objects do not share same BankId.");
        }

        return variableRecurringPaymentsApiEntity;
    }

    private IApiRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> ApiRequests(
        VariableRecurringPaymentsApiVersion variableRecurringPaymentsApiVersion,
        string bankFinancialId,
        string accessToken,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =>
        variableRecurringPaymentsApiVersion switch
        {
            VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiRequests<
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
                    false,
                    processedSoftwareStatementProfile)),
            _ => throw new ArgumentOutOfRangeException(
                $"VRP API version {variableRecurringPaymentsApiVersion} not supported.")
        };


    private IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>
        ApiRequestsFundsConfirmation(
            VariableRecurringPaymentsApiVersion variableRecurringPaymentsApiVersion,
            string bankFinancialId,
            string accessToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =>
        variableRecurringPaymentsApiVersion switch
        {
            VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiGetRequests<
                VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse,
                VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>(
                new ApiGetRequestProcessor(
                    bankFinancialId,
                    accessToken)),
            _ => throw new ArgumentOutOfRangeException(
                $"VRP API version {variableRecurringPaymentsApiVersion} not supported.")
        };
}
