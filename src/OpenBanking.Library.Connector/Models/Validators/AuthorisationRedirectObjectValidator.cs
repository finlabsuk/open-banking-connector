// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators
{
    public class AuthorisationRedirectObjectValidator : AbstractValidator<AuthorisationRedirectObject>
    {
        public AuthorisationRedirectObjectValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.ResponseMode)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Missing {nameof(AuthorisationRedirectObject.ResponseMode)}.");


            RuleFor(x => x.Response)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"Missing {nameof(AuthorisationRedirectObject.Response)}.");

            RuleFor(x => x.Response)
                .SetValidator(new AuthResultValidator());
        }
    }
}
