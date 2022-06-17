﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
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

        public async Task ValidateIdTokenAuthEndpoint(
            OAuth2RedirectData redirectData,
            ConsentAuthGetCustomBehaviour? consentAuthGetCustomBehaviour,
            string jwksUri,
            JwksGetCustomBehaviour? jwksGetCustomBehaviour,
            string bankIssuerUrl,
            string externalApiClientId,
            string externalApiConsentId,
            string nonce,
            bool supportsSca)
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
                consentAuthGetCustomBehaviour?.IdTokenSubClaimIsClientIdNotConsentId ?? false,
                nonce,
                supportsSca);

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
        }

        public async Task<ClientCredentialsGrantResponse> PostClientCredentialsGrantAsync(
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
                // Create JWT
                var claims = new
                {
                    iss = bankRegistration.ExternalApiObject.ExternalApiId,
                    sub = bankRegistration.ExternalApiObject.ExternalApiId,
                    aud = tokenEndpoint,
                    jti = Guid.NewGuid().ToString(),
                    iat = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    exp = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds()
                };
                string jwt = JwtFactory.CreateJwt(
                    JwtFactory.DefaultJwtHeadersExcludingTyp(processedSoftwareStatementProfile.SigningKeyId),
                    claims,
                    processedSoftwareStatementProfile.SigningKey,
                    processedSoftwareStatementProfile.SigningCertificate);
                StringBuilder requestTraceSb = new StringBuilder()
                    .AppendLine("#### JWT (Client Auth)")
                    .Append(jwt);
                instrumentationClient.Trace(requestTraceSb.ToString());

                // Add parameters
                keyValuePairs["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                keyValuePairs["client_assertion"] = jwt;
            }

            var response = await PostGrantAsync<ClientCredentialsGrantResponse>(
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

        public async Task<AuthCodeGrantResponse> PostAuthCodeGrantAsync(
            string authCode,
            string redirectUrl,
            string bankIssuerUrl,
            string externalApiClientId,
            string externalApiConsentId,
            string nonce,
            string? requestScope,
            BankRegistration bankRegistration,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient matlsApiClient)
        {
            var keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
                { "code", authCode }
            };

            var response = await PostGrantAsync<AuthCodeGrantResponse>(
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
                    bankRegistration.BankNavigation.SupportsSca);
            }

            return response;
        }

        public async Task<RefreshTokenGrantResponse> PostRefreshTokenGrantAsync(
            string refreshToken,
            string redirectUrl,
            string bankIssuerUrl,
            string externalApiClientId,
            string externalApiConsentId,
            string nonce,
            string? requestScope,
            BankRegistration bankRegistration,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient mtlsApiClient)
        {
            var keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var response = await PostGrantAsync<RefreshTokenGrantResponse>(
                keyValuePairs,
                bankRegistration,
                jsonSerializerSettings,
                mtlsApiClient,
                requestScope,
                false);

            GrantPostCustomBehaviour? customBehaviour =
                bankRegistration.BankNavigation.CustomBehaviour?.RefreshTokenGrantPost;
            bool doNotValidateIdToken = customBehaviour?.DoNotValidateIdToken ?? false;
            if (doNotValidateIdToken is false)
            {
                string jwksUri = bankRegistration.BankNavigation.JwksUri;
                JwksGetCustomBehaviour? jwksGetCustomBehaviour =
                    bankRegistration.BankNavigation.CustomBehaviour?.JwksGet;
                await ValidateIdTokenTokenEndpoint(
                    response.IdToken,
                    response.AccessToken,
                    customBehaviour,
                    jwksUri,
                    jwksGetCustomBehaviour,
                    bankIssuerUrl,
                    externalApiClientId,
                    externalApiConsentId,
                    nonce,
                    bankRegistration.BankNavigation.SupportsSca);
            }

            return response;
        }

        private void ValidateIdTokenCommon(
            IdTokenBase idToken,
            string bankIssuerUrl,
            string externalApiClientId,
            string externalApiConsentId,
            bool idTokenSubClaimIsClientIdNotConsentId,
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

            string expectedSubject = idTokenSubClaimIsClientIdNotConsentId ? externalApiClientId : externalApiConsentId;
            if (!string.Equals(idToken.Subject, expectedSubject))
            {
                throw new Exception("Subject from ID token does not match expected subject.");
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
            bool supportsSca)
        {
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
                grantPostCustomBehaviour?.IdTokenSubClaimIsClientIdNotConsentId ?? false,
                nonce,
                supportsSca);

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

        private async Task<TGrantResponse> PostGrantAsync<TGrantResponse>(
            Dictionary<string, string> keyValuePairs,
            BankRegistration bankRegistration,
            JsonSerializerSettings? responseJsonSerializerSettings,
            IApiClient mtlsApiClient,
            string? requestScope,
            bool doNotValidateScopeResponse)
            where TGrantResponse : GrantResponseBase
        {
            // POST request
            var uri = new Uri(bankRegistration.BankNavigation.TokenEndpoint);
            IPostRequestProcessor<Dictionary<string, string>> postRequestProcessor =
                new AuthGrantPostRequestProcessor<Dictionary<string, string>>(bankRegistration);
            var response = await postRequestProcessor.PostAsync<TGrantResponse>(
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
                else
                {
                    if (bankRegistration.BankNavigation.SupportsSca)
                    {
                        throw new Exception("Expected scope from bank (even if not strictly required by spec).");
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
