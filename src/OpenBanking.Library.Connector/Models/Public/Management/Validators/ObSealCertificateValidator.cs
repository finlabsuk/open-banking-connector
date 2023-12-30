// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Validators;

public class ObSealCertificateValidator : AbstractValidator<ObSealCertificate>
{
    public ObSealCertificateValidator()
    {
        RuleFor(x => x.Certificate)
            .Must(ValidationRules.IsNonWhitespace);

        RuleFor(x => x.AssociatedKeyId)
            .Must(ValidationRules.IsNonWhitespace);

        RuleFor(x => x.AssociatedKey)
            .NotNull()
            .SetValidator(new SecretDescriptionValidator());
    }
}
