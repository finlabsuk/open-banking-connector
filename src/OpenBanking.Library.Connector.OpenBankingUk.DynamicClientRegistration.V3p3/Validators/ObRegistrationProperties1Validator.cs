// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Models;
using FluentValidation;
using MSValidationException = Microsoft.Rest.ValidationException;

namespace FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Validators
{
    public class
        ObRegistrationProperties1Validator : AbstractValidator<OBRegistrationProperties1>
    {
        public ObRegistrationProperties1Validator()
        {
            CascadeMode = CascadeMode.Continue;
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
                    $"Invalid value for (nested) member of {typeof(OBRegistrationProperties1).FullName}: " +
                    "{ExceptionMessage}");
        }
    }
}
