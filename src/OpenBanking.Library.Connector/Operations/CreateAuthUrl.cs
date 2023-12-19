// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
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

    private static (string sessionId, string nonce) GenerateSessionIdAndNonce()
    {
        const int lengthInBytes = 32;

        // Generate session ID
        var sessionIdBytes = new byte[lengthInBytes];
        RandomNumberGenerator.Fill(sessionIdBytes);
        string sessionId = Base64UrlEncoder.Encode(sessionIdBytes, 0, lengthInBytes);

        // Generate nonce
        using var sha256 = SHA256.Create();
        byte[] nonceBytes = sha256.ComputeHash(sessionIdBytes);
        if (nonceBytes.Length != lengthInBytes)
        {
            throw new InvalidOperationException();
        }

        string nonce = Base64UrlEncoder.Encode(nonceBytes, 0, lengthInBytes);

        return (sessionId, nonce);
    }

    internal static (string authUrl, string state, string nonce, string appSessionId) Create(
        string externalApiConsentId,
        OBSealKey obSealKey,
        BankRegistration bankRegistration,
        string clientExternalApiId,
        ConsentAuthGetCustomBehaviour? customBehaviourConsentAuthGet,
        string authorisationEndpoint,
        string consentAuthGetAudClaim,
        bool supportsSca,
        string redirectUrl,
        string scopeString,
        IInstrumentationClient instrumentationClient)
    {
        const int lengthInBytes = 24;
        string state = GenerateRandomString(lengthInBytes);
        string nonce = GenerateRandomString(lengthInBytes);
        string appSessionId = GenerateRandomString(32);

        OAuth2RequestObjectClaims oAuth2RequestObjectClaims =
            OAuth2RequestObjectClaimsFactory.CreateOAuth2RequestObjectClaims(
                clientExternalApiId,
                customBehaviourConsentAuthGet,
                redirectUrl,
                new[] { "openid", scopeString },
                externalApiConsentId,
                consentAuthGetAudClaim,
                supportsSca,
                state,
                nonce);
        var jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        string payloadJson = JsonConvert.SerializeObject(
            oAuth2RequestObjectClaims,
            jsonSerializerSettings);
        string requestObjectJwt = JwtFactory.CreateJwt(
            JwtFactory.DefaultJwtHeadersExcludingTyp(obSealKey.KeyId),
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

        // Create auth URL
        var keyValuePairs = new Dictionary<string, string>
        {
            { "response_type", oAuth2RequestObjectClaims.ResponseType },
            { "client_id", oAuth2RequestObjectClaims.ClientId },
            { "scope", oAuth2RequestObjectClaims.Scope },
            { "request", requestObjectJwt },
            { "redirect_uri", oAuth2RequestObjectClaims.RedirectUri } // required by some banks but not by spec
        };
        // if (customBehaviourConsentAuthGet?.AddRedundantOAuth2RedirectUriRequestParameter ?? false)
        // {
        //     keyValuePairs.Add(
        //         "redirect_uri",
        //         oAuth2RequestObjectClaims.RedirectUri); // required by some banks but not by spec
        // }
        if (customBehaviourConsentAuthGet?.AddRedundantOAuth2StateRequestParameter ?? false)
        {
            keyValuePairs.Add("state", oAuth2RequestObjectClaims.State); // required by some banks but not by spec
        }
        if (customBehaviourConsentAuthGet?.AddRedundantOAuth2NonceRequestParameter ?? false)
        {
            keyValuePairs.Add("nonce", oAuth2RequestObjectClaims.Nonce); // required by some banks but not by spec
        }
        string queryString = keyValuePairs.ToUrlEncoded();
        string authUrl = authorisationEndpoint + "?" + queryString;
        StringBuilder authUrlTraceSb = new StringBuilder()
            .AppendLine("#### Auth URL (Consent)")
            .Append(authUrl);
        instrumentationClient.Trace(authUrlTraceSb.ToString());
        return (authUrl, state, nonce, appSessionId);
    }
}
