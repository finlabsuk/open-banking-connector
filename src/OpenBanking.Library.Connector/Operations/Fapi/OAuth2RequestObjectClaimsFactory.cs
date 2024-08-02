// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Fapi;

internal static class OAuth2RequestObjectClaimsFactory
{
    public static OAuth2RequestObjectClaims CreateOAuth2RequestObjectClaims(
        string clientExternalApiId,
        string redirectUrl,
        string consentAuthGetAudClaim,
        bool supportsSca,
        string state,
        string nonce,
        string consentIdClaim,
        string responseTypeString,
        string scopeString) =>
        new()
        {
            Iss = clientExternalApiId,
            Iat = DateTimeOffset.Now,
            Nbf = DateTimeOffset.Now,
            Exp = DateTimeOffset.UtcNow.AddSeconds(300),
            Aud = consentAuthGetAudClaim,
            Jti = Guid.NewGuid().ToString(),
            ResponseType = responseTypeString,
            //ResponseMode = "fragment",
            ClientId = clientExternalApiId,
            RedirectUri = redirectUrl,
            Scope = scopeString,
            MaxAge = 86400,
            Claims = new OAuth2RequestObjectInnerClaims
            {
                UserInfo = new UserInfoClaims
                {
                    OpenbankingIntentId = new StringClaim
                    {
                        Essential = true,
                        Value = consentIdClaim
                    }
                },
                IdToken = new IdTokenClaims
                {
                    ConsentIdClaim = new StringClaim
                    {
                        Essential = true,
                        Value = consentIdClaim
                    },
                    AcrClaim = supportsSca
                        ? new AcrClaim
                        {
                            Essential = true,
                            Values = new List<Acr>
                            {
                                Acr.Ca,
                                Acr.Sca
                            }
                        }
                        : new AcrClaim
                        {
                            Essential = true,
                            Value = Acr.Ca
                        }
                }
            },
            State = state,
            Nonce = nonce
        };
}
