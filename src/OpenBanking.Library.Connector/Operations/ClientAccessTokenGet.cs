// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class ClientAccessTokenGet
{
    private readonly IEncryptionKeyInfo _encryptionKeyInfo;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ITimeProvider _timeProvider;

    public ClientAccessTokenGet(
        ITimeProvider timeProvider,
        IGrantPost grantPost,
        IInstrumentationClient instrumentationClient,
        IMemoryCache memoryCache,
        IEncryptionKeyInfo encryptionKeyInfo)
    {
        _timeProvider = timeProvider;
        _grantPost = grantPost;
        _instrumentationClient = instrumentationClient;
        _memoryCache = memoryCache;
        _encryptionKeyInfo = encryptionKeyInfo;
    }

    public async Task<string> GetAccessToken(
        string? requestScope,
        OBSealKey obSealKey,
        BankRegistrationEntity bankRegistration,
        ExternalApiSecretEntity? externalApiSecretEntity,
        ClientCredentialsGrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour,
        IApiClient mtlsApiClient,
        BankProfileEnum? bankProfileEnum)
    {
        string bankRegistrationAssociatedData = bankRegistration.GetAssociatedData();
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod = bankRegistration.TokenEndpointAuthMethod;
        string tokenEndpoint = bankRegistration.TokenEndpoint;

        async Task<TokenEndpointResponse> GetAccessTokenAsync()
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
                byte[] encryptionKey = _encryptionKeyInfo.GetEncryptionKey(externalApiSecretEntity.KeyId);
                clientSecret = externalApiSecretEntity
                    .GetClientSecret(bankRegistrationAssociatedData, encryptionKey);
            }

            // POST client credentials grant
            string externalApiClientId = bankRegistration.ExternalApiId;
            JsonSerializerSettings? jsonSerializerSettings = null;
            TokenEndpointResponse tokenEndpointResponse =
                await _grantPost.PostClientCredentialsGrantAsync(
                    requestScope,
                    obSealKey,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    externalApiClientId,
                    clientSecret,
                    jsonSerializerSettings,
                    clientCredentialsGrantPostCustomBehaviour,
                    mtlsApiClient,
                    bankProfileEnum);

            return tokenEndpointResponse;
        }

        // Get or create cache entry
        string accessToken =
            (await _memoryCache.GetOrCreateAsync(
                bankRegistration.GetCacheKey(requestScope),
                async cacheEntry =>
                {
                    // Else get new access token
                    TokenEndpointResponse response = await GetAccessTokenAsync();
                    cacheEntry.AbsoluteExpirationRelativeToNow =
                        _grantPost.GetTokenAdjustedDuration(response.ExpiresIn);

                    return response.AccessToken;
                }))!;

        return accessToken;
    }
}
