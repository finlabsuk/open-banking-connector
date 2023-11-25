// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
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
    private readonly IEncryptionKeyInfo _encryptionKeyInfo;
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
        IMemoryCache memoryCache,
        IEncryptionKeyInfo encryptionKeyInfo)
    {
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _grantPost = grantPost;
        _instrumentationClient = instrumentationClient;
        _memoryCache = memoryCache;
        _encryptionKeyInfo = encryptionKeyInfo;
    }

    public async Task<string> GetAccessTokenAndUpdateConsent<TConsentEntity>(
        TConsentEntity consent,
        string bankIssuerUrl,
        string? requestScope,
        BankRegistration bankRegistration,
        AccessTokenEntity? storedAccessTokenEntity,
        RefreshTokenEntity? storedRefreshTokenEntity,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        OAuth2ResponseMode defaultResponseMode,
        IdTokenSubClaimType idTokenSubClaimType,
        GrantPostCustomBehaviour? refreshTokenGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string? modifiedBy)
        where TConsentEntity : BaseConsent
    {
        string? consentAssociatedData = consent.GetAssociatedData(bankRegistration);

        // Check nonce available
        string nonce =
            consent.AuthContextNonce ??
            throw new InvalidOperationException("No nonce is available for Consent.");

        async Task<AccessToken> GetNewAccessTokenAsync()
        {
            // Get stored refresh token, throw if doesn't exist
            if (storedRefreshTokenEntity is null)
            {
                throw new InvalidOperationException("No unexpired access token or refresh token is available.");
            }

            // Extract token
            byte[] encryptionKey = _encryptionKeyInfo.GetEncryptionKey(storedRefreshTokenEntity.KeyId);
            string storedRefreshToken =
                storedRefreshTokenEntity
                    .GetRefreshToken(consentAssociatedData, encryptionKey);

            // Obtain new refresh and access tokens
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementProfileOverride);
            string redirectUrl = processedSoftwareStatementProfile.GetRedirectUri(
                defaultResponseMode,
                bankRegistration.DefaultFragmentRedirectUri,
                bankRegistration.DefaultQueryRedirectUri);
            string externalApiClientId = bankRegistration.ExternalApiId;
            JsonSerializerSettings? jsonSerializerSettings = null;
            TokenEndpointResponseRefreshTokenGrant tokenEndpointResponse =
                await _grantPost.PostRefreshTokenGrantAsync(
                    storedRefreshToken,
                    bankRegistration.JwksUri,
                    bankIssuerUrl,
                    externalApiClientId,
                    bankRegistration.ExternalApiSecret,
                    consent.ExternalApiId,
                    consent.ExternalApiUserId,
                    nonce,
                    requestScope,
                    processedSoftwareStatementProfile.OBSealKey,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    supportsSca,
                    idTokenSubClaimType,
                    jsonSerializerSettings,
                    refreshTokenGrantPostCustomBehaviour,
                    jwksGetCustomBehaviour,
                    processedSoftwareStatementProfile.ApiClient);

            // Delete old access token (is expired else would have been used)
            DateTimeOffset modified = _timeProvider.GetUtcNow();
            if (storedAccessTokenEntity is not null)
            {
                storedAccessTokenEntity.UpdateIsDeleted(true, modified, modifiedBy);
            }

            // Conditionally store new access token
            var newAccessToken = new AccessToken(tokenEndpointResponse.AccessToken, tokenEndpointResponse.ExpiresIn);

            const int expiryThresholdForSaving = 24 * 60 * 60; // one day
            if (newAccessToken.ExpiresIn > expiryThresholdForSaving)
            {
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

            // Store new refresh token if different from old stored refresh token
            if (tokenEndpointResponse.RefreshToken != storedRefreshToken)
            {
                // Delete old refresh token
                storedRefreshTokenEntity.UpdateIsDeleted(true, modified, modifiedBy);

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

            return newAccessToken;
        }

        // Get or create cache entry
        string accessTokenOut =
            (await _memoryCache.GetOrCreateAsync(
                consent.GetCacheKey(),
                async cacheEntry =>
                {
                    // Use stored access token if unexpired, else delete
                    if (storedAccessTokenEntity is not null)
                    {
                        // Extract token
                        byte[] encryptionKey = _encryptionKeyInfo.GetEncryptionKey(storedAccessTokenEntity.KeyId);
                        AccessToken storedAccessToken =
                            storedAccessTokenEntity
                                .GetAccessToken(consentAssociatedData, encryptionKey);

                        // Calculate time since token stored
                        DateTimeOffset timeWhenStored = storedAccessTokenEntity.Created;
                        TimeSpan elapsedTime =
                            _timeProvider.GetUtcNow()
                                .Subtract(timeWhenStored); // subtract time when token stored

                        // Calculate remaining token duration
                        TimeSpan tokenRemainingDuration =
                            _grantPost.GetTokenAdjustedDuration(storedAccessToken.ExpiresIn)
                                .Subtract(elapsedTime);

                        // Return unexpired access token if available
                        if (tokenRemainingDuration > TimeSpan.Zero)
                        {
                            cacheEntry.AbsoluteExpirationRelativeToNow = tokenRemainingDuration;
                            return storedAccessToken.Token;
                        }
                    }

                    // Else get new access token
                    AccessToken response = await GetNewAccessTokenAsync();
                    cacheEntry.AbsoluteExpirationRelativeToNow =
                        _grantPost.GetTokenAdjustedDuration(response.ExpiresIn);

                    // Persist updates (this happens last so as not to happen if there are any previous errors)
                    await _dbSaveChangesMethod.SaveChangesAsync();

                    return response.Token;
                }))!;

        return accessTokenOut;
    }
}
