// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public partial class OBPartyRelationships1Account
    {
        public static OBPartyRelationships1Account DeserializeOBPartyRelationships1Account(JsonElement element)
        {
            string related = default;
            string id = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Related"))
                {
                    related = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Id"))
                {
                    id = property.Value.GetString();
                    continue;
                }
            }
            return new OBPartyRelationships1Account(related, id);
        }
    }
}
