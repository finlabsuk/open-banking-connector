// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using FluentValidation;
using MSValidationException = Microsoft.Rest.ValidationException;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Validators
{
    public class
        OBWriteDomesticConsent4Validator : AbstractValidator<OBWriteDomesticConsent4>
    {
        public OBWriteDomesticConsent4Validator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;
            RuleLevelCascadeMode = CascadeMode.Continue;

            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x)
                // Validator based on Swagger
                .Must(
                    (_, value, context) =>
                    {
                        try
                        {
                            value.Validate();
                            return true;
                        }
                        catch (MSValidationException e)
                        {
                            context.MessageFormatter.AppendArgument("ExceptionMessage", e.Message);
                            return false;
                        }
                    }).WithMessage(
                    $"Invalid value for (nested) member of {typeof(OBWriteDomesticConsent4).FullName}: " +
                    "{ExceptionMessage}");

            // Custom validation
            RuleFor(x => x.Data)
                .SetValidator(new OBWriteDomesticConsent4DataValidator());

            RuleFor(x => x.Risk)
                .SetValidator(new OBRisk1Validator());
        }
    }
}
