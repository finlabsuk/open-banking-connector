﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;

/// <summary>
///     Response to DomesticVrp Read and Create requests
/// </summary>
public class DomesticVrpResponse : BaseResponse
{
    /// <summary>
    ///     Response object OBDomesticVRPResponse from UK Open Banking Read-Write Variable Recurring Payments API spec.
    /// </summary>
    public required VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse ExternalApiResponse { get; init; }

    /// <summary>
    ///     Additional info relating to response from external (bank) API.
    /// </summary>
    public required ExternalApiResponseInfo ExternalApiResponseInfo { get; init; }
}
