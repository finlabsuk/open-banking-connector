// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;

public static partial class DomesticVrpTemplates
{
    public static VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest
        DomesticVrpExternalApiRequest(
            DomesticVrpTemplateRequest domesticVrpConsentTemplateRequest,
            string externalApiConsentId) =>
        domesticVrpConsentTemplateRequest.Type switch
        {
            DomesticVrpTemplateType.VrpWithDebtorAccountSpecifiedByPisp => new
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest
                {
                    Data = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequestData
                    {
                        ConsentId = externalApiConsentId,
                        Initiation = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiation
                        {
                            DebtorAccount =
                                new VariableRecurringPaymentsModelsPublic.OBCashAccountDebtorWithName
                                {
                                    SchemeName = "UK.OBIE.IBAN",
                                    Identification = "GB76LOYD30949301273801",
                                    SecondaryIdentification = null,
                                    Name = "Marcus Sweepimus"
                                },
                            CreditorAgent = null,
                            CreditorAccount =
                                new VariableRecurringPaymentsModelsPublic.OBCashAccountCreditor3
                                {
                                    SchemeName = "SortCodeAccountNumber",
                                    Identification = "30949330000010",
                                    SecondaryIdentification = "Roll 90210",
                                    Name = "Marcus Sweepimus"
                                },
                            RemittanceInformation =
                                new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiationRemittanceInformation
                                {
                                    Unstructured = null,
                                    Reference = "Sweepco"
                                }
                        }
                    },
                    Risk = new VariableRecurringPaymentsModelsPublic.OBRisk1
                    {
                        PaymentContextCode = VariableRecurringPaymentsModelsPublic.OBRisk1PaymentContextCodeEnum
                            .PartyToParty,
                        MerchantCategoryCode = null,
                        MerchantCustomerIdentification = null,
                        DeliveryAddress = null
                    }
                },
            DomesticVrpTemplateType
                .VrpWithDebtorAccountSpecifiedDuringConsentAuthorisation => new
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest
                {
                    Data = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequestData
                    {
                        ConsentId = externalApiConsentId,
                        Initiation = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiation
                        {
                            DebtorAccount = null,
                            CreditorAgent = null,
                            CreditorAccount =
                                new VariableRecurringPaymentsModelsPublic.OBCashAccountCreditor3
                                {
                                    SchemeName = "SortCodeAccountNumber",
                                    Identification = "30949330000010",
                                    SecondaryIdentification = "Roll 90210",
                                    Name = "Marcus Sweepimus"
                                },
                            RemittanceInformation =
                                new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiationRemittanceInformation
                                {
                                    Unstructured = null,
                                    Reference = "Sweepco"
                                }
                        }
                    },
                    Risk = new VariableRecurringPaymentsModelsPublic.OBRisk1
                    {
                        PaymentContextCode = VariableRecurringPaymentsModelsPublic.OBRisk1PaymentContextCodeEnum
                            .PartyToParty,
                        MerchantCategoryCode = null,
                        MerchantCustomerIdentification = null,
                        DeliveryAddress = null
                    }
                },
            DomesticVrpTemplateType
                    .VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation
                =>
                new VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest
                {
                    Data = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequestData
                    {
                        ConsentId = externalApiConsentId,
                        Initiation = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiation
                        {
                            RemittanceInformation =
                                new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiationRemittanceInformation
                                {
                                    Unstructured = null,
                                    Reference = "Sweepco"
                                }
                        }
                    },
                    Risk = new VariableRecurringPaymentsModelsPublic.OBRisk1
                    {
                        PaymentContextCode = VariableRecurringPaymentsModelsPublic.OBRisk1PaymentContextCodeEnum
                            .PartyToParty,
                        MerchantCategoryCode = null,
                        MerchantCustomerIdentification = null,
                        DeliveryAddress = null
                    }
                },
            _ => throw new ArgumentOutOfRangeException(
                nameof(domesticVrpConsentTemplateRequest.Type),
                domesticVrpConsentTemplateRequest.Type,
                null)
        };
}
