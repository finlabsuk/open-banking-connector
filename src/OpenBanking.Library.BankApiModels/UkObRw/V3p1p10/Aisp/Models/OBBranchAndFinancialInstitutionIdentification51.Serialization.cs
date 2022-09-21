// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public partial class OBBranchAndFinancialInstitutionIdentification51
    {
        internal static OBBranchAndFinancialInstitutionIdentification51 DeserializeOBBranchAndFinancialInstitutionIdentification51(JsonElement element)
        {
            string schemeName = default;
            string identification = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("SchemeName"))
                {
                    schemeName = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Identification"))
                {
                    identification = property.Value.GetString();
                    continue;
                }
            }
            return new OBBranchAndFinancialInstitutionIdentification51(schemeName, identification);
        }
    }
}
