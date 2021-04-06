// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base.Json
{
    [Flags]
    public enum DateTimeOffsetUnixConverterOptions
    {
        None = 0,
        MilliSecondsNotSeconds = 1
    }

    public abstract class
        DateTimeOffsetGenericUnixConverter<TDateTimeOffset> : JsonConverterWithOptions<TDateTimeOffset,
            DateTimeOffsetUnixConverterOptions>
    {
        public DateTimeOffsetGenericUnixConverter() { }

        public DateTimeOffsetGenericUnixConverter(DateTimeOffsetUnixConverterOptions activeOptions) : base(
            activeOptions) { }

        protected long GetUnixTime(DateTimeOffset time, DateTimeOffsetUnixConverterOptions options)
        {
            long seconds = time.ToUnixTimeSeconds();
            long timeValue = options.HasFlag(DateTimeOffsetUnixConverterOptions.MilliSecondsNotSeconds)
                ? seconds * 1000
                : seconds;
            return timeValue;
        }

        protected DateTimeOffset GetTime(long unixTime, DateTimeOffsetUnixConverterOptions options)
        {
            long seconds = options.HasFlag(DateTimeOffsetUnixConverterOptions.MilliSecondsNotSeconds)
                ? unixTime / 1000
                : unixTime;
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }
    }

    public class DateTimeOffsetUnixConverter : DateTimeOffsetGenericUnixConverter<DateTimeOffset>
    {
        public DateTimeOffsetUnixConverter() { }

        public DateTimeOffsetUnixConverter(DateTimeOffsetUnixConverterOptions activeOptions) : base(activeOptions) { }

        public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
        {
            DateTimeOffsetUnixConverterOptions options = GetOptions(serializer);
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

            DateTimeOffsetUnixConverterOptions options = GetOptions(serializer);
            long timeValue = long.Parse(reader.Value.ToString());
            return GetTime(timeValue, options);
        }
    }

    public class DateTimeOffsetNullableUnixConverter : DateTimeOffsetGenericUnixConverter<DateTimeOffset?>
    {
        public DateTimeOffsetNullableUnixConverter() { }

        public DateTimeOffsetNullableUnixConverter(DateTimeOffsetUnixConverterOptions activeOptions) : base(
            activeOptions) { }

        public override void WriteJson(JsonWriter writer, DateTimeOffset? value, JsonSerializer serializer)
        {
            DateTimeOffsetUnixConverterOptions options = GetOptions(serializer);
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

            DateTimeOffsetUnixConverterOptions options = GetOptions(serializer);
            long timeValue = long.Parse(reader.Value.ToString());
            return GetTime(timeValue, options);
        }
    }
}
