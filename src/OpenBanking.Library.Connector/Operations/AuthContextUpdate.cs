// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

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
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
    private readonly ITimeProvider _timeProvider;

    public AuthContextUpdate(
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IDbReadWriteEntityMethods<AuthContext>
            authContextMethods,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IGrantPost grantPost,
        IBankProfileService bankProfileService,
        IMemoryCache memoryCache,
        IEncryptionKeyInfo encryptionKeyInfo)
    {
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _authContextMethods = authContextMethods;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
        _instrumentationClient = instrumentationClient;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _memoryCache = memoryCache;
        _encryptionKeyInfo = encryptionKeyInfo;
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

        _instrumentationClient.Trace("Received ID token: " + request.RedirectData.IdToken);

        // Read auth context and consent from database
        string state = request.RedirectData.State;
        AuthContext authContext =
            _authContextMethods
                .DbSet
                .Include(
                    o => ((AccountAccessConsentAuthContext) o)
                        .AccountAccessConsentNavigation
                        .BankRegistrationNavigation)
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
                        .BankRegistrationNavigation)
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
                        .BankRegistrationNavigation)
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
                string? requestScope,
                AccessTokenEntity? storedAccessToken,
                RefreshTokenEntity? storedRefreshToken) =
            authContext switch
            {
                AccountAccessConsentAuthContext ac1 => (
                    (BaseConsent) ac1.AccountAccessConsentNavigation,
                    "openid accounts",
                    (AccessTokenEntity?) ac1.AccountAccessConsentNavigation
                        .AccountAccessConsentAccessTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted),
                    (RefreshTokenEntity?) ac1.AccountAccessConsentNavigation
                        .AccountAccessConsentRefreshTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted)),
                DomesticPaymentConsentAuthContext ac2 => (
                    ac2.DomesticPaymentConsentNavigation,
                    "openid payments",
                    ac2.DomesticPaymentConsentNavigation
                        .DomesticPaymentConsentAccessTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted),
                    ac2.DomesticPaymentConsentNavigation
                        .DomesticPaymentConsentRefreshTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted)),
                DomesticVrpConsentAuthContext ac3 => (
                    ac3.DomesticVrpConsentNavigation,
                    "openid payments",
                    ac3.DomesticVrpConsentNavigation
                        .DomesticVrpConsentAccessTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted),
                    ac3.DomesticVrpConsentNavigation
                        .DomesticVrpConsentRefreshTokensNavigation
                        .SingleOrDefault(x => !x.IsDeleted)),
                _ => throw new ArgumentOutOfRangeException()
            };
        BankRegistration bankRegistration = consent.BankRegistrationNavigation;
        string redirectUrl = bankRegistration.DefaultFragmentRedirectUri;
        Guid consentId = consent.Id;
        string externalApiConsentId = consent.ExternalApiId;
        string tokenEndpoint = bankRegistration.TokenEndpoint;
        string externalApiClientId = bankRegistration.ExternalApiObject.ExternalApiId;
        string consentAssociatedData = consent.GetAssociatedData(bankRegistration);

        // Validate redirect URL
        if (request.RedirectUrl is not null)
        {
            if (!string.Equals(request.RedirectUrl, redirectUrl))
            {
                throw new Exception("Redirect URL supplied does not match that which was expected");
            }
        }

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        TokenEndpointAuthMethod tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        OAuth2ResponseMode defaultResponseMode = bankProfile.DefaultResponseMode;
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

        // Validate response mode
        if (request.ResponseMode != defaultResponseMode)
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
            consentAuthGetCustomBehaviour?.IdTokenNonceClaimIsPreviousValue ?? false;
        string nonce = nonceClaimIsInitialValue && consent.AuthContextNonce is not null
            ? consent.AuthContextNonce
            : authContextNonce;

        // Validate ID token including nonce
        string? requestObjectAudClaim = consentAuthGetCustomBehaviour?.AudClaim;
        string bankTokenIssuerClaim =
            requestObjectAudClaim ??
            issuerUrl;
        DateTimeOffset modified = _timeProvider.GetUtcNow();
        bool doNotValidateIdToken = consentAuthGetCustomBehaviour?.DoNotValidateIdToken ?? false;
        if (doNotValidateIdToken is false)
        {
            string jwksUri = bankRegistration.JwksUri;
            string? newExternalApiUserId = await _grantPost.ValidateIdTokenAuthEndpoint(
                request.RedirectData,
                consentAuthGetCustomBehaviour,
                jwksUri,
                customBehaviour?.JwksGet,
                bankTokenIssuerClaim,
                externalApiClientId,
                externalApiConsentId,
                nonce,
                supportsSca,
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

        // Valid ID token means nonce has been validated so we delete auth context to ensure nonce can only be used once
        authContext.UpdateIsDeleted(true, modified, modifiedBy);

        // Wrap remaining processing in try block to ensure DB changes persisted
        try
        {
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementProfileOverride);

            // Obtain token for consent
            JsonSerializerSettings? jsonSerializerSettings = null;
            TokenEndpointResponseAuthCodeGrant tokenEndpointResponse =
                await _grantPost.PostAuthCodeGrantAsync(
                    request.RedirectData.Code,
                    redirectUrl,
                    bankTokenIssuerClaim,
                    externalApiClientId,
                    externalApiConsentId,
                    consent.ExternalApiUserId,
                    nonce,
                    requestScope,
                    processedSoftwareStatementProfile.OBSealKey,
                    bankRegistration,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    supportsSca,
                    idTokenSubClaimType,
                    jsonSerializerSettings,
                    customBehaviour?.AuthCodeGrantPost,
                    customBehaviour?.JwksGet,
                    processedSoftwareStatementProfile.ApiClient);

            // Update consent with nonce, token
            consent.UpdateAuthContext(
                authContext.State,
                nonce,
                modified,
                modifiedBy);

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

            // Delete old stored access token if exists
            storedAccessToken?.UpdateIsDeleted(true, modified, modifiedBy);

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

            // Store new refresh token if available and different from old stored refresh token
            if (tokenEndpointResponse.RefreshToken is not null)
            {
                string? storedRefreshTokenValue = null;
                if (storedRefreshToken is not null)
                {
                    // Extract token
                    byte[] encryptionKey = _encryptionKeyInfo.GetEncryptionKey(storedRefreshToken.KeyId);
                    storedRefreshTokenValue =
                        storedRefreshToken
                            .GetRefreshToken(consentAssociatedData, encryptionKey);
                }

                if (tokenEndpointResponse.RefreshToken != storedRefreshTokenValue)
                {
                    // Delete old refresh token if exists
                    storedRefreshToken?.UpdateIsDeleted(true, modified, modifiedBy);

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
