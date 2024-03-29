// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class
    AccountAccessConsentOperations :
        IObjectCreate<AccountAccessConsentRequest, AccountAccessConsentCreateResponse, ConsentCreateParams>,
        IObjectRead<AccountAccessConsentCreateResponse, ConsentReadParams>
{
    private readonly IDbReadWriteEntityMethods<AccountAccessConsentAccessToken> _accessTokenEntityMethods;
    private readonly AccountAccessConsentCommon _accountAccessConsentCommon;

    private readonly IBankProfileService _bankProfileService;

    private readonly ConsentCommon<AccountAccessConsentPersisted,
        AccountAccessConsentRequest,
        AccountAccessConsentCreateResponse,
        AccountAndTransactionModelsPublic.OBReadConsent1,
        AccountAndTransactionModelsPublic.OBReadConsentResponse1> _consentCommon;

    private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _consentEntityMethods;

    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;

    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly IDbReadWriteEntityMethods<AccountAccessConsentRefreshToken> _refreshTokenEntityMethods;
    private readonly ITimeProvider _timeProvider;

    public AccountAccessConsentOperations(
        IDbReadWriteEntityMethods<AccountAccessConsentPersisted> consentEntityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IGrantPost grantPost,
        IBankProfileService bankProfileService,
        IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods,
        IDbReadWriteEntityMethods<AccountAccessConsentAccessToken> accessTokenEntityMethods,
        IDbReadWriteEntityMethods<AccountAccessConsentRefreshToken> refreshTokenEntityMethods)
    {
        _consentEntityMethods = consentEntityMethods;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _accessTokenEntityMethods = accessTokenEntityMethods;
        _refreshTokenEntityMethods = refreshTokenEntityMethods;
        _accountAccessConsentCommon =
            new AccountAccessConsentCommon(consentEntityMethods, softwareStatementProfileRepo);
        _mapper = mapper;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _consentCommon =
            new ConsentCommon<AccountAccessConsentPersisted, AccountAccessConsentRequest,
                AccountAccessConsentCreateResponse,
                AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1>(
                bankRegistrationMethods,
                instrumentationClient,
                softwareStatementProfileRepo);
    }

    private string ClientCredentialsGrantScope => "accounts";

    private string RelativePathBeforeId => "/account-access-consents";

    public async Task<(AccountAccessConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)>
        CreateAsync(
            AccountAccessConsentRequest request,
            ConsentCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Determine entity ID
        var entityId = Guid.NewGuid();

        // Create new or use existing external API object
        AccountAndTransactionModelsPublic.OBReadConsentResponse1? externalApiResponse;
        string externalApiId;
        if (request.ExternalApiObject is null)
        {
            // Load BankRegistration and related
            (BankRegistration bankRegistration, string tokenEndpoint,
                    ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);

            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi();
            TokenEndpointAuthMethod tokenEndpointAuthMethod =
                bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
            bool supportsSca = bankProfile.SupportsSca;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
            string bankFinancialId = bankProfile.FinancialId;

            // Get client credentials grant access token
            string ccGrantAccessToken =
                await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile.OBSealKey,
                    bankRegistration,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    supportsSca,
                    null,
                    customBehaviour?.ClientCredentialsGrantPost,
                    processedSoftwareStatementProfile.ApiClient);

            // Create new object at external API
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiPostRequests<AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1> apiRequests =
                ApiRequests(
                    accountAndTransactionApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            var externalApiUrl = new Uri(accountAndTransactionApi.BaseUrl + RelativePathBeforeId);
            AccountAndTransactionModelsPublic.OBReadConsent1 externalApiRequest =
                AccountAccessConsentPublicMethods.ResolveExternalApiRequest(
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
            bool responseLinksOmitId = customBehaviour?.AccountAccessConsentPost?.ResponseLinksOmitId ?? false;
            bool responseLinksAddSlash = customBehaviour?.AccountAccessConsentPost?.ResponseLinksAddSlash ?? false;
            Uri expectedLinkUrlWithoutQuery = responseLinksOmitId
                ? externalApiUrl
                : new Uri(externalApiUrl + $"/{externalApiId}");
            if (responseLinksAddSlash)
            {
                expectedLinkUrlWithoutQuery = new Uri(expectedLinkUrlWithoutQuery + "/");
            }

            string? publicUrlWithoutQuery = createParams.PublicRequestUrlWithoutQuery switch
            {
                { } x => x + $"/{entityId}",
                null => null
            };
            var validQueryParameters = new List<string>();
            var linksUrlOperations = new LinksUrlOperations(
                expectedLinkUrlWithoutQuery,
                publicUrlWithoutQuery,
                true,
                validQueryParameters);
            externalApiResponse.Links.Self = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Self);
            externalApiResponse.Links.First =
                linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.First);
            externalApiResponse.Links.Prev = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Prev);
            externalApiResponse.Links.Next = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Next);
            externalApiResponse.Links.Last = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Last);
        }
        else
        {
            externalApiResponse = null;
            externalApiId = request.ExternalApiObject.ExternalApiId;
        }

        // Create persisted entity and return response
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var persistedConsent = new AccountAccessConsentPersisted(
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
            null);

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
            new AccountAccessConsentCreateResponse(
                persistedConsent.Id,
                persistedConsent.Created,
                persistedConsent.CreatedBy,
                persistedConsent.Reference,
                null,
                persistedConsent.BankRegistrationId,
                persistedConsent.ExternalApiId,
                persistedConsent.ExternalApiUserId,
                persistedConsent.AuthContextModified,
                persistedConsent.AuthContextModifiedBy,
                externalApiResponse);

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    public async
        Task<(AccountAccessConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ConsentReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load AccountAccessConsent and related
        (AccountAccessConsentPersisted persistedConsent, BankRegistration bankRegistration, _, _,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(readParams.Id, false);

        bool includeExternalApiOperation =
            readParams.IncludeExternalApiOperation;
        AccountAndTransactionModelsPublic.OBReadConsentResponse1? externalApiResponse;
        if (includeExternalApiOperation)
        {
            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi();
            TokenEndpointAuthMethod tokenEndpointAuthMethod =
                bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
            bool supportsSca = bankProfile.SupportsSca;
            string bankFinancialId = bankProfile.FinancialId;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

            // Get client credentials grant access token
            string ccGrantAccessToken =
                await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile.OBSealKey,
                    bankRegistration,
                    tokenEndpointAuthMethod,
                    bankRegistration.TokenEndpoint,
                    supportsSca,
                    null,
                    customBehaviour?.ClientCredentialsGrantPost,
                    processedSoftwareStatementProfile.ApiClient);

            // Read object from external API
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiGetRequests<AccountAndTransactionModelsPublic.OBReadConsentResponse1> apiRequests =
                ApiRequests(
                    accountAndTransactionApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            string externalApiConsentId = persistedConsent.ExternalApiId;
            var externalApiUrl = new Uri(
                accountAndTransactionApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
            (externalApiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.GetAsync(
                    externalApiUrl,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Transform links 
            bool responseLinksAddSlash = customBehaviour?.AccountAccessConsentGet?.ResponseLinksAddSlash ?? false;
            Uri expectedLinkUrlWithoutQuery = externalApiUrl;
            if (responseLinksAddSlash)
            {
                expectedLinkUrlWithoutQuery = new Uri(expectedLinkUrlWithoutQuery + "/");
            }
            var validQueryParameters = new List<string>();
            var linksUrlOperations = new LinksUrlOperations(
                expectedLinkUrlWithoutQuery,
                readParams.PublicRequestUrlWithoutQuery,
                true,
                validQueryParameters);
            externalApiResponse.Links.Self = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Self);
            externalApiResponse.Links.First =
                linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.First);
            externalApiResponse.Links.Prev = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Prev);
            externalApiResponse.Links.Next = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Next);
            externalApiResponse.Links.Last = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Last);
        }
        else
        {
            externalApiResponse = null;
        }

        // Create response
        var response =
            new AccountAccessConsentCreateResponse(
                persistedConsent.Id,
                persistedConsent.Created,
                persistedConsent.CreatedBy,
                persistedConsent.Reference,
                null,
                persistedConsent.BankRegistrationId,
                persistedConsent.ExternalApiId,
                persistedConsent.ExternalApiUserId,
                persistedConsent.AuthContextModified,
                persistedConsent.AuthContextModifiedBy,
                externalApiResponse);

        return (response, nonErrorMessages);
    }

    private IApiRequests<AccountAndTransactionModelsPublic.OBReadConsent1,
        AccountAndTransactionModelsPublic.OBReadConsentResponse1> ApiRequests(
        AccountAndTransactionApiVersion accountAndTransactionApiVersion,
        string bankFinancialId,
        string accessToken,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =>
        accountAndTransactionApiVersion switch
        {
            AccountAndTransactionApiVersion.Version3p1p7 => new ApiRequests<
                AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1,
                AccountAndTransactionModelsV3p1p7.OBReadConsent1,
                AccountAndTransactionModelsV3p1p7.OBReadConsentResponse1>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken),
                new AccountAndTransactionPostRequestProcessor<
                    AccountAndTransactionModelsV3p1p7.OBReadConsent1>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient)),
            AccountAndTransactionApiVersion.VersionPublic => new ApiRequests<
                AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1,
                AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken),
                new AccountAndTransactionPostRequestProcessor<
                    AccountAndTransactionModelsPublic.OBReadConsent1>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient)),
            _ => throw new ArgumentOutOfRangeException(
                $"AISP API version {accountAndTransactionApiVersion} not supported.")
        };
}
