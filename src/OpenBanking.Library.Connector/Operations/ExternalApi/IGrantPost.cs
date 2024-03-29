﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using Newtonsoft.Json;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal interface IGrantPost
{
    Task<string> PostClientCredentialsGrantAsync(
        string? scope,
        OBSealKey obSealKey,
        BankRegistration bankRegistration,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        JsonSerializerSettings? jsonSerializerSettings,
        GrantPostCustomBehaviour? clientCredentialsGrantPostCustomBehaviour,
        IApiClient mtlsApiClient);

    Task<TokenEndpointResponseAuthCodeGrant> PostAuthCodeGrantAsync(
        string authCode,
        string redirectUrl,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string? externalApiUserId,
        string expectedNonce,
        string? requestScope,
        OBSealKey obSealKey,
        BankRegistration bankRegistration,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        IdTokenSubClaimType idTokenSubClaimType,
        JsonSerializerSettings? jsonSerializerSettings,
        GrantPostCustomBehaviour? authCodeGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        IApiClient matlsApiClient);

    Task<TokenEndpointResponseRefreshTokenGrant> PostRefreshTokenGrantAsync(
        string refreshToken,
        string redirectUrl,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string? externalApiUserId,
        string expectedNonce,
        string? requestScope,
        OBSealKey obSealKey,
        BankRegistration bankRegistration,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        string tokenEndpoint,
        bool supportsSca,
        IdTokenSubClaimType idTokenSubClaimType,
        JsonSerializerSettings? jsonSerializerSettings,
        GrantPostCustomBehaviour? refreshTokenGrantPostCustomBehaviour,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        IApiClient mtlsApiClient);

    Task<string?> ValidateIdTokenAuthEndpoint(
        OAuth2RedirectData redirectData,
        ConsentAuthGetCustomBehaviour? consentAuthGetCustomBehaviour,
        string jwksUri,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string expectedNonce,
        bool supportsSca,
        IdTokenSubClaimType idTokenSubClaimType,
        string? externalApiUserId);

    TimeSpan GetTokenAdjustedDuration(int expiresInSeconds);
}
