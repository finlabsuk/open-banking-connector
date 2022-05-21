﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public static class CreateAuthUrl
    {
        internal static string Create(
            string externalApiConsentId,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            string externalApiId,
            ConsentAuthGetCustomBehaviour? customBehaviourConsentAuthGet,
            string authorisationEndpoint,
            string issuerUrl,
            string state,
            string scopeString,
            IInstrumentationClient instrumentationClient)
        {
            string redirectUrl =
                processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;

            OAuth2RequestObjectClaims oAuth2RequestObjectClaims =
                OAuth2RequestObjectClaimsFactory.CreateOAuth2RequestObjectClaims(
                    externalApiId,
                    customBehaviourConsentAuthGet,
                    redirectUrl,
                    new[] { "openid", scopeString },
                    externalApiConsentId,
                    issuerUrl,
                    state);
            string requestObjectJwt = JwtFactory.CreateJwt(
                JwtFactory.DefaultJwtHeadersExcludingTyp(processedSoftwareStatementProfile.SigningKeyId),
                oAuth2RequestObjectClaims,
                processedSoftwareStatementProfile.SigningKey,
                processedSoftwareStatementProfile.SigningCertificate);
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
                { "redirect_uri", oAuth2RequestObjectClaims.RedirectUri },
                { "scope", oAuth2RequestObjectClaims.Scope },
                { "request", requestObjectJwt },
                { "nonce", oAuth2RequestObjectClaims.Nonce },
                //{ "response_mode", "fragment"},
                { "state", oAuth2RequestObjectClaims.State }
            };
            string queryString = keyValuePairs.ToUrlEncoded();
            string authUrl = authorisationEndpoint + "?" + queryString;
            StringBuilder authUrlTraceSb = new StringBuilder()
                .AppendLine("#### Auth URL (Consent)")
                .Append(authUrl);
            instrumentationClient.Trace(authUrlTraceSb.ToString());
            return authUrl;
        }
    }
}
