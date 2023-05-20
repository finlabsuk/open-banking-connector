// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class ConsentAccessTokenGet
{
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IMemoryCache _memoryCache;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
    private readonly ITimeProvider _timeProvider;

    public ConsentAccessTokenGet(
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IGrantPost grantPost,
        IInstrumentationClient instrumentationClient,
        IMemoryCache memoryCache)
    {
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _grantPost = grantPost;
        _instrumentationClient = instrumentationClient;
        _memoryCache = memoryCache;
    }

    public async Task<string> GetAccessTokenAndUpdateConsent<TConsentEntity>(
        TConsentEntity consent,
        string bankIssuerUrl,
        string? requestScope,
        BankRegistration bankRegistration,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        IdTokenSubClaimType idTokenSubClaimType,
        GrantPostCustomBehaviour? refreshTokenGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string? modifiedBy)
        where TConsentEntity : BaseConsent
    {
        // Get token
        AccessToken accessToken = consent.AccessToken;
        if (accessToken.Token is null)
        {
            throw new InvalidOperationException("No access token is available for Consent.");
        }

        // Check nonce available
        string nonce =
            consent.AuthContextNonce ??
            throw new InvalidOperationException("No nonce is available for Consent.");
        string externalApiClientId = bankRegistration.ExternalApiObject.ExternalApiId;

        async Task<TokenEndpointResponseRefreshTokenGrant> GetTokenAsync()
        {
            // Check that refresh token is available
            if (accessToken.RefreshToken is null)
            {
                throw new InvalidOperationException("Access token has expired and no refresh token is available.");
            }

            // Get new access token using refresh token 
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementProfileOverride);

            // Obtain token for consent
            string redirectUrl = bankRegistration.DefaultRedirectUri;
            JsonSerializerSettings? jsonSerializerSettings = null;
            TokenEndpointResponseRefreshTokenGrant tokenEndpointResponse =
                await _grantPost.PostRefreshTokenGrantAsync(
                    accessToken.RefreshToken,
                    redirectUrl,
                    bankIssuerUrl,
                    externalApiClientId,
                    consent.ExternalApiId,
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
                    refreshTokenGrantPostCustomBehaviour,
                    jwksGetCustomBehaviour,
                    processedSoftwareStatementProfile.ApiClient);

            // Update consent with token
            consent.UpdateAccessToken(
                tokenEndpointResponse.AccessToken,
                tokenEndpointResponse.ExpiresIn,
                tokenEndpointResponse.RefreshToken,
                _timeProvider.GetUtcNow(),
                modifiedBy);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();

            return tokenEndpointResponse;
        }

        // Get or create cache entry
        string consentType = consent switch
        {
            AccountAccessConsent => "aisp",
            DomesticPaymentConsent => "pisp_dom",
            DomesticVrpConsent => "vrp_dom",
            _ => throw new ArgumentOutOfRangeException(nameof(consent), consent, null)
        };
        string cacheKey = string.Join(":", "token", consentType, consent.Id.ToString());
        string accessTokenOut =
            (await _memoryCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    // Prefer stored access token if unexpired
                    if (consent.AccessToken.Token is not null)
                    {
                        // Calculate time since token stored
                        TimeSpan elapsedTime =
                            _timeProvider.GetUtcNow()
                                .Subtract(consent.AccessToken.Modified); // subtract time when token stored

                        // Calculate remaining token duration
                        TimeSpan tokenRemainingDuration =
                            _grantPost.GetTokenAdjustedDuration(consent.AccessToken.ExpiresIn)
                                .Subtract(elapsedTime);

                        // Return unexpired access token if available
                        if (tokenRemainingDuration > TimeSpan.Zero)
                        {
                            cacheEntry.AbsoluteExpirationRelativeToNow = tokenRemainingDuration;
                            return consent.AccessToken.Token;
                        }
                    }

                    // Else get new access token
                    TokenEndpointResponseRefreshTokenGrant response =
                        await GetTokenAsync();
                    cacheEntry.AbsoluteExpirationRelativeToNow =
                        _grantPost.GetTokenAdjustedDuration(response.ExpiresIn);
                    return response.AccessToken;
                }))!;

        return accessTokenOut;
    }
}
