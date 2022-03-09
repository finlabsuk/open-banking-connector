// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    /// <summary>
    ///     Key secrets options. Note that .Net Generic Host apps usually also
    ///     have access to local user secrets when running in Development envrionment.
    /// </summary>
    public class KeySecretOptions
    {
        public string Provider { get; set; } = "Azure";

        public string VaultName { get; set; } = "OpenBankingConnector";

        public KeySecretProvider ProcessedProvider => KeySecretProviderHelper.KeySecretProvider(Provider);
    }

    /// <summary>
    ///     Settings for an app using Open Banking Connector which configure Open Banking Connector.
    ///     These are used by the sample web app included with Open Banking Connector
    ///     and also the Bank Tests project which effectively includes a test app.
    /// </summary>
    public class OpenBankingConnectorSettings : ISettings<OpenBankingConnectorSettings>
    {
        /// <summary>
        ///     Key Secrets options.
        ///     Please note that null is only valid in Development environment and configures the use of local
        ///     user secrets. For .NET Generic Host apps, local user secrets are in fact always
        ///     used in Development environment even if another key secret provider is specified here (in which case
        ///     both are used).
        /// </summary>
        public KeySecretOptions? KeySecrets { get; set; }

        public string SettingsGroupName => "Database";

        public OpenBankingConnectorSettings Validate()
        {
            // Note that all values have defaults
            return this;
        }
    }
}
