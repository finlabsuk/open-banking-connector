// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class OBRiskDeliveryAddressValidator : AbstractValidator<OBRiskDeliveryAddress>
    {
        private const int MainlineLength = 70;
        private const int LineLength = 35;
        private const int SublineLength = 16;
        private static readonly Regex CountryRegex = new Regex(@"^[A-Z]{2,2}$", RegexOptions.CultureInvariant);

        public OBRiskDeliveryAddressValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(a => a.StreetName)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, MainlineLength))
                .WithMessage(
                    $"Invalid value for {nameof(OBRiskDeliveryAddress.StreetName)}, length must be less than {MainlineLength}.");

            RuleFor(a => a.StreetName)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, MainlineLength))
                .WithMessage(
                    $"Invalid value for {nameof(OBRiskDeliveryAddress.StreetName)}, length must be less than {MainlineLength}.");

            RuleFor(a => a.BuildingNumber)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, SublineLength))
                .WithMessage(
                    $"Invalid value for {nameof(OBRiskDeliveryAddress.BuildingNumber)}, length must be less than {SublineLength}.");

            RuleFor(a => a.PostCode)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, SublineLength))
                .WithMessage(
                    $"Invalid value for {nameof(OBRiskDeliveryAddress.PostCode)}, length must be less than {SublineLength}.");

            RuleFor(a => a.TownName)
                .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, LineLength))
                .WithMessage(
                    $"Invalid value for {nameof(OBRiskDeliveryAddress.TownName)}, length must be less than {LineLength}.");

            RuleFor(a => a.Country)
                .Must((x, y, z) => ValidationRules.IsMatch(x, y, z, CountryRegex))
                .WithMessage(
                    $"Invalid value for {nameof(OBRiskDeliveryAddress.Country)}, must match a pattern of {CountryRegex}.");
        }
    }
}
