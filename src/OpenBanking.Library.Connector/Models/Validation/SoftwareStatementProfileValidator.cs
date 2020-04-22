// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation
{
    public class SoftwareStatementProfileValidator : AbstractValidator<SoftwareStatementProfile>
    {
        public SoftwareStatementProfileValidator()
        {
            CascadeMode = CascadeMode.Continue;

            CreateRules();
        }


        private void CreateRules()
        {
            RuleFor(p => p.SoftwareStatement)
                .Must(ValidationRules.IsNonWhitespace)
                .WithMessage($"Please provide a {nameof(SoftwareStatementProfile.SoftwareStatement)}.");

            RuleFor(p => p.SoftwareStatement)
                .Must((x, y, z) => ValidationRules.HasDelimiters(x, y, z, '.', 2))
                .WithMessage($"Please provide a valid {nameof(SoftwareStatementProfile.SoftwareStatement)}.");

            RuleFor(p => p.SigningKeyId)
                .Must(ValidationRules.IsNonWhitespace)
                .WithMessage("Please provide a Signing Key ID.");

            RuleFor(p => p.SigningKeySecretName)
                .Must(ValidationRules.IsNonWhitespace)
                .WithMessage("Please provide a SigningKey.");

            RuleFor(p => p.SigningCertificate)
                .Must(ValidationRules.IsNonWhitespace)
                .WithMessage("Please provide a SigningCertificate.");

            RuleFor(p => p.TransportKeySecretName)
                .Must(ValidationRules.IsNonWhitespace)
                .WithMessage("Please provide a TransportKey.");

            RuleFor(p => p.TransportCertificate)
                .Must(ValidationRules.IsNonWhitespace)
                .WithMessage("Please provide a TransportCertificate.");

            RuleFor(p => p.DefaultFragmentRedirectUrl)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(ValidationRules.IsNonWhitespace)
                .WithMessage($"Please provide a {nameof(SoftwareStatementProfile.DefaultFragmentRedirectUrl)}.")
                .Must(ValidationRules.IsUrl)
                .WithMessage(
                    $"Please provide a valid URL for {nameof(SoftwareStatementProfile.DefaultFragmentRedirectUrl)}.");
        }
    }
}
