// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class OpenBankingDomesticConsentContextValidator : AbstractValidator<DomesticPaymentConsentContext>
    {
        public OpenBankingDomesticConsentContextValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.ApiProfileId)
                .Must(ValidationRules.IsNonWhitespace)
                .WithMessage($"Invalid {nameof(DomesticPaymentConsentContext.ApiProfileId)}.");

            RuleFor(x => x.Data)
                .SetValidator(new OBWriteDomesticConsentValidator())
                .WithMessage($"Invalid {nameof(DomesticPaymentConsentContext.Data)}.");
        }
    }
}
