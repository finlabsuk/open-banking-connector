// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class OBReadStandingOrder6Data
    {
        internal static OBReadStandingOrder6Data DeserializeOBReadStandingOrder6Data(JsonElement element)
        {
            Optional<IReadOnlyList<OBStandingOrder6>> standingOrder = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("StandingOrder"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBStandingOrder6> array = new List<OBStandingOrder6>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBStandingOrder6.DeserializeOBStandingOrder6(item));
                    }
                    standingOrder = array;
                    continue;
                }
            }
            return new OBReadStandingOrder6Data(Optional.ToList(standingOrder));
        }
    }
}
