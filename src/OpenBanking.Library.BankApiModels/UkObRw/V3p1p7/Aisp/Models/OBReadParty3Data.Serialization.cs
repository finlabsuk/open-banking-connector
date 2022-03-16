// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class OBReadParty3Data
    {
        internal static OBReadParty3Data DeserializeOBReadParty3Data(JsonElement element)
        {
            Optional<IReadOnlyList<OBParty2>> party = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Party"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBParty2> array = new List<OBParty2>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBParty2.DeserializeOBParty2(item));
                    }
                    party = array;
                    continue;
                }
            }
            return new OBReadParty3Data(Optional.ToList(party));
        }
    }
}
