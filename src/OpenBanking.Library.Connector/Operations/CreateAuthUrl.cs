// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

public static class CreateAuthUrl
{
    private static string GenerateRandomString(int numBytes)
    {
        var buffer = new byte[numBytes];
        RandomNumberGenerator.Fill(buffer);
        return Base64UrlEncoder.Encode(buffer, 0, numBytes);
    }

    private static (string codeVerifier, string codeChallenge) GeneratePkceValues()
    {
        const int lengthInBytes = 32;

        // Generate code verifier
        var codeVerifierBytes = new byte[lengthInBytes];
        RandomNumberGenerator.Fill(codeVerifierBytes);
        string codeVerifier = Base64UrlEncoder.Encode(codeVerifierBytes, 0, lengthInBytes);

        // Generate code challenge
        using var sha256 = SHA256.Create();
        byte[] codeChallengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        if (codeChallengeBytes.Length != lengthInBytes)
        {
            throw new InvalidOperationException();
        }

        string codeChallenge = Base64UrlEncoder.Encode(codeChallengeBytes, 0, lengthInBytes);

        return (codeVerifier, codeChallenge);
    }

    internal static (string authUrl, string state, string nonce, string? codeVerifier, string appSessionId) Create(
        string externalApiConsentId,
        OBSealKey obSealKey,
        string clientExternalApiId,
        bool useOpenIdConnect,
        ConsentAuthGetCustomBehaviour? customBehaviourConsentAuthGet,
        string authorisationEndpoint,
        string consentAuthGetAudClaim,
        bool supportsSca,
        string redirectUrl,
        string scopeString,
        OAuth2ResponseType responseType,
        IInstrumentationClient instrumentationClient)
    {
        const int lengthInBytes = 24;
        string state = GenerateRandomString(lengthInBytes);
        string nonce = GenerateRandomString(lengthInBytes);
        string appSessionId = GenerateRandomString(32);
        string responseTypeString = JsonConvert.SerializeObject(responseType).Replace("\"", "");

        // Add common parameters
        var keyValuePairs = new Dictionary<string, string>
        {
            { "response_type", responseTypeString },
            { "client_id", clientExternalApiId },
            { "redirect_uri", redirectUrl } // required by some banks but not by spec for Open ID Connect
        };

        if (useOpenIdConnect)
        {
            // Add scope
            string newScopeString = "openid " + scopeString;
            keyValuePairs.Add("scope", newScopeString);

            // Add state
            if (customBehaviourConsentAuthGet?.AddRedundantOAuth2StateRequestParameter ?? false)
            {
                keyValuePairs.Add("state", state);
            }

            // Add request object
            string requestObjectJwt = GenerateRequestObjectJwt(
                externalApiConsentId,
                obSealKey,
                clientExternalApiId,
                customBehaviourConsentAuthGet,
                consentAuthGetAudClaim,
                supportsSca,
                redirectUrl,
                instrumentationClient,
                state,
                nonce,
                responseTypeString,
                newScopeString);
            keyValuePairs.Add("request", requestObjectJwt);
        }
        else
        {
            // Add scope and state
            keyValuePairs.Add("scope", scopeString);
            keyValuePairs.Add("state", state);
        }

        // Add nonce
        if (customBehaviourConsentAuthGet?.AddRedundantOAuth2NonceRequestParameter ?? false)
        {
            keyValuePairs.Add("nonce", nonce);
        }

        // Add PKCE parameters
        string? codeVerifier = null;
        bool usePkce = customBehaviourConsentAuthGet?.UsePkce ?? false;
        if (usePkce)
        {
            (codeVerifier, string codeChallenge) = GeneratePkceValues();
            keyValuePairs.Add("code_challenge_method", "S256");
            keyValuePairs.Add("code_challenge", codeChallenge);
        }

        if (customBehaviourConsentAuthGet?.ExtraParameters is not null)
        {
            foreach ((string key, string value) in customBehaviourConsentAuthGet.ExtraParameters)
            {
                keyValuePairs.Add(key, value);
            }
        }

        if (customBehaviourConsentAuthGet?.ExtraConsentParameterName is not null)
        {
            keyValuePairs.Add(customBehaviourConsentAuthGet.ExtraConsentParameterName, externalApiConsentId);
        }

        bool doNotUseUrlPathEncoding = customBehaviourConsentAuthGet?.DoNotUseUrlPathEncoding ?? false;
        string queryString = keyValuePairs.ToUrlParameterString(doNotUseUrlPathEncoding);

        if (customBehaviourConsentAuthGet?.SingleBase64EncodedParameterName is not null)
        {
            queryString =
                $"{customBehaviourConsentAuthGet.SingleBase64EncodedParameterName}=" +
                Base64UrlEncoder.Encode(queryString);
        }

        string authUrl = authorisationEndpoint + "?" + queryString;
        StringBuilder authUrlTraceSb = new StringBuilder()
            .AppendLine("#### Auth URL (Consent)")
            .Append(authUrl);
        instrumentationClient.Trace(authUrlTraceSb.ToString());

        return (authUrl, state, nonce, codeVerifier, appSessionId);
    }

    private static string GenerateRequestObjectJwt(
        string externalApiConsentId,
        OBSealKey obSealKey,
        string clientExternalApiId,
        ConsentAuthGetCustomBehaviour? customBehaviourConsentAuthGet,
        string consentAuthGetAudClaim,
        bool supportsSca,
        string redirectUrl,
        IInstrumentationClient instrumentationClient,
        string state,
        string nonce,
        string responseTypeString,
        string scopeString2)
    {
        OAuth2RequestObjectClaims oAuth2RequestObjectClaims =
            OAuth2RequestObjectClaimsFactory.CreateOAuth2RequestObjectClaims(
                clientExternalApiId,
                redirectUrl,
                consentAuthGetAudClaim,
                supportsSca,
                state,
                nonce,
                customBehaviourConsentAuthGet?.ConsentIdClaimPrefix + externalApiConsentId,
                responseTypeString,
                scopeString2);
        var jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        string payloadJson = JsonConvert.SerializeObject(
            oAuth2RequestObjectClaims,
            jsonSerializerSettings);
        string requestObjectJwt = JwtFactory.CreateJwt(
            JwtFactory.JwtHeaders(obSealKey.KeyId, null),
            payloadJson,
            obSealKey.Key,
            null);
        StringBuilder requestTraceSb = new StringBuilder()
            .AppendLine("#### Claims (Request Object)")
            .AppendLine(
                JsonConvert.SerializeObject(
                    oAuth2RequestObjectClaims,
                    Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }))
            .AppendLine("#### JWT (Request Object)")
            .Append(requestObjectJwt);
        instrumentationClient.Trace(requestTraceSb.ToString());
        return requestObjectJwt;
    }
}
