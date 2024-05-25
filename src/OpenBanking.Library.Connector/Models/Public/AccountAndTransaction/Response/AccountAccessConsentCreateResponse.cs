// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;

/// <summary>
///     Response to AccountAccessConsent Create and Read requests.
/// </summary>
public class AccountAccessConsentCreateResponse : ConsentBaseResponse
{
    /// <summary>
    ///     Response object OBReadConsentResponse1 from UK Open Banking Read-Write Account and Transaction API spec.
    /// </summary>
    public required AccountAndTransactionModelsPublic.OBReadConsentResponse1? ExternalApiResponse { get; init; }

    /// <summary>
    ///     Additional info relating to response from external (bank) API.
    /// </summary>
    public required ExternalApiResponseInfo? ExternalApiResponseInfo { get; init; }
}
