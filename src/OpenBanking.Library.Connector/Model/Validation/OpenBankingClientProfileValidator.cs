// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Validation
{
    public class OpenBankingClientProfileValidator : AbstractValidator<OpenBankingClientProfile>
    {
        public OpenBankingClientProfileValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.PaymentInitiationApiBaseUrl)
                .Must(ValidationRules.IsUrl)
                .WithMessage($"Invalid {nameof(OpenBankingClientProfile.PaymentInitiationApiBaseUrl)}: must be a URL.");

            RuleFor(x => x.OpenBankingClient)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"Missing {nameof(OpenBankingClientProfile.OpenBankingClient)}.");

            RuleFor(x => x.OpenBankingClient)
                .SetValidator(new OpenBankingClientValidator());
        }
    }
}
