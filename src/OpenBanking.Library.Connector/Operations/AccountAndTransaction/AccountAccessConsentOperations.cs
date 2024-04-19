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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class
    AccountAccessConsentOperations :
    IObjectCreate<AccountAccessConsentRequest, AccountAccessConsentCreateResponse, ConsentCreateParams>,
    IObjectRead<AccountAccessConsentCreateResponse, ConsentReadParams>
{
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
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;
    private readonly ITimeProvider _timeProvider;

    public AccountAccessConsentOperations(
        IDbReadWriteEntityMethods<AccountAccessConsentPersisted> consentEntityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IGrantPost grantPost,
        IBankProfileService bankProfileService,
        IDbReadOnlyEntityMethods<BankRegistrationEntity> bankRegistrationMethods,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods)
    {
        _consentEntityMethods = consentEntityMethods;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _accountAccessConsentCommon =
            new AccountAccessConsentCommon(consentEntityMethods);
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
                instrumentationClient);
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
        ExternalApiResponseInfo? externalApiResponseInfo;
        string externalApiId;
        if (request.ExternalApiObject is null)
        {
            // Load BankRegistration and related
            (BankRegistrationEntity bankRegistration, string tokenEndpoint,
                    SoftwareStatementEntity softwareStatement) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);

            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi();
            TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
                bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
            string bankFinancialId = bankProfile.FinancialId;

            // Get IApiClient
            // IApiClient apiClient = bankRegistration.UseSimulatedBank
            //     ? bankProfile.ReplayApiClient
            //     : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;
            IApiClient apiClient =
                (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

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
            IApiPostRequests<AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1> apiRequests =
                ApiRequests(
                    accountAndTransactionApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken);
            var externalApiUrl = new Uri(accountAndTransactionApi.BaseUrl + RelativePathBeforeId);
            AccountAndTransactionModelsPublic.OBReadConsent1 externalApiRequest =
                AccountAccessConsentPublicMethods.ResolveExternalApiRequest(
                    request.ExternalApiRequest,
                    request.TemplateRequest,
                    bankProfile);
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      POST {AispBaseUrl}{{RelativePathBeforeId}}
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

            // Transform links
            if (externalApiResponse.Links is not null)
            {
                bool responseLinksOmitId = customBehaviour?.AccountAccessConsentPost?.ResponseLinksOmitId ?? false;
                bool responseLinksAddSlash = customBehaviour?.AccountAccessConsentPost?.ResponseLinksAddSlash ?? false;
                Uri expectedLinkUrlWithoutQuery = responseLinksOmitId
                    ? externalApiUrl
                    : new Uri(externalApiUrl + $"/{externalApiId}");
                if (responseLinksAddSlash)
                {
                    expectedLinkUrlWithoutQuery = new Uri(expectedLinkUrlWithoutQuery + "/");
                }

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
            new AccountAccessConsentCreateResponse
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
        Task<(AccountAccessConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ConsentReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load AccountAccessConsent and related
        (AccountAccessConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration, _, _,
                SoftwareStatementEntity softwareStatement) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(readParams.Id, false);
        string externalApiConsentId = persistedConsent.ExternalApiId;

        bool includeExternalApiOperation =
            readParams.IncludeExternalApiOperation;
        AccountAndTransactionModelsPublic.OBReadConsentResponse1? externalApiResponse;
        ExternalApiResponseInfo? externalApiResponseInfo;
        if (includeExternalApiOperation)
        {
            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi();
            TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
                bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
            string bankFinancialId = bankProfile.FinancialId;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

            // Get IApiClient
            // IApiClient apiClient = bankRegistration.UseSimulatedBank
            //     ? bankProfile.ReplayApiClient
            //     : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;
            IApiClient apiClient =
                (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

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
            IApiGetRequests<AccountAndTransactionModelsPublic.OBReadConsentResponse1> apiRequests =
                ApiRequests(
                    accountAndTransactionApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken);
            var externalApiUrl = new Uri(
                accountAndTransactionApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      GET {AispBaseUrl}{{RelativePathBeforeId}}/{ConsentId}
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

            // Transform links 
            if (externalApiResponse.Links is not null)
            {
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
            new AccountAccessConsentCreateResponse
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

    private IApiRequests<AccountAndTransactionModelsPublic.OBReadConsent1,
        AccountAndTransactionModelsPublic.OBReadConsentResponse1> ApiRequests(
        AccountAndTransactionApiVersion accountAndTransactionApiVersion,
        string bankFinancialId,
        string accessToken) =>
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
