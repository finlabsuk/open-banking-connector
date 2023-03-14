﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;

public class AuthResultValidator : AbstractValidator<OAuth2RedirectData>
{
    public AuthResultValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        CreateRules();
    }

    private void CreateRules()
    {
        RuleFor(x => x.Code)
            .Must(ValidationRules.IsNotNullOrEmpty)
            .WithMessage($"Missing or invalid {nameof(OAuth2RedirectData.Code)}.");

        RuleFor(x => x.State)
            .Must(ValidationRules.IsNotNullOrEmpty)
            .WithMessage($"Missing or invalid {nameof(OAuth2RedirectData.State)}.");
    }
}
