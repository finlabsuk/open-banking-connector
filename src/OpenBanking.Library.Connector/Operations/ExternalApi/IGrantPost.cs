// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal interface IGrantPost
{
    Task<ClientCredentialsGrantResponse> PostClientCredentialsGrantAsync(
        string? scope,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
        BankRegistration bankRegistration,
        string tokenEndpoint,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient mtlsApiClient,
        IInstrumentationClient instrumentationClient);

    Task<AuthCodeGrantResponse> PostAuthCodeGrantAsync(
        string authCode,
        string redirectUrl,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string nonce,
        BankRegistration bankRegistration,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient matlsApiClient);

    Task<RefreshTokenGrantResponse> PostRefreshTokenGrantAsync(
        string refreshToken,
        string redirectUrl,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string nonce,
        BankRegistration bankRegistration,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient mtlsApiClient);

    Task ValidateIdTokenAuthEndpoint(
        OAuth2RedirectData redirectData,
        string jwksUri,
        bool jwksGetResponseHasNoRootProperty,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string nonce,
        bool supportsSca);
}
