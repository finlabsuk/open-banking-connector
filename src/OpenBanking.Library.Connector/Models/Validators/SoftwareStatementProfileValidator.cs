// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;

public class SoftwareStatementProfileValidator : AbstractValidator<SoftwareStatementProfile>
{
    public SoftwareStatementProfileValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;
        CreateRules();
    }

    internal static bool HasDelimiters<T>(
        T arg1,
        string? arg2,
        ValidationContext<T> arg3,
        char delimiter,
        int maxLength) =>
        arg2 != null && arg2.DelimiterCount(delimiter) == maxLength;


    private void CreateRules()
    {
        RuleFor(p => p.SoftwareStatement)
            .Must(ValidationRules.IsNonWhitespace)
            .WithMessage($"Please provide a {nameof(SoftwareStatementProfile.SoftwareStatement)}.");

        RuleFor(p => p.SoftwareStatement)
            .Must((x, y, z) => HasDelimiters(x, y, z, '.', 2))
            .WithMessage($"Please provide a valid {nameof(SoftwareStatementProfile.SoftwareStatement)}.");

        When(
            x => !string.IsNullOrEmpty(x.DefaultFragmentRedirectUrl),
            () =>
            {
                RuleFor(p => p.DefaultFragmentRedirectUrl)
                    .Must(ValidationRules.IsUrl)
                    .WithMessage(
                        $"Please provide a valid URL for {nameof(SoftwareStatementProfile.DefaultFragmentRedirectUrl)}.");
            });

        When(
            x => !string.IsNullOrEmpty(x.DefaultQueryRedirectUrl),
            () =>
            {
                RuleFor(p => p.DefaultQueryRedirectUrl)
                    .Must(ValidationRules.IsUrl)
                    .WithMessage(
                        $"Please provide a valid URL for {nameof(SoftwareStatementProfile.DefaultQueryRedirectUrl)}.");
            });
    }
}
