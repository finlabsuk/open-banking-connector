// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.PaymentInitialisation
{
    public class DomesticPaymentValidator : AbstractValidator<DomesticPayment>
    {
        public DomesticPaymentValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.DomesticPaymentConsentId)
                .NotEmpty()
                .WithMessage($"Missing or invalid {nameof(DomesticPayment.DomesticPaymentConsentId)}.");

            // RuleFor(x => x.RedirectUrl)
            //     .Must(ValidationRules.IsNotNull)
            //     .Must(ValidationRules.IsAbsoluteUrl)
            //     .WithMessage($"Missing or invalid {nameof(DomesticPaymentRequest.RedirectUrl)}.");
        }
    }
}
