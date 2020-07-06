// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation
{
    public class
        OBWriteDomesticDataInitiationInstructedAmountValidator : AbstractValidator<
            OBWriteDomesticDataInitiationInstructedAmount>
    {
        private static readonly Regex RegexAmount = new Regex(@"^\\d{1,13}\\.\\d{1,5}$", RegexOptions.CultureInvariant);
        private static readonly Regex RegexCurrency = new Regex(@"^[A-Z]{3,3}$", RegexOptions.CultureInvariant);

        public OBWriteDomesticDataInitiationInstructedAmountValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.Amount)
                .Must((x, y, z) => ValidationRules.IsMatch(x, y, z, RegexAmount))
                .WithMessage($"Invalid value for Amount, must match a pattern of {RegexAmount}");

            RuleFor(x => x.Currency)
                .Must((x, y, z) => ValidationRules.IsMatch(x, y, z, RegexCurrency))
                .WithMessage($"Invalid value for Amount, must match a pattern of {RegexCurrency}");
        }
    }
}
