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
        ///     Whitelist of software statement profiles (<see cref="SoftwareStatementProfile" />) specified by ID to be extracted
        ///     from key secrets. Each ID should be separated by spaces.
        ///     Software statement profiles are
        ///     specified by key secrets which implement <see cref="SoftwareStatementProfilesSettings" /> where the
        ///     dictionary keys are the IDs specified here.
        ///     Only software statement profiles listed here will be extracted from key secrets by Open Banking Connector.
        /// </summary>
        public string SoftwareStatementProfileIds { get; set; } = string.Empty;

        /// <summary>
        ///     Whitelist of override cases for software statement (<see cref="SoftwareStatementProfile" />), transport certificate
        ///     (<see cref="TransportCertificateProfile" />) and signing certificate (<see cref="SigningCertificateProfile" />)
        ///     profiles to be extracted from key secrets. Each override case should be separated by spaces.
        ///     Override cases are keys in dictionary properties of those profiles with end in "Overrides". For example
        ///     <see cref="SoftwareStatementProfile.TransportCertificateProfileIdOverrides" /> allows override cases for
        ///     <see cref="SoftwareStatementProfile.TransportCertificateProfileId" />.
        ///     They are typically bank names allowing profiles to customised for a particular bank.
        ///     Only override cases listed here will be extracted from key secrets by Open Banking Connector.
        /// </summary>
        public string SoftwareStatementAndCertificateOverrideCases { get; set; } = string.Empty;

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

        public List<string> SoftwareStatementProfileIdsAsList =>
            SoftwareStatementProfileIds.Split(' ').ToList();

        public List<string> SoftwareStatementAndCertificateOverrideCasesAsList =>
            SoftwareStatementAndCertificateOverrideCases.Split(' ').ToList();

        public string SettingsSectionName => "OpenBankingConnector";

        public OpenBankingConnectorSettings Validate()
        {
            // Note that all values have defaults
            return this;
        }
    }
}
