// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using BankClientProfile = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankClientProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public
{
    internal static class Factories
    {
        public static OBClientRegistration1 CreateRegistrationClaims(
            SoftwareStatementProfile sProfile,
            BankClientProfile bProfile)
        {
            sProfile.ArgNotNull(nameof(sProfile));

            OBClientRegistration1 registrationClaims = new OBClientRegistration1
            {
                Iss = sProfile.SoftwareStatementPayload.SoftwareId,
                Iat = DateTimeOffset.Now,
                Exp = DateTimeOffset.UtcNow.AddHours(1),
                Aud = bProfile.BankClientRegistrationClaimsOverrides?.RequestAudience ?? bProfile.XFapiFinancialId,
                Jti = Guid.NewGuid().ToString(),
                TokenEndpointAuthMethod = TokenEndpointAuthMethodEnum.TlsClientAuth,
                GrantTypes = new[]
                {
                    GrantTypesItemEnum.ClientCredentials,
                    GrantTypesItemEnum.AuthorizationCode,
                    GrantTypesItemEnum.RefreshToken
                },
                ResponseTypes = new[]
                {
                    ResponseTypesItemEnum.CodeidToken
                },
                ApplicationType = ApplicationTypeEnum.Web,
                IdTokenSignedResponseAlg = SupportedAlgorithmsEnum.PS256,
                RequestObjectSigningAlg = SupportedAlgorithmsEnum.PS256,
                RedirectUris = sProfile.SoftwareStatementPayload.SoftwareRedirectUris,
                SoftwareId = sProfile.SoftwareStatementPayload.SoftwareId,
                Scope = sProfile.SoftwareStatementPayload.Scope,
                SoftwareStatement = sProfile.SoftwareStatement,
                TlsClientAuthSubjectDn =
                    $"CN={sProfile.SoftwareStatementPayload.SoftwareId},OU={sProfile.SoftwareStatementPayload.OrgId},O=OpenBanking,C=GB"
            };

            return registrationClaims;
        }

        public static OAuth2RequestObjectClaims CreateOAuth2RequestObjectClaims(
            Persistent.BankClientProfile openBankingClient,
            string redirectUrl,
            string[] scope,
            string intentId)
        {
            OAuth2RequestObjectClaims oAuth2RequestObjectClaims = new OAuth2RequestObjectClaims
            {
                Iss = openBankingClient.BankClientRegistrationData.ClientId,
                Iat = DateTimeOffset.Now,
                Exp = DateTimeOffset.UtcNow.AddHours(1),
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
