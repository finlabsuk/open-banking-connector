// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public partial class OBBankTransactionCodeStructure1
    {
        internal static OBBankTransactionCodeStructure1 DeserializeOBBankTransactionCodeStructure1(JsonElement element)
        {
            string code = default;
            string subCode = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Code"))
                {
                    code = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("SubCode"))
                {
                    subCode = property.Value.GetString();
                    continue;
                }
            }
            return new OBBankTransactionCodeStructure1(code, subCode);
        }
    }
}
