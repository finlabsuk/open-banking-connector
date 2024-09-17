// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class AuthContextUpdate :
    IObjectUpdate<AuthResult, AuthContextUpdateAuthResultResponse>
{
    private readonly IDbReadWriteEntityMethods<AuthContext> _authContextMethods;
    private readonly IBankProfileService _bankProfileService;
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IEncryptionKeyInfo _encryptionKeyInfo;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;
    private readonly ITimeProvider _timeProvider;

    public AuthContextUpdate(
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IDbReadWriteEntityMethods<AuthContext>
            authContextMethods,
        IInstrumentationClient instrumentationClient,
        IGrantPost grantPost,
        IBankProfileService bankProfileService,
        IMemoryCache memoryCache,
        IEncryptionKeyInfo encryptionKeyInfo,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods)
    {
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _authContextMethods = authContextMethods;
        _instrumentationClient = instrumentationClient;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _memoryCache = memoryCache;
        _encryptionKeyInfo = encryptionKeyInfo;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
    }

    public async
        Task<(AuthContextUpdateAuthResultResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)>
        CreateAsync(
            AuthResult request,
            string? modifiedBy)
    {
        request.ArgNotNull(nameof(request));

        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Validate state and read auth context and consent from database
        string state = request.State;
        AuthContext authContext =
            _authContextMethods
                .DbSet
                .Include(
                    o => ((AccountAccessConsentAuthContext) o)
                        .AccountAccessConsentNavigation
                        .BankRegistrationNavigation.SoftwareStatementNavigation)
                .Include(
                    o => ((AccountAccessConsentAuthContext) o)
                        .AccountAccessConsentNavigation
                        .BankRegistrationNavigation.ExternalApiSecretsNavigation)
                .Include(
                    o => ((AccountAccessConsentAuthContext) o)
                        .AccountAccessConsentNavigation
                        .AccountAccessConsentAccessTokensNavigation)
                .Include(
                    o => ((AccountAccessConsentAuthContext) o)
                        .AccountAccessConsentNavigation
                        .AccountAccessConsentRefreshTokensNavigation)
                .Include(
                    o => ((DomesticPaymentConsentAuthContext) o)
                        .DomesticPaymentConsentNavigation
                        .BankRegistrationNavigation.SoftwareStatementNavigation)
                .Include(
                    o => ((DomesticPaymentConsentAuthContext) o)
                        .DomesticPaymentConsentNavigation
                        .BankRegistrationNavigation.ExternalApiSecretsNavigation)
                .Include(
                    o => ((DomesticPaymentConsentAuthContext) o)
                        .DomesticPaymentConsentNavigation
                        .DomesticPaymentConsentAccessTokensNavigation)
                .Include(
                    o => ((DomesticPaymentConsentAuthContext) o)
                        .DomesticPaymentConsentNavigation
                        .DomesticPaymentConsentRefreshTokensNavigation)
                .Include(
                    o => ((DomesticVrpConsentAuthContext) o)
                        .DomesticVrpConsentNavigation
                        .BankRegistrationNavigation.SoftwareStatementNavigation)
                .Include(
                    o => ((DomesticVrpConsentAuthContext) o)
                        .DomesticVrpConsentNavigation
                        .BankRegistrationNavigation.ExternalApiSecretsNavigation)
                .Include(
                    o => ((DomesticVrpConsentAuthContext) o)
                        .DomesticVrpConsentNavigation
                        .DomesticVrpConsentAccessTokensNavigation)
                .Include(
                    o => ((DomesticVrpConsentAuthContext) o)
                        .DomesticVrpConsentNavigation
                        .DomesticVrpConsentRefreshTokensNavigation)
                .AsSplitQuery() // Load collections in separate SQL queries
                .SingleOrDefault(x => x.State == state) ??
            throw new KeyNotFoundException($"No record found for Auth Context with state {state}.");
        string authContextNonce = authContext.Nonce;
        string authContextAppSessionId = authContext.AppSessionId;

        // Only accept redirects within 10 mins of auth context (session) creation
        const int authContextExpiryIntervalInSeconds = 10 * 60;
        DateTimeOffset authContextExpiryTime = authContext.Created
            .AddSeconds(authContextExpiryIntervalInSeconds);
        if (_timeProvider.GetUtcNow() > authContextExpiryTime)
        {
            throw new InvalidOperationException(
                "Auth context exists but now stale (more than 10 minutes old) so will not process redirect. " +
                "Please create a new auth context and authenticate again.");
        }

        // Validate error parameter
        if (request.OAuth2RedirectOptionalParameters.Error is not null)
        {
            throw new InvalidOperationException(
                $"OAuth2 error parameter received: {request.OAuth2RedirectOptionalParameters.Error}");
        }

        // Validate code parameter
        if (string.IsNullOrEmpty(request.OAuth2RedirectOptionalParameters.Code))
        {
            throw new InvalidOperationException("OAuth2 code parameter is null or empty.");
        }
        string code = request.OAuth2RedirectOptionalParameters.Code;

        // Validate auth context app session ID
        if (request.AppSessionId is not null)
        {
            if (request.AppSessionId != authContextAppSessionId)
            {
                throw new InvalidOperationException("App session ID supplied does not match that of auth context.");
            }
        }

        // Get consent info
        (BaseConsent consent,
                AccessTokenEntity? storedAccessToken,
                RefreshTokenEntity? storedRefreshToken) =
            authContext switch
            {
                AccountAccessConsentAuthContext ac1 => (
                    (BaseConsent) ac1.AccountAccessConsentNavigation,
                    (AccessTokenEntity?) ac1.AccountAccessConsentNavigation
                        .AccountAccessConsentAccessTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted),
                    (RefreshTokenEntity?) ac1.AccountAccessConsentNavigation
                        .AccountAccessConsentRefreshTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted)),
                DomesticPaymentConsentAuthContext ac2 => (
                    ac2.DomesticPaymentConsentNavigation,
                    ac2.DomesticPaymentConsentNavigation
                        .DomesticPaymentConsentAccessTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted),
                    ac2.DomesticPaymentConsentNavigation
                        .DomesticPaymentConsentRefreshTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted)),
                DomesticVrpConsentAuthContext ac3 => (
                    ac3.DomesticVrpConsentNavigation,
                    ac3.DomesticVrpConsentNavigation
                        .DomesticVrpConsentAccessTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted),
                    ac3.DomesticVrpConsentNavigation
                        .DomesticVrpConsentRefreshTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted)),
                _ => throw new ArgumentOutOfRangeException()
            };
        Guid consentId = consent.Id;
        string externalApiConsentId = consent.ExternalApiId;
        BankRegistrationEntity bankRegistration = consent.BankRegistrationNavigation;
        string tokenEndpoint = bankRegistration.TokenEndpoint;
        string externalApiClientId = bankRegistration.ExternalApiId;
        string jwksUri = bankRegistration.JwksUri;
        string consentAssociatedData = consent.GetAssociatedData(bankRegistration);
        SoftwareStatementEntity softwareStatement = bankRegistration.SoftwareStatementNavigation!;
        ExternalApiSecretEntity? externalApiSecretEntity =
            bankRegistration.ExternalApiSecretsNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankRegistration.TokenEndpointAuthMethod;
        string bankRegistrationAssociatedData = bankRegistration.GetAssociatedData();

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        OAuth2ResponseMode defaultResponseMode =
            bankRegistration.DefaultResponseModeOverride ?? bankProfile.DefaultResponseMode;
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        string redirectUrl = softwareStatement.GetRedirectUri(
            defaultResponseMode,
            bankRegistration.DefaultFragmentRedirectUri,
            bankRegistration.DefaultQueryRedirectUri);
        OAuth2ResponseType responseType = bankProfile.DefaultResponseType;
        bool useOpenIdConnect = bankProfile.UseOpenIdConnect;

        // Validate ID token parameter presence/absence
        string? idToken = request.OAuth2RedirectOptionalParameters.IdToken;
        if (idToken is null)
        {
            // Check valid to not get ID token
            if (responseType is not OAuth2ResponseType.Code)
            {
                throw new InvalidOperationException(
                    "Parameter id_token not received when response_type is 'code id_token'.");
            }
        }
        else
        {
            // Check valid to get ID token
            if (responseType is not OAuth2ResponseType.CodeIdToken)
            {
                throw new InvalidOperationException("Parameter id_token received when response_type is 'code'.");
            }
        }

        // Get IApiClient
        // IApiClient apiClient = bankRegistration.UseSimulatedBank
        //     ? bankProfile.ReplayApiClient
        //     : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;
        IApiClient apiClient = (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId))
            .ApiClient;

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Validate redirect URL
        if (request.RedirectUrl is not null &&
            !string.Equals(request.RedirectUrl, redirectUrl))
        {
            throw new Exception("Redirect URL supplied does not match that which was expected");
        }

        // Validate response mode
        if (request.ResponseMode is not null &&
            request.ResponseMode != defaultResponseMode)
        {
            throw new Exception("Response mode supplied does not match that which was expected");
        }

        // Determine nonce
        ConsentAuthGetCustomBehaviour? consentAuthGetCustomBehaviour = authContext switch
        {
            AccountAccessConsentAuthContext => customBehaviour?.AccountAccessConsentAuthGet,
            DomesticPaymentConsentAuthContext => customBehaviour?.DomesticPaymentConsentAuthGet,
            DomesticVrpConsentAuthContext => customBehaviour?.DomesticVrpConsentAuthGet,
            _ => throw new ArgumentOutOfRangeException()
        };
        bool nonceClaimIsInitialValue =
            consentAuthGetCustomBehaviour?.IdTokenProcessingCustomBehaviour?.IdTokenNonceClaimIsPreviousValue ?? false;
        string nonce = nonceClaimIsInitialValue && consent.AuthContextNonce is not null
            ? consent.AuthContextNonce
            : authContextNonce;

        // Validate ID token including nonce
        string? requestObjectAudClaim = consentAuthGetCustomBehaviour?.AudClaim;
        string bankTokenIssuerClaim =
            requestObjectAudClaim ??
            issuerUrl;
        DateTimeOffset modified = _timeProvider.GetUtcNow();
        if (idToken is not null)
        {
            bool doNotValidateIdToken =
                consentAuthGetCustomBehaviour?.IdTokenProcessingCustomBehaviour?.DoNotValidateIdToken ?? false;
            if (doNotValidateIdToken is false)
            {
                string? newExternalApiUserId = await _grantPost.ValidateIdTokenAuthEndpoint(
                    idToken,
                    code,
                    state,
                    consentAuthGetCustomBehaviour?.IdTokenProcessingCustomBehaviour,
                    jwksUri,
                    customBehaviour?.JwksGet,
                    bankTokenIssuerClaim,
                    externalApiClientId,
                    externalApiConsentId,
                    nonce,
                    supportsSca,
                    bankProfile.BankProfileEnum,
                    idTokenSubClaimType,
                    consent.ExternalApiUserId);
                if (newExternalApiUserId != consent.ExternalApiUserId)
                {
                    consent.UpdateExternalApiUserId(
                        newExternalApiUserId,
                        modified,
                        modifiedBy);
                }
            }
        }

        // Valid ID token means nonce has been validated so we delete auth context to ensure nonce can only be used once
        authContext.UpdateIsDeleted(true, modified, modifiedBy);

        // Wrap remaining processing in try block to ensure DB changes persisted
        try
        {
            // Get client secret if required
            string? clientSecret = null;
            if (tokenEndpointAuthMethod is TokenEndpointAuthMethodSupportedValues.ClientSecretBasic
                or TokenEndpointAuthMethodSupportedValues.ClientSecretPost)
            {
                if (externalApiSecretEntity is null)
                {
                    throw new InvalidOperationException("No client secret is available.");
                }

                // Extract client secret
                clientSecret = externalApiSecretEntity
                    .GetClientSecret(
                        bankRegistrationAssociatedData,
                        _encryptionKeyInfo.GetEncryptionKey(externalApiSecretEntity.KeyId));
            }

            // Obtain token for consent
            JsonSerializerSettings? jsonSerializerSettings = null;
            (AuthCodeGrantPostCustomBehaviour? consentAuthCodeGrantPostCustomBehaviour, bool expectRefreshToken,
                    string defaultScope) =
                authContext switch
                {
                    AccountAccessConsentAuthContext => (customBehaviour?.AccountAccessConsentAuthCodeGrantPost, true,
                        "accounts"),
                    DomesticPaymentConsentAuthContext => (customBehaviour?.DomesticPaymentConsentAuthCodeGrantPost,
                        true, "payments"),
                    DomesticVrpConsentAuthContext => (customBehaviour?.DomesticVrpConsentAuthCodeGrantPost, true,
                        "payments"),
                    _ => throw new ArgumentOutOfRangeException()
                };

            string scope = consentAuthCodeGrantPostCustomBehaviour?.Scope ?? defaultScope;
            if (useOpenIdConnect)
            {
                scope = "openid " + scope;
            }
            TokenEndpointResponse tokenEndpointResponse =
                await _grantPost.PostAuthCodeGrantAsync(
                    code,
                    redirectUrl,
                    bankTokenIssuerClaim,
                    externalApiClientId,
                    clientSecret,
                    externalApiConsentId,
                    consent.ExternalApiUserId,
                    nonce,
                    scope,
                    obSealKey,
                    jwksUri,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    supportsSca,
                    expectRefreshToken,
                    bankProfile.BankProfileEnum,
                    idTokenSubClaimType,
                    authContext.CodeVerifier,
                    jsonSerializerSettings,
                    consentAuthCodeGrantPostCustomBehaviour,
                    customBehaviour?.JwksGet,
                    apiClient);

            // Update consent with state, nonce
            consent.UpdateAuthContext(
                authContext.State,
                nonce,
                authContext.CodeVerifier,
                modified,
                modifiedBy);

            // Delete old cached/stored access token if exists
            _memoryCache.Remove(consent.GetCacheKey());
            storedAccessToken?.UpdateIsDeleted(true, modified, modifiedBy);

            // Delete old stored refresh token if exists
            storedRefreshToken?.UpdateIsDeleted(true, modified, modifiedBy);

            // Create cache entry
            MemoryCacheEntryOptions cacheEntryOptions =
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(
                        _grantPost.GetTokenAdjustedDuration(
                            //11,
                            tokenEndpointResponse.ExpiresIn));
            _memoryCache.Set(
                consent.GetCacheKey(),
                tokenEndpointResponse.AccessToken,
                cacheEntryOptions);

            // Conditionally store new access token
            const int expiryThresholdForSaving = 24 * 60 * 60; // one day
            if (tokenEndpointResponse.ExpiresIn > expiryThresholdForSaving)
            {
                var newAccessToken = new AccessToken(
                    tokenEndpointResponse.AccessToken,
                    //11,
                    tokenEndpointResponse.ExpiresIn);
                AccessTokenEntity newAccessTokenObject = consent.AddNewAccessToken(
                    Guid.NewGuid(),
                    null,
                    false,
                    modified,
                    modifiedBy,
                    modified,
                    modifiedBy);
                string? currentKeyId = _encryptionKeyInfo.GetCurrentKeyId();
                newAccessTokenObject.UpdateAccessToken(
                    newAccessToken,
                    consentAssociatedData,
                    _encryptionKeyInfo.GetEncryptionKey(currentKeyId),
                    modified,
                    modifiedBy,
                    currentKeyId);
            }

            // Store new refresh token if available
            if (tokenEndpointResponse.RefreshToken is not null)
            {
                // Store new refresh token
                RefreshTokenEntity newRefreshTokenObject = consent.AddNewRefreshToken(
                    Guid.NewGuid(),
                    null,
                    false,
                    modified,
                    modifiedBy,
                    modified,
                    modifiedBy);
                string? currentKeyId = _encryptionKeyInfo.GetCurrentKeyId();
                newRefreshTokenObject.UpdateRefreshToken(
                    tokenEndpointResponse.RefreshToken,
                    consentAssociatedData,
                    _encryptionKeyInfo.GetEncryptionKey(currentKeyId),
                    modified,
                    modifiedBy,
                    currentKeyId);
            }

            // Create response (may involve additional processing based on entity)
            var response =
                new AuthContextUpdateAuthResultResponse(
                    consent.GetConsentType(),
                    consentId,
                    null);

            return (response, nonErrorMessages);
        }
        finally
        {
            await _dbSaveChangesMethod.SaveChangesAsync();
        }
    }
}
