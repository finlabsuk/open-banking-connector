// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class OBReadScheduledPayment3Data
    {
        public static OBReadScheduledPayment3Data DeserializeOBReadScheduledPayment3Data(JsonElement element)
        {
            Optional<IReadOnlyList<OBScheduledPayment3>> scheduledPayment = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("ScheduledPayment"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBScheduledPayment3> array = new List<OBScheduledPayment3>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBScheduledPayment3.DeserializeOBScheduledPayment3(item));
                    }
                    scheduledPayment = array;
                    continue;
                }
            }
            return new OBReadScheduledPayment3Data(Optional.ToList(scheduledPayment));
        }
    }
}
