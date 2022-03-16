// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public partial class OBErrorResponse1
    {
        internal static OBErrorResponse1 DeserializeOBErrorResponse1(JsonElement element)
        {
            string code = default;
            Optional<string> id = default;
            string message = default;
            IReadOnlyList<OBError1> errors = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("Code"))
                {
                    code = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Id"))
                {
                    id = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Message"))
                {
                    message = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Errors"))
                {
                    List<OBError1> array = new List<OBError1>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBError1.DeserializeOBError1(item));
                    }
                    errors = array;
                    continue;
                }
            }
            return new OBErrorResponse1(code, id.Value, message, errors);
        }
    }
}
