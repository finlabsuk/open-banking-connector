// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using Microsoft.Extensions.Configuration;
using IConfigurationProvider = FinnovationLabs.OpenBanking.Library.Connector.Configuration.IConfigurationProvider;

namespace FinnovationLabs.OpenBanking.Library.Connector.NetGenericHost.Configuration
{
    internal class AppsettingsConfigurationProvider : IConfigurationProvider
    {
        private const string ConfigPrefix = "OBC.NET";
        private readonly IConfiguration _config;

        public AppsettingsConfigurationProvider(IConfiguration config)
        {
            _config = config;
        }

        public RuntimeConfiguration GetRuntimeConfiguration()
        {
            return ReadConfiguration();
        }

        private RuntimeConfiguration ReadConfiguration()
        {
            return new RuntimeConfiguration
            {
                DefaultCurrency = _config.GetValue<string>($"{ConfigPrefix}:defaultCurrency"),
                SoftwareId = _config.GetValue<string>($"{ConfigPrefix}:softwareId"),
                DefaultFragmentRedirectUrl = _config.GetValue<string>($"{ConfigPrefix}:defaultFragmentRedirectUrl")
            };
        }
    }
}
