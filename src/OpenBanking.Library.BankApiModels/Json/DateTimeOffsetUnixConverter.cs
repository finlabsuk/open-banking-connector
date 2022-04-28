// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DateTimeOffsetToUnixConverterOptions
    {
        [EnumMember(Value = "None")]
        None = 0,

        [EnumMember(Value = "JsonUsesMilliSecondsNotSeconds")]
        JsonUsesMilliSecondsNotSeconds = 1
    }

    public abstract class
        DateTimeOffsetGenericUnixConverter<TDateTimeOffset> : JsonConverterWithOptions<TDateTimeOffset,
            DateTimeOffsetToUnixConverterOptions>
    {
        public DateTimeOffsetGenericUnixConverter() { }

        public DateTimeOffsetGenericUnixConverter(JsonConverterLabel jsonConverterLabel) :
            base(jsonConverterLabel) { }

        protected long GetUnixTime(DateTimeOffset time, DateTimeOffsetToUnixConverterOptions options)
        {
            long seconds = time.ToUnixTimeSeconds();
            long timeValue = options.HasFlag(DateTimeOffsetToUnixConverterOptions.JsonUsesMilliSecondsNotSeconds)
                ? seconds * 1000
                : seconds;
            return timeValue;
        }

        protected DateTimeOffset GetTime(long unixTime, DateTimeOffsetToUnixConverterOptions options)
        {
            long seconds = options.HasFlag(DateTimeOffsetToUnixConverterOptions.JsonUsesMilliSecondsNotSeconds)
                ? unixTime / 1000
                : unixTime;
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }
    }

    public class DateTimeOffsetUnixConverter : DateTimeOffsetGenericUnixConverter<DateTimeOffset>
    {
        public DateTimeOffsetUnixConverter() { }

        public DateTimeOffsetUnixConverter(JsonConverterLabel jsonConverterLabel) :
            base(jsonConverterLabel) { }

        public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
        {
            DateTimeOffsetToUnixConverterOptions options = GetOptions(serializer);
            long timeValue = GetUnixTime(value, options);
            JToken jt = JToken.FromObject(timeValue);
            jt.WriteTo(writer);
        }

        public override DateTimeOffset ReadJson(
            JsonReader reader,
            Type objectType,
            DateTimeOffset existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (objectType != typeof(DateTimeOffset))
            {
                throw new NotSupportedException($"The type {objectType} is not supported.");
            }

            if (reader.Value is null)
            {
                throw new NullReferenceException("Null received when type is non-nullable DateTime.");
            }

            DateTimeOffsetToUnixConverterOptions options = GetOptions(serializer);
            long timeValue = long.Parse(reader.Value.ToString()!);
            return GetTime(timeValue, options);
        }
    }

    public class DateTimeOffsetNullableUnixConverter : DateTimeOffsetGenericUnixConverter<DateTimeOffset?>
    {
        public DateTimeOffsetNullableUnixConverter() { }

        public DateTimeOffsetNullableUnixConverter(JsonConverterLabel jsonConverterLabel) :
            base(jsonConverterLabel) { }

        public override void WriteJson(JsonWriter writer, DateTimeOffset? value, JsonSerializer serializer)
        {
            DateTimeOffsetToUnixConverterOptions options = GetOptions(serializer);
            if (value is null)
            {
                writer.WriteNull();
            }
            else
            {
                long timeValue = GetUnixTime(value.Value, options);
                JToken jt = JToken.FromObject(timeValue);
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

            DateTimeOffsetToUnixConverterOptions options = GetOptions(serializer);
            long timeValue = long.Parse(reader.Value.ToString()!);
            return GetTime(timeValue, options);
        }
    }
}
