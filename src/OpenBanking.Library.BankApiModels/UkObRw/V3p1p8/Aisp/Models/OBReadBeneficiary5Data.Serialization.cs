// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public partial class OBReadBeneficiary5Data
    {
        internal static OBReadBeneficiary5Data DeserializeOBReadBeneficiary5Data(JsonElement element)
        {
            Optional<IReadOnlyList<OBBeneficiary5>> beneficiary = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Beneficiary"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBBeneficiary5> array = new List<OBBeneficiary5>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBBeneficiary5.DeserializeOBBeneficiary5(item));
                    }
                    beneficiary = array;
                    continue;
                }
            }
            return new OBReadBeneficiary5Data(Optional.ToList(beneficiary));
        }
    }
}
