// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public partial class OBBranchAndFinancialInstitutionIdentification61
    {
        public static OBBranchAndFinancialInstitutionIdentification61 DeserializeOBBranchAndFinancialInstitutionIdentification61(JsonElement element)
        {
            Optional<string> schemeName = default;
            Optional<string> identification = default;
            Optional<string> name = default;
            Optional<OBPostalAddress6> postalAddress = default;
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
                if (property.NameEquals("Name"))
                {
                    name = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("PostalAddress"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    postalAddress = OBPostalAddress6.DeserializeOBPostalAddress6(property.Value);
                    continue;
                }
            }
            return new OBBranchAndFinancialInstitutionIdentification61(schemeName.Value, identification.Value, name.Value, postalAddress.Value);
        }
    }
}
