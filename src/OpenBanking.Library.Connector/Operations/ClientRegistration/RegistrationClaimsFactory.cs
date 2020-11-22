﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ClientRegistration
{
    internal static class RegistrationClaimsFactory
    {
        /// <summary>
        ///     Convert API types to scope string
        /// </summary>
        /// <param name="registrationScopeApiSet"></param>
        /// <returns></returns>
        private static string ApiTypesToScope(RegistrationScopeApiSet registrationScopeApiSet)
        {
            // Combine scope words for individual API types prepending "openid"
            IEnumerable<string> scopeWordsIEnumerable = RegistrationScopeApiHelper.AllApiTypes
                .Where(x => registrationScopeApiSet.HasFlag(RegistrationScopeApiHelper.ApiTypeSetWithSingleApiType(x)))
                .Select(RegistrationScopeApiHelper.ScopeWord)
                .Prepend("openid")
                .Distinct(); // for safety

            return scopeWordsIEnumerable.JoinString(" ");
        }

        public static OBClientRegistration1 CreateRegistrationClaims(
            SoftwareStatementProfile sProfile,
            RegistrationScopeApiSet registrationScopeApiSet,
            BankRegistrationClaimsOverrides? bankClientRegistrationClaimsOverrides,
            string bankXFapiFinancialId)
        {
            sProfile.ArgNotNull(nameof(sProfile));

            // Check API types
            bool apiTypesIsSubset =
                (registrationScopeApiSet & sProfile.SoftwareStatementPayload.RegistrationScopeApiSet) ==
                registrationScopeApiSet;
            if (!apiTypesIsSubset)
            {
                throw new InvalidOperationException(
                    "Software statement does not support API types specified in Bank object.");
            }

            string scope = ApiTypesToScope(registrationScopeApiSet);

            OBClientRegistration1 registrationClaims = new OBClientRegistration1
            {
                Iss = bankClientRegistrationClaimsOverrides?.IssuerIsSoftwareStatementXFapiFinancialId ?? false
                    ? sProfile.SoftwareStatementPayload.OrgId
                    : sProfile.SoftwareStatementPayload.SoftwareId,
                Iat = DateTimeOffset.Now,
                Exp = DateTimeOffset.UtcNow.AddHours(1),
                Aud = bankClientRegistrationClaimsOverrides?.Audience ??
                      bankXFapiFinancialId,
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
                Scope = scope,
                SoftwareStatement = sProfile.SoftwareStatement,
                TlsClientAuthSubjectDn =
                    $"CN={sProfile.SoftwareStatementPayload.SoftwareId},OU={sProfile.SoftwareStatementPayload.OrgId},O=OpenBanking,C=GB"
            };

            return registrationClaims;
        }
    }
}
