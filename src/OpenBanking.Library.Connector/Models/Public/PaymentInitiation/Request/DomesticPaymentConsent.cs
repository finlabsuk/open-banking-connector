﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.PaymentInitialisation;
using FluentValidation.Results;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;

public class DomesticPaymentConsentRequest : ConsentRequestBase, ISupportsValidation
{
    /// <summary>
    ///     Use external API request object created from template.
    ///     The first non-null of ExternalApiObject, ExternalApiRequest, and TemplateRequest (in that order) is used
    ///     and the others are ignored. At least one of these three must be non-null.
    ///     Specifies template used to create external API request object.
    /// </summary>
    public DomesticPaymentTemplateRequest? TemplateRequest { get; set; }

    /// <summary>
    ///     Request object from recent version of UK Open Banking spec. Open Banking Connector can be configured
    ///     to translate this for banks supporting an earlier spec version.
    /// </summary>
    public PaymentInitiationModelsPublic.OBWriteDomesticConsent4? ExternalApiRequest { get; set; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new DomesticPaymentConsentValidator()
            .ValidateAsync(this)!;
}
