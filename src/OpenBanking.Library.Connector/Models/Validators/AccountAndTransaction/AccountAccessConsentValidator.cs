// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.AccountAndTransaction;

public class AccountAccessConsentValidator : AbstractValidator<AccountAccessConsentRequest>
{
    public AccountAccessConsentValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        CreateRules();
    }

    private void CreateRules()
    {
        // ExternalApiRequest
        // When(
        //     x => x.ExternalApiRequest is not null,
        //     () =>
        //         RuleFor(x => x.ExternalApiRequest!)
        //             .SetValidator(new AccountAndTransactionModelsPublic.OBReadConsent1Validator()));
    }
}
