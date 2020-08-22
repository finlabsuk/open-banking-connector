﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using BankRegistration = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ClientRegistration
{
    internal static class RegistrationClaimsFactory
    {
        public static OBClientRegistration1 CreateRegistrationClaims(
            SoftwareStatementProfile sProfile,
            string audValue)
        {
            sProfile.ArgNotNull(nameof(sProfile));

            OBClientRegistration1 registrationClaims = new OBClientRegistration1
            {
                Iss = sProfile.SoftwareStatementPayload.SoftwareId,
                Iat = DateTimeOffset.Now,
                Exp = DateTimeOffset.UtcNow.AddHours(1),
                Aud = audValue,
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
    }
}
