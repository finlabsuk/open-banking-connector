// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

/// <summary>
///     Response to DomesticPayment Read and Create requests.
/// </summary>
public class DomesticPaymentResponse : BaseResponse
{
    /// <summary>
    ///     Response object OBWriteDomesticResponse5 from UK Open Banking Read-Write Payment Initiation API spec.
    /// </summary>
    public required PaymentInitiationModelsPublic.OBWriteDomesticResponse5 ExternalApiResponse { get; init; }

    /// <summary>
    ///     Additional info relating to response from external (bank) API.
    /// </summary>
    public required ExternalApiResponseInfo ExternalApiResponseInfo { get; init; }
}
