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
using Jose;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using JsonWebKey = FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi.JsonWebKey;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal class GrantPost : IGrantPost
    {
        // Non-MTLS client for accessing JwksUri
        private readonly IApiClient _apiClient;

        public GrantPost(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<string?> ValidateIdTokenAuthEndpoint(
            OAuth2RedirectData redirectData,
            ConsentAuthGetCustomBehaviour? consentAuthGetCustomBehaviour,
            string jwksUri,
            JwksGetCustomBehaviour? jwksGetCustomBehaviour,
            string bankIssuerUrl,
            string externalApiClientId,
            string externalApiConsentId,
            string nonce,
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
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

            IdTokenAuthEndpoint idToken =
                JsonConvert.DeserializeObject<IdTokenAuthEndpoint>(
                    idTokenDecoded,
                    jsonSerializerSettings) ??
                throw new Exception("Can't deserialise ID token.");

            ValidateIdTokenCommon(
                idToken,
                bankIssuerUrl,
                externalApiClientId,
                externalApiConsentId,
                nonce,
                supportsSca);

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

        public async Task<TokenEndpointResponseClientCredentialsGrant> PostClientCredentialsGrantAsync(
            string? scope,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            BankRegistration bankRegistration,
            string tokenEndpoint,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient mtlsApiClient,
            IInstrumentationClient instrumentationClient)
        {
            var keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            if (scope is not null)
            {
                keyValuePairs["scope"] = scope;
            }

            if (bankRegistration.TokenEndpointAuthMethod is
                TokenEndpointAuthMethod.PrivateKeyJwt)
            {
                string jwt = CreateClientAssertionJwt(
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    tokenEndpoint,
                    instrumentationClient);

                // Add parameters
                keyValuePairs["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                keyValuePairs["client_assertion"] = jwt;
            }

            var response = await PostGrantAsync<TokenEndpointResponseClientCredentialsGrant>(
                keyValuePairs,
                bankRegistration,
                jsonSerializerSettings,
                mtlsApiClient,
                scope,
                bankRegistration.BankNavigation.CustomBehaviour?.ClientCredentialsGrantPost
                    ?.DoNotValidateScopeResponse ?? false);

            if (response.IdToken is not null)
            {
                throw new Exception("Unexpectedly received ID token with client credentials grant.");
            }

            return response;
        }

        public async Task<TokenEndpointResponseAuthCodeGrant> PostAuthCodeGrantAsync(
            string authCode,
            string redirectUrl,
            string bankIssuerUrl,
            string externalApiClientId,
            string externalApiConsentId,
            string? externalApiUserId,
            string nonce,
            string? requestScope,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            BankRegistration bankRegistration,
            string tokenEndpoint,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient matlsApiClient,
            IInstrumentationClient instrumentationClient)
        {
            var keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
                { "code", authCode }
            };

            if (bankRegistration.TokenEndpointAuthMethod is
                TokenEndpointAuthMethod.PrivateKeyJwt)
            {
                string jwt = CreateClientAssertionJwt(
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    tokenEndpoint,
                    instrumentationClient);

                // Add parameters
                keyValuePairs["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                keyValuePairs["client_assertion"] = jwt;
            }

            var response = await PostGrantAsync<TokenEndpointResponseAuthCodeGrant>(
                keyValuePairs,
                bankRegistration,
                jsonSerializerSettings,
                matlsApiClient,
                requestScope,
                false);

            GrantPostCustomBehaviour? grantPostCustomBehaviour =
                bankRegistration.BankNavigation.CustomBehaviour?.AuthCodeGrantPost;
            bool doNotValidateIdToken = grantPostCustomBehaviour?.DoNotValidateIdToken ?? false;
            if (doNotValidateIdToken is false)
            {
                string jwksUri = bankRegistration.BankNavigation.JwksUri;
                JwksGetCustomBehaviour? jwksGetCustomBehaviour =
                    bankRegistration.BankNavigation.CustomBehaviour?.JwksGet;
                await ValidateIdTokenTokenEndpoint(
                    response.IdToken,
                    response.AccessToken,
                    grantPostCustomBehaviour,
                    jwksUri,
                    jwksGetCustomBehaviour,
                    bankIssuerUrl,
                    externalApiClientId,
                    externalApiConsentId,
                    nonce,
                    bankRegistration.BankNavigation.SupportsSca,
                    bankRegistration.BankNavigation.IdTokenSubClaimType,
                    externalApiUserId);
            }

            return response;
        }

        public async Task<TokenEndpointResponseRefreshTokenGrant> PostRefreshTokenGrantAsync(
            string refreshToken,
            string redirectUrl,
            string bankIssuerUrl,
            string externalApiClientId,
            string externalApiConsentId,
            string? externalApiUserId,
            string nonce,
            string? requestScope,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            BankRegistration bankRegistration,
            string tokenEndpoint,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient mtlsApiClient,
            IInstrumentationClient instrumentationClient)
        {
            var keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            if (bankRegistration.TokenEndpointAuthMethod is
                TokenEndpointAuthMethod.PrivateKeyJwt)
            {
                string jwt = CreateClientAssertionJwt(
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    tokenEndpoint,
                    instrumentationClient);

                // Add parameters
                keyValuePairs["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                keyValuePairs["client_assertion"] = jwt;
            }

            var response = await PostGrantAsync<TokenEndpointResponseRefreshTokenGrant>(
                keyValuePairs,
                bankRegistration,
                jsonSerializerSettings,
                mtlsApiClient,
                requestScope,
                false);

            GrantPostCustomBehaviour? customBehaviour =
                bankRegistration.BankNavigation.CustomBehaviour?.RefreshTokenGrantPost;
            bool doNotValidateIdToken = customBehaviour?.DoNotValidateIdToken ?? false;
            string? responseIdToken = response.IdToken;
            if (doNotValidateIdToken is false &&
                responseIdToken is not null)
            {
                string jwksUri = bankRegistration.BankNavigation.JwksUri;
                JwksGetCustomBehaviour? jwksGetCustomBehaviour =
                    bankRegistration.BankNavigation.CustomBehaviour?.JwksGet;

                await ValidateIdTokenTokenEndpoint(
                    responseIdToken,
                    response.AccessToken,
                    customBehaviour,
                    jwksUri,
                    jwksGetCustomBehaviour,
                    bankIssuerUrl,
                    externalApiClientId,
                    externalApiConsentId,
                    nonce,
                    bankRegistration.BankNavigation.SupportsSca,
                    bankRegistration.BankNavigation.IdTokenSubClaimType,
                    externalApiUserId);
            }

            return response;
        }

        private static string CreateClientAssertionJwt(
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            BankRegistration bankRegistration,
            string tokenEndpoint,
            IInstrumentationClient instrumentationClient)
        {
            // Create JWT
            var claims = new
            {
                iss = bankRegistration.ExternalApiObject.ExternalApiId,
                sub = bankRegistration.ExternalApiObject.ExternalApiId,
                aud = tokenEndpoint,
                jti = Guid.NewGuid().ToString(),
                iat = DateTimeOffset.Now.ToUnixTimeSeconds(),
                exp = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds()
            };
            string jwt = JwtFactory.CreateJwt(
                JwtFactory.DefaultJwtHeadersExcludingTyp(processedSoftwareStatementProfile.SigningKeyId),
                claims,
                processedSoftwareStatementProfile.SigningKey);
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
            string nonce,
            bool supportsSca)
        {
            if (idToken.Exp < DateTimeOffset.UtcNow)
            {
                throw new Exception("ID token has expired.");
            }

            if (!string.Equals(idToken.ConsentId, externalApiConsentId))
            {
                throw new Exception("Consent ID from ID token does not match expected consent ID.");
            }

            if (!string.Equals(idToken.Nonce, nonce))
            {
                throw new Exception("Nonce from ID token does not match expected nonce.");
            }

            if (supportsSca && idToken.AuthTime is null)
            {
                throw new Exception("Auth time is null.");
            }

            if (supportsSca && idToken.Acr != Acr.Sca)
            {
                throw new Exception("Acr from ID token does not match expected Acr.");
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
            string nonce,
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
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

            IdTokenTokenEndpoint idToken =
                JsonConvert.DeserializeObject<IdTokenTokenEndpoint>(
                    idTokenDecoded,
                    jsonSerializerSettings) ??
                throw new Exception("Can't deserialise ID token.");

            ValidateIdTokenCommon(
                idToken,
                bankIssuerUrl,
                externalApiClientId,
                externalApiConsentId,
                nonce,
                supportsSca);

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
            BankRegistration bankRegistration,
            JsonSerializerSettings? responseJsonSerializerSettings,
            IApiClient mtlsApiClient,
            string? requestScope,
            bool doNotValidateScopeResponse)
            where TokenEndpointResponse : TokenEndpointResponseBase
        {
            // POST request
            var uri = new Uri(bankRegistration.BankNavigation.TokenEndpoint);
            IPostRequestProcessor<Dictionary<string, string>> postRequestProcessor =
                new AuthGrantPostRequestProcessor<Dictionary<string, string>>(bankRegistration);
            var response = await postRequestProcessor.PostAsync<TokenEndpointResponse>(
                uri,
                keyValuePairs,
                null,
                responseJsonSerializerSettings,
                mtlsApiClient);

            // Check token endpoint response
            StringComparison stringComparison = bankRegistration.BankNavigation.SupportsSca
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            if (!string.Equals(response.TokenType, "Bearer", stringComparison))
            {
                throw new InvalidDataException("Access token received does not have token type equal to Bearer.");
            }

            // Ensure received token scope when provided matches that requested
            if (requestScope is not null)
            {
                if (response.Scope is not null)
                {
                    IOrderedEnumerable<string> requestScopeOrdered = requestScope.Split(" ").OrderBy(t => t);
                    IOrderedEnumerable<string> responseScopeOrdered = response.Scope.Split(" ").OrderBy(t => t);
                    if (!requestScopeOrdered.SequenceEqual(responseScopeOrdered) &&
                        !doNotValidateScopeResponse)
                    {
                        throw new Exception("Requested and received scope for access token differ.");
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
}
