// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators
{
    public class BankApiInformationValidator : AbstractValidator<BankApiInformation>
    {
        public BankApiInformationValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            // RuleFor(x => x.BaseUrl)
            //     .Must(ValidationRules.IsUrl)
            //     .WithMessage($"Invalid {nameof(PaymentInitiationApiProfile.BaseUrl)}: must be a URL.");

            RuleFor(x => x.PaymentInitiationApi)
                .Must(ValidationRules.IsNotNull)
                .WithMessage($"Missing {nameof(BankApiInformation.PaymentInitiationApi)}.");
            //
            // RuleFor(x => x.BankClient)
            //     .SetValidator(new OpenBankingClientValidator());
        }
    }
}
