// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ClientRegistration
{
    internal static class RegistrationClaimsFactory
    {
        /// <summary>
        ///     Convert API types to scope string
        /// </summary>
        /// <param name="registrationScope"></param>
        /// <returns></returns>
        private static string ApiTypesToScope(RegistrationScope registrationScope)
        {
            // Combine scope words for individual API types prepending "openid"
            IEnumerable<string> scopeWordsIEnumerable = RegistrationScopeApiHelper.AllApiTypes
                .Where(x => registrationScope.HasFlag(RegistrationScopeApiHelper.ApiTypeSetWithSingleApiType(x)))
                .Select(RegistrationScopeApiHelper.ScopeWord)
                .Prepend("openid")
                .Distinct(); // for safety

            return scopeWordsIEnumerable.JoinString(" ");
        }

        public static ClientRegistrationModelsPublic.OBClientRegistration1 CreateRegistrationClaims(
            SoftwareStatementProfile sProfile,
            RegistrationScope registrationScope,
            BankRegistrationClaimsOverrides? bankClientRegistrationClaimsOverrides,
            string bankXFapiFinancialId)
        {
            sProfile.ArgNotNull(nameof(sProfile));

            // Check API types
            bool apiTypesIsSubset =
                (registrationScope & sProfile.SoftwareStatementPayload.RegistrationScope) ==
                registrationScope;
            if (!apiTypesIsSubset)
            {
                throw new InvalidOperationException(
                    "Software statement does not support API types specified in Bank object.");
            }

            string scope = ApiTypesToScope(registrationScope);

            ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum
                tokenEndpointAuthMethod = bankClientRegistrationClaimsOverrides?.TokenEndpointAuthMethod switch
                {
                    ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum
                        .ClientSecretBasic => ClientRegistrationModelsPublic
                        .OBRegistrationProperties1tokenEndpointAuthMethodEnum.ClientSecretBasic,
                    ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum.TlsClientAuth =>
                        ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum
                            .TlsClientAuth,
                    null => ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum
                        .TlsClientAuth,
                    _ => throw new ArgumentOutOfRangeException(
                        $"{bankClientRegistrationClaimsOverrides.TokenEndpointAuthMethod}")
                };
            IList<ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum> grantTypes =
                bankClientRegistrationClaimsOverrides?.GrantTypes ??
                new[]
                {
                    ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum
                        .ClientCredentials,
                    ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum
                        .AuthorizationCode,
                    ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum.RefreshToken
                };
            ClientRegistrationModelsPublic.OBClientRegistration1 registrationClaims =
                new ClientRegistrationModelsPublic.OBClientRegistration1
                {
                    Iss = bankClientRegistrationClaimsOverrides?.IssuerIsSoftwareStatementXFapiFinancialId ?? false
                        ? sProfile.SoftwareStatementPayload.OrgId
                        : sProfile.SoftwareStatementPayload.SoftwareId,
                    Iat = DateTimeOffset.Now,
                    Exp = DateTimeOffset.UtcNow.AddHours(1),
                    Aud = bankClientRegistrationClaimsOverrides?.Audience ??
                          bankXFapiFinancialId,
                    Jti = Guid.NewGuid().ToString(),
                    TokenEndpointAuthMethod = tokenEndpointAuthMethod,
                    GrantTypes = grantTypes,
                    ResponseTypes = new[]
                    {
                        ClientRegistrationModelsPublic.OBRegistrationProperties1responseTypesItemEnum.CodeidToken
                    },
                    ApplicationType = ClientRegistrationModelsPublic.OBRegistrationProperties1applicationTypeEnum.Web,
                    IdTokenSignedResponseAlg = ClientRegistrationModelsPublic.SupportedAlgorithmsEnum.PS256,
                    RequestObjectSigningAlg = ClientRegistrationModelsPublic.SupportedAlgorithmsEnum.PS256,
                    RedirectUris = sProfile.SoftwareStatementPayload.SoftwareRedirectUris,
                    SoftwareId = sProfile.SoftwareStatementPayload.SoftwareId,
                    Scope = scope,
                    SoftwareStatement = sProfile.SoftwareStatement,
                    TlsClientAuthSubjectDn =
                        $"CN={sProfile.SoftwareStatementPayload.SoftwareId},OU={sProfile.SoftwareStatementPayload.OrgId},O=OpenBanking,C=GB"
                };

            if (!(bankClientRegistrationClaimsOverrides?.SubjectType is null))
            {
                registrationClaims.SubjectType = bankClientRegistrationClaimsOverrides.SubjectType;
            }

            return registrationClaims;
        }
    }
}
