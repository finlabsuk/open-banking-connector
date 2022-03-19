// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class OBScheduledPayment3
    {
        public static OBScheduledPayment3 DeserializeOBScheduledPayment3(JsonElement element)
        {
            string accountId = default;
            Optional<string> scheduledPaymentId = default;
            DateTimeOffset scheduledPaymentDateTime = default;
            OBExternalScheduleType1CodeEnum scheduledType = default;
            Optional<string> reference = default;
            Optional<string> debtorReference = default;
            OBActiveOrHistoricCurrencyAndAmount1 instructedAmount = default;
            Optional<OBBranchAndFinancialInstitutionIdentification51> creditorAgent = default;
            Optional<OBCashAccount51> creditorAccount = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("AccountId"))
                {
                    accountId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("ScheduledPaymentId"))
                {
                    scheduledPaymentId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("ScheduledPaymentDateTime"))
                {
                    scheduledPaymentDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("ScheduledType"))
                {
                    scheduledType = property.Value.GetString().ToOBExternalScheduleType1CodeEnum();
                    continue;
                }
                if (property.NameEquals("Reference"))
                {
                    reference = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("DebtorReference"))
                {
                    debtorReference = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("InstructedAmount"))
                {
                    instructedAmount = OBActiveOrHistoricCurrencyAndAmount1.DeserializeOBActiveOrHistoricCurrencyAndAmount1(property.Value);
                    continue;
                }
                if (property.NameEquals("CreditorAgent"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    creditorAgent = OBBranchAndFinancialInstitutionIdentification51.DeserializeOBBranchAndFinancialInstitutionIdentification51(property.Value);
                    continue;
                }
                if (property.NameEquals("CreditorAccount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    creditorAccount = OBCashAccount51.DeserializeOBCashAccount51(property.Value);
                    continue;
                }
            }
            return new OBScheduledPayment3(accountId, scheduledPaymentId.Value, scheduledPaymentDateTime, scheduledType, reference.Value, debtorReference.Value, instructedAmount, creditorAgent.Value, creditorAccount.Value);
        }
    }
}
