// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public partial class Meta
    {
        public static Meta DeserializeMeta(JsonElement element)
        {
            Optional<int> totalPages = default;
            Optional<DateTimeOffset> firstAvailableDateTime = default;
            Optional<DateTimeOffset> lastAvailableDateTime = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("TotalPages"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    totalPages = property.Value.GetInt32();
                    continue;
                }
                if (property.NameEquals("FirstAvailableDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    firstAvailableDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("LastAvailableDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    lastAvailableDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
            }
            return new Meta(Optional.ToNullable(totalPages), Optional.ToNullable(firstAvailableDateTime), Optional.ToNullable(lastAvailableDateTime));
        }
    }
}
