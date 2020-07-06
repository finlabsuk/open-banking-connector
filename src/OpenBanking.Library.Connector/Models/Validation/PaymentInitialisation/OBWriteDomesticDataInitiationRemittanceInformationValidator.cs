// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class
        OBWriteDomesticDataInitiationRemittanceInformationValidator : AbstractValidator<
            OBWriteDomesticDataInitiationRemittanceInformation>
    {
        public OBWriteDomesticDataInitiationRemittanceInformationValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.Unstructured)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, 140))
                .WithMessage("Invalid value for Unstructured, length must be less than 140.");

            RuleFor(x => x.Reference)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, 35))
                .WithMessage("Invalid value for Reference, length must be less than 35.");
        }
    }
}
