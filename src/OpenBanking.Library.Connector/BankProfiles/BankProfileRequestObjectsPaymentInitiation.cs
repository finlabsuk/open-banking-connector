// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public enum DomesticPaymentTypeEnum
    {
        PersonToPerson,
        PersonToMerchant
    }

    public partial class BankProfile
    {
        public PaymentInitiationModelsPublic.OBWriteDomesticConsent4 DomesticPaymentConsent(
            DomesticPaymentTypeEnum domesticPaymentType,
            string instructionIdentification,
            string endToEndIdentification)
        {
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4 domesticPaymentConsent =
                domesticPaymentType switch
                {
                    DomesticPaymentTypeEnum.PersonToPerson => new
                        PaymentInitiationModelsPublic.OBWriteDomesticConsent4
                        {
                            Data = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4Data
                            {
                                ReadRefundAccount = null,
                                Initiation = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiation
                                {
                                    InstructionIdentification = instructionIdentification,
                                    EndToEndIdentification = endToEndIdentification,
                                    LocalInstrument = "UK.OBIE.FPS",
                                    InstructedAmount =
                                        new PaymentInitiationModelsPublic.
                                            OBWriteDomesticConsent4DataInitiationInstructedAmount
                                            {
                                                Amount = "15.00",
                                                Currency = "GBP"
                                            },
                                    DebtorAccount =
                                        new PaymentInitiationModelsPublic.
                                            OBWriteDomesticConsent4DataInitiationDebtorAccount
                                            {
                                                SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                                Identification = "08080021325645",
                                                Name = "A Person"
                                            },
                                    CreditorAccount =
                                        new PaymentInitiationModelsPublic.
                                            OBWriteDomesticConsent4DataInitiationCreditorAccount
                                            {
                                                SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                                Identification = "08080021325698",
                                                Name = "Another Person"
                                            },
                                    CreditorPostalAddress = null,
                                    RemittanceInformation =
                                        new PaymentInitiationModelsPublic.
                                            OBWriteDomesticConsent4DataInitiationRemittanceInformation
                                            {
                                                Unstructured = "Unstructured string",
                                                Reference = "MyRef"
                                            },
                                    SupplementaryData = null
                                },
                                Authorisation =
                                    new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataAuthorisation
                                    {
                                        AuthorisationType = PaymentInitiationModelsPublic
                                            .OBWriteDomesticConsent4DataAuthorisationAuthorisationTypeEnum.Any,
                                        CompletionDateTime = DateTimeOffset.UtcNow.AddDays(1)
                                    },
                                SCASupportData = null
                            },
                            Risk = null
                        },
                    DomesticPaymentTypeEnum.PersonToMerchant =>
                        new PaymentInitiationModelsPublic.OBWriteDomesticConsent4
                        {
                            Data = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4Data
                            {
                                ReadRefundAccount = PaymentInitiationModelsPublic
                                    .OBWriteDomesticConsent4DataReadRefundAccountEnum
                                    .Yes,
                                Initiation = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiation
                                {
                                    InstructionIdentification = instructionIdentification,
                                    EndToEndIdentification = endToEndIdentification,
                                    InstructedAmount =
                                        new PaymentInitiationModelsPublic.
                                            OBWriteDomesticConsent4DataInitiationInstructedAmount
                                            {
                                                Amount = "5.00",
                                                Currency = "GBP"
                                            },
                                    CreditorAccount =
                                        new PaymentInitiationModelsPublic.
                                            OBWriteDomesticConsent4DataInitiationCreditorAccount
                                            {
                                                SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                                Identification = "08080021325698",
                                                Name = "ACME Inc",
                                                SecondaryIdentification = "0002"
                                            },
                                    RemittanceInformation =
                                        new PaymentInitiationModelsPublic.
                                            OBWriteDomesticConsent4DataInitiationRemittanceInformation
                                            {
                                                Unstructured = "Internal ops code 5120101",
                                                Reference = "FRESCO-101"
                                            },
                                }
                            },
                            Risk = new PaymentInitiationModelsPublic.OBRisk1
                            {
                                PaymentContextCode = PaymentInitiationModelsPublic.OBRisk1PaymentContextCodeEnum
                                    .EcommerceGoods,
                                MerchantCategoryCode = "5967",
                                MerchantCustomerIdentification = "053598653254",
                                DeliveryAddress = new PaymentInitiationModelsPublic.OBRisk1DeliveryAddress
                                {
                                    AddressLine = new List<string>
                                    {
                                        "Flat 7",
                                        "Acacia Lodge"
                                    },
                                    BuildingNumber = "27",
                                    StreetName = "Acacia Avenue",
                                    TownName = "Sparsholt",
                                    PostCode = "GU31 2ZZ",
                                    CountrySubDivision = "Wessex",
                                    Country = "UK",
                                }
                            },
                        },
                    _ => throw new ArgumentException(
                        $"{nameof(DomesticPaymentTypeEnum)} is not valid ${nameof(DomesticPaymentTypeEnum)} or needs to be added to this switch statement.")
                };
            return domesticPaymentConsent;
        }
    }
}
