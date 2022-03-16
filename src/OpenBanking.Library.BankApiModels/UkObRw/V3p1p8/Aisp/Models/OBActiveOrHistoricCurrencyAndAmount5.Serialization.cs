// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace AccountAndTransactionAPISpecification.Models
{
    public partial class OBActiveOrHistoricCurrencyAndAmount5
    {
        internal static OBActiveOrHistoricCurrencyAndAmount5 DeserializeOBActiveOrHistoricCurrencyAndAmount5(JsonElement element)
        {
            string amount = default;
            string currency = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Amount"))
                {
                    amount = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Currency"))
                {
                    currency = property.Value.GetString();
                    continue;
                }
            }
            return new OBActiveOrHistoricCurrencyAndAmount5(amount, currency);
        }
    }
}
