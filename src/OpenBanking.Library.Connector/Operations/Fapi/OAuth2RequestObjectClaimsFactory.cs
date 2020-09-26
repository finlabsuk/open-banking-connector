// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Fapi
{
    internal static class OAuth2RequestObjectClaimsFactory
    {
        public static OAuth2RequestObjectClaims CreateOAuth2RequestObjectClaims(
            BankRegistration openBankingClient,
            string redirectUrl,
            string[] scope,
            string intentId,
            string issuerUrl)
        {
            OAuth2RequestObjectClaims oAuth2RequestObjectClaims = new OAuth2RequestObjectClaims
            {
                Iss = openBankingClient.BankClientRegistration.ClientId,
                Iat = DateTimeOffset.Now,
                Exp = DateTimeOffset.UtcNow.AddHours(1),
                Aud = issuerUrl,
                Jti = Guid.NewGuid().ToString(),
                ResponseType = "code id_token",
                ClientId = openBankingClient.BankClientRegistration.ClientId,
                RedirectUri = redirectUrl,
                Scope = scope.JoinString(" "),
                MaxAge = 86400,
                Claims = new OAuth2RequestObjectInnerClaims(intentId)
            };

            return oAuth2RequestObjectClaims;
        }
    }
}
