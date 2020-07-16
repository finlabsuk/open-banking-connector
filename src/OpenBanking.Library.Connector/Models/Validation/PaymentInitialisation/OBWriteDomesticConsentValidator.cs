// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class OBWriteDomesticConsentValidator : AbstractValidator<DomesticPaymentConsent>
    {
        public OBWriteDomesticConsentValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.ApiProfileId)
                .Must(ValidationRules.IsNonWhitespace)
                .WithMessage($"Missing or invalid {nameof(DomesticPaymentConsent.ApiProfileId)}.");

            RuleFor(x => x.DomesticConsent.Data)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"The {nameof(DomesticPaymentConsent.DomesticConsent.Data)} data is missing.");

            RuleFor(x => x.DomesticConsent.Risk)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"The {nameof(DomesticPaymentConsent.DomesticConsent.Risk)} data is missing.");
        }
    }
}
