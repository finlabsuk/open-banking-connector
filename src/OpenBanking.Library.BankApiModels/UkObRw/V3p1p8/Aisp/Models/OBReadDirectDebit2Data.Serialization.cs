// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public partial class OBReadDirectDebit2Data
    {
        public static OBReadDirectDebit2Data DeserializeOBReadDirectDebit2Data(JsonElement element)
        {
            Optional<IReadOnlyList<OBReadDirectDebit2DataDirectDebitItem>> directDebit = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("DirectDebit"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBReadDirectDebit2DataDirectDebitItem> array = new List<OBReadDirectDebit2DataDirectDebitItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBReadDirectDebit2DataDirectDebitItem.DeserializeOBReadDirectDebit2DataDirectDebitItem(item));
                    }
                    directDebit = array;
                    continue;
                }
            }
            return new OBReadDirectDebit2Data(Optional.ToList(directDebit));
        }
    }
}
