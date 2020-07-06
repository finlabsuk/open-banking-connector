// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class OBPostalAddressValidator : AbstractValidator<OBPostalAddress>
    {
        private const int MainlineLength = 70;
        private const int LineLength = 35;
        private const int SublineLength = 16;
        private static readonly Regex CountryRegex = new Regex(@"^[A-Z]{2,2}$", RegexOptions.CultureInvariant);

        public OBPostalAddressValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(a => a.Department)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, MainlineLength))
                .WithMessage($"Invalid value for Department, length must be less than {MainlineLength}.");

            RuleFor(a => a.SubDepartment)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, MainlineLength))
                .WithMessage($"Invalid value for SubDepartment, length must be less than {MainlineLength}.");

            RuleFor(a => a.StreetName)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, MainlineLength))
                .WithMessage($"Invalid value for StreetName, length must be less than {MainlineLength}.");

            RuleFor(a => a.BuildingNumber)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, SublineLength))
                .WithMessage($"Invalid value for BuildingNumber, length must be less than {SublineLength}.");

            RuleFor(a => a.PostCode)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, SublineLength))
                .WithMessage($"Invalid value for PostCode, length must be less than {SublineLength}.");

            RuleFor(a => a.TownName)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, LineLength))
                .WithMessage($"Invalid value for TownName, length must be less than {LineLength}.");

            RuleFor(a => a.CountrySubDivision)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, LineLength))
                .WithMessage($"Invalid value for CountrySubDivision, length must be less than {LineLength}.");

            RuleFor(a => a.Country)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must((x, y, z) => ValidationRules.IsMatch(x, y, z, CountryRegex))
                .WithMessage($"Invalid value for Department, must match a pattern of {CountryRegex}.");
        }
    }
}
