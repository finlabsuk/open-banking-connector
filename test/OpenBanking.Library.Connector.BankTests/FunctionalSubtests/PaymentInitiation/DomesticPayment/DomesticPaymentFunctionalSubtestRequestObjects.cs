// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment
{
    public partial class DomesticPaymentFunctionalSubtest
    {
        private DomesticPaymentConsent DomesticPaymentConsent(BankProfile bankProfile, Guid bankId)
        {
            DomesticPaymentConsent domesticPaymentConsent = DomesticPaymentFunctionalSubtestEnum switch
            {
                DomesticPaymentFunctionalSubtestEnum.PersonToPersonSubtest => new DomesticPaymentConsent
                {
                    DebtorAccount = new OBWriteDomestic2DataInitiationDebtorAccount
                    {
                        SchemeName = "UK.OBIE.SortCodeAccountNumber",
                        Identification = "08080021325645",
                        Name = "A Person"
                    },
                    CreditorAccount = new OBWriteDomestic2DataInitiationCreditorAccount
                    {
                        SchemeName = "UK.OBIE.SortCodeAccountNumber",
                        Identification = "08080021325698",
                        Name = "Another Person"
                    },
                    InstructedAmount = new OBWriteDomestic2DataInitiationInstructedAmount
                    {
                        Amount = "15.00",
                        Currency = "GBP"
                    },
                    RemittanceInformation = new OBWriteDomestic2DataInitiationRemittanceInformation
                    {
                        Unstructured = "Unstructured string",
                        Reference = "MyRef"
                    },
                    InstructionIdentification = "SIDP01",
                    EndToEndIdentification = "FRESCO.21302.GFX.20",
                    LocalInstrument = "UK.OBIE.FPS",
                    Authorisation = new OBWriteDomesticConsent4DataAuthorisation
                    {
                        AuthorisationType = OBWriteDomesticConsent4DataAuthorisation.AuthorisationTypeEnum.Any,
                        CompletionDateTime = DateTimeOffset.UtcNow.AddDays(1)
                    },
                    BankId = bankId,
                    UseStagingNotDefaultBankProfile = true,
                    UseStagingNotDefaultBankRegistration = true
                },
                DomesticPaymentFunctionalSubtestEnum.PersonToMerchantSubtest => new DomesticPaymentConsent
                {
                    Merchant = new OBRisk1
                    {
                        PaymentContextCode = OBRisk1.PaymentContextCodeEnum.EcommerceGoods,
                        DeliveryAddress = new OBRisk1DeliveryAddress
                        {
                            BuildingNumber = "42",
                            StreetName = "Oxford Street",
                            TownName = "London",
                            Country = "UK",
                            PostCode = "SW1 1AA"
                        }
                    },
                    CreditorAccount = new OBWriteDomestic2DataInitiationCreditorAccount
                    {
                        SchemeName = "UK.OBIE.SortCodeAccountNumber",
                        Identification = "08080021325698",
                        Name = "ACME DIY",
                        SecondaryIdentification = "secondary-identif"
                    },
                    InstructedAmount = new OBWriteDomestic2DataInitiationInstructedAmount
                    {
                        Amount = "50.00",
                        Currency = "GBP"
                    },
                    InstructionIdentification = "instr-identification",
                    EndToEndIdentification = "e2e-identification",
                    RemittanceInformation = new OBWriteDomestic2DataInitiationRemittanceInformation
                    {
                        Unstructured = "Tools",
                        Reference = "Tools"
                    },
                    BankId = bankId,
                    UseStagingNotDefaultBankProfile = true,
                    UseStagingNotDefaultBankRegistration = true
                },
                _ => throw new ArgumentException(
                    $"{nameof(DomesticPaymentFunctionalSubtestEnum)} is not valid ${nameof(DomesticPayment.DomesticPaymentFunctionalSubtestEnum)} or needs to be added to this switch statement.")
            };
            domesticPaymentConsent = bankProfile.DomesticPaymentConsentAdjustments(domesticPaymentConsent);
            return domesticPaymentConsent;
        }
    }
}
