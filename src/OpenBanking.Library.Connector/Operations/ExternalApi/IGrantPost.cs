// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
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
        string? requestScope,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
        BankRegistration bankRegistration,
        string tokenEndpoint,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient matlsApiClient,
        IInstrumentationClient instrumentationClient);

    Task<RefreshTokenGrantResponse> PostRefreshTokenGrantAsync(
        string refreshToken,
        string redirectUrl,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string nonce,
        string? requestScope,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
        BankRegistration bankRegistration,
        string tokenEndpoint,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient mtlsApiClient,
        IInstrumentationClient instrumentationClient);

    Task ValidateIdTokenAuthEndpoint(
        OAuth2RedirectData redirectData,
        ConsentAuthGetCustomBehaviour? consentAuthGetCustomBehaviour,
        string jwksUri,
        JwksGetCustomBehaviour? jwksGetCustomBehaviour,
        string bankIssuerUrl,
        string externalApiClientId,
        string externalApiConsentId,
        string nonce,
        bool supportsSca);
}
