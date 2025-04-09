// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Json;

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

        var value = new SerialisedEntity { DateAndTime = dateTimeOffset };

        Func<bool> rule = () =>
        {
            string json = JsonConvert.SerializeObject(value);
            var newValue = JsonConvert.DeserializeObject<SerialisedEntity>(json)!;

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

        var value = new SerialisedEntity { DateAndTime = dto };

        Func<bool> rule = () =>
        {
            string json = JsonConvert.SerializeObject(value);
            var newValue = JsonConvert.DeserializeObject<SerialisedEntity>(json)!;

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
            // Set up converter and JSON serialiser settings
            var converter = new DateTimeOffsetUnixConverter(JsonConverterLabel.DcrRegScope);
            var optionsDict = new Dictionary<JsonConverterLabel, int>
            {
                [JsonConverterLabel.DcrRegScope] = (int) DateTimeOffsetUnixConverterEnum.UnixSecondsJsonFormat
            };
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                Context = new StreamingContext(StreamingContextStates.All, optionsDict)
            };

            var stringWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(stringWriter);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("iat");

            var jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);

            converter.WriteJson(jsonWriter, value, jsonSerializer);

            jsonWriter.WriteEndObject();

            jsonWriter.Flush();
            var json = stringWriter.ToString();

            var x = JsonConvert.DeserializeObject<DeserialisedEntity>(json)!;

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
