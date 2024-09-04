// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal delegate Task<AccessTokenEntity?> GetAccessTokenDelegate(Guid consentId, bool dbTracking);

internal delegate Task<RefreshTokenEntity?> GetRefreshTokenDelegate(Guid consentId, bool dbTracking);

internal class ConsentAccessTokenGet
{
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IEncryptionKeyInfo _encryptionKeyInfo;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ITimeProvider _timeProvider;

    public ConsentAccessTokenGet(
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IGrantPost grantPost,
        IInstrumentationClient instrumentationClient,
        IMemoryCache memoryCache,
        IEncryptionKeyInfo encryptionKeyInfo)
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
        string tokenEndpoint,
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
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod = bankRegistration.TokenEndpointAuthMethod;

        // Check nonce available
        string nonce =
            consent.AuthContextNonce ??
            throw new InvalidOperationException("No nonce is available for Consent.");

        async Task<AccessToken> GetNewAccessTokenAsync()
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
                        _encryptionKeyInfo.GetEncryptionKey(storedRefreshTokenEntity.KeyId));

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
                string? currentKeyId = _encryptionKeyInfo.GetCurrentKeyId();
                newAccessTokenObject.UpdateAccessToken(
                    newAccessToken,
                    consentAssociatedData,
                    _encryptionKeyInfo.GetEncryptionKey(currentKeyId),
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
                    // Load stored access token
                    AccessTokenEntity? storedAccessTokenEntity = await getAccessToken(consent.Id, true);

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

                        // Delete expired access token
                        DateTimeOffset modified = _timeProvider.GetUtcNow();
                        storedAccessTokenEntity.UpdateIsDeleted(true, modified, modifiedBy);
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
