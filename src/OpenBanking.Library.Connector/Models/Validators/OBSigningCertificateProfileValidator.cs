// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;

public class OBSigningCertificateProfileValidator : AbstractValidator<SigningCertificateProfile>
{
    public OBSigningCertificateProfileValidator()
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
        RuleFor(p => p.AssociatedKeyId)
            .Must(ValidationRules.IsNonWhitespace)
            .WithMessage("Please provide a Signing Key ID.");

        RuleFor(p => p.AssociatedKey)
            .Must(ValidationRules.IsNonWhitespace)
            .WithMessage("Please provide a SigningKey.");
    }
}
