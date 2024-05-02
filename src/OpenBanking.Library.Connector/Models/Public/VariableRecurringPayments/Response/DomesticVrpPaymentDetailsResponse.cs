// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;

/// <summary>
///     Response to DomesticVrp ReadPaymentDetails requests
/// </summary>
public class DomesticVrpPaymentDetailsResponse : BaseResponse
{
    /// <summary>
    ///     Response object OBDomesticVRPDetails from UK Open Banking Read-Write Variable Recurring Payments API spec.
    /// </summary>
    public required VariableRecurringPaymentsModelsPublic.OBDomesticVRPDetails ExternalApiResponse { get; init; }

    /// <summary>
    ///     Additional info relating to response from external (bank) API.
    /// </summary>
    public required ExternalApiResponseInfo ExternalApiResponseInfo { get; init; }
}
