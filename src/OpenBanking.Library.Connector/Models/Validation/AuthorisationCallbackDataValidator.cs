// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation
{
    public class AuthorisationCallbackDataValidator : AbstractValidator<AuthorisationCallbackData>
    {
        public AuthorisationCallbackDataValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.Method)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Missing {nameof(AuthorisationCallbackData.Method)}.");

            RuleFor(x => x.Mode)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Missing {nameof(AuthorisationCallbackData.Mode)}.");


            RuleFor(x => x.Body)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"Missing {nameof(AuthorisationCallbackData.Body)}.");

            RuleFor(x => x.Body)
                .SetValidator(new AuthorisationCallbackBodyValidator());
        }
    }
}
