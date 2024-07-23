// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Validators;

public class AuthResultValidator : AbstractValidator<AuthResult>
{
    public AuthResultValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        CreateRules();
    }

    private void CreateRules()
    {
        RuleFor(x => x.OAuth2RedirectOptionalParameters)
            .Must(ValidationRules.IsNotNull)
            .WithMessage($"Missing {nameof(AuthResult.OAuth2RedirectOptionalParameters)}.");

        RuleFor(x => x.State)
            .Must(ValidationRules.IsNotNullOrEmpty)
            .WithMessage($"Missing or invalid {nameof(AuthResult.State)}.");
    }
}
