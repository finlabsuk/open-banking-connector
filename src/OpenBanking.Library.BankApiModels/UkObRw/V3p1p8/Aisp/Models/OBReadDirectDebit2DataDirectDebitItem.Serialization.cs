// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public partial class OBReadDirectDebit2DataDirectDebitItem
    {
        internal static OBReadDirectDebit2DataDirectDebitItem DeserializeOBReadDirectDebit2DataDirectDebitItem(JsonElement element)
        {
            string accountId = default;
            Optional<string> directDebitId = default;
            string mandateIdentification = default;
            Optional<OBExternalDirectDebitStatus1CodeEnum> directDebitStatusCode = default;
            string name = default;
            Optional<DateTimeOffset> previousPaymentDateTime = default;
            Optional<string> frequency = default;
            Optional<OBActiveOrHistoricCurrencyAndAmount0> previousPaymentAmount = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("AccountId"))
                {
                    accountId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("DirectDebitId"))
                {
                    directDebitId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("MandateIdentification"))
                {
                    mandateIdentification = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("DirectDebitStatusCode"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    directDebitStatusCode = property.Value.GetString().ToOBExternalDirectDebitStatus1CodeEnum();
                    continue;
                }
                if (property.NameEquals("Name"))
                {
                    name = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("PreviousPaymentDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    previousPaymentDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("Frequency"))
                {
                    frequency = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("PreviousPaymentAmount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    previousPaymentAmount = OBActiveOrHistoricCurrencyAndAmount0.DeserializeOBActiveOrHistoricCurrencyAndAmount0(property.Value);
                    continue;
                }
            }
            return new OBReadDirectDebit2DataDirectDebitItem(accountId, directDebitId.Value, mandateIdentification, Optional.ToNullable(directDebitStatusCode), name, Optional.ToNullable(previousPaymentDateTime), frequency.Value, previousPaymentAmount.Value);
        }
    }
}
