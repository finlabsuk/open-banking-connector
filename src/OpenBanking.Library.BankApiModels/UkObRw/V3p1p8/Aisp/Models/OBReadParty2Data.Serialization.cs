// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace AccountAndTransactionAPISpecification.Models
{
    public partial class OBReadParty2Data
    {
        internal static OBReadParty2Data DeserializeOBReadParty2Data(JsonElement element)
        {
            Optional<OBParty2> party = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Party"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    party = OBParty2.DeserializeOBParty2(property.Value);
                    continue;
                }
            }
            return new OBReadParty2Data(party.Value);
        }
    }
}
