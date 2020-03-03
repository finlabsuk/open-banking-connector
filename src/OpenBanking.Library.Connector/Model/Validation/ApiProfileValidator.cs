// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.Request.PaymentInitiation;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Validation
{
    public class ApiProfileValidator : AbstractValidator<ApiProfile>
    {
        public ApiProfileValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.BaseUrl)
                .Must(ValidationRules.IsUrl)
                .WithMessage($"Invalid {nameof(ApiProfile.BaseUrl)}: must be a URL.");

            // RuleFor(x => x.BankClient)
            //     .Must(ValidationRules.IsNotNull)
            //     .WithMessage($"Missing {nameof(BankClientProfile.BankClient)}.");
            //
            // RuleFor(x => x.BankClient)
            //     .SetValidator(new OpenBankingClientValidator());
        }
    }
}
