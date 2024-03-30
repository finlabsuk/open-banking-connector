// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;

public static partial class DomesticVrpTemplates
{
    public static VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest
        DomesticVrpExternalApiRequest(
            DomesticVrpTemplateRequest domesticVrpConsentTemplateRequest,
            string externalApiConsentId) =>
        domesticVrpConsentTemplateRequest.Type switch
        {
            DomesticVrpTemplateType.SweepingVrp => new
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest
                {
                    Data = new VariableRecurringPaymentsModelsPublic.Data3
                    {
                        ConsentId = externalApiConsentId,
                        PSUAuthenticationMethod = "UK.OBIE.SCANotRequired",
                        PSUInteractionType = VariableRecurringPaymentsModelsPublic.OBVRPInteractionTypes.OffSession,
                        VRPType = "UK.OBIE.VRPType.Sweeping",
                        Initiation = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiation
                        {
                            DebtorAccount =
                                domesticVrpConsentTemplateRequest.Parameters.IncludeDebtorInInitiation
                                    ? new VariableRecurringPaymentsModelsPublic.OBCashAccountDebtorWithName
                                    {
                                        SchemeName = "UK.OBIE.IBAN",
                                        Identification = "GB76LOYD30949301273801",
                                        Name = "Marcus Sweepimus"
                                    }
                                    : null,
                            CreditorAccount = domesticVrpConsentTemplateRequest.Parameters.IncludeCreditorInInitiation
                                ? new VariableRecurringPaymentsModelsPublic.OBCashAccountCreditor3
                                {
                                    SchemeName = "SortCodeAccountNumber",
                                    Identification = "30949330000010",
                                    SecondaryIdentification = "Roll 90210",
                                    Name = "Marcus Sweepimus"
                                }
                                : null,
                            RemittanceInformation =
                                new VariableRecurringPaymentsModelsPublic.RemittanceInformation
                                {
                                    Reference = "Sweepco"
                                }
                        },
                        Instruction = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInstruction
                        {
                            InstructionIdentification =
                                domesticVrpConsentTemplateRequest.Parameters
                                    .InstructionIdentification, // not found in usage examples
                            EndToEndIdentification =
                                domesticVrpConsentTemplateRequest.Parameters
                                    .EndToEndIdentification, // not found in usage examples
                            CreditorAccount = new VariableRecurringPaymentsModelsPublic.OBCashAccountCreditor3
                            {
                                SchemeName = "SortCodeAccountNumber",
                                Identification = "30949330000010",
                                SecondaryIdentification = "Roll 90210",
                                Name = "Marcus Sweepimus"
                            },
                            InstructedAmount =
                                new VariableRecurringPaymentsModelsPublic.OBActiveOrHistoricCurrencyAndAmount
                                {
                                    Amount = "10.00",
                                    Currency = "GBP"
                                },
                            RemittanceInformation =
                                new VariableRecurringPaymentsModelsPublic.OBVRPRemittanceInformation
                                {
                                    Reference = "Sweepco"
                                }
                        }
                    },
                    Risk = new VariableRecurringPaymentsModelsPublic.OBRisk1
                    {
                        PaymentContextCode = VariableRecurringPaymentsModelsPublic.OBRisk1PaymentContextCode
                            .TransferToThirdParty
                    }
                },
            _ => throw new ArgumentOutOfRangeException(
                nameof(domesticVrpConsentTemplateRequest.Type),
                domesticVrpConsentTemplateRequest.Type,
                null)
        };
}
