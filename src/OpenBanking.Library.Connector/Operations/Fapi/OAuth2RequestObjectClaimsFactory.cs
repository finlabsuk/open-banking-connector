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
            BankRegistration bankRegistration,
            string redirectUrl,
            string[] scope,
            string intentId,
            string issuerUrl,
            string state)
        {
            var oAuth2RequestObjectClaims = new OAuth2RequestObjectClaims
            {
                Iss = bankRegistration.OAuth2RequestObjectClaimsOverrides?.Issuer ??
                      bankRegistration.ExternalApiId,
                Iat = DateTimeOffset.Now,
                Nbf = DateTimeOffset.Now,
                Exp = DateTimeOffset.UtcNow.AddHours(1),
                Aud = bankRegistration.OAuth2RequestObjectClaimsOverrides?.Audience ??
                      issuerUrl,
                Jti = Guid.NewGuid().ToString(),
                ResponseType = "code id_token",
                ClientId = bankRegistration.ExternalApiId,
                RedirectUri = redirectUrl,
                Scope = scope.JoinString(" "),
                MaxAge = 86400,
                Claims = new OAuth2RequestObjectInnerClaims(intentId),
                State = state
            };

            return oAuth2RequestObjectClaims;
        }
    }
}
