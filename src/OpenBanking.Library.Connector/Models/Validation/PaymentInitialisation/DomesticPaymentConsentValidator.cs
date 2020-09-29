// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class DomesticPaymentConsentValidator : AbstractValidator<DomesticPaymentConsent>
    {
        public DomesticPaymentConsentValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            const int idLength = 35;

            // RuleFor(x => x.BankProfileId)
            //     .Must(ValidationRules.IsNonWhitespace)
            //     .WithMessage($"Missing or invalid {nameof(DomesticPaymentConsent.BankProfileId)}.");

            RuleFor(x => x.CreditorAccount)
                .SetValidator(new OBWriteDomesticDataInitiationCreditorAccountValidator())
                .WithMessage($"Invalid {nameof(DomesticPaymentConsent.CreditorAccount)}.");

            RuleFor(x => x.Merchant)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"The {nameof(DomesticPaymentConsent.Merchant)} data is missing.");

            RuleFor(d => d.InstructionIdentification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(arg1: x, arg2: y, arg3: z, maxLength: idLength))
                .WithMessage(
                    $"Invalid value for InstructionIdentification, it must not be empty and length must be less than {idLength}.");

            RuleFor(d => d.EndToEndIdentification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(arg1: x, arg2: y, arg3: z, maxLength: idLength))
                .WithMessage(
                    $"Invalid value for EndToEndIdentification, it must not be empty and length must be less than {idLength}.");
        }
    }
}
