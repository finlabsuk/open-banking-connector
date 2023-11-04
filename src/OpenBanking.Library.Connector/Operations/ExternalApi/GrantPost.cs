// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Jose;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using JsonWebKey = FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi.JsonWebKey;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal class GrantPost : IGrantPost
{
    // Non-MTLS client for accessing JwksUri
    private readonly IApiClient _apiClient;

    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ITimeProvider _timeProvider;

    public GrantPost(
        IApiClient apiClient,
        IInstrumentationClient instrumentationClient,
        IMemoryCache memoryCache,
        ITimeProvider timeProvider)
    {
        _apiClient = apiClient;
        _instrumentationClient = instrumentationClient;
        _memoryCache = memoryCache;
        _timeProvider = timeProvider;
    }

    public async Task<string?> ValidateIdTokenAuthEndpoint(
        OAuth2RedirectData redirectData,
        ConsentAuthGetCustomBehaviour? consentAuthGetCustomBehaviour,
        string jwksUri,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string expectedNonce,
        bool supportsSca,
        IdTokenSubClaimType idTokenSubClaimType,
        string? externalApiUserId)
    {
        bool jwksGetResponseHasNoRootProperty =
            jwksGetCustomBehaviour?.ResponseHasNoRootProperty ?? false;
        string idTokenEncoded = redirectData.IdToken;
        string idTokenDecoded = await DecodeIdTokenAsync(jwksUri, jwksGetResponseHasNoRootProperty, idTokenEncoded);

        // Deserialise IT token claims
        var jsonSerializerSettings =
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        IdTokenAuthEndpoint idToken =
            JsonConvert.DeserializeObject<IdTokenAuthEndpoint>(
                idTokenDecoded,
                jsonSerializerSettings) ??
            throw new Exception("Can't deserialise ID token.");
        bool doNotValidateIdTokenAcrClaim =
            consentAuthGetCustomBehaviour?.DoNotValidateIdTokenAcrClaim
            ?? false;
        bool idTokenMayNotHaveAcrClaim =
            consentAuthGetCustomBehaviour?.IdTokenMayNotHaveAcrClaim
            ?? false;
        ValidateIdTokenCommon(
            idToken,
            bankIssuerUrl,
            externalApiClientId,
            externalApiConsentId,
            expectedNonce,
            supportsSca,
            !idTokenMayNotHaveAcrClaim,
            !doNotValidateIdTokenAcrClaim);

        // Validate ID token subject claim
        string? outputExternalApiUserId = externalApiUserId; // unchanged by default
        switch (idTokenSubClaimType)
        {
            case IdTokenSubClaimType.EndUserId:
                if (externalApiUserId is null)
                {
                    if (string.IsNullOrEmpty(idToken.Subject))
                    {
                        throw new Exception("Subject from ID token is null or empty.");
                    }

                    outputExternalApiUserId = idToken.Subject;
                }
                else
                {
                    if (!string.Equals(idToken.Subject, externalApiUserId))
                    {
                        throw new Exception("Subject from ID token does not match user ID.");
                    }
                }

                break;
            case IdTokenSubClaimType.ConsentId:
                if (!string.Equals(idToken.Subject, externalApiConsentId))
                {
                    throw new Exception("Subject from ID token does not match consent ID.");
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(idTokenSubClaimType), idTokenSubClaimType, null);
        }

        string codeHash = ComputeHash(redirectData.Code);
        if (!string.Equals(idToken.CodeHash, codeHash))
        {
            throw new Exception("Code hash from ID token does not match code hash.");
        }

        string stateHash = ComputeHash(redirectData.State);
        if (!string.Equals(idToken.StateHash, stateHash))
        {
            throw new Exception("State hash from ID token does not match state hash.");
        }

        return outputExternalApiUserId;
    }

    public async Task<string> PostClientCredentialsGrantAsync(
        string? scope,
        OBSealKey obSealKey,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        string tokenEndpoint,
        string externalApiClientId,
        string? externalApiClientSecret,
        Guid bankRegistrationId,
        JsonSerializerSettings? jsonSerializerSettings,
        GrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour,
        IApiClient mtlsApiClient,
        JwsAlgorithm? jwsAlgorithm = null)
    {
        async Task<TokenEndpointResponseClientCredentialsGrant> GetTokenAsync()
        {
            var keyValuePairs = new Dictionary<string, string> { { "grant_type", "client_credentials" } };

            if (scope is not null)
            {
                keyValuePairs["scope"] = scope;
            }

            if (tokenEndpointAuthMethod is TokenEndpointAuthMethod.PrivateKeyJwt)
            {
                string jwt = CreateClientAssertionJwt(
                    obSealKey,
                    externalApiClientId,
                    tokenEndpoint,
                    _instrumentationClient,
                    jwsAlgorithm);

                // Add parameters
                keyValuePairs["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                keyValuePairs["client_assertion"] = jwt;
            }

            var response =
                await PostGrantAsync<TokenEndpointResponseClientCredentialsGrant>(
                    keyValuePairs,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    externalApiClientId,
                    externalApiClientSecret,
                    jsonSerializerSettings,
                    mtlsApiClient,
                    scope,
                    clientCredentialsGrantPostCustomBehaviour?.TokenTypeResponseStartsWithLowerCaseLetter ?? false,
                    clientCredentialsGrantPostCustomBehaviour?.ScopeResponseIsEmptyString ?? false);

            if (response.IdToken is not null)
            {
                throw new Exception("Unexpectedly received ID token with client credentials grant.");
            }

            return response;
        }

        // Get or create cache entry
        string cacheKey = string.Join(":", "token", "client", bankRegistrationId.ToString(), scope ?? "");
        string accessToken =
            (await _memoryCache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    TokenEndpointResponseClientCredentialsGrant response =
                        await GetTokenAsync();
                    // DateTimeOffset currentTime = _timeProvider.GetUtcNow();
                    // cacheEntry.AbsoluteExpiration =
                    //     currentTime.AddSeconds(response.ExpiresIn);
                    cacheEntry.AbsoluteExpirationRelativeToNow =
                        GetTokenAdjustedDuration(response.ExpiresIn);
                    return response.AccessToken;
                }))!;

        return accessToken;
    }

    public async Task<TokenEndpointResponseAuthCodeGrant> PostAuthCodeGrantAsync(
        string authCode,
        string redirectUrl,
        string bankIssuerUrl,
        string externalApiClientId,
        string? externalApiClientSecret,
        string externalApiConsentId,
        string? externalApiUserId,
        string expectedNonce,
        string? requestScope,
        OBSealKey obSealKey,
        string jwksUri,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        IdTokenSubClaimType idTokenSubClaimType,
        JsonSerializerSettings? jsonSerializerSettings,
        GrantPostCustomBehaviour? authCodeGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        IApiClient matlsApiClient)
    {
        var keyValuePairs = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "redirect_uri", redirectUrl },
            { "code", authCode }
        };

        if (tokenEndpointAuthMethod is
            TokenEndpointAuthMethod.PrivateKeyJwt)
        {
            string jwt = CreateClientAssertionJwt(
                obSealKey,
                externalApiClientId,
                tokenEndpoint,
                _instrumentationClient,
                null);

            // Add parameters
            keyValuePairs["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
            keyValuePairs["client_assertion"] = jwt;
        }

        var response = await PostGrantAsync<TokenEndpointResponseAuthCodeGrant>(
            keyValuePairs,
            tokenEndpointAuthMethod,
            tokenEndpoint,
            externalApiClientId,
            externalApiClientSecret,
            jsonSerializerSettings,
            matlsApiClient,
            requestScope,
            authCodeGrantPostCustomBehaviour?.TokenTypeResponseStartsWithLowerCaseLetter ?? false,
            authCodeGrantPostCustomBehaviour?.ScopeResponseIsEmptyString ?? false);

        // Check for refresh token
        bool allowNullRefreshTokenResponse = authCodeGrantPostCustomBehaviour?.AllowNullResponseRefreshToken ?? false;
        if (!allowNullRefreshTokenResponse &&
            response.RefreshToken is null)
        {
            throw new Exception("Did not receive refresh token when using auth code grant.");
        }

        bool doNotValidateIdToken = authCodeGrantPostCustomBehaviour?.DoNotValidateIdToken ?? false;
        if (doNotValidateIdToken is false)
        {
            await ValidateIdTokenTokenEndpoint(
                response.IdToken,
                response.AccessToken,
                authCodeGrantPostCustomBehaviour,
                jwksUri,
                jwksGetCustomBehaviour,
                bankIssuerUrl,
                externalApiClientId,
                externalApiConsentId,
                expectedNonce,
                supportsSca,
                idTokenSubClaimType,
                externalApiUserId);
        }

        return response;
    }

    public async Task<TokenEndpointResponseRefreshTokenGrant> PostRefreshTokenGrantAsync(
        string refreshToken,
        string jwksUri,
        string bankIssuerUrl,
        string externalApiClientId,
        string? externalApiClientSecret,
        string externalApiConsentId,
        string? externalApiUserId,
        string expectedNonce,
        string? requestScope,
        OBSealKey obSealKey,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        IdTokenSubClaimType idTokenSubClaimType,
        JsonSerializerSettings? jsonSerializerSettings,
        GrantPostCustomBehaviour? refreshTokenGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        IApiClient mtlsApiClient)
    {
        var keyValuePairs = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        };

        if (tokenEndpointAuthMethod is
            TokenEndpointAuthMethod.PrivateKeyJwt)
        {
            string jwt = CreateClientAssertionJwt(
                obSealKey,
                externalApiClientId,
                tokenEndpoint,
                _instrumentationClient,
                null);

            // Add parameters
            keyValuePairs["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
            keyValuePairs["client_assertion"] = jwt;
        }

        var response = await PostGrantAsync<TokenEndpointResponseRefreshTokenGrant>(
            keyValuePairs,
            tokenEndpointAuthMethod,
            tokenEndpoint,
            externalApiClientId,
            externalApiClientSecret,
            jsonSerializerSettings,
            mtlsApiClient,
            requestScope,
            refreshTokenGrantPostCustomBehaviour?.TokenTypeResponseStartsWithLowerCaseLetter ?? false,
            refreshTokenGrantPostCustomBehaviour?.ScopeResponseIsEmptyString ?? false);

        bool doNotValidateIdToken = refreshTokenGrantPostCustomBehaviour?.DoNotValidateIdToken ?? false;
        string? responseIdToken = response.IdToken;
        if (doNotValidateIdToken is false &&
            responseIdToken is not null)
        {
            await ValidateIdTokenTokenEndpoint(
                responseIdToken,
                response.AccessToken,
                refreshTokenGrantPostCustomBehaviour,
                jwksUri,
                jwksGetCustomBehaviour,
                bankIssuerUrl,
                externalApiClientId,
                externalApiConsentId,
                expectedNonce,
                supportsSca,
                idTokenSubClaimType,
                externalApiUserId);
        }

        return response;
    }

    public TimeSpan GetTokenAdjustedDuration(int expiresInSeconds)
    {
        const int tokenEarlyExpiryIntervalInSeconds =
            10; // margin to allow for time required to obtain token and later to present token
        int tokenExpiryRelativeToNow = expiresInSeconds - tokenEarlyExpiryIntervalInSeconds;
        if (tokenExpiryRelativeToNow < 0)
        {
            throw new InvalidOperationException("Negative token expiry value encountered.");
        }

        return TimeSpan.FromSeconds(tokenExpiryRelativeToNow);
    }

    private static string CreateClientAssertionJwt(
        OBSealKey obSealKey,
        string externalApiClientId,
        string tokenEndpoint,
        IInstrumentationClient instrumentationClient,
        JwsAlgorithm? jwsAlgorithm)
    {
        // Create JWT
        var claims = new
        {
            iss = externalApiClientId,
            sub = externalApiClientId,
            aud = tokenEndpoint,
            jti = Guid.NewGuid().ToString(),
            iat = DateTimeOffset.Now.ToUnixTimeSeconds(),
            exp = DateTimeOffset.UtcNow.AddSeconds(300).ToUnixTimeSeconds()
        };
        var jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        string payloadJson = JsonConvert.SerializeObject(
            claims,
            jsonSerializerSettings);
        string jwt = JwtFactory.CreateJwt(
            JwtFactory.DefaultJwtHeadersExcludingTyp(obSealKey.KeyId),
            payloadJson,
            obSealKey.Key,
            jwsAlgorithm);
        StringBuilder requestTraceSb = new StringBuilder()
            .AppendLine("#### JWT (Client Auth)")
            .Append(jwt);
        instrumentationClient.Trace(requestTraceSb.ToString());
        return jwt;
    }

    private void ValidateIdTokenCommon(
        IdTokenBase idToken,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string expectedNonce,
        bool supportsSca,
        bool idTokenHasAcrClaim,
        bool validateAcr)
    {
        if (idToken.Exp < DateTimeOffset.UtcNow)
        {
            throw new Exception("ID token has expired.");
        }

        if (!string.Equals(idToken.ConsentId, externalApiConsentId))
        {
            throw new Exception("Consent ID from ID token does not match expected consent ID.");
        }

        if (!string.Equals(idToken.Nonce, expectedNonce))
        {
            throw new Exception("Nonce from ID token does not match expected nonce.");
        }

        if (supportsSca && idToken.AuthTime is null)
        {
            throw new Exception("Auth time is null.");
        }

        if (idTokenHasAcrClaim)
        {
            if (idToken.Acr is null)
            {
                throw new Exception("Acr not provided in ID token.");
            }
        }

        if (idToken.Acr is not null)
        {
            if (supportsSca &&
                validateAcr &&
                idToken.Acr is not Acr.Sca)
            {
                throw new Exception("Acr from ID token does not match expected Acr.");
            }
        }

        if (!string.Equals(idToken.Issuer, bankIssuerUrl))
        {
            throw new Exception("Issuer from ID token does not match expected issuer.");
        }

        if (!string.Equals(idToken.Audience, externalApiClientId))
        {
            throw new Exception("Audience from ID token does not match expected audience.");
        }
    }

    private async Task<Jwks> GetJwksAsync(string jwksUrl, bool responseHasNoRootProperty)
    {
        var uri = new Uri(jwksUrl);

        HttpRequestMessage message = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(uri)
            .Create();

        Jwks jwks = responseHasNoRootProperty
            ? new Jwks { Keys = await message.RequestJsonAsync<List<JsonWebKey>>(_apiClient) }
            : await message.RequestJsonAsync<Jwks>(_apiClient);

        return jwks;
    }

    private async Task ValidateIdTokenTokenEndpoint(
        string idTokenEncoded,
        string accessToken,
        GrantPostCustomBehaviour? grantPostCustomBehaviour,
        string jwksUri,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string expectedNonce,
        bool supportsSca,
        IdTokenSubClaimType idTokenSubClaimType,
        string? externalApiUserId)
    {
        // Decode ID token
        bool jwksGetResponseHasNoRootProperty =
            jwksGetCustomBehaviour?.ResponseHasNoRootProperty ?? false;
        string idTokenDecoded = await DecodeIdTokenAsync(jwksUri, jwksGetResponseHasNoRootProperty, idTokenEncoded);

        // Deserialise IT token claims
        var jsonSerializerSettings =
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        IdTokenTokenEndpoint idToken =
            JsonConvert.DeserializeObject<IdTokenTokenEndpoint>(
                idTokenDecoded,
                jsonSerializerSettings) ??
            throw new Exception("Can't deserialise ID token.");

        bool doNotValidateIdTokenAcrClaim =
            grantPostCustomBehaviour?.DoNotValidateIdTokenAcrClaim
            ?? false;
        bool idTokenMayNotHaveAcrClaim =
            grantPostCustomBehaviour?.IdTokenMayNotHaveAcrClaim
            ?? false;
        ValidateIdTokenCommon(
            idToken,
            bankIssuerUrl,
            externalApiClientId,
            externalApiConsentId,
            expectedNonce,
            supportsSca,
            !idTokenMayNotHaveAcrClaim,
            !doNotValidateIdTokenAcrClaim);

        // Validate ID token subject claim
        switch (idTokenSubClaimType)
        {
            case IdTokenSubClaimType.EndUserId:
                if (externalApiUserId is null)
                {
                    throw new Exception("No user ID available to use in ID token validation.");
                }

                if (!string.Equals(idToken.Subject, externalApiUserId))
                {
                    throw new Exception("Subject from ID token does not match user ID.");
                }

                break;
            case IdTokenSubClaimType.ConsentId:
                if (!string.Equals(idToken.Subject, externalApiConsentId))
                {
                    throw new Exception("Subject from ID token does not match consent ID.");
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(idTokenSubClaimType), idTokenSubClaimType, null);
        }

        if (idToken.AccessTokenHash is not null)
        {
            string accessTokenHash = ComputeHash(accessToken);
            if (!string.Equals(idToken.AccessTokenHash, accessTokenHash))
            {
                throw new Exception("Access token hash from ID token does not match access token.");
            }
        }
    }

    private async Task<string> DecodeIdTokenAsync(
        string jwksUri,
        bool jwksGetResponseHasNoRootProperty,
        string idTokenEncoded)
    {
        string idTokenDecoded;
        // Get and validate ID token headers
        IDictionary<string, object>? headers = JWT.Headers(idTokenEncoded);
        string kId;
        string algName;
        try
        {
            kId = (string) headers["kid"];
        }
        catch
        {
            throw new Exception("Cannot get key ID from ID token header.");
        }

        try
        {
            algName = (string) headers["alg"];
        }
        catch
        {
            throw new Exception("Cannot get alg from ID token header.");
        }

        if (!string.Equals(algName, "PS256", StringComparison.Ordinal))
        {
            throw new Exception("Invalid alg in ID token header (should be PS256).");
        }

        // Get and validate Json Web Key
        Jwks jwks = await GetJwksAsync(
            jwksUri,
            jwksGetResponseHasNoRootProperty);
        JsonWebKey jsonWebKey =
            jwks.Keys.SingleOrDefault(x => x.KId == kId) ??
            throw new Exception($"No Jwks found with key ID {kId}");
        if ((jsonWebKey.Alg ?? JsonWebKeyAlg.Ps256) != JsonWebKeyAlg.Ps256)
        {
            throw new Exception("JSON web key has invalid alg.");
        }

        if (jsonWebKey.KeyType != JsonWebKeyType.Rsa)
        {
            throw new Exception("JSON web key has invalid key type.");
        }

        if (jsonWebKey.Use != JsonWebKeyUse.Signing)
        {
            throw new Exception("JSON web key has invalid use field.");
        }

        // Decode ID token including signature validation
        var rsaKey = new Jwk(jsonWebKey.RsaExponent, jsonWebKey.RsaModulus);
        try
        {
            idTokenDecoded = JWT.Decode(
                idTokenEncoded,
                rsaKey,
                JwsAlgorithm.PS256);
        }
        catch
        {
            throw new Exception("Invalid ID token signature.");
        }

        return idTokenDecoded;
    }

    private static string ComputeHash(string input)
    {
        using var mySha = SHA256.Create();
        byte[] hashValue = mySha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Base64UrlEncoder.Encode(hashValue, 0, 16);
    }

    private async Task<TokenEndpointResponse> PostGrantAsync<TokenEndpointResponse>(
        Dictionary<string, string> keyValuePairs,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        string tokenEndpoint,
        string externalApiClientId,
        string? externalApiClientSecret,
        JsonSerializerSettings? responseJsonSerializerSettings,
        IApiClient mtlsApiClient,
        string? requestScope,
        bool tokenTypeResponseStartsWithLowerCaseLetter,
        bool expectedScopeResponseIsEmptyString)
        where TokenEndpointResponse : TokenEndpointResponseBase
    {
        // POST request
        var uri = new Uri(tokenEndpoint);
        IPostRequestProcessor<Dictionary<string, string>> postRequestProcessor =
            new AuthGrantPostRequestProcessor<Dictionary<string, string>>(
                externalApiClientId,
                externalApiClientSecret,
                tokenEndpointAuthMethod);
        var response = await postRequestProcessor.PostAsync<TokenEndpointResponse>(
            uri,
            keyValuePairs,
            null,
            responseJsonSerializerSettings,
            mtlsApiClient);

        // Check token endpoint response
        string expectedTokenTypeResponse = tokenTypeResponseStartsWithLowerCaseLetter ? "bearer" : "Bearer";

        if (!string.Equals(response.TokenType, expectedTokenTypeResponse, StringComparison.Ordinal))
        {
            throw new InvalidDataException("Access token received does not have token type equal to Bearer.");
        }

        // Ensure received token scope when provided matches that requested
        if (requestScope is not null)
        {
            if (response.Scope is not null)
            {
                if (expectedScopeResponseIsEmptyString)
                {
                    if (response.Scope != string.Empty)
                    {
                        throw new Exception("Received scope for access token unexpectedly non-empty.");
                    }
                }
                else
                {
                    IOrderedEnumerable<string> requestScopeOrdered = requestScope.Split(" ").OrderBy(t => t);
                    IOrderedEnumerable<string> responseScopeOrdered = response.Scope.Split(" ").OrderBy(t => t);
                    if (!requestScopeOrdered.SequenceEqual(responseScopeOrdered))
                    {
                        throw new Exception("Requested and received scope for access token differ.");
                    }
                }
            }
        }
        else
        {
            if (response.Scope is not null)
            {
                throw new Exception("Received scope for access token but none requested.");
            }
        }

        return response;
    }
}
