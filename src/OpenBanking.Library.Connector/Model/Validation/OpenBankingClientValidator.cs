// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Validation
{
    public class OpenBankingClientValidator : AbstractValidator<BankClient>
    {
        public OpenBankingClientValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.SoftwareStatementProfileId)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Missing or invalid {nameof(BankClient.SoftwareStatementProfileId)}.");

            RuleFor(x => x.IssuerUrl)
                .Must(ValidationRules.IsUrl)
                .WithMessage($"Missing or invalid {nameof(BankClient.IssuerUrl)}.");

            RuleFor(x => x.XFapiFinancialId)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Missing or invalid {nameof(BankClient.XFapiFinancialId)}.");

            RuleFor(x => x.HttpMtlsOverrides)
                .SetValidator(new HttpClientMtlsConfigurationOverridesValidator())
                .WithMessage($"Missing or invalid {nameof(BankClient.HttpMtlsOverrides)}.");

            RuleFor(x => x.OpenIdConfigurationOverrides)
                .SetValidator(new OpenIdConfigurationOverridesValidator())
                .WithMessage($"Missing or invalid {nameof(BankClient.OpenIdConfigurationOverrides)}.");
        }
    }
}
