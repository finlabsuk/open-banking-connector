// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public partial class OBReadBalance1DataBalanceItem
    {
        internal static OBReadBalance1DataBalanceItem DeserializeOBReadBalance1DataBalanceItem(JsonElement element)
        {
            string accountId = default;
            OBCreditDebitCode2Enum creditDebitIndicator = default;
            OBBalanceType1CodeEnum type = default;
            DateTimeOffset dateTime = default;
            OBReadBalance1DataBalanceItemAmount amount = default;
            Optional<IReadOnlyList<OBReadBalance1DataBalancePropertiesItemsItem>> creditLine = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("AccountId"))
                {
                    accountId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("CreditDebitIndicator"))
                {
                    creditDebitIndicator = property.Value.GetString().ToOBCreditDebitCode2Enum();
                    continue;
                }
                if (property.NameEquals("Type"))
                {
                    type = property.Value.GetString().ToOBBalanceType1CodeEnum();
                    continue;
                }
                if (property.NameEquals("DateTime"))
                {
                    dateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("Amount"))
                {
                    amount = OBReadBalance1DataBalanceItemAmount.DeserializeOBReadBalance1DataBalanceItemAmount(property.Value);
                    continue;
                }
                if (property.NameEquals("CreditLine"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBReadBalance1DataBalancePropertiesItemsItem> array = new List<OBReadBalance1DataBalancePropertiesItemsItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBReadBalance1DataBalancePropertiesItemsItem.DeserializeOBReadBalance1DataBalancePropertiesItemsItem(item));
                    }
                    creditLine = array;
                    continue;
                }
            }
            return new OBReadBalance1DataBalanceItem(accountId, creditDebitIndicator, type, dateTime, amount, Optional.ToList(creditLine));
        }
    }
}
