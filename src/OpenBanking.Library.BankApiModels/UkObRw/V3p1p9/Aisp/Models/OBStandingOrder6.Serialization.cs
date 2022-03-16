// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public partial class OBStandingOrder6
    {
        internal static OBStandingOrder6 DeserializeOBStandingOrder6(JsonElement element)
        {
            string accountId = default;
            Optional<string> standingOrderId = default;
            string frequency = default;
            Optional<string> reference = default;
            Optional<DateTimeOffset> firstPaymentDateTime = default;
            Optional<DateTimeOffset> nextPaymentDateTime = default;
            Optional<DateTimeOffset> lastPaymentDateTime = default;
            Optional<DateTimeOffset> finalPaymentDateTime = default;
            Optional<string> numberOfPayments = default;
            Optional<OBExternalStandingOrderStatus1CodeEnum> standingOrderStatusCode = default;
            Optional<OBActiveOrHistoricCurrencyAndAmount2> firstPaymentAmount = default;
            Optional<OBActiveOrHistoricCurrencyAndAmount3> nextPaymentAmount = default;
            Optional<OBActiveOrHistoricCurrencyAndAmount11> lastPaymentAmount = default;
            Optional<OBActiveOrHistoricCurrencyAndAmount4> finalPaymentAmount = default;
            Optional<OBBranchAndFinancialInstitutionIdentification51> creditorAgent = default;
            Optional<OBCashAccount51> creditorAccount = default;
            Optional<IReadOnlyDictionary<string, object>> supplementaryData = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("AccountId"))
                {
                    accountId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("StandingOrderId"))
                {
                    standingOrderId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Frequency"))
                {
                    frequency = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Reference"))
                {
                    reference = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("FirstPaymentDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    firstPaymentDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("NextPaymentDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    nextPaymentDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("LastPaymentDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    lastPaymentDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("FinalPaymentDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    finalPaymentDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("NumberOfPayments"))
                {
                    numberOfPayments = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("StandingOrderStatusCode"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    standingOrderStatusCode = property.Value.GetString().ToOBExternalStandingOrderStatus1CodeEnum();
                    continue;
                }
                if (property.NameEquals("FirstPaymentAmount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    firstPaymentAmount = OBActiveOrHistoricCurrencyAndAmount2.DeserializeOBActiveOrHistoricCurrencyAndAmount2(property.Value);
                    continue;
                }
                if (property.NameEquals("NextPaymentAmount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    nextPaymentAmount = OBActiveOrHistoricCurrencyAndAmount3.DeserializeOBActiveOrHistoricCurrencyAndAmount3(property.Value);
                    continue;
                }
                if (property.NameEquals("LastPaymentAmount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    lastPaymentAmount = OBActiveOrHistoricCurrencyAndAmount11.DeserializeOBActiveOrHistoricCurrencyAndAmount11(property.Value);
                    continue;
                }
                if (property.NameEquals("FinalPaymentAmount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    finalPaymentAmount = OBActiveOrHistoricCurrencyAndAmount4.DeserializeOBActiveOrHistoricCurrencyAndAmount4(property.Value);
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
                if (property.NameEquals("SupplementaryData"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    foreach (var property0 in property.Value.EnumerateObject())
                    {
                        dictionary.Add(property0.Name, property0.Value.GetObject());
                    }
                    supplementaryData = dictionary;
                    continue;
                }
            }
            return new OBStandingOrder6(accountId, standingOrderId.Value, frequency, reference.Value, Optional.ToNullable(firstPaymentDateTime), Optional.ToNullable(nextPaymentDateTime), Optional.ToNullable(lastPaymentDateTime), Optional.ToNullable(finalPaymentDateTime), numberOfPayments.Value, Optional.ToNullable(standingOrderStatusCode), firstPaymentAmount.Value, nextPaymentAmount.Value, lastPaymentAmount.Value, finalPaymentAmount.Value, creditorAgent.Value, creditorAccount.Value, Optional.ToDictionary(supplementaryData));
        }
    }
}
