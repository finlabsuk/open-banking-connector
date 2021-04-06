﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Validators
{
    public class
        OBWriteDomesticConsent4DataSCASupportDataValidator : AbstractValidator<
            OBWriteDomesticConsent4DataSCASupportData>
    {
        public OBWriteDomesticConsent4DataSCASupportDataValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.ReferencePaymentOrderId)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, 128))
                .WithMessage("Invalid value for ReferencePaymentOrderId, length must be less than 128.");
        }
    }
}
