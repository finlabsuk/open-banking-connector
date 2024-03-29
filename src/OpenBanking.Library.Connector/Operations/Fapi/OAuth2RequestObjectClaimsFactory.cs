﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Fapi;

internal static class OAuth2RequestObjectClaimsFactory
{
    public static OAuth2RequestObjectClaims CreateOAuth2RequestObjectClaims(
        string clientExternalApiId,
        ConsentAuthGetCustomBehaviour? consentAuthGet,
        string redirectUrl,
        string[] scope,
        string externalApiConsentId,
        string consentAuthGetAudClaim,
        bool supportsSca,
        string state,
        string nonce)
    {
        var oAuth2RequestObjectClaims = new OAuth2RequestObjectClaims
        {
            Iss = clientExternalApiId,
            Iat = DateTimeOffset.Now,
            Nbf = DateTimeOffset.Now,
            Exp = DateTimeOffset.UtcNow.AddSeconds(300),
            Aud = consentAuthGetAudClaim,
            Jti = Guid.NewGuid().ToString(),
            ResponseType = "code id_token",
            //ResponseMode = "fragment",
            ClientId = clientExternalApiId,
            RedirectUri = redirectUrl,
            Scope = scope.JoinString(" "),
            MaxAge = 86400,
            Claims = new OAuth2RequestObjectInnerClaims(
                externalApiConsentId,
                consentAuthGet?.ConsentIdClaimPrefix,
                supportsSca),
            State = state,
            Nonce = nonce
        };

        return oAuth2RequestObjectClaims;
    }
}
