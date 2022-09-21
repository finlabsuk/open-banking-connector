// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public partial class OBMerchantDetails1
    {
        internal static OBMerchantDetails1 DeserializeOBMerchantDetails1(JsonElement element)
        {
            Optional<string> merchantName = default;
            Optional<string> merchantCategoryCode = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("MerchantName"))
                {
                    merchantName = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("MerchantCategoryCode"))
                {
                    merchantCategoryCode = property.Value.GetString();
                    continue;
                }
            }
            return new OBMerchantDetails1(merchantName.Value, merchantCategoryCode.Value);
        }
    }
}
