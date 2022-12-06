// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public static class CreateAuthUrl
    {
        private static string GenerateNonceOrState()
        {
            const int lengthInBytes = 24;
            var buffer = new byte[lengthInBytes];
            // Generate random bytes
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(buffer);
            return Base64UrlEncoder.Encode(buffer, 0, lengthInBytes);
        }

        internal static (string authUrl, string state, string nonce) Create(
            string externalApiConsentId,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            BankRegistration bankRegistration,
            string clientExternalApiId,
            ConsentAuthGetCustomBehaviour? customBehaviourConsentAuthGet,
            string authorisationEndpoint,
            string consentAuthGetAudClaim,
            bool supportsSca,
            string scopeString,
            IInstrumentationClient instrumentationClient)
        {
            string nonce = GenerateNonceOrState();
            string state = GenerateNonceOrState();
            string redirectUrl = bankRegistration.DefaultRedirectUri;

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
            string requestObjectJwt = JwtFactory.CreateJwt(
                JwtFactory.DefaultJwtHeadersExcludingTyp(processedSoftwareStatementProfile.SigningKeyId),
                oAuth2RequestObjectClaims,
                processedSoftwareStatementProfile.SigningKey);
            StringBuilder requestTraceSb = new StringBuilder()
                .AppendLine("#### Claims (Request Object)")
                .AppendLine(
                    JsonConvert.SerializeObject(
                        oAuth2RequestObjectClaims,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                        }))
                .AppendLine("#### JWT (Request Object)")
                .Append(requestObjectJwt);
            instrumentationClient.Info(requestTraceSb.ToString());

            // Create auth URL
            var keyValuePairs = new Dictionary<string, string>
            {
                { "response_type", oAuth2RequestObjectClaims.ResponseType },
                { "client_id", oAuth2RequestObjectClaims.ClientId },
                { "redirect_uri", oAuth2RequestObjectClaims.RedirectUri }, // required by some banks but not by spec
                { "scope", oAuth2RequestObjectClaims.Scope },
                { "request", requestObjectJwt }
                //{ "nonce", oAuth2RequestObjectClaims.Nonce },
                //{ "response_mode", "fragment"},
                //{ "state", oAuth2RequestObjectClaims.State }
            };
            string queryString = keyValuePairs.ToUrlEncoded();
            string authUrl = authorisationEndpoint + "?" + queryString;
            StringBuilder authUrlTraceSb = new StringBuilder()
                .AppendLine("#### Auth URL (Consent)")
                .Append(authUrl);
            instrumentationClient.Trace(authUrlTraceSb.ToString());
            return (authUrl, state, nonce);
        }
    }
}
