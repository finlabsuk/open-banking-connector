﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    /// <summary>
    ///     Settings for an app using Open Banking Connector which configure Open Banking Connector.
    ///     These are used by the sample web app included with Open Banking Connector
    ///     and also the Bank Tests project which effectively includes a test app.
    /// </summary>
    public class DatabaseSettings : ISettings<DatabaseSettings>
    {
        /// <summary>
        ///     Determines which database provider Open Banking Connector uses.
        /// </summary>
        public DbProvider Provider { get; set; } = DbProvider.Sqlite;

        /// <summary>
        ///     Connection strings for each database provider. Open Banking Connector only reads the one for the provider specified
        ///     by OpenBankingConnector.Database.Provider. The BankTests project, however, may use multiple connection strings to
        ///     test with multiple databases.
        /// </summary>
        public Dictionary<DbProvider, string> ConnectionStrings { get; set; } =
            new Dictionary<DbProvider, string>
            {
                [DbProvider.Sqlite] = string.Empty,
                [DbProvider.PostgreSql] = string.Empty
            };

        /// <summary>
        ///     Ensures DB is created if does not exist. Intended for use in Development environment only.
        /// </summary>
        public bool EnsureDbCreated { get; set; } = false;

        public string SettingsGroupName => "OpenBankingConnector:Database";

        public DatabaseSettings Validate()
        {
            // Note that all values have defaults
            return this;
        }
    }
}