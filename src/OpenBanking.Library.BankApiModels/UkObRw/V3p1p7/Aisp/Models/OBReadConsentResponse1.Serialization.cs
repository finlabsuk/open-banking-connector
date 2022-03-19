// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class OBReadConsentResponse1
    {
        public static OBReadConsentResponse1 DeserializeOBReadConsentResponse1(JsonElement element)
        {
            OBReadConsentResponse1Data data = default;
            object risk = default;
            Optional<Links> links = default;
            Optional<Meta> meta = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Data"))
                {
                    data = OBReadConsentResponse1Data.DeserializeOBReadConsentResponse1Data(property.Value);
                    continue;
                }
                if (property.NameEquals("Risk"))
                {
                    risk = property.Value.GetObject();
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
            return new OBReadConsentResponse1(data, risk, links.Value, meta.Value);
        }
    }
}
