// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi.Validators;

public class OpenBankingOpenIdConfigurationResponseValidator : AbstractValidator<OpenIdConfiguration>
{
    public OpenBankingOpenIdConfigurationResponseValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;
        CreateRules();
    }

    private void CreateRules()
    {
        // Check Issuer
        RuleFor(x => x.Issuer)
            .Must(ValidationRules.IsNotNullOrEmpty)
            .Must(ValidationRules.IsUrl)
            .WithMessage($"{nameof(OpenIdConfiguration.Issuer)} is missing or invalid.");

        // Check AuthorizationEndpoint
        RuleFor(x => x.AuthorizationEndpoint)
            .Must(ValidationRules.IsNotNullOrEmpty)
            .Must(ValidationRules.IsUrl)
            .WithMessage($"{nameof(OpenIdConfiguration.AuthorizationEndpoint)} is missing or invalid.");

        // Check TokenEndpoint
        RuleFor(x => x.TokenEndpoint)
            .Must(ValidationRules.IsNotNullOrEmpty)
            .Must(ValidationRules.IsUrl)
            .WithMessage($"{nameof(OpenIdConfiguration.TokenEndpoint)} is missing or invalid.");

        // Check JwksUri
        RuleFor(x => x.JwksUri)
            .Must(ValidationRules.IsNotNullOrEmpty)
            .Must(ValidationRules.IsUrl)
            .WithMessage($"{nameof(OpenIdConfiguration.JwksUri)} is missing or invalid.");

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
