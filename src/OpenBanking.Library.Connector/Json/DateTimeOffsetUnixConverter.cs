// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FinnovationLabs.OpenBanking.Library.Connector.Json
{
    public class DateTimeOffsetUnixConverter : JsonConverter<DateTimeOffset>
    {
        public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
        {
            long seconds = value.ToUnixTimeSeconds();
            JToken jt = JToken.FromObject(seconds);

            jt.WriteTo(writer);
        }

        public override DateTimeOffset ReadJson(
            JsonReader reader,
            Type objectType,
            DateTimeOffset existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (objectType == typeof(DateTimeOffset))
            {
                long seconds = long.Parse(reader.Value.ToString());

                return DateTimeOffset.FromUnixTimeSeconds(seconds);
            }

            throw new NotSupportedException($"The type {objectType} is not supported.");
        }
    }
}
