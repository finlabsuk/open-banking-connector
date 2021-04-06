// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base.Json;
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
            var dateTimeOffset = new DateTimeOffset(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                TimeSpan.Zero);

            SerialisedEntity value = new SerialisedEntity
            {
                DateAndTime = dateTimeOffset
            };

            Func<bool> rule = () =>
            {
                string json = JsonConvert.SerializeObject(value);
                SerialisedEntity newValue = JsonConvert.DeserializeObject<SerialisedEntity>(json)!;

                return dateTimeOffset == newValue.DateAndTime;
            };

            return rule.When(true);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property DateTimeOffset_MillisecondsIgnored(DateTime dateTime, int milliseconds)
        {
            DateTime dt = new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                DateTimeKind.Utc).AddMilliseconds(milliseconds);
            var dto = new DateTimeOffset(dt, TimeSpan.Zero);

            SerialisedEntity value = new SerialisedEntity
            {
                DateAndTime = dto
            };

            Func<bool> rule = () =>
            {
                string json = JsonConvert.SerializeObject(value);
                SerialisedEntity newValue = JsonConvert.DeserializeObject<SerialisedEntity>(json)!;

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
                DateTimeOffsetUnixConverter converter = new DateTimeOffsetUnixConverter();

                StringWriter stringWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter);
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("iat");
                converter.WriteJson(jsonWriter, value, new JsonSerializer());

                jsonWriter.WriteEndObject();

                jsonWriter.Flush();
                string json = stringWriter.ToString();

                DeserialisedEntity x = JsonConvert.DeserializeObject<DeserialisedEntity>(json)!;

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
