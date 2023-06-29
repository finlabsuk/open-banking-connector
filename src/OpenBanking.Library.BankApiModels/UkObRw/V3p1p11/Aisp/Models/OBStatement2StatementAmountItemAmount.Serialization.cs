// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    public partial class OBStatement2StatementAmountItemAmount
    {
        internal static OBStatement2StatementAmountItemAmount DeserializeOBStatement2StatementAmountItemAmount(JsonElement element)
        {
            string amount = default;
            string currency = default;
            Optional<OBStatement2StatementAmountSubTypeEnum> subType = default;
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
                if (property.NameEquals("SubType"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    subType = property.Value.GetString().ToOBStatement2StatementAmountSubTypeEnum();
                    continue;
                }
            }
            return new OBStatement2StatementAmountItemAmount(amount, currency, Optional.ToNullable(subType));
        }
    }
}
