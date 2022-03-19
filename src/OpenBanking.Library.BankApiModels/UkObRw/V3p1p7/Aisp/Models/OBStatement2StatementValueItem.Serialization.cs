// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class OBStatement2StatementValueItem
    {
        public static OBStatement2StatementValueItem DeserializeOBStatement2StatementValueItem(JsonElement element)
        {
            string value = default;
            string type = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Value"))
                {
                    value = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Type"))
                {
                    type = property.Value.GetString();
                    continue;
                }
            }
            return new OBStatement2StatementValueItem(value, type);
        }
    }
}
