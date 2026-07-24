// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json;

[Flags]
[JsonConverter(typeof(StringEnumConverter))]
public enum DomesticVrpRefundConverterOptions
{
    [EnumMember(Value = "Default")]
    Default = 0,

    [EnumMember(Value = "ContainsNestedAccountProperty")]
    ContainsNestedAccountProperty = 1
}

public class DomesticVrpRefundConverter<TRefund> : JsonConverterWithOptions<TRefund?,
    DomesticVrpRefundConverterOptions>
    where TRefund : class
{
    public DomesticVrpRefundConverter() : base(null) { } // required for case where no label used

    public DomesticVrpRefundConverter(JsonConverterLabel jsonConverterLabel) :
        base(jsonConverterLabel)
    { }

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

        // Perform de-serialisation
        var options = GetOptions(serializer);
        var token = JToken.Load(reader);
        TRefund? refund;
        if (options is DomesticVrpRefundConverterOptions.ContainsNestedAccountProperty)
        {
            if (token.Type is not JTokenType.Object)
            {
                throw new Exception("Refund is not object.");
            }
            var accountToken = token["Account"];
            if (accountToken is null)
            {
                throw new Exception("Refund does not contain nested property Account.");
            }
            refund = accountToken.ToObject<TRefund>(serializer);
        }
        else
        {
            refund = token.ToObject<TRefund>(serializer);
        }

        if (refund is null)
        {
            throw new Exception("Could not deserialise Refund account.");
        }

        return refund;
    }
}
