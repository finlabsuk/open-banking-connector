// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;

/// <summary>
///     Response to DomesticVrpConsent Create and Read requests.
/// </summary>
public class DomesticVrpConsentCreateResponse : ConsentBaseResponse
{
    /// <summary>
    ///     Migrated to v4 external (bank) API consent
    /// </summary>
    public required bool MigratedToV4 { get; init; }

    public required DateTimeOffset MigratedToV4Modified { get; init; }

    /// <summary>
    ///     Response object OBDomesticVRPConsentResponse from UK Open Banking Read-Write Variable Recurring Payments API spec.
    /// </summary>
    public VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? ExternalApiResponse { get; init; }

    /// <summary>
    ///     Additional info relating to response from external (bank) API.
    /// </summary>
    public required ExternalApiResponseInfo? ExternalApiResponseInfo { get; init; }
}
