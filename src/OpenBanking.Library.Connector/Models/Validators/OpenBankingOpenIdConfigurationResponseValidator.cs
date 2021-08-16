// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators
{
    public class OpenBankingOpenIdConfigurationResponseValidator : AbstractValidator<OpenIdConfiguration>
    {
        public OpenBankingOpenIdConfigurationResponseValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.RegistrationEndpoint)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must(ValidationRules.IsUrl)
                .WithMessage($"{nameof(OpenIdConfiguration.RegistrationEndpoint)} is missing or invalid.");
        }
    }
}
