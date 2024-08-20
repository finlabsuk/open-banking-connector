// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Nodes;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using Jose;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal interface IGrantPost
{
    Task<TokenEndpointResponse> PostClientCredentialsGrantAsync(
        string? requestScope,
        OBSealKey obSealKey,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        string tokenEndpoint,
        string externalApiClientId,
        string? externalApiClientSecret,
        JsonSerializerSettings? jsonSerializerSettings,
        ClientCredentialsGrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour,
        IApiClient mtlsApiClient,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        Dictionary<string, JsonNode?>? extraClaims = null,
        bool includeClientIdWithPrivateKeyJwt = false,
        JwsAlgorithm? jwsAlgorithm = null);

    Task<TokenEndpointResponse> PostAuthCodeGrantAsync(
        string authCode,
        string redirectUrl,
        string bankIssuerUrl,
        string externalApiClientId,
        string? externalApiClientSecret,
        string externalApiConsentId,
        string? externalApiUserId,
        string expectedNonce,
        string requestScope,
        OBSealKey obSealKey,
        string jwksUri,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        bool expectRefreshToken,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        IdTokenSubClaimType idTokenSubClaimType,
        string? codeVerifier,
        JsonSerializerSettings? jsonSerializerSettings,
        AuthCodeGrantPostCustomBehaviour? authCodeGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        IApiClient mtlsApiClient);

    Task<TokenEndpointResponse> PostRefreshTokenGrantAsync(
        string refreshToken,
        string bankIssuerUrl,
        string externalApiClientId,
        string? externalApiClientSecret,
        string externalApiConsentId,
        string? externalApiUserId,
        string expectedNonce,
        string refreshTokenScope,
        OBSealKey obSealKey,
        string jwksUri,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        IdTokenSubClaimType idTokenSubClaimType,
        JsonSerializerSettings? jsonSerializerSettings,
        RefreshTokenGrantPostCustomBehaviour? refreshTokenGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        IApiClient mtlsApiClient);

    Task<string?> ValidateIdTokenAuthEndpoint(
        string idToken,
        string code,
        string state,
        IdTokenProcessingCustomBehaviour? idTokenProcessingCustomBehaviour,
        string jwksUri,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string expectedNonce,
        bool supportsSca,
        BankProfileEnum? bankProfileForTppReportingMetrics,
        IdTokenSubClaimType idTokenSubClaimType,
        string? externalApiUserId);

    TimeSpan GetTokenAdjustedDuration(int expiresInSeconds);
}
