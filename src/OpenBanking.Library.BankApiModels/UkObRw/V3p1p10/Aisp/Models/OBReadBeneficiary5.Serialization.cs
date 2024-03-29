// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public partial class OBReadBeneficiary5
    {
        public static OBReadBeneficiary5 DeserializeOBReadBeneficiary5(JsonElement element)
        {
            OBReadBeneficiary5Data data = default;
            Optional<Links> links = default;
            Optional<Meta> meta = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Data"))
                {
                    data = OBReadBeneficiary5Data.DeserializeOBReadBeneficiary5Data(property.Value);
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
            return new OBReadBeneficiary5(data, links.Value, meta.Value);
        }
    }
}
