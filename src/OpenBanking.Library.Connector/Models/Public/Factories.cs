// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public
{
    internal static class Factories
    {
        public static OpenBankingClientRegistrationClaims CreateRegistrationClaims(
            string issuerUrl,
            SoftwareStatementProfile sProfile,
            bool concatScopes)
        {
            sProfile.ArgNotNull(nameof(sProfile));

            OpenBankingClientRegistrationClaims registrationClaims = new OpenBankingClientRegistrationClaims
            {
                Iss = sProfile.SoftwareStatementPayload.SoftwareId,
                Aud = issuerUrl,
                RedirectUris = sProfile.SoftwareStatementPayload.SoftwareRedirectUris,
                SoftwareId = sProfile.SoftwareStatementPayload.SoftwareId,
                Scope = sProfile.SoftwareStatementPayload.Scope,
                /*                Scope = concatScopes
                                    ? new[] { sProfile.SoftwareStatementPayload.Scope }
                                    : sProfile.SoftwareStatementPayload.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries),
                */
                SoftwareStatement = sProfile.SoftwareStatement,
                TlsClientAuthSubjectDn =
                    $"CN={sProfile.SoftwareStatementPayload.SoftwareId},OU={sProfile.SoftwareStatementPayload.OrgId},O=OpenBanking,C=GB"
            };

            return registrationClaims;
        }

        public static OAuth2RequestObjectClaims CreateOAuth2RequestObjectClaims(
            BankClientProfile openBankingClient,
            string redirectUrl,
            string[] scope,
            string intentId)
        {
            OAuth2RequestObjectClaims oAuth2RequestObjectClaims = new OAuth2RequestObjectClaims
            {
                Iss = openBankingClient.BankClientRegistrationData.ClientId,
                Aud = openBankingClient.IssuerUrl,
                Jti = Guid.NewGuid().ToString(),
                ResponseType = "code id_token",
                ClientId = openBankingClient.BankClientRegistrationData.ClientId,
                RedirectUri = redirectUrl,
                Scope = scope.JoinString(" "),
                MaxAge = 86400,
                Claims = new OAuth2RequestObjectInnerClaims(intentId)
            };

            return oAuth2RequestObjectClaims;
        }
    }
}
