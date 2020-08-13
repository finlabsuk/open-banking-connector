// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Configuration
{
    public class NetGenericHostObcConfigurationProvider : IObcConfigurationProvider
    {
        private readonly IConfiguration _config;

        public NetGenericHostObcConfigurationProvider(IConfiguration config)
        {
            _config = config;
        }

        public ObcConfiguration GetObcConfiguration()
        {
            // Load Open Banking Connector configuration options
            ObcConfiguration? obcConfig = _config
                .GetSection(ObcConfiguration.OpenBankingConnector)
                .Get<ObcConfiguration>();

            return obcConfig;
        }
    }
}
