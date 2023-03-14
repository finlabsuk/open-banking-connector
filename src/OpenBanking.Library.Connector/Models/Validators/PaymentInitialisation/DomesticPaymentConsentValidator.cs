// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FluentValidation;
using PaymentInitiationValidatorsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Validators;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.PaymentInitialisation;

public class DomesticPaymentConsentValidator : AbstractValidator<DomesticPaymentConsentRequest>
{
    public DomesticPaymentConsentValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;
        CreateRules();
    }

    private void CreateRules()
    {
        // ExternalApiRequest
        When(
            x => x.ExternalApiRequest is not null,
            () =>
                RuleFor(x => x.ExternalApiRequest!)
                    .SetValidator(new PaymentInitiationValidatorsPublic.OBWriteDomesticConsent4Validator()));
    }
}
