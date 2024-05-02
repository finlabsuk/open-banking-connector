// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
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
        IdTokenProcessingCustomBehaviour? idTokenProcessingCustomBehaviour,
        string jwksUri,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string expectedNonce,
        bool supportsSca,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        IdTokenSubClaimType idTokenSubClaimType,
        string? externalApiUserId)
    {
        // Decode and deserialise ID token
        var idToken = await DeserialiseIdToken<IdTokenAuthEndpoint>(
            redirectData.IdToken,
            idTokenProcessingCustomBehaviour,
            jwksUri,
            bankProfileForTppReportingMetrics,
            jwksGetCustomBehaviour);

        ValidateIdTokenCommon(
            idToken,
            bankIssuerUrl,
            externalApiClientId,
            externalApiConsentId,
            expectedNonce,
            supportsSca,
            idTokenProcessingCustomBehaviour);

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
        string? requestScope,
        OBSealKey obSealKey,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        string tokenEndpoint,
        string externalApiClientId,
        string? externalApiClientSecret,
        string cacheKeyId,
        JsonSerializerSettings? jsonSerializerSettings,
        ClientCredentialsGrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour,
        IApiClient mtlsApiClient,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        Dictionary<string, JsonNode?>? extraClientAssertionClaims = null,
        bool includeClientIdWithPrivateKeyJwt = false,
        JwsAlgorithm? jwsAlgorithm = null)
    {
        async Task<TokenEndpointResponseClientCredentialsGrant> GetTokenAsync()
        {
            var keyValuePairs = new Dictionary<string, string> { { "grant_type", "client_credentials" } };

            if (requestScope is not null)
            {
                keyValuePairs["scope"] = requestScope;
            }

            if (tokenEndpointAuthMethod is TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt)
            {
                string jwt = CreateClientAssertionJwt(
                    obSealKey,
                    externalApiClientId,
                    tokenEndpoint,
                    _instrumentationClient,
                    jwsAlgorithm,
                    extraClientAssertionClaims ?? new Dictionary<string, JsonNode?>());

                // Add parameters
                keyValuePairs["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                keyValuePairs["client_assertion"] = jwt;

                if (includeClientIdWithPrivateKeyJwt)
                {
                    keyValuePairs["client_id"] = externalApiClientId;
                }
            }

            var response =
                await PostGrantAsync<TokenEndpointResponseClientCredentialsGrant>(
                    keyValuePairs,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    externalApiClientId,
                    externalApiClientSecret,
                    jsonSerializerSettings,
                    bankProfileForTppReportingMetrics,
                    mtlsApiClient,
                    requestScope,
                    clientCredentialsGrantPostCustomBehaviour);

            return response;
        }

        // Get or create cache entry
        string cacheKey = string.Join(":", "token", "client", cacheKeyId, requestScope ?? "");
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
        string requestScope,
        OBSealKey obSealKey,
        string jwksUri,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        bool expectRefreshToken,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        IdTokenSubClaimType idTokenSubClaimType,
        JsonSerializerSettings? jsonSerializerSettings,
        AuthCodeGrantPostCustomBehaviour? authCodeGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        IApiClient mtlsApiClient)
    {
        var keyValuePairs = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "redirect_uri", redirectUrl },
            { "code", authCode }
        };

        if (tokenEndpointAuthMethod is
            TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt)
        {
            string jwt = CreateClientAssertionJwt(
                obSealKey,
                externalApiClientId,
                tokenEndpoint,
                _instrumentationClient,
                null,
                new Dictionary<string, JsonNode?>());

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
            bankProfileForTppReportingMetrics,
            mtlsApiClient,
            requestScope,
            authCodeGrantPostCustomBehaviour);

        // Validate response refresh token
        if (expectRefreshToken)
        {
            bool expectedResponseRefreshTokenMayBeAbsent =
                authCodeGrantPostCustomBehaviour?.ExpectedResponseRefreshTokenMayBeAbsent ?? false;
            if (!expectedResponseRefreshTokenMayBeAbsent &&
                response.RefreshToken is null)
            {
                throw new Exception("Expected and did not receive refresh token when using auth code grant.");
            }
        }
        else
        {
            bool unexpectedResponseRefreshTokenMayBePresent =
                authCodeGrantPostCustomBehaviour?.UnexpectedResponseRefreshTokenMayBePresent ?? false;
            if (!unexpectedResponseRefreshTokenMayBePresent &&
                response.RefreshToken is not null)
            {
                throw new Exception("Did not expect but received refresh token when using auth code grant.");
            }
        }

        // Validate response ID token
        IdTokenProcessingCustomBehaviour? idTokenProcessingCustomBehaviour =
            authCodeGrantPostCustomBehaviour?.IdTokenProcessingCustomBehaviour;
        bool doNotValidateIdToken =
            idTokenProcessingCustomBehaviour?.DoNotValidateIdToken ?? false;
        if (doNotValidateIdToken is false)
        {
            await ValidateIdTokenTokenEndpoint(
                response.IdToken,
                response.AccessToken,
                idTokenProcessingCustomBehaviour,
                jwksUri,
                jwksGetCustomBehaviour,
                bankIssuerUrl,
                externalApiClientId,
                externalApiConsentId,
                expectedNonce,
                bankProfileForTppReportingMetrics,
                supportsSca,
                idTokenSubClaimType,
                externalApiUserId);
        }

        return response;
    }

    public async Task<TokenEndpointResponseRefreshTokenGrant> PostRefreshTokenGrantAsync(
        string refreshToken,
        string bankIssuerUrl,
        string externalApiClientId,
        string? externalApiClientSecret,
        string externalApiConsentId,
        string? externalApiUserId,
        string expectedNonce,
        string refreshTokenScope,
        OBSealKey obSealKey,
        string jwksUri,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        IdTokenSubClaimType idTokenSubClaimType,
        JsonSerializerSettings? jsonSerializerSettings,
        RefreshTokenGrantPostCustomBehaviour? refreshTokenGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        IApiClient mtlsApiClient)
    {
        var keyValuePairs = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        };

        if (tokenEndpointAuthMethod is
            TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt)
        {
            string jwt = CreateClientAssertionJwt(
                obSealKey,
                externalApiClientId,
                tokenEndpoint,
                _instrumentationClient,
                null,
                new Dictionary<string, JsonNode?>());

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
            bankProfileForTppReportingMetrics,
            mtlsApiClient,
            refreshTokenScope,
            refreshTokenGrantPostCustomBehaviour);

        // Validate response ID token
        string? responseIdToken = string.IsNullOrEmpty(response.IdToken) ? null : response.IdToken;
        bool idTokenMayBeAbsent = refreshTokenGrantPostCustomBehaviour?.IdTokenMayBeAbsent ?? false;
        if (!idTokenMayBeAbsent &&
            responseIdToken is null)
        {
            throw new Exception("Expected but did not receive ID token when using refresh token grant.");
        }

        IdTokenProcessingCustomBehaviour? idTokenProcessingCustomBehaviour =
            refreshTokenGrantPostCustomBehaviour?.IdTokenProcessingCustomBehaviour;
        bool doNotValidateIdToken =
            idTokenProcessingCustomBehaviour?.DoNotValidateIdToken ?? false;
        if (responseIdToken is not null &&
            doNotValidateIdToken is false)
        {
            await ValidateIdTokenTokenEndpoint(
                responseIdToken,
                response.AccessToken,
                idTokenProcessingCustomBehaviour,
                jwksUri,
                jwksGetCustomBehaviour,
                bankIssuerUrl,
                externalApiClientId,
                externalApiConsentId,
                expectedNonce,
                bankProfileForTppReportingMetrics,
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
        JwsAlgorithm? jwsAlgorithm,
        Dictionary<string, JsonNode?> extraClaims)
    {
        ArgumentNullException.ThrowIfNull(extraClaims);

        // Create JWT
        var claims = new JsonObject(extraClaims)
        {
            ["iss"] = externalApiClientId,
            ["sub"] = externalApiClientId,
            ["aud"] = tokenEndpoint,
            ["jti"] = Guid.NewGuid().ToString(),
            ["iat"] = DateTimeOffset.Now.ToUnixTimeSeconds(),
            ["exp"] = DateTimeOffset.UtcNow.AddSeconds(300).ToUnixTimeSeconds()
        };
        string payloadJson = claims.ToJsonString();
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
        IdTokenProcessingCustomBehaviour? idTokenProcessingCustomBehaviour)
    {
        if (idToken.Exp < DateTimeOffset.UtcNow)
        {
            throw new Exception("ID token has expired.");
        }

        bool idTokenMayNotHaveConsentIdClaim =
            idTokenProcessingCustomBehaviour?.IdTokenMayNotHaveConsentIdClaim ?? false;
        if (!idTokenMayNotHaveConsentIdClaim &&
            idToken.ConsentId is null)
        {
            throw new Exception("Consent ID not provided in ID token.");
        }
        if (idToken.ConsentId is not null &&
            !string.Equals(idToken.ConsentId, externalApiConsentId))
        {
            throw new Exception("Consent ID from ID token does not match expected consent ID.");
        }

        if (!string.Equals(idToken.Nonce, expectedNonce))
        {
            throw new Exception("Nonce from ID token does not match expected nonce.");
        }

        bool idTokenMayNotHaveAuthTimeClaim = idTokenProcessingCustomBehaviour?.IdTokenMayNotHaveAuthTimeClaim ?? false;
        if (!idTokenMayNotHaveAuthTimeClaim &&
            idToken.AuthTime is null)
        {
            throw new Exception("Auth time not provided in ID token.");
        }

        bool idTokenMayNotHaveAcrClaim =
            idTokenProcessingCustomBehaviour?.IdTokenMayNotHaveAcrClaim
            ?? false;
        if (!idTokenMayNotHaveAcrClaim &&
            idToken.Acr is null)
        {
            throw new Exception("Acr not provided in ID token.");
        }

        bool doNotValidateIdTokenAcrClaim =
            idTokenProcessingCustomBehaviour?.DoNotValidateIdTokenAcrClaim
            ?? false;
        if (idToken.Acr is not null)
        {
            if (supportsSca &&
                !doNotValidateIdTokenAcrClaim &&
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

    private async Task<Jwks> GetJwksAsync(
        string jwksUrl,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        bool responseHasNoRootProperty)
    {
        var uri = new Uri(jwksUrl);

        HttpRequestMessage message = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(uri)
            .Create();

        TppReportingRequestInfo? tppReportingRequestInfo = bankProfileForTppReportingMetrics is not null
            ? new TppReportingRequestInfo
            {
                EndpointDescription = "GET {JwksUri}",
                BankProfile = bankProfileForTppReportingMetrics.Value
            }
            : null;

        Jwks jwks = responseHasNoRootProperty
            ? new Jwks
            {
                Keys = (await message.SendExpectingJsonResponseAsync<List<JsonWebKey>>(
                    _apiClient,
                    tppReportingRequestInfo)).response
            }
            : (await message.SendExpectingJsonResponseAsync<Jwks>(_apiClient, tppReportingRequestInfo)).response;

        return jwks;
    }

    private async Task ValidateIdTokenTokenEndpoint(
        string idTokenEncoded,
        string accessToken,
        IdTokenProcessingCustomBehaviour? idTokenProcessingCustomBehaviour,
        string jwksUri,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string expectedNonce,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        bool supportsSca,
        IdTokenSubClaimType idTokenSubClaimType,
        string? externalApiUserId)
    {
        // Decode and deserialise ID token
        var idToken = await DeserialiseIdToken<IdTokenTokenEndpoint>(
            idTokenEncoded,
            idTokenProcessingCustomBehaviour,
            jwksUri,
            bankProfileForTppReportingMetrics,
            jwksGetCustomBehaviour);

        ValidateIdTokenCommon(
            idToken,
            bankIssuerUrl,
            externalApiClientId,
            externalApiConsentId,
            expectedNonce,
            supportsSca,
            idTokenProcessingCustomBehaviour);

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

    private async Task<TIdToken> DeserialiseIdToken<TIdToken>(
        string idTokenEncoded,
        IdTokenProcessingCustomBehaviour? idTokenProcessingCustomBehaviour,
        string jwksUri,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour)
    {
        // Decode ID token
        bool jwksGetResponseHasNoRootProperty =
            jwksGetCustomBehaviour?.ResponseHasNoRootProperty ?? false;
        string idTokenDecoded = await DecodeIdTokenAsync(
            jwksUri,
            jwksGetResponseHasNoRootProperty,
            idTokenEncoded,
            bankProfileForTppReportingMetrics);

        // Deserialise IT token claims
        var optionsDict = new Dictionary<JsonConverterLabel, int>();
        DateTimeOffsetUnixConverterEnum? idTokenExpirationTimeClaimJsonConverter =
            idTokenProcessingCustomBehaviour?.IdTokenExpirationTimeClaimJsonConverter;
        if (idTokenExpirationTimeClaimJsonConverter is not null)
        {
            optionsDict.Add(
                JsonConverterLabel.IdTokenExpirationTimeClaim,
                (int) idTokenExpirationTimeClaimJsonConverter);
        }
        var jsonSerializerSettings =
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Context = new StreamingContext(
                    StreamingContextStates.All,
                    optionsDict)
            };

        TIdToken idToken =
            JsonConvert.DeserializeObject<TIdToken>(
                idTokenDecoded,
                jsonSerializerSettings) ??
            throw new Exception("Can't deserialise ID token.");
        return idToken;
    }

    private async Task<string> DecodeIdTokenAsync(
        string jwksUri,
        bool jwksGetResponseHasNoRootProperty,
        string idTokenEncoded,
        BankProfileEnum? bankProfileForTppReportingMetrics)
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
            bankProfileForTppReportingMetrics,
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
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        string tokenEndpoint,
        string externalApiClientId,
        string? externalApiClientSecret,
        JsonSerializerSettings? responseJsonSerializerSettings,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        IApiClient mtlsApiClient,
        string? implicitOrExplicitRequestScope,
        GrantPostCustomBehaviour? grantPostCustomBehaviour)
        where TokenEndpointResponse : TokenEndpointResponseBase
    {
        // POST grant to token endpoint
        var uri = new Uri(tokenEndpoint);
        IPostRequestProcessor<Dictionary<string, string>> postRequestProcessor =
            new AuthGrantPostRequestProcessor<Dictionary<string, string>>(
                externalApiClientId,
                externalApiClientSecret,
                tokenEndpointAuthMethod);
        TppReportingRequestInfo? tppReportingRequestInfo = bankProfileForTppReportingMetrics is not null
            ? new TppReportingRequestInfo
            {
                EndpointDescription = "POST {TokenEndpoint}",
                BankProfile = bankProfileForTppReportingMetrics.Value
            }
            : null;
        (TokenEndpointResponse response, _) = await postRequestProcessor.PostAsync<TokenEndpointResponse>(
            uri,
            null,
            keyValuePairs,
            tppReportingRequestInfo,
            null,
            responseJsonSerializerSettings,
            mtlsApiClient);

        // Validate response "token_type"
        bool responseTokenTypeCaseMayBeIncorrect =
            grantPostCustomBehaviour?.ResponseTokenTypeCaseMayBeIncorrect ?? false;
        StringComparison stringComparison = responseTokenTypeCaseMayBeIncorrect
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        if (!string.Equals(response.TokenType, "Bearer", stringComparison))
        {
            throw new InvalidDataException("Received access token with token type not equal to Bearer.");
        }

        // Validate response "scope"
        bool responseScopeMayIncludeExtraValues =
            grantPostCustomBehaviour?.ResponseScopeMayIncludeExtraValues ?? false;
        if (string.IsNullOrEmpty(implicitOrExplicitRequestScope))
        {
            // Check for unexpected value(s)
            if (string.IsNullOrEmpty(response.Scope))
            {
                throw new Exception("Received access token without scope and unable to infer scope.");
            }
        }
        else
        {
            // If response scope provided, check it. (Missing response scope implies value equal to requested scope.)
            if (!string.IsNullOrEmpty(response.Scope))
            {
                string[] expectedScopeArray = implicitOrExplicitRequestScope.Split(" ");
                string[] responseScopeArray = response.Scope.Split(" ");

                // Check for missing values
                string[] missingScopeValues = expectedScopeArray.Except(responseScopeArray).ToArray();
                if (missingScopeValues.Any())
                {
                    throw new Exception($"Access token scope does not contain expected values {missingScopeValues}.");
                }

                // Check for extra values
                string[] extraScopeValues = responseScopeArray.Except(expectedScopeArray).ToArray();
                if (!responseScopeMayIncludeExtraValues &&
                    extraScopeValues.Any())
                {
                    throw new Exception($"Access token scope contains extra values {extraScopeValues}.");
                }
            }
        }

        return response;
    }
}
