// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Validators
{
    public class
        OBWriteDomesticConsentResponse5DataValidator : AbstractValidator<
            OBWriteDomesticConsentResponse5Data>
    {
        public OBWriteDomesticConsentResponse5DataValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;
            RuleLevelCascadeMode = CascadeMode.Continue;

            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.Initiation)
                .SetValidator(new OBWriteDomesticConsentResponse5DataInitiationValidator());
        }
    }
}
