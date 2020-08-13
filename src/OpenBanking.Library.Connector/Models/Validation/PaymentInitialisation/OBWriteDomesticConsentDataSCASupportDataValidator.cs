// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class
        OBWriteDomesticConsentDataScaSupportDataValidator : AbstractValidator<OBWriteDomesticConsent4DataSCASupportData>
    {
        public OBWriteDomesticConsentDataScaSupportDataValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.ReferencePaymentOrderId)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(arg1: x, arg2: y, arg3: z, maxLength: 128))
                .WithMessage("Invalid value for ReferencePaymentOrderId, length must be less than 128.");
        }
    }
}
