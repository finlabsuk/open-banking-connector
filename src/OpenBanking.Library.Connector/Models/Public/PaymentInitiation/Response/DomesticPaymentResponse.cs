﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

/// <summary>
///     Response to Read and Create requests
/// </summary>
public class DomesticPaymentResponse
{
    public DomesticPaymentResponse(PaymentInitiationModelsPublic.OBWriteDomesticResponse5 externalApiResponse)
    {
        ExternalApiResponse = externalApiResponse;
    }

    public PaymentInitiationModelsPublic.OBWriteDomesticResponse5 ExternalApiResponse { get; }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; set; }
}
