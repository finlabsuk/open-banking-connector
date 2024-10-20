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

    [Fact]
    public void ConvertOBTransactionCashBalance()
    {
        var obTransactionCashBalance = new AccountAndTransactionModelsPublic.OBTransactionCashBalance
        {
            CreditDebitIndicator = AccountAndTransactionModelsPublic.OBCreditDebitCode_2.Credit,
            Type = AccountAndTransactionModelsPublic.OBBalanceType1Code.InterimBooked,
            Amount = null
        };

        string jsonString = JsonConvert.SerializeObject(
            obTransactionCashBalance,
            Formatting.Indented,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        var result =
            JsonConvert.DeserializeObject<AccountAndTransactionModelsPublic.OBTransactionCashBalance>(jsonString);

        result.Should().BeEquivalentTo(obTransactionCashBalance);
    }

    [Fact]
    public void ConvertOBAccount6()
    {
        var initialValue = new AccountAndTransactionModelsPublic.OBAccount6
        {
            AccountId = "1234",
            AccountSubType = AccountAndTransactionModelsPublic.OBExternalAccountSubType1Code.Wallet
        };

        string jsonString = JsonConvert.SerializeObject(
            initialValue,
            Formatting.Indented,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        var result =
            JsonConvert.DeserializeObject<AccountAndTransactionModelsPublic.OBAccount6>(jsonString);

        result.Should().BeEquivalentTo(initialValue);
    }
}
