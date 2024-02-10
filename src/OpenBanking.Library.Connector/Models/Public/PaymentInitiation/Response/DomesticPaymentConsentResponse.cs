// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

/// <summary>
///     Response to DomesticPaymentConsent Create and Read requests
/// </summary>
public class DomesticPaymentConsentCreateResponse : ConsentBaseResponse
{
    public PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5? ExternalApiResponse { get; init; }
}

/// <summary>
///     Response to DomesticPaymentConsent ReadFundsConfirmation requests
/// </summary>
public class DomesticPaymentConsentReadFundsConfirmationResponse : BaseResponse
{
    public required PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 ExternalApiResponse { get; init; }
}
