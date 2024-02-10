// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.PaymentInitiation;

[JsonConverter(typeof(StringEnumConverter))]
public enum DomesticPaymentTemplateType
{
    [EnumMember(Value = "PersonToPersonExample")]
    PersonToPersonExample,

    [EnumMember(Value = "PersonToMerchantExample")]
    PersonToMerchantExample
}

public class DomesticPaymentTemplateParameters
{
    public string InstructionIdentification { get; set; } = null!;
    public string EndToEndIdentification { get; set; } = null!;
}

public class DomesticPaymentTemplateRequest
{
    /// <summary>
    ///     Template type to use.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required DomesticPaymentTemplateType Type { get; init; }

    /// <summary>
    ///     Template parameters.
    /// </summary>
    public DomesticPaymentTemplateParameters Parameters { get; set; } = null!;
}

public static partial class DomesticPaymentTemplates
{
    public static PaymentInitiationModelsPublic.OBWriteDomesticConsent4
        DomesticPaymentConsentExternalApiRequest(DomesticPaymentTemplateRequest domesticPaymentTemplateRequest) =>
        domesticPaymentTemplateRequest.Type switch
        {
            DomesticPaymentTemplateType.PersonToPersonExample => new
                PaymentInitiationModelsPublic.OBWriteDomesticConsent4
                {
                    Data = new PaymentInitiationModelsPublic.Data2
                    {
                        Initiation = new PaymentInitiationModelsPublic.Initiation2
                        {
                            InstructionIdentification =
                                domesticPaymentTemplateRequest.Parameters.InstructionIdentification,
                            EndToEndIdentification =
                                domesticPaymentTemplateRequest.Parameters.EndToEndIdentification,
                            InstructedAmount =
                                new PaymentInitiationModelsPublic.InstructedAmount2
                                {
                                    Amount = "20.00",
                                    Currency = "GBP"
                                },
                            DebtorAccount =
                                new PaymentInitiationModelsPublic.DebtorAccount2
                                {
                                    SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                    Identification = "11280001234567",
                                    Name = "Andrea Smith"
                                },
                            CreditorAccount =
                                new PaymentInitiationModelsPublic.CreditorAccount2
                                {
                                    SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                    Identification = "08080021325698",
                                    Name = "Bob Clements"
                                },
                            RemittanceInformation =
                                new PaymentInitiationModelsPublic.RemittanceInformation2
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
                new PaymentInitiationModelsPublic.OBWriteDomesticConsent4
                {
                    Data = new PaymentInitiationModelsPublic.Data2
                    {
                        Initiation = new PaymentInitiationModelsPublic.Initiation2
                        {
                            InstructionIdentification =
                                domesticPaymentTemplateRequest.Parameters.InstructionIdentification,
                            EndToEndIdentification =
                                domesticPaymentTemplateRequest.Parameters.EndToEndIdentification,
                            InstructedAmount =
                                new PaymentInitiationModelsPublic.InstructedAmount2
                                {
                                    Amount = "165.88",
                                    Currency = "GBP"
                                },
                            CreditorAccount =
                                new PaymentInitiationModelsPublic.CreditorAccount2
                                {
                                    SchemeName = "UK.OBIE.SortCodeAccountNumber",
                                    Identification = "08080021325698",
                                    Name = "ACME Inc",
                                    SecondaryIdentification = "0002"
                                },
                            RemittanceInformation =
                                new PaymentInitiationModelsPublic.RemittanceInformation2
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
