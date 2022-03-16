// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace AccountAndTransactionAPISpecification.Models
{
    public partial class OBError1
    {
        internal static OBError1 DeserializeOBError1(JsonElement element)
        {
            string errorCode = default;
            string message = default;
            Optional<string> path = default;
            Optional<string> url = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("ErrorCode"))
                {
                    errorCode = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Message"))
                {
                    message = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Path"))
                {
                    path = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Url"))
                {
                    url = property.Value.GetString();
                    continue;
                }
            }
            return new OBError1(errorCode, message, path.Value, url.Value);
        }
    }
}
