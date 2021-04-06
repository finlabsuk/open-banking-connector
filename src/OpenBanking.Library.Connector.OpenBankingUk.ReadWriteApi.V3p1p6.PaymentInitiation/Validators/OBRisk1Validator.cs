// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Validators
{
    public class OBRisk1Validator : AbstractValidator<OBRisk1>
    {
        public OBRisk1Validator()
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
                        x,
                        y,
                        z,
                        merchantCategoryMinLength))
                .Must(
                    (x, y, z) => ValidationRules.HasLengthAtMost(
                        x,
                        y,
                        z,
                        merchantCategoryMaxLength))
                .WithMessage(
                    $"Invalid value for MerchantCategoryCode, length must be between {merchantCategoryMinLength} & {merchantCategoryMaxLength}.");

            RuleFor(r => r.MerchantCustomerIdentification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must(
                    (x, y, z) => ValidationRules.HasLengthAtMost(
                        x,
                        y,
                        z,
                        merchantCustomerIdLength))
                .WithMessage(
                    $"Invalid value for MerchantCategoryCode, length must be up to {merchantCustomerIdLength}.");

            RuleFor(x => x.DeliveryAddress)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"The {nameof(OBRisk1.DeliveryAddress)} is missing.");

            RuleFor(x => x.DeliveryAddress)
                .SetValidator(new OBRisk1DeliveryAddressValidator());
        }
    }
}
