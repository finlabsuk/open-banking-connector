// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

public class HttpClientSettings : ISettings<HttpClientSettings>
{
    public int PooledConnectionLifetimeSeconds { get; set; } = 15 * 60;

    public string SettingsGroupName => "OpenBankingConnector:HttpClient";

    public HttpClientSettings Validate()
    {
        if (PooledConnectionLifetimeSeconds < 5)
        {
            throw new ArgumentException(
                "Configuration or key secrets error: " +
                "HttpClient:PooledConnectionLifetimeSeconds should be at least 5.");
        }

        return this;
    }
}
