// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json;

/// <summary>
///     Converter for VRP Refund responses where the target model may nest the account fields under an
///     "Account" property, as introduced by OB v4.0.1's `OBDomesticRefundAccount1` (in contrast to earlier
///     versions, where `Data.Refund` held the account fields directly).
/// </summary>
public class DomesticVrpRefundConverterOptionalNesting<TRefund> : JsonConverter<TRefund?>
    where TRefund : class
{
    private const string AccountPropertyName = "Account";

    public override void WriteJson(JsonWriter writer, TRefund? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
        }
        else
        {
            JToken jt = JToken.FromObject(value);
            jt.WriteTo(writer);
        }
    }

    public override TRefund? ReadJson(
        JsonReader reader,
        Type objectType,
        TRefund? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType is JsonToken.Null)
        {
            return null;
        }

        var token = JToken.Load(reader);
        if (token.Type is not JTokenType.Object)
        {
            throw new Exception("Refund is not object.");
        }

        // If the response already nests fields under "Account", use it as-is (v4.0.1-compliant shape).
        // Otherwise assume the bank has returned the flat v4.0-style shape and wrap it, so that no
        // per-bank indicator is required to handle both cases.
        JToken sourceToken = token[AccountPropertyName] is not null
            ? token
            : new JObject { [AccountPropertyName] = token };

        return sourceToken.ToObject<TRefund>(serializer) ??
               throw new Exception("Could not deserialise Refund account.");
    }
}
