// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public partial class OBReadAccount6Data
    {
        internal static OBReadAccount6Data DeserializeOBReadAccount6Data(JsonElement element)
        {
            Optional<IReadOnlyList<OBAccount6>> account = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Account"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBAccount6> array = new List<OBAccount6>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBAccount6.DeserializeOBAccount6(item));
                    }
                    account = array;
                    continue;
                }
            }
            return new OBReadAccount6Data(Optional.ToList(account));
        }
    }
}
