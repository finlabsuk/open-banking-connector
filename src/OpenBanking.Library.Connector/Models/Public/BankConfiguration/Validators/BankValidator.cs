// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators
{
    public class BankValidator : AbstractValidator<Bank>
    {
        public BankValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            When(
                x => !string.IsNullOrEmpty(x.IssuerUrl),
                () =>
                {
                    RuleFor(x => x.IssuerUrl)
                        .Must(ValidationRules.IsUrl)
                        .WithMessage($"Missing or invalid {nameof(Bank.IssuerUrl)}.");
                });

            When(
                x => !string.IsNullOrEmpty(x.FinancialId),
                () =>
                {
                    RuleFor(x => x.FinancialId)
                        .Must(ValidationRules.IsNotNullOrEmpty)
                        .WithMessage($"Missing or invalid {nameof(Bank.FinancialId)}.");
                });
        }
    }
}
