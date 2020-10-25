﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class PaymentRequestValidator : AbstractValidator<DomesticPayment>
    {
        public PaymentRequestValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            // RuleFor(x => x.ConsentId)
            //     .Must(ValidationRules.IsNonWhitespace)
            //     .WithMessage($"Missing or invalid {nameof(DomesticPayment.ConsentId)}.");

            // RuleFor(x => x.RedirectUrl)
            //     .Must(ValidationRules.IsNotNull)
            //     .Must(ValidationRules.IsAbsoluteUrl)
            //     .WithMessage($"Missing or invalid {nameof(DomesticPaymentRequest.RedirectUrl)}.");
        }
    }
}
