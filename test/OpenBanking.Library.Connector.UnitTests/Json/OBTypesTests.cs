// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Json;

public class OBTypesTests
{
    [Fact]
    public void ConvertOBCurrencyExchange5()
    {
        var obCurrencyExchange5 = new AccountAndTransactionModelsPublic.OBCurrencyExchange5
        {
            ExchangeRate = null,
            SourceCurrency = "USD"
        };

        string jsonString = JsonConvert.SerializeObject(
            obCurrencyExchange5,
            Formatting.Indented,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        var result = JsonConvert.DeserializeObject<AccountAndTransactionModelsPublic.OBCurrencyExchange5>(jsonString);

        result.Should().BeEquivalentTo(obCurrencyExchange5);
    }
}
