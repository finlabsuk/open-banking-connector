// Licensed to Finnovation Labs Limited under one or more agreements.
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
        ///     Database provider to be used by Open Banking Connector.
        /// </summary>
        public DbProvider Provider { get; set; } = DbProvider.Sqlite;

        /// <summary>
        ///     Connection string to use to connect to database.
        /// </summary>
        public Dictionary<DbProvider, string> ConnectionStrings { get; set; } =
            new Dictionary<DbProvider, string>();

        /// <summary>
        ///     Ensures DB is created if does not exist. Intended for use in Development environment only.
        /// </summary>
        public bool EnsureDbCreated { get; set; } = false;

        public string SettingsGroupName => "Database";

        public DatabaseSettings Validate()
        {
            // Note that all values have defaults
            return this;
        }
    }
}
