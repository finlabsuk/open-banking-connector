﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators
{
    public class OpenIdConfigurationOverridesValidator : AbstractValidator<OpenIdConfigurationOverrides>
    {
        public OpenIdConfigurationOverridesValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            // RuleFor(x => x.RegistrationEndpoint)
            //     .Must(ValidationRules.IsUrl)
            //     .WithMessage($"Missing or invalid {nameof(OpenIdConfigurationOverrides.RegistrationEndpoint)}.");
        }
    }
}
