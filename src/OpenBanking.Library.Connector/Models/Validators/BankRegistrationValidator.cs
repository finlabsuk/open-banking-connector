// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators
{
    public class BankRegistrationValidator : AbstractValidator<BankRegistration>
    {
        public BankRegistrationValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.RegistrationScope)
                .Must(x => (x & RegistrationScopeEnum.All) != RegistrationScopeEnum.None)
                .WithMessage(
                    $"{nameof(BankRegistration.RegistrationScope)} did not include one or more valid API types.");

            RuleFor(x => x.SoftwareStatementProfileId)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Missing or invalid {nameof(BankRegistration.SoftwareStatementProfileId)}.");

            RuleFor(x => x.OpenIdConfigurationOverrides)
                .SetValidator(
                    new NullableValidator<OpenIdConfigurationOverrides>(new OpenIdConfigurationOverridesValidator()))
                .WithMessage($"Missing or invalid {nameof(BankRegistration.OpenIdConfigurationOverrides)}.");
        }
    }
}
