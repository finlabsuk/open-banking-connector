// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Validators;

public class EncryptionKeyDescriptionValidator : AbstractValidator<EncryptionKeyDescription>
{
    public EncryptionKeyDescriptionValidator()
    {
        RuleFor(x => x.Key)
            .NotNull()
            .SetValidator(new SecretDescriptionValidator());
    }
}
