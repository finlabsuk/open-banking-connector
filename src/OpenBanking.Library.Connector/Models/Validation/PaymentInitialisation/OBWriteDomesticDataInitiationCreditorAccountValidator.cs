// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class
        OBWriteDomesticDataInitiationCreditorAccountValidator : AbstractValidator<
            OBWriteDomesticDataInitiationCreditorAccount>
    {
        public OBWriteDomesticDataInitiationCreditorAccountValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            const int idLength = 256;
            const int nameLength = 70;
            const int secondaryIdLength = 34;

            RuleFor(x => x.Identification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, idLength))
                .WithMessage($"Invalid value for Identification, length must be less than {idLength}.");

            RuleFor(x => x.Name)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, nameLength))
                .WithMessage($"Invalid value for Name, length must be less than {nameLength}.");

            RuleFor(x => x.SecondaryIdentification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, secondaryIdLength))
                .WithMessage(
                    $"Invalid value for SecondaryIdentification, length must be less than {secondaryIdLength}.");
        }
    }
}
