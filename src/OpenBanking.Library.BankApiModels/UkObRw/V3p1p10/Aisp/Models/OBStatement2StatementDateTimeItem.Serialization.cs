// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public partial class OBStatement2StatementDateTimeItem
    {
        internal static OBStatement2StatementDateTimeItem DeserializeOBStatement2StatementDateTimeItem(JsonElement element)
        {
            DateTimeOffset dateTime = default;
            string type = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("DateTime"))
                {
                    dateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("Type"))
                {
                    type = property.Value.GetString();
                    continue;
                }
            }
            return new OBStatement2StatementDateTimeItem(dateTime, type);
        }
    }
}
