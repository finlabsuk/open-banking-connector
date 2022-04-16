﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators
{
    public class PaymentInitiationApiRequestValidator : AbstractValidator<PaymentInitiationApiRequest>
    {
        public PaymentInitiationApiRequestValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.BaseUrl)
                .Must(ValidationRules.IsUrl)
                .WithMessage($"Invalid {nameof(PaymentInitiationApiRequest.BaseUrl)}: must be a URL.");
        }
    }
}