// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using FluentValidation;

//using MSValidationException = Microsoft.Rest.ValidationException;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Validators
{
    public class
        OBClientRegistration1ResponseValidator : AbstractValidator<OBClientRegistration1Response>
    {
        public OBClientRegistration1ResponseValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;
            RuleLevelCascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            // RuleFor(x => x)
            //     // Validator based on Swagger
            //     .Must(
            //         (_, value, context) =>
            //         {
            //             try
            //             {
            //                 value.Validate();
            //                 return true;
            //             }
            //             catch (MSValidationException e)
            //             {
            //                 context.MessageFormatter.AppendArgument("ExceptionMessage", e.Message);
            //                 return false;
            //             }
            //         }).WithMessage(
            //         $"Invalid value for (nested) member of {typeof(OBClientRegistration1Response).FullName}: " +
            //         "{ExceptionMessage}");

            RuleFor(x => x.ClientId)
                .NotNull().WithMessage("No client ID provided by bank.");
            When(
                x => x.TokenEndpointAuthMethod ==
                     OBRegistrationProperties1tokenEndpointAuthMethodEnum.ClientSecretBasic,
                () =>
                {
                    RuleFor(x => x.ClientSecret)
                        .NotNull();
                });
        }
    }
}
