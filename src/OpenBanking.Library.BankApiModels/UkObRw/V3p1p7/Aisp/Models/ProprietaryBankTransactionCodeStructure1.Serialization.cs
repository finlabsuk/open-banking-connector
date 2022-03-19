// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class ProprietaryBankTransactionCodeStructure1
    {
        public static ProprietaryBankTransactionCodeStructure1 DeserializeProprietaryBankTransactionCodeStructure1(JsonElement element)
        {
            string code = default;
            Optional<string> issuer = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Code"))
                {
                    code = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Issuer"))
                {
                    issuer = property.Value.GetString();
                    continue;
                }
            }
            return new ProprietaryBankTransactionCodeStructure1(code, issuer.Value);
        }
    }
}
