// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal delegate Task<AccessTokenEntity?> GetAccessTokenDelegate(Guid consentId, bool dbTracking);

internal delegate Task<RefreshTokenEntity?> GetRefreshTokenDelegate(Guid consentId, bool dbTracking);

internal class ConsentAccessTokenGet
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> LockDictionary = new();

    private readonly IDbMethods _dbSaveChangesMethod;
    private readonly IEncryptionKeyDescription _encryptionKeyInfo;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ITimeProvider _timeProvider;

    public ConsentAccessTokenGet(
        IDbMethods dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IGrantPost grantPost,
        IInstrumentationClient instrumentationClient,
        IMemoryCache memoryCache,
        IEncryptionKeyDescription encryptionKeyInfo)
    {
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
        string defaultRequestScope,
        BankRegistrationEntity bankRegistration,
        GetAccessTokenDelegate getAccessToken,
        GetRefreshTokenDelegate getRefreshToken,
        ExternalApiSecretEntity? externalApiSecretEntity,
        bool useOpenIdConnect,
        IApiClient apiClient,
        OBSealKey obSealKey,
        bool supportsSca,
        BankProfileEnum bankProfile,
        IdTokenSubClaimType idTokenSubClaimType,
        RefreshTokenGrantPostCustomBehaviour? refreshTokenGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string? modifiedBy)
        where TConsentEntity : BaseConsent
    {
        string consentAssociatedData = consent.GetAssociatedData(bankRegistration);
        string bankRegistrationAssociatedData = bankRegistration.GetAssociatedData();
        string tokenEndpoint = bankRegistration.TokenEndpoint;
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod = bankRegistration.TokenEndpointAuthMethod;

        // Check nonce available
        string nonce =
            consent.AuthContextNonce ??
            throw new InvalidOperationException(
                "No nonce is available for consent. " +
                "The most likely cause is that the consent has not been successfully authorised.");

        async Task<string> GetAccessTokenAsync(ICacheEntry cacheEntry)
        {
            // Load stored access token
            AccessTokenEntity? storedAccessTokenEntity = await getAccessToken(consent.Id, true);

            // Use stored access token if unexpired, else delete
            if (storedAccessTokenEntity is not null)
            {
                // Extract token
                byte[] encryptionKey =
                    await _encryptionKeyInfo.GetEncryptionKey(storedAccessTokenEntity.EncryptionKeyDescriptionId);
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

                // Delete expired access token
                storedAccessTokenEntity.UpdateIsDeleted(true, _timeProvider.GetUtcNow(), modifiedBy);
            }

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
                        await _encryptionKeyInfo.GetEncryptionKey(externalApiSecretEntity.EncryptionKeyDescriptionId));
            }

            // Load stored refresh token
            RefreshTokenEntity? storedRefreshTokenEntity = await getRefreshToken(consent.Id, true);
            if (storedRefreshTokenEntity is null)
            {
                throw new InvalidOperationException("No unexpired access token or refresh token is available.");
            }

            // Extract token
            string storedRefreshToken =
                storedRefreshTokenEntity
                    .GetRefreshToken(
                        consentAssociatedData,
                        await _encryptionKeyInfo.GetEncryptionKey(storedRefreshTokenEntity.EncryptionKeyDescriptionId));

            // POST refresh token grant
            string externalApiClientId = bankRegistration.ExternalApiId;
            JsonSerializerSettings? jsonSerializerSettings = null;

            string requestScope = refreshTokenGrantPostCustomBehaviour?.Scope ?? defaultRequestScope;
            if (useOpenIdConnect)
            {
                requestScope = "openid " + requestScope;
            }
            TokenEndpointResponse tokenEndpointResponse =
                await _grantPost.PostRefreshTokenGrantAsync(
                    storedRefreshToken,
                    bankIssuerUrl,
                    externalApiClientId,
                    clientSecret,
                    consent.ExternalApiId,
                    consent.ExternalApiUserId,
                    nonce,
                    requestScope,
                    obSealKey,
                    bankRegistration.JwksUri,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    supportsSca,
                    bankProfile,
                    idTokenSubClaimType,
                    jsonSerializerSettings,
                    refreshTokenGrantPostCustomBehaviour,
                    jwksGetCustomBehaviour,
                    apiClient);

            // Conditionally store new access token
            var newAccessToken = new AccessToken(tokenEndpointResponse.AccessToken, tokenEndpointResponse.ExpiresIn);
            DateTimeOffset modified = _timeProvider.GetUtcNow();
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
                Guid? currentKeyId = _encryptionKeyInfo.GetCurrentKeyId();
                newAccessTokenObject.UpdateAccessToken(
                    newAccessToken,
                    consentAssociatedData,
                    await _encryptionKeyInfo.GetEncryptionKey(currentKeyId),
                    modified,
                    modifiedBy,
                    currentKeyId);
            }

            // Replace refresh token if rotated (i.e. new one provided)
            if (tokenEndpointResponse.RefreshToken is not null)
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
                Guid? currentKeyId = _encryptionKeyInfo.GetCurrentKeyId();
                newRefreshTokenObject.UpdateRefreshToken(
                    tokenEndpointResponse.RefreshToken,
                    consentAssociatedData,
                    await _encryptionKeyInfo.GetEncryptionKey(currentKeyId),
                    modified,
                    modifiedBy,
                    currentKeyId);
            }

            cacheEntry.AbsoluteExpirationRelativeToNow =
                _grantPost.GetTokenAdjustedDuration(newAccessToken.ExpiresIn);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();

            return newAccessToken.Token;
        }

        // Get or create cache entry
        string cacheKey = consent.GetCacheKey();
        string accessTokenOut =
            (await _memoryCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    SemaphoreSlim tokenRetrievalLock = LockDictionary.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

                    if (!await tokenRetrievalLock.WaitAsync(TimeSpan.FromSeconds(8)))
                    {
                        throw new TimeoutException(
                            "Token retrieval reached timeout waiting for other threads to release lock.");
                    }
                    try
                    {
                        // Get access token
                        return await GetAccessTokenAsync(cacheEntry);
                    }
                    finally
                    {
                        tokenRetrievalLock.Release();
                        if (tokenRetrievalLock.CurrentCount == 1)
                        {
                            if (!LockDictionary.TryRemove(cacheKey, out _))
                            {
                                throw new InvalidOperationException("This thread cannot release token retrieval lock");
                            }
                        }
                    }
                }))!;

        return accessTokenOut;
    }
}
