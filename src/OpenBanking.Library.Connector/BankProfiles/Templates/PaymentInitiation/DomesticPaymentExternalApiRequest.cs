// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

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
                    Data = new PaymentInitiationModelsPublic.OBWriteDomestic2Data
                    {
                        ConsentId = externalApiConsentId,
                        Initiation = new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiation
                        {
                            InstructionIdentification =
                                domesticPaymentTemplateRequest.Parameters.InstructionIdentification,
                            EndToEndIdentification =
                                domesticPaymentTemplateRequest.Parameters.EndToEndIdentification,
                            LocalInstrument = "UK.OBIE.FPS",
                            InstructedAmount =
                                new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationInstructedAmount
                                {
                                    Amount = "15.00",
                                    Currency = "GBP"
                                },
                            DebtorAccount =
                                new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationDebtorAccount
                                {
                                    SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                    Identification = "08080021325645",
                                    Name = "A Person"
                                },
                            CreditorAccount =
                                new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationCreditorAccount
                                {
                                    SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                    Identification = "08080021325698",
                                    Name = "Another Person"
                                },
                            CreditorPostalAddress = null,
                            RemittanceInformation =
                                new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationRemittanceInformation
                                {
                                    Unstructured = "Unstructured string",
                                    Reference = "MyRef"
                                },
                            SupplementaryData = null
                        }
                    },
                    Risk = null
                },
            DomesticPaymentTemplateType.PersonToMerchantExample =>
                new PaymentInitiationModelsPublic.OBWriteDomestic2
                {
                    Data = new PaymentInitiationModelsPublic.OBWriteDomestic2Data
                    {
                        ConsentId = externalApiConsentId,
                        Initiation = new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiation
                        {
                            InstructionIdentification =
                                domesticPaymentTemplateRequest.Parameters.InstructionIdentification,
                            EndToEndIdentification =
                                domesticPaymentTemplateRequest.Parameters.EndToEndIdentification,
                            InstructedAmount =
                                new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationInstructedAmount
                                {
                                    Amount = "5.00",
                                    Currency = "GBP"
                                },
                            CreditorAccount =
                                new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationCreditorAccount
                                {
                                    SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                    Identification = "08080021325698",
                                    Name = "ACME Inc",
                                    SecondaryIdentification = "0002"
                                },
                            RemittanceInformation =
                                new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationRemittanceInformation
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
            _ => throw new ArgumentOutOfRangeException(
                nameof(domesticPaymentTemplateRequest.Type),
                domesticPaymentTemplateRequest.Type,
                null)
        };
}
