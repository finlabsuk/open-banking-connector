// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using FinnovationLabs.OpenBanking.Library.Connector.ObApi.Base.Json;
using FsCheck;
using FsCheck.Xunit;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Json
{
    public class DateTimeOffsetUnixConverterTests
    {
        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property DateTimeOffset_SerialisationIsSymmetric(DateTime dateTime)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(
                year: dateTime.Year,
                month: dateTime.Month,
                day: dateTime.Day,
                hour: dateTime.Hour,
                minute: dateTime.Minute,
                second: dateTime.Second,
                offset: TimeSpan.Zero);

            var value = new SerialisedEntity
            {
                DateAndTime = dateTimeOffset
            };

            Func<bool> rule = () =>
            {
                var json = JsonConvert.SerializeObject(value);
                var newValue = JsonConvert.DeserializeObject<SerialisedEntity>(json);

                return dateTimeOffset == newValue.DateAndTime;
            };

            return rule.When(true);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property DateTimeOffset_MillisecondsIgnored(DateTime dateTime, int milliseconds)
        {
            DateTime dt = new DateTime(
                year: dateTime.Year,
                month: dateTime.Month,
                day: dateTime.Day,
                hour: dateTime.Hour,
                minute: dateTime.Minute,
                second: dateTime.Second,
                kind: DateTimeKind.Utc).AddMilliseconds(milliseconds);
            DateTimeOffset dto = new DateTimeOffset(dateTime: dt, offset: TimeSpan.Zero);

            var value = new SerialisedEntity
            {
                DateAndTime = dto
            };

            Func<bool> rule = () =>
            {
                var json = JsonConvert.SerializeObject(value);
                var newValue = JsonConvert.DeserializeObject<SerialisedEntity>(json);

                double delta = Math.Abs((value.DateAndTime - newValue.DateAndTime).TotalMilliseconds);
                return delta == milliseconds;
            };

            return rule.When(milliseconds > 0);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property WriteJson_OutputIsInteger(DateTimeOffset value)
        {
            Func<bool> rule = () =>
            {
                var converter = new DateTimeOffsetUnixConverter();

                var stringWriter = new StringWriter();
                var jsonWriter = new JsonTextWriter(stringWriter);
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("iat");
                converter.WriteJson(writer: jsonWriter, value: value, serializer: new JsonSerializer());

                jsonWriter.WriteEndObject();

                jsonWriter.Flush();
                var json = stringWriter.ToString();

                var x = JsonConvert.DeserializeObject<DeserialisedEntity>(json);

                return x.UnixDateAndTime != 0;
            };

            return rule.When(value > DateTimeOffset.MinValue);
        }

        public class SerialisedEntity
        {
            [JsonProperty("iat")]
            [JsonConverter(typeof(DateTimeOffsetUnixConverter))]
            public DateTimeOffset DateAndTime { get; set; }
        }

        public class DeserialisedEntity
        {
            [JsonProperty("iat")]
            public long UnixDateAndTime { get; set; }
        }
    }
}
