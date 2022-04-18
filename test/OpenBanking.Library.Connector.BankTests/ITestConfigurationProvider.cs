// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public interface ITestConfigurationProvider
    {
        bool? GetBooleanValue(string key);

        string? GetValue(string key);

        T? GetEnumValue<T>(string key)
            where T : struct;

        BankRegistrationClaimsOverrides? GetOpenBankingClientRegistrationClaimsOverrides();

        OpenIdConfiguration? GetOpenBankingOpenIdConfiguration();
    }
}
