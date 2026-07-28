// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Json;

public class NullableAmountStringConverterTests
{
    [Theory]
    [InlineData("\"1209.06\"", "1209.06")]
    [InlineData("1209", "1209")]
    [InlineData("1209.06", "1209.06")]
    [InlineData("null", null)]
    public void ReadJson_ReturnsExpectedString(string json, string? expected)
    {
        var wrapped = $"{{\"Amount\":{json}}}";

        var result = JsonConvert.DeserializeObject<SerialisedEntity>(wrapped)!;

        result.Amount.Should().Be(expected);
    }

    [Fact]
    public void ReadJson_InvalidToken_Throws()
    {
        const string wrapped = "{\"Amount\":true}";

        Action act = () => JsonConvert.DeserializeObject<SerialisedEntity>(wrapped);

        act.Should().Throw<JsonSerializationException>();
    }

    [Fact]
    public void WriteJson_OutputsPlainJsonString()
    {
        var value = new SerialisedEntity { Amount = "1209.06" };

        string json = JsonConvert.SerializeObject(value);

        json.Should().Be("{\"Amount\":\"1209.06\"}");
    }

    public class SerialisedEntity
    {
        [JsonProperty("Amount")]
        [JsonConverter(typeof(NullableAmountStringConverter))]
        public string? Amount { get; set; }
    }
}
