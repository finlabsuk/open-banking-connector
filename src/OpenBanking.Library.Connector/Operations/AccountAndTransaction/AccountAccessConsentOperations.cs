// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
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
    private readonly ClientAccessTokenGet _clientAccessTokenGet;

    private readonly ConsentCommon<AccountAccessConsentPersisted,
        AccountAccessConsentRequest,
        AccountAccessConsentCreateResponse,
        AccountAndTransactionModelsPublic.OBReadConsent1,
        AccountAndTransactionModelsPublic.OBReadConsentResponse1> _consentCommon;

    private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _consentEntityMethods;
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
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
        IBankProfileService bankProfileService,
        IDbReadOnlyEntityMethods<BankRegistrationEntity> bankRegistrationMethods,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods,
        ClientAccessTokenGet clientAccessTokenGet,
        AccountAccessConsentCommon accountAccessConsentCommon)
    {
        _consentEntityMethods = consentEntityMethods;
        _bankProfileService = bankProfileService;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _clientAccessTokenGet = clientAccessTokenGet;
        _accountAccessConsentCommon = accountAccessConsentCommon;
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

    public static string ClientCredentialsGrantScope => "accounts";

    public static string RelativePathBeforeId => "/account-access-consents";

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
                    SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);

            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi();
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
                await _clientAccessTokenGet.GetAccessToken(
                    ClientCredentialsGrantScope,
                    obSealKey,
                    bankRegistration,
                    externalApiSecret,
                    customBehaviour?.ClientCredentialsGrantPost,
                    apiClient,
                    bankProfile.BankProfileEnum);

            // Create new object at external API
            var externalApiUrl = new Uri(accountAndTransactionApi.BaseUrl + RelativePathBeforeId);
            AccountAndTransactionModelsPublic.OBReadConsent1 externalApiRequest =
                request.ExternalApiRequest ??
                AccountAccessTemplates.AccountAccessConsentExternalApiRequest(
                    request.TemplateRequest?.Type ??
                    throw new InvalidOperationException(
                        "Both ExternalApiRequest and TemplateRequest specified as null so not possible to create external API request."),
                    bankProfile.AccountAndTransactionApiSettings);
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      POST {AispBaseUrl}{{RelativePathBeforeId}}
                      """,
                BankProfile = bankProfile.BankProfileEnum
            };
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            string? xFapiInteractionId;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            switch (accountAndTransactionApi.ApiVersion)
            {
                case AccountAndTransactionApiVersion.Version3p1p11:
                    AccountAndTransactionModelsV3p1p11.OBReadConsent1 externalApiRequestV3 =
                        AccountAndTransactionModelsPublic.Mappings.MapFromOBReadConsent1(externalApiRequest);
                    var apiRequestsV3 =
                        new ApiRequests<AccountAndTransactionModelsV3p1p11.OBReadConsent1,
                            AccountAndTransactionModelsV3p1p11.OBReadConsentResponse1,
                            AccountAndTransactionModelsV3p1p11.OBReadConsent1,
                            AccountAndTransactionModelsV3p1p11.OBReadConsentResponse1>(
                            new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                            new AccountAndTransactionPostRequestProcessor<
                                AccountAndTransactionModelsV3p1p11.OBReadConsent1>(
                                bankFinancialId,
                                ccGrantAccessToken,
                                _instrumentationClient));
                    (AccountAndTransactionModelsV3p1p11.OBReadConsentResponse1 externalApiResponseV3,
                            xFapiInteractionId,
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
                        AccountAndTransactionModelsPublic.Mappings.MapToOBReadConsentResponse1(externalApiResponseV3);
                    break;
                case AccountAndTransactionApiVersion.VersionPublic:
                    var apiRequests =
                        new ApiRequests<AccountAndTransactionModelsPublic.OBReadConsent1,
                            AccountAndTransactionModelsPublic.OBReadConsentResponse1,
                            AccountAndTransactionModelsPublic.OBReadConsent1,
                            AccountAndTransactionModelsPublic.OBReadConsentResponse1>(
                            new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                            new AccountAndTransactionPostRequestProcessor<
                                AccountAndTransactionModelsPublic.OBReadConsent1>(
                                bankFinancialId,
                                ccGrantAccessToken,
                                _instrumentationClient));
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
                        $"AISP API version {accountAndTransactionApi.ApiVersion} not supported.");
            }

            nonErrorMessages.AddRange(newNonErrorMessages);
            externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };
            externalApiId = externalApiResponse.Data.ConsentId;

            // Transform links
            if (externalApiResponse.Links is not null)
            {
                ReadWritePostCustomBehaviour? readWritePostCustomBehaviour = customBehaviour?.AccountAccessConsentPost;
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
        (AccountAccessConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(readParams.Id, false);
        string externalApiConsentId = persistedConsent.ExternalApiId;

        bool excludeExternalApiOperation =
            readParams.ExcludeExternalApiOperation;
        AccountAndTransactionModelsPublic.OBReadConsentResponse1? externalApiResponse;
        ExternalApiResponseInfo? externalApiResponseInfo;
        if (!excludeExternalApiOperation)
        {
            // Get bank profile
            BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi();
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
                accountAndTransactionApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      GET {AispBaseUrl}{{RelativePathBeforeId}}/{ConsentId}
                      """,
                BankProfile = bankProfile.BankProfileEnum
            };
            JsonSerializerSettings? responseJsonSerializerSettings = null;

            string? xFapiInteractionId;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            switch (accountAndTransactionApi.ApiVersion)
            {
                case AccountAndTransactionApiVersion.Version3p1p11:
                    var apiRequestsV3 =
                        new ApiRequests<AccountAndTransactionModelsV3p1p11.OBReadConsent1,
                            AccountAndTransactionModelsV3p1p11.OBReadConsentResponse1,
                            AccountAndTransactionModelsV3p1p11.OBReadConsent1,
                            AccountAndTransactionModelsV3p1p11.OBReadConsentResponse1>(
                            new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                            new AccountAndTransactionPostRequestProcessor<
                                AccountAndTransactionModelsV3p1p11.OBReadConsent1>(
                                bankFinancialId,
                                ccGrantAccessToken,
                                _instrumentationClient));
                    (AccountAndTransactionModelsV3p1p11.OBReadConsentResponse1 externalApiResponseV3,
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
                        AccountAndTransactionModelsPublic.Mappings.MapToOBReadConsentResponse1(externalApiResponseV3);
                    break;
                case AccountAndTransactionApiVersion.VersionPublic:
                    var apiRequests =
                        new ApiRequests<AccountAndTransactionModelsPublic.OBReadConsent1,
                            AccountAndTransactionModelsPublic.OBReadConsentResponse1,
                            AccountAndTransactionModelsPublic.OBReadConsent1,
                            AccountAndTransactionModelsPublic.OBReadConsentResponse1>(
                            new ApiGetRequestProcessor(bankFinancialId, ccGrantAccessToken),
                            new AccountAndTransactionPostRequestProcessor<
                                AccountAndTransactionModelsPublic.OBReadConsent1>(
                                bankFinancialId,
                                ccGrantAccessToken,
                                _instrumentationClient));
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
                        $"AISP API version {accountAndTransactionApi.ApiVersion} not supported.");
            }

            nonErrorMessages.AddRange(newNonErrorMessages);
            externalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId };

            // Transform links 
            if (externalApiResponse.Links is not null)
            {
                ReadWriteGetCustomBehaviour? readWriteGetCustomBehaviour =
                    customBehaviour?.AccountAccessConsentGet;
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
}
