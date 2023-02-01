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
public enum DateTimeOffsetConverterEnum
{
    [EnumMember(Value = "Default")]
    Default = 0,

    // Applicable to nullable converter only
    [EnumMember(Value = "JsonInvalidStringBecomesNull")]
    JsonInvalidStringBecomesNull = 1
}

public class DateTimeOffsetNullableConverter : JsonConverterWithOptions<DateTimeOffset?,
    DateTimeOffsetConverterEnum>

{
    public DateTimeOffsetNullableConverter() : base(null) { } // required for case where no label used

    public DateTimeOffsetNullableConverter(JsonConverterLabel jsonConverterLabel) :
        base(jsonConverterLabel) { }

    public override void WriteJson(JsonWriter writer, DateTimeOffset? value, JsonSerializer serializer)
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


    public override DateTimeOffset? ReadJson(
        JsonReader reader,
        Type objectType,
        DateTimeOffset? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (objectType != typeof(DateTimeOffset?))
        {
            throw new NotSupportedException($"The type {objectType} is not supported.");
        }

        if (reader.Value is null)
        {
            return null;
        }

        // if (reader.TokenType is JsonToken.Date)
        // {
        //     var dateTime = (DateTime) reader.Value;
        //     if (dateTime.Kind is not DateTimeKind.Utc)
        //     {
        //         throw new JsonSerializationException("Invalid time value received (not UTC).");
        //     }
        //
        //     DateTimeOffset dateTimeOffset = dateTime;
        //     return dateTimeOffset;
        // }

        DateTimeOffsetConverterEnum options = GetOptions(serializer);
        if (reader.TokenType is JsonToken.String)
        {
            try
            {
                var value = (string) reader.Value;
                using var stringReader = new StringReader("\"" + value + "\"");
                using var jsonReader = new JsonTextReader(stringReader);
                return jsonReader.ReadAsDateTimeOffset();
            }
            catch
            {
                if (options.HasFlag(DateTimeOffsetConverterEnum.JsonInvalidStringBecomesNull))
                {
                    return null;
                }

                throw;
            }
        }

        throw new JsonSerializationException($"Invalid TokenType {reader.TokenType} received.");
    }
}
