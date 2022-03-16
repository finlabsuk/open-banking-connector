// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public partial class OBReadBalance1Data
    {
        internal static OBReadBalance1Data DeserializeOBReadBalance1Data(JsonElement element)
        {
            IReadOnlyList<OBReadBalance1DataBalanceItem> balance = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Balance"))
                {
                    List<OBReadBalance1DataBalanceItem> array = new List<OBReadBalance1DataBalanceItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBReadBalance1DataBalanceItem.DeserializeOBReadBalance1DataBalanceItem(item));
                    }
                    balance = array;
                    continue;
                }
            }
            return new OBReadBalance1Data(balance);
        }
    }
}
