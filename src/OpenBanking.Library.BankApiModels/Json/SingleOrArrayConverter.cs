// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json;

/// <summary>
///     Custom converter for deserialising a field that may be returned either as a single value or as a JSON array containing a single value.
/// </summary>
public class SingleOrArrayConverter<T> : JsonConverter
{
    public override bool CanConvert(Type objectType) => true;

    public override object? ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Array)
        {
            var array = (JArray) token;
            if (array.Count != 1)
            {
                throw new JsonSerializationException(
                    $"We can accept JSON array with exactly one element when deserialising {typeof(T).Name}, but received array with {array.Count} elements.");
            }
            return array[0].ToObject<T>(serializer);
        }

        return token.ToObject<T>(serializer);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
