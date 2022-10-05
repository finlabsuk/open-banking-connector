// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public partial class OBBranchAndFinancialInstitutionIdentification50
    {
        public static OBBranchAndFinancialInstitutionIdentification50 DeserializeOBBranchAndFinancialInstitutionIdentification50(JsonElement element)
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
            return new OBBranchAndFinancialInstitutionIdentification50(schemeName, identification);
        }
    }
}
