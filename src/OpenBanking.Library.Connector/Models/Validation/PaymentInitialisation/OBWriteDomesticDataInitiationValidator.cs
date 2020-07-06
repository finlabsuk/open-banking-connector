// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class OBWriteDomesticDataInitiationValidator : AbstractValidator<OBWriteDomesticDataInitiation>
    {
        public OBWriteDomesticDataInitiationValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            const int idLength = 35;

            RuleFor(d => d.InstructionIdentification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, idLength))
                .WithMessage(
                    $"Invalid value for InstructionIdentification, it must not be empty and length must be less than {idLength}.");

            RuleFor(d => d.EndToEndIdentification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, idLength))
                .WithMessage(
                    $"Invalid value for EndToEndIdentification, it must not be empty and length must be less than {idLength}.");

            // TODO: OBWriteDomesticDataInitiationInstructedAmount & other nested properties
        }
    }
}
