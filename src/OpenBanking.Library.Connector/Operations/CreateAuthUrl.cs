// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public static class CreateAuthUrl
    {
        internal static string Create(
            string consentId,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            BankRegistration bankRegistration,
            string issuerUrl,
            string state,
            string scopeString,
            IInstrumentationClient instrumentationClient)
        {
            string redirectUrl = processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;

            OAuth2RequestObjectClaims oAuth2RequestObjectClaims =
                OAuth2RequestObjectClaimsFactory.CreateOAuth2RequestObjectClaims(
                    bankRegistration,
                    redirectUrl,
                    new[] { "openid", scopeString },
                    consentId,
                    issuerUrl,
                    state);
            string requestObjectJwt = JwtFactory.CreateJwt(
                JwtFactory.DefaultJwtHeadersExcludingTyp(processedSoftwareStatementProfile.SigningKeyId),
                oAuth2RequestObjectClaims,
                processedSoftwareStatementProfile.SigningKey,
                processedSoftwareStatementProfile.SigningCertificate);
            StringBuilder requestTraceSb = new StringBuilder()
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
                { "state", oAuth2RequestObjectClaims.State }
            };
            string queryString = keyValuePairs.ToUrlEncoded();
            string authUrl = bankRegistration.AuthorizationEndpoint + "?" + queryString;
            StringBuilder authUrlTraceSb = new StringBuilder()
                .AppendLine("#### Auth URL (Consent)")
                .Append(authUrl);
            instrumentationClient.Info(authUrlTraceSb.ToString());
            return authUrl;
        }
    }
}
