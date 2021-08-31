// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Validators
{
    public class
        OBWriteDomesticConsent4DataValidator : AbstractValidator<
            OBWriteDomesticConsent4Data>
    {
        public OBWriteDomesticConsent4DataValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.Initiation)
                .SetValidator(new OBWriteDomesticConsent4DataInitiationValidator());

            RuleFor(x => x.SCASupportData)
                .SetValidator(new OBWriteDomesticConsent4DataSCASupportDataValidator());
        }
    }
}
