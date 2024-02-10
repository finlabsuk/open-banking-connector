// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;

public static partial class DomesticVrpTemplates
{
    public static VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest
        DomesticVrpExternalApiFundsConfirmationRequest(
            DomesticVrpTemplateRequest domesticVrpConsentTemplateRequest,
            string externalApiConsentId) =>
        domesticVrpConsentTemplateRequest.Type switch
        {
            DomesticVrpTemplateType.SweepingVrp => new
                VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest
                {
                    Data = new VariableRecurringPaymentsModelsPublic.Data6
                    {
                        ConsentId = externalApiConsentId,
                        Reference = "Sweepco",
                        InstructedAmount = new VariableRecurringPaymentsModelsPublic.OBActiveOrHistoricCurrencyAndAmount
                        {
                            Amount = "10.00",
                            Currency = "GBP"
                        }
                    }
                },
            _ => throw new ArgumentOutOfRangeException(
                nameof(domesticVrpConsentTemplateRequest.Type),
                domesticVrpConsentTemplateRequest.Type,
                null)
        };
}
