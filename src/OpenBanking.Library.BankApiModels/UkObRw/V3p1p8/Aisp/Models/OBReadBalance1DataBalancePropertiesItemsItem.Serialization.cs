// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace AccountAndTransactionAPISpecification.Models
{
    public partial class OBReadBalance1DataBalancePropertiesItemsItem
    {
        internal static OBReadBalance1DataBalancePropertiesItemsItem DeserializeOBReadBalance1DataBalancePropertiesItemsItem(JsonElement element)
        {
            bool included = default;
            Optional<OBReadBalance1DataBalanceCreditLineTypeEnum> type = default;
            Optional<OBReadBalance1DataBalanceItemCreditLineItemAmount> amount = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Included"))
                {
                    included = property.Value.GetBoolean();
                    continue;
                }
                if (property.NameEquals("Type"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    type = property.Value.GetString().ToOBReadBalance1DataBalanceCreditLineTypeEnum();
                    continue;
                }
                if (property.NameEquals("Amount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    amount = OBReadBalance1DataBalanceItemCreditLineItemAmount.DeserializeOBReadBalance1DataBalanceItemCreditLineItemAmount(property.Value);
                    continue;
                }
            }
            return new OBReadBalance1DataBalancePropertiesItemsItem(included, Optional.ToNullable(type), amount.Value);
        }
    }
}
