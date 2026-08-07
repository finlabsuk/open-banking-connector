// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Globalization;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json;

/// <summary>
///     Converter for `OBRemittanceInformationStructured.ReferredDocumentAmount`, where the target model represents
///     the amount as a JSON string, as introduced by OB v4.0.1 (in contrast to OB v4.0, where this field was a
///     JSON integer). Auto-detects which shape was received so that no per-bank indicator is required to handle
///     both cases.
/// </summary>
public class ReferredDocumentAmountConverter : JsonConverter<string?>
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
                // v4.0.1-compliant shape: value already a string.
                return (string?) reader.Value;
            case JsonToken.Integer:
            case JsonToken.Float:
                // Assume the bank has returned the v4.0-style shape (a JSON number) and convert to string.
                return Convert.ToString(reader.Value, CultureInfo.InvariantCulture);
            default:
                throw new JsonSerializationException(
                    $"Could not deserialise ReferredDocumentAmount from token type {reader.TokenType}.");
        }
    }
}
