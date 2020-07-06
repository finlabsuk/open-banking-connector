// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class OBWriteDomesticConsentValidator : AbstractValidator<OBWriteDomesticConsent>
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
                .WithMessage($"Missing or invalid {nameof(OBWriteDomesticConsent.ApiProfileId)}.");

            RuleFor(x => x.Data)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"The {nameof(OBWriteDomesticConsent.Data)} data is missing.");

            RuleFor(x => x.Risk)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"The {nameof(OBWriteDomesticConsent.Risk)} data is missing.");
        }
    }
}
