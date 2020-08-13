// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    public class DefaultConfigurationProvider : IObcConfigurationProvider
    {
        public ObcConfiguration GetObcConfiguration()
        {
            return new ObcConfiguration
            {
                DefaultCurrency = "GBP",
                SoftwareId = "",
                EnsureDbCreated = "false"
            };
        }
    }
}
