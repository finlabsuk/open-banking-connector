// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal static class RegistrationClaimsFactory
    {
        /// <summary>
        ///     Convert API types to scope string
        /// </summary>
        /// <param name="registrationScope"></param>
        /// <returns></returns>
        private static string ApiTypesToScope(RegistrationScopeEnum registrationScope)
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
            TokenEndpointAuthMethod tokenEndpointAuthMethod,
            ProcessedSoftwareStatementProfile sProfile,
            RegistrationScopeEnum registrationScope,
            BankRegistrationClaimsOverrides? bankClientRegistrationClaimsOverrides,
            string bankXFapiFinancialId,
            bool useTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues)
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

            // Convert tokenEndpointAuthMethod to spec type
            ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum
                tokenEndpointAuthMethodLocal;
            tokenEndpointAuthMethodLocal = tokenEndpointAuthMethod switch
            {
                TokenEndpointAuthMethod.ClientSecretBasic => ClientRegistrationModelsPublic
                    .OBRegistrationProperties1tokenEndpointAuthMethodEnum.ClientSecretBasic,
                TokenEndpointAuthMethod.PrivateKeyJwt => ClientRegistrationModelsPublic
                    .OBRegistrationProperties1tokenEndpointAuthMethodEnum.PrivateKeyJwt,
                TokenEndpointAuthMethod.TlsClientAuth => ClientRegistrationModelsPublic
                    .OBRegistrationProperties1tokenEndpointAuthMethodEnum.TlsClientAuth,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(tokenEndpointAuthMethod),
                    tokenEndpointAuthMethod,
                    null)
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
            string tlsClientAuthSubjectDn;
            if (sProfile.TransportCertificateType is TransportCertificateType.OBLegacy)
            {
                tlsClientAuthSubjectDn =
                    $"CN={sProfile.SoftwareStatementPayload.SoftwareId},OU={sProfile.SoftwareStatementPayload.OrgId},O=OpenBanking,C=GB";
            }
            else
            {
                tlsClientAuthSubjectDn = useTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues
                    ? sProfile.TransportCertificateDnWithStringDottedDecimalAttributeValues
                    : sProfile.TransportCertificateDnWithHexDottedDecimalAttributeValues;
            }

            var registrationClaims =
                new ClientRegistrationModelsPublic.OBClientRegistration1
                {
                    Iss = bankClientRegistrationClaimsOverrides?.IssuerIsSoftwareStatementXFapiFinancialId ?? false
                        ? sProfile.SoftwareStatementPayload.OrgId
                        : sProfile.SoftwareStatementPayload.SoftwareId,
                    Iat = DateTimeOffset.Now,
                    Exp = DateTimeOffset.UtcNow.AddMinutes(30),
                    Aud = bankClientRegistrationClaimsOverrides?.Audience ??
                          bankXFapiFinancialId,
                    Jti = Guid.NewGuid().ToString(),
                    TokenEndpointAuthMethod = tokenEndpointAuthMethodLocal,
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
                    TlsClientAuthSubjectDn = tlsClientAuthSubjectDn
                };

            if (!(bankClientRegistrationClaimsOverrides?.SubjectType is null))
            {
                registrationClaims.SubjectType = bankClientRegistrationClaimsOverrides.SubjectType;
            }

            return registrationClaims;
        }
    }
}
