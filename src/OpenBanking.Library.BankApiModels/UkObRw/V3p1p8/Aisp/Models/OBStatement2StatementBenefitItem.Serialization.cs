// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace AccountAndTransactionAPISpecification.Models
{
    public partial class OBStatement2StatementBenefitItem
    {
        internal static OBStatement2StatementBenefitItem DeserializeOBStatement2StatementBenefitItem(JsonElement element)
        {
            string type = default;
            OBActiveOrHistoricCurrencyAndAmount5 amount = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Type"))
                {
                    type = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Amount"))
                {
                    amount = OBActiveOrHistoricCurrencyAndAmount5.DeserializeOBActiveOrHistoricCurrencyAndAmount5(property.Value);
                    continue;
                }
            }
            return new OBStatement2StatementBenefitItem(type, amount);
        }
    }
}
