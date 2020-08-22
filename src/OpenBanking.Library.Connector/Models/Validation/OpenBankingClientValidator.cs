// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation
{
    public class OpenBankingClientValidator : AbstractValidator<BankRegistration>
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
                .WithMessage($"Missing or invalid {nameof(BankRegistration.SoftwareStatementProfileId)}.");

            // RuleFor(x => x.IssuerUrl)
            //     .Must(ValidationRules.IsUrl)
            //     .WithMessage($"Missing or invalid {nameof(BankRegistration.IssuerUrl)}.");
            //
            // RuleFor(x => x.XFapiFinancialId)
            //     .Must(ValidationRules.IsNotNullOrEmpty)
            //     .WithMessage($"Missing or invalid {nameof(BankRegistration.XFapiFinancialId)}.");

            RuleFor(x => x.HttpMtlsConfigurationOverrides)
                .SetValidator(new HttpClientMtlsConfigurationOverridesValidator())
                .WithMessage($"Missing or invalid {nameof(BankRegistration.HttpMtlsConfigurationOverrides)}.");

            RuleFor(x => x.OpenIdConfigurationOverrides)
                .SetValidator(new OpenIdConfigurationOverridesValidator())
                .WithMessage($"Missing or invalid {nameof(BankRegistration.OpenIdConfigurationOverrides)}.");
        }
    }
}
