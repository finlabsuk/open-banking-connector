// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace AccountAndTransactionAPISpecification.Models
{
    public partial class OBReadOffer1
    {
        internal static OBReadOffer1 DeserializeOBReadOffer1(JsonElement element)
        {
            OBReadOffer1Data data = default;
            Optional<Links> links = default;
            Optional<Meta> meta = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Data"))
                {
                    data = OBReadOffer1Data.DeserializeOBReadOffer1Data(property.Value);
                    continue;
                }
                if (property.NameEquals("Links"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    links = Links.DeserializeLinks(property.Value);
                    continue;
                }
                if (property.NameEquals("Meta"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    meta = Meta.DeserializeMeta(property.Value);
                    continue;
                }
            }
            return new OBReadOffer1(data, links.Value, meta.Value);
        }
    }
}
