// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Validators
{
    public class OBWriteDomesticConsentResponse5DataInitiationValidator : AbstractValidator<
        OBWriteDomesticConsentResponse5DataInitiation>
    {
        private static readonly Regex RegexAmount = new Regex(
            @"^\d{1,13}\.\d{1,5}$",
            RegexOptions.CultureInvariant);

        private static readonly Regex RegexCurrency = new Regex(
            @"^[A-Z]{3,3}$",
            RegexOptions.CultureInvariant);

        public OBWriteDomesticConsentResponse5DataInitiationValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            // IMPORTANT: Below validation would normally be broken out into separate file per class but because
            // OBWriteDomestic2DataInitiation and OBWriteDomesticConsent4DataInitiation are identical (due to
            // repetition in Swagger) their validators are each in a single file to allow quick comparison to
            // ensure they stay aligned.
            // Need to see if Swagger can be edited to remove duplicate class structures
            const int idLength = 35;
            RuleFor(d => d.InstructionIdentification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must(
                    (x, y, z) => ValidationRules.HasLengthAtMost(
                        x,
                        y,
                        z,
                        idLength))
                .WithMessage(
                    $"Invalid value for {typeof(OBWriteDomesticConsent4DataInitiation).FullName}.{nameof(OBWriteDomesticConsent4DataInitiation.InstructionIdentification)}: " +
                    $"It must not be empty and length must be less than {idLength}.");

            RuleFor(d => d.EndToEndIdentification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .Must(
                    (x, y, z) => ValidationRules.HasLengthAtMost(
                        x,
                        y,
                        z,
                        idLength))
                .WithMessage(
                    $"Invalid value for {typeof(OBWriteDomesticConsent4DataInitiation).FullName}.{nameof(OBWriteDomesticConsent4DataInitiation.EndToEndIdentification)}: " +
                    $"It must not be empty and length must be less than {idLength}.");


            // InstructedAmount
            When(
                x => !(x.InstructedAmount is null),
                () =>
                {
                    RuleFor(x => x.InstructedAmount.Amount)
                        .Must((x, y, z) => ValidationRules.IsMatch(x, y, z, RegexAmount))
                        .WithMessage($"Invalid value for Amount, must match a pattern of {RegexAmount}");

                    RuleFor(x => x.InstructedAmount.Currency)
                        .Must((x, y, z) => ValidationRules.IsMatch(x, y, z, RegexCurrency))
                        .WithMessage($"Invalid value for Currency, must match a pattern of {RegexCurrency}");
                });

            // DebtorAccount
            const int idLength2 = 256;
            const int nameLength = 70;
            const int secondaryIdLength = 34;

            When(
                x => !(x.DebtorAccount is null),
                () =>
                {
                    RuleFor(x => x.DebtorAccount.Identification)
                        .Must(ValidationRules.IsNotNullOrEmpty)
                        .Must(
                            (x, y, z) => ValidationRules.HasLengthAtMost(
                                x,
                                y,
                                z,
                                idLength2))
                        .WithMessage($"Invalid value for Identification, length must be less than {idLength2}.");

                    RuleFor(x => x.DebtorAccount.Name)
                        .Must(ValidationRules.IsNotNullOrEmpty)
                        .Must(
                            (x, y, z) => ValidationRules.HasLengthAtMost(
                                x,
                                y,
                                z,
                                nameLength))
                        .WithMessage($"Invalid value for Name, length must be less than {nameLength}.");

                    RuleFor(x => x.DebtorAccount.SecondaryIdentification)
                        .Must(ValidationRules.IsNotNullOrEmpty)
                        .Must(
                            (x, y, z) => ValidationRules.HasLengthAtMost(
                                x,
                                y,
                                z,
                                secondaryIdLength))
                        .WithMessage(
                            $"Invalid value for SecondaryIdentification, length must be less than {secondaryIdLength}.");
                });


            // CreditorAccount
            When(
                x => !(x.CreditorAccount is null),
                () =>
                {
                    RuleFor(x => x.CreditorAccount.Identification)
                        .Must(ValidationRules.IsNotNullOrEmpty)
                        .Must(
                            (x, y, z) => ValidationRules.HasLengthAtMost(
                                x,
                                y,
                                z,
                                idLength))
                        .WithMessage($"Invalid value for Identification, length must be less than {idLength}.");

                    RuleFor(x => x.CreditorAccount.Name)
                        .Must(ValidationRules.IsNotNullOrEmpty)
                        .Must(
                            (x, y, z) => ValidationRules.HasLengthAtMost(
                                x,
                                y,
                                z,
                                nameLength))
                        .WithMessage($"Invalid value for Name, length must be less than {nameLength}.");

                    RuleFor(x => x.CreditorAccount.SecondaryIdentification)
                        .Must(ValidationRules.IsNotNullOrEmpty)
                        .Must(
                            (x, y, z) => ValidationRules.HasLengthAtMost(
                                x,
                                y,
                                z,
                                secondaryIdLength))
                        .WithMessage(
                            $"Invalid value for SecondaryIdentification, length must be less than {secondaryIdLength}.");
                });

            // CreditorPostalAddress
            RuleFor(x => x.CreditorPostalAddress)
                .SetValidator(new OBPostalAddress6Validator());

            // RemittanceInformation
            When(
                x => !(x.RemittanceInformation is null),
                () =>
                {
                    RuleFor(x => x.RemittanceInformation.Unstructured)
                        .Must(ValidationRules.IsNotNullOrEmpty)
                        .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, 140))
                        .WithMessage("Invalid value for Unstructured, length must be less than 140.");

                    RuleFor(x => x.RemittanceInformation.Reference)
                        .Must(ValidationRules.IsNotNullOrEmpty)
                        .Must((x, y, z) => ValidationRules.HasLengthAtMost(x, y, z, 35))
                        .WithMessage("Invalid value for Reference, length must be less than 35.");
                });
        }
    }
}
