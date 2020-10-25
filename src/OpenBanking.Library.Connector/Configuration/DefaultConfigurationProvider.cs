// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    public class DefaultConfigurationProvider : IObcConfigurationProvider
    {
        private readonly ObcConfiguration _obcConfiguration;

        public DefaultConfigurationProvider(ObcConfiguration obcConfiguration)
        {
            _obcConfiguration = obcConfiguration;
        }

        public ObcConfiguration GetObcConfiguration()
        {
            return _obcConfiguration;
        }
    }
}
