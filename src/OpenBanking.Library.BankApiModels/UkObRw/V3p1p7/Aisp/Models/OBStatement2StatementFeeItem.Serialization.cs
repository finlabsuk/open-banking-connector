// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class OBStatement2StatementFeeItem
    {
        public static OBStatement2StatementFeeItem DeserializeOBStatement2StatementFeeItem(JsonElement element)
        {
            Optional<string> description = default;
            OBCreditDebitCode0Enum creditDebitIndicator = default;
            string type = default;
            Optional<float> rate = default;
            Optional<string> rateType = default;
            Optional<string> frequency = default;
            OBActiveOrHistoricCurrencyAndAmount6 amount = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Description"))
                {
                    description = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("CreditDebitIndicator"))
                {
                    creditDebitIndicator = property.Value.GetString().ToOBCreditDebitCode0Enum();
                    continue;
                }
                if (property.NameEquals("Type"))
                {
                    type = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Rate"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    rate = property.Value.GetSingle();
                    continue;
                }
                if (property.NameEquals("RateType"))
                {
                    rateType = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Frequency"))
                {
                    frequency = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Amount"))
                {
                    amount = OBActiveOrHistoricCurrencyAndAmount6.DeserializeOBActiveOrHistoricCurrencyAndAmount6(property.Value);
                    continue;
                }
            }
            return new OBStatement2StatementFeeItem(description.Value, creditDebitIndicator, type, Optional.ToNullable(rate), rateType.Value, frequency.Value, amount);
        }
    }
}
