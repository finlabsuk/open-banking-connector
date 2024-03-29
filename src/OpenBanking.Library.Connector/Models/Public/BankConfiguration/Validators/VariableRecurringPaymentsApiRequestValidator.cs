﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;

public class VariableRecurringPaymentsApiRequestValidator : AbstractValidator<VariableRecurringPaymentsApiRequest>
{
    public VariableRecurringPaymentsApiRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;
        CreateRules();
    }

    private void CreateRules()
    {
        When(
            x => !string.IsNullOrEmpty(x.BaseUrl),
            () =>
            {
                RuleFor(x => x.BaseUrl)
                    .Must(ValidationRules.IsUrl)
                    .WithMessage($"Invalid {nameof(VariableRecurringPaymentsApiRequest.BaseUrl)}: must be a URL.");
            });
    }
}
