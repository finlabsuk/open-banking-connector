// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public partial class OBCashAccount51
    {
        internal static OBCashAccount51 DeserializeOBCashAccount51(JsonElement element)
        {
            string schemeName = default;
            string identification = default;
            Optional<string> name = default;
            Optional<string> secondaryIdentification = default;
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
                if (property.NameEquals("SecondaryIdentification"))
                {
                    secondaryIdentification = property.Value.GetString();
                    continue;
                }
            }
            return new OBCashAccount51(schemeName, identification, name.Value, secondaryIdentification.Value);
        }
    }
}
