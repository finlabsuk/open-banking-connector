﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.VariableRecurringPayments
{
    public class DomesticVrpValidator : AbstractValidator<DomesticVrp>
    {
        public DomesticVrpValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.DomesticVrpConsentId)
                .NotEmpty()
                .WithMessage($"Missing or invalid {nameof(DomesticVrp.DomesticVrpConsentId)}.");

            // RuleFor(x => x.RedirectUrl)
            //     .Must(ValidationRules.IsNotNull)
            //     .Must(ValidationRules.IsAbsoluteUrl)
            //     .WithMessage($"Missing or invalid {nameof(DomesticPaymentRequest.RedirectUrl)}.");
        }
    }
}