// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
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
            // Check RegistrationEndpoint
            RuleFor(x => x.RegistrationEndpoint)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must(ValidationRules.IsUrl)
                .WithMessage($"{nameof(OpenIdConfiguration.RegistrationEndpoint)} is missing or invalid.");

            // Check TokenEndpointAuthMethodsSupported
            RuleFor(x => x.TokenEndpointAuthMethodsSupported)
                .Must(ValidationRules.IsNotNull) // not null
                .Must(x => x.Any()) // contains at least one value
                .WithMessage($"{nameof(OpenIdConfiguration.TokenEndpointAuthMethodsSupported)} is missing or empty.");
            RuleForEach(x => x.TokenEndpointAuthMethodsSupported)
                .IsInEnum() // values are valid enums
                .WithMessage($"{nameof(OpenIdConfiguration.TokenEndpointAuthMethodsSupported)} has invalid values.");
        }
    }
}
