// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public partial class OBReadDataStatement2
    {
        public static OBReadDataStatement2 DeserializeOBReadDataStatement2(JsonElement element)
        {
            Optional<IReadOnlyList<OBStatement2>> statement = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Statement"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBStatement2> array = new List<OBStatement2>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBStatement2.DeserializeOBStatement2(item));
                    }
                    statement = array;
                    continue;
                }
            }
            return new OBReadDataStatement2(Optional.ToList(statement));
        }
    }
}
