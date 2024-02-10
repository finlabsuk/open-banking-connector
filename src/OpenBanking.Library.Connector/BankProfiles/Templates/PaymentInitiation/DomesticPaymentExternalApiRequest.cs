// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.PaymentInitiation;

public static partial class DomesticPaymentTemplates
{
    public static PaymentInitiationModelsPublic.OBWriteDomestic2
        DomesticPaymentExternalApiRequest(
            DomesticPaymentTemplateRequest domesticPaymentTemplateRequest,
            string externalApiConsentId) =>
        domesticPaymentTemplateRequest.Type switch
        {
            DomesticPaymentTemplateType.PersonToPersonExample => new
                PaymentInitiationModelsPublic.OBWriteDomestic2
                {
                    Data = new PaymentInitiationModelsPublic.Data
                    {
                        ConsentId = externalApiConsentId,
                        Initiation = new PaymentInitiationModelsPublic.Initiation
                        {
                            InstructionIdentification =
                                domesticPaymentTemplateRequest.Parameters.InstructionIdentification,
                            EndToEndIdentification =
                                domesticPaymentTemplateRequest.Parameters.EndToEndIdentification,
                            InstructedAmount =
                                new PaymentInitiationModelsPublic.InstructedAmount
                                {
                                    Amount = "20.00",
                                    Currency = "GBP"
                                },
                            DebtorAccount =
                                new PaymentInitiationModelsPublic.DebtorAccount
                                {
                                    SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                    Identification = "11280001234567",
                                    Name = "Andrea Smith"
                                },
                            CreditorAccount =
                                new PaymentInitiationModelsPublic.CreditorAccount
                                {
                                    SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                    Identification = "08080021325698",
                                    Name = "Bob Clements"
                                },
                            RemittanceInformation =
                                new PaymentInitiationModelsPublic.RemittanceInformation
                                {
                                    Reference = "FRESCO-037",
                                    Unstructured = "Internal ops code 5120103"
                                }
                        }
                    },
                    Risk =
                    {
                        PaymentContextCode = PaymentInitiationModelsPublic.OBRisk1PaymentContextCode
                            .TransferToThirdParty
                    }
                },
            DomesticPaymentTemplateType.PersonToMerchantExample =>
                new PaymentInitiationModelsPublic.OBWriteDomestic2
                {
                    Data = new PaymentInitiationModelsPublic.Data
                    {
                        ConsentId = externalApiConsentId,
                        Initiation = new PaymentInitiationModelsPublic.Initiation
                        {
                            InstructionIdentification =
                                domesticPaymentTemplateRequest.Parameters.InstructionIdentification,
                            EndToEndIdentification =
                                domesticPaymentTemplateRequest.Parameters.EndToEndIdentification,
                            InstructedAmount =
                                new PaymentInitiationModelsPublic.InstructedAmount
                                {
                                    Amount = "165.88",
                                    Currency = "GBP"
                                },
                            CreditorAccount =
                                new PaymentInitiationModelsPublic.CreditorAccount
                                {
                                    SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                    Identification = "08080021325698",
                                    Name = "ACME Inc",
                                    SecondaryIdentification = "0002"
                                },
                            RemittanceInformation =
                                new PaymentInitiationModelsPublic.RemittanceInformation
                                {
                                    Reference = "FRESCO-101",
                                    Unstructured = "Internal ops code 5120101"
                                }
                        }
                    },
                    Risk = new PaymentInitiationModelsPublic.OBRisk1
                    {
                        PaymentContextCode =
                            PaymentInitiationModelsPublic.OBRisk1PaymentContextCode
                                .EcommerceMerchantInitiatedPayment,
                        //ContractPresentIndicator = false,
                        PaymentPurposeCode = "EPAY",
                        BeneficiaryPrepopulatedIndicator = false,
                        BeneficiaryAccountType =
                            PaymentInitiationModelsPublic.OBExternalExtendedAccountType1Code.Business,
                        MerchantCustomerIdentification = "053598653254",
                        DeliveryAddress = new PaymentInitiationModelsPublic.DeliveryAddress
                        {
                            AddressLine = new List<string>
                            {
                                "Flat 7",
                                "Acacia Lodge"
                            },
                            StreetName = "Acacia Avenue",
                            BuildingNumber = "27",
                            PostCode = "GU31 2ZZ",
                            TownName = "Sparsholt",
                            CountrySubDivision = "Wessex",
                            Country = "UK"
                        }
                    }
                },
            _ => throw new ArgumentOutOfRangeException(
                nameof(domesticPaymentTemplateRequest.Type),
                domesticPaymentTemplateRequest.Type,
                null)
        };
}
