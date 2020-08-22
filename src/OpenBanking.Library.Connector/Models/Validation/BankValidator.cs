// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation
{
    public class BankValidator : AbstractValidator<Bank>
    {
        public  BankValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.Name)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Missing or invalid {nameof(Bank.Name)}.");

            RuleFor(x => x.IssuerUrl)
                .Must(ValidationRules.IsUrl)
                .WithMessage($"Missing or invalid {nameof(Bank.IssuerUrl)}.");

            RuleFor(x => x.XFapiFinancialId)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Missing or invalid {nameof(Bank.XFapiFinancialId)}.");
        }
    }
}
