// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal static class RegistrationClaimsFactory
{
    public static ClientRegistrationModelsPublic.OBClientRegistration1 CreateRegistrationClaims(
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        IList<string> redirectUris,
        string softwareId,
        string tlsClientAuthSubjectDn,
        string softwareStatementAssertion,
        RegistrationScopeEnum registrationScope,
        BankRegistrationPostCustomBehaviour? bankRegistrationPostCustomBehaviour,
        string bankFinancialId)
    {
        string scope = registrationScope.GetScopeString();

        // Convert tokenEndpointAuthMethod to spec type
        ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum
            tokenEndpointAuthMethodLocal = tokenEndpointAuthMethod switch
            {
                TokenEndpointAuthMethodSupportedValues.ClientSecretBasic => ClientRegistrationModelsPublic
                    .OBRegistrationProperties1tokenEndpointAuthMethodEnum.ClientSecretBasic,
                TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt => ClientRegistrationModelsPublic
                    .OBRegistrationProperties1tokenEndpointAuthMethodEnum.PrivateKeyJwt,
                TokenEndpointAuthMethodSupportedValues.TlsClientAuth => ClientRegistrationModelsPublic
                    .OBRegistrationProperties1tokenEndpointAuthMethodEnum.TlsClientAuth,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(tokenEndpointAuthMethod),
                    tokenEndpointAuthMethod,
                    null)
            };

        IList<ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum> grantTypes =
            bankRegistrationPostCustomBehaviour?.GrantTypesClaim ??
            new[]
            {
                ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum
                    .ClientCredentials,
                ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum
                    .AuthorizationCode,
                ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum.RefreshToken
            };

        var registrationClaims =
            new ClientRegistrationModelsPublic.OBClientRegistration1
            {
                Iss = softwareId,
                Iat = DateTimeOffset.Now,
                Exp = DateTimeOffset.UtcNow.AddSeconds(300),
                Aud = bankRegistrationPostCustomBehaviour?.AudClaim ??
                      bankFinancialId,
                Jti = Guid.NewGuid().ToString(),
                TokenEndpointAuthMethod = tokenEndpointAuthMethodLocal,
                GrantTypes = grantTypes,
                ResponseTypes =
                    new[] { ClientRegistrationModelsPublic.OBRegistrationProperties1responseTypesItemEnum.CodeidToken },
                ApplicationType = ClientRegistrationModelsPublic.OBRegistrationProperties1applicationTypeEnum.Web,
                IdTokenSignedResponseAlg = ClientRegistrationModelsPublic.SupportedAlgorithmsEnum.PS256,
                RequestObjectSigningAlg = ClientRegistrationModelsPublic.SupportedAlgorithmsEnum.PS256,
                RedirectUris = redirectUris,
                SoftwareId = softwareId,
                Scope = scope,
                SoftwareStatement = softwareStatementAssertion,
                TlsClientAuthSubjectDn = tlsClientAuthSubjectDn
            };

        if (tokenEndpointAuthMethodLocal is ClientRegistrationModelsPublic
                .OBRegistrationProperties1tokenEndpointAuthMethodEnum.PrivateKeyJwt)
        {
            registrationClaims.TokenEndpointAuthSigningAlg =
                ClientRegistrationModelsPublic.SupportedAlgorithmsEnum.PS256;
        }

        if (!(bankRegistrationPostCustomBehaviour?.SubjectTypeClaim is null))
        {
            registrationClaims.SubjectType = bankRegistrationPostCustomBehaviour.SubjectTypeClaim;
        }

        return registrationClaims;
    }
}
