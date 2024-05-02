// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;

public class DomesticPaymentConsentRequest : ConsentBase, ISupportsValidation
{
    /// <summary>
    ///     Request object OBWriteDomesticConsent4 from UK Open Banking Read-Write Payment Initiation API spec.
    /// </summary>
    public PaymentInitiationModelsPublic.OBWriteDomesticConsent4? ExternalApiRequest { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new DomesticPaymentConsentValidator()
            .ValidateAsync(this);
}
