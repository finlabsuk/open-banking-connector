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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AccountAccessConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.
    AccountAccessConsentAuthContext;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using AccountAccessConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.
    AccountAccessConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class
    AccountAccessConsentAuthContextOperations : IObjectCreate<AccountAccessConsentAuthContextRequest,
        AccountAccessConsentAuthContextCreateResponse, LocalCreateParams>,
    IObjectRead<AccountAccessConsentAuthContextReadResponse, LocalReadParams>
{
    private readonly AccountAccessConsentCommon _accountAccessConsentCommon;
    private readonly IBankProfileService _bankProfileService;
    private readonly ClientAccessTokenGet _clientAccessTokenGet;
    private readonly IDbMethods _dbSaveChangesMethod;
    private readonly IDbEntityMethods<AccountAccessConsentAuthContextPersisted> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;
    private readonly ITimeProvider _timeProvider;

    public AccountAccessConsentAuthContextOperations(
        IDbEntityMethods<AccountAccessConsentAuthContextPersisted>
            entityMethods,
        IDbMethods dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods,
        ClientAccessTokenGet clientAccessTokenGet,
        AccountAccessConsentCommon accountAccessConsentCommon,
        IApiVariantMapper mapper)
    {
        _entityMethods = entityMethods;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _bankProfileService = bankProfileService;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _clientAccessTokenGet = clientAccessTokenGet;
        _accountAccessConsentCommon = accountAccessConsentCommon;
        _mapper = mapper;
    }

    public async Task<(AccountAccessConsentAuthContextCreateResponse response,
            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(
            AccountAccessConsentAuthContextRequest request,
            LocalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load AccountAccessConsent and related
        (AccountAccessConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(request.AccountAccessConsentId, false);
        string authorizationEndpoint = bankRegistration.AuthorizationEndpoint;
        string externalApiConsentId = persistedConsent.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        bool aispUseV4 = bankRegistration.AispUseV4;
        AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi(aispUseV4);
        string bankFinancialId = bankProfile.FinancialId;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        OAuth2ResponseType responseType = bankProfile.DefaultResponseType;

        string redirectUri = softwareStatement.GetRedirectUri(
            bankRegistration.DefaultResponseModeOverride ?? bankProfile.DefaultResponseMode,
            bankRegistration.DefaultFragmentRedirectUri,
            bankRegistration.DefaultQueryRedirectUri);

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Create auth URL
        string consentAuthGetAudClaim =
            customBehaviour?.AccountAccessConsentAuthGet?.AudClaim ??
            issuerUrl;

        // Detect re-auth case
        bool authPreviouslySuccessfullyPerformed = persistedConsent.AuthPreviouslySucceessfullyPerformed();
        bool reAuthNotInitialAuth = authPreviouslySuccessfullyPerformed;

        // Detect re-auth case via status
        bool detectReAuthCaseViaConsentStatus =
            customBehaviour?.AccountAccessConsentAuthGet?.DetectReAuthCaseViaConsentStatus ?? false;
        if (!reAuthNotInitialAuth && detectReAuthCaseViaConsentStatus)
        {
            // Get IApiClient
            // IApiClient apiClient = bankRegistration.UseSimulatedBank
            //     ? bankProfile.ReplayApiClient
            //     : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;
            IApiClient apiClient =
                (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

            // Get client credentials grant access token
            string ccGrantAccessToken =
                await _clientAccessTokenGet.GetAccessToken(
                    AccountAccessConsentOperations.ClientCredentialsGrantScope,
                    obSealKey,
                    bankRegistration,
                    externalApiSecret,
                    customBehaviour?.ClientCredentialsGrantPost,
                    apiClient,
                    bankProfile.BankProfileEnum);

            // Read object from external API
            var externalApiUrl = new Uri(
                accountAndTransactionApi.BaseUrl + AccountAccessConsentOperations.RelativePathBeforeId +
                $"/{externalApiConsentId}");
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      GET {AispBaseUrl}{{AccountAccessConsentOperations.RelativePathBeforeId}}/{ConsentId}
                      """,
                BankProfile = bankProfile.BankProfileEnum
            };
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            AccountAndTransactionModelsPublic.OBReadConsentResponse1 externalApiResponse;
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
                            [],
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
                    (externalApiResponse,
                            xFapiInteractionId,
                            newNonErrorMessages) =
                        await apiRequests.GetAsync(
                            externalApiUrl,
                            [],
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

            AccountAndTransactionModelsPublic.Data5Status status = externalApiResponse.Data.Status;
            reAuthNotInitialAuth =
                status is not AccountAndTransactionModelsPublic.Data5Status.AWAU;
        }

        string scope = customBehaviour?.AccountAccessConsentAuthGet?.Scope ?? "accounts";

        (string authUrl, string state, string nonce, string? codeVerifier, string sessionId) = CreateAuthUrl.Create(
            persistedConsent.ExternalApiId,
            obSealKey,
            bankRegistration.ExternalApiId,
            bankProfile.UseOpenIdConnect,
            customBehaviour?.AccountAccessConsentAuthGet,
            authorizationEndpoint,
            consentAuthGetAudClaim,
            supportsSca,
            redirectUri,
            scope,
            responseType,
            reAuthNotInitialAuth,
            _instrumentationClient);

        // Create persisted entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var entity = new AccountAccessConsentAuthContextPersisted(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            state,
            nonce,
            codeVerifier,
            sessionId,
            request.AccountAccessConsentId);

        // Add entity
        await _entityMethods.AddAsync(entity);

        var response =
            new AccountAccessConsentAuthContextCreateResponse
            {
                Id = entity.Id,
                Created = entity.Created,
                CreatedBy = entity.CreatedBy,
                Reference = entity.Reference,
                AccountAccessConsentId = entity.AccountAccessConsentId,
                State = state,
                AuthUrl = authUrl,
                AppSessionId = sessionId
            };

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    public async Task<(AccountAccessConsentAuthContextReadResponse response, IList<IFluentResponseInfoOrWarningMessage>
        nonErrorMessages)> ReadAsync(LocalReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Create persisted entity
        AccountAccessConsentAuthContextPersisted persistedObject =
            await _entityMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
            throw new KeyNotFoundException(
                $"No record found for AccountAccessConsentAuthContext with ID {readParams.Id}.");

        // Create response
        AccountAccessConsentAuthContextReadResponse response = persistedObject.PublicGetLocalResponse;

        return (response, nonErrorMessages);
    }
}
