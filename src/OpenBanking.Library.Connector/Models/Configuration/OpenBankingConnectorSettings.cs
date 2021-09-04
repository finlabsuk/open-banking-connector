// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    /// <summary>
    ///     Database options
    /// </summary>
    public class DatabaseOptions
    {
        public string Provider { get; set; } = "Sqlite";

        public string ConnectionStringName { get; set; } = "SqliteDbConnectionString";

        /// <summary>
        ///     Ensures DB is created if does not exist. Intended for use in Development environment only.
        /// </summary>
        public string EnsureDbCreated { get; set; } = "false";

        public bool ProcessedEnsureDbCreated => string.Equals(
            EnsureDbCreated,
            "true",
            StringComparison.InvariantCultureIgnoreCase);

        public DbProvider ProcessedProvider => DbProviderHelper.DbProvider(Provider);
    }

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
        ///     Whitelist of software statement profile IDs to be extracted from key secrets.
        ///     Each ID should be separated by spaces. Software statement profiles are specified by
        ///     key secrets which implement <see cref="SoftwareStatementProfilesSettings" /> where the
        ///     dictionary keys are the IDs specified here.
        /// </summary>
        public string SoftwareStatementProfileIds { get; set; } = "";

        /// <summary>
        ///     Database options.
        /// </summary>
        public DatabaseOptions Database { get; set; } = new DatabaseOptions();

        /// <summary>
        ///     Key Secrets options.
        ///     Please note that null is only valid in Development environment and configures the use of local
        ///     user secrets. For .NET Generic Host apps, local user secrets are in fact always
        ///     used in Development environment even if another key secret provider is specified here (in which case
        ///     both are used).
        /// </summary>
        public KeySecretOptions? KeySecrets { get; set; }

        public IList<string> ProcessedSoftwareStatementProfileIds => SoftwareStatementProfileIds.Split(' ').ToList();

        public string SettingsSectionName => "OpenBankingConnector";

        public OpenBankingConnectorSettings Validate()
        {
            // Note that all values have defaults
            return this;
        }
    }
}
