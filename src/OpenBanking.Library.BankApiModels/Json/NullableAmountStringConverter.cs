// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Globalization;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json;

/// <summary>
///     Converts an amount field that may be received as a JSON string or a JSON number (integer/float) into a
///     string, and always writes it back out as a JSON string. Some ASPSPs send fields such as
///     ReferredDocumentAmount as numbers even though the OBIE schema specifies type string.
/// </summary>
public class NullableAmountStringConverter : JsonConverter<string?>
{
    public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }

    public override string? ReadJson(
        JsonReader reader,
        Type objectType,
        string? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.Null:
                return null;
            case JsonToken.String:
                return (string) reader.Value!;
            case JsonToken.Integer:
            case JsonToken.Float:
                return Convert.ToString(reader.Value, CultureInfo.InvariantCulture);
            default:
                throw new JsonSerializationException($"Invalid TokenType {reader.TokenType} received.");
        }
    }
}
