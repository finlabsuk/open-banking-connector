// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Validators;

public class SecretDescriptionValidator : AbstractValidator<SecretDescription>
{
    public SecretDescriptionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
