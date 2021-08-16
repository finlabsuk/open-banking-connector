// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FluentValidation;
using PaymentInitiationValidatorsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.UkRwApi.V3p1p6.PaymentInitiation.Validators;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.PaymentInitialisation
{
    public class DomesticPaymentConsentValidator : AbstractValidator<DomesticPaymentConsent>
    {
        public DomesticPaymentConsentValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            // WriteDomesticConsent
            RuleFor(x => x.WriteDomesticConsent)
                .SetValidator(new PaymentInitiationValidatorsPublic.OBWriteDomesticConsent4Validator());
        }
    }
}
