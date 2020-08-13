// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class OBRiskValidator : AbstractValidator<OBRisk1>
    {
        public OBRiskValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            const int merchantCustomerIdLength = 70;
            const int merchantCategoryMinLength = 3;
            const int merchantCategoryMaxLength = 4;

            RuleFor(r => r.MerchantCategoryCode)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must(
                    (x, y, z) => ValidationRules.HasLengthAtLeast(
                        arg1: x,
                        arg2: y,
                        arg3: z,
                        minLength: merchantCategoryMinLength))
                .Must(
                    (x, y, z) => ValidationRules.HasLengthAtMost(
                        arg1: x,
                        arg2: y,
                        arg3: z,
                        maxLength: merchantCategoryMaxLength))
                .WithMessage(
                    $"Invalid value for MerchantCategoryCode, length must be between {merchantCategoryMinLength} & {merchantCategoryMaxLength}.");

            RuleFor(r => r.MerchantCustomerIdentification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must(
                    (x, y, z) => ValidationRules.HasLengthAtMost(
                        arg1: x,
                        arg2: y,
                        arg3: z,
                        maxLength: merchantCustomerIdLength))
                .WithMessage(
                    $"Invalid value for MerchantCategoryCode, length must be up to {merchantCustomerIdLength}.");

            RuleFor(x => x.DeliveryAddress)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"The {nameof(OBRisk1.DeliveryAddress)} is missing.");

            RuleFor(x => x.DeliveryAddress)
                .SetValidator(new OBRiskDeliveryAddressValidator());
        }
    }
}
