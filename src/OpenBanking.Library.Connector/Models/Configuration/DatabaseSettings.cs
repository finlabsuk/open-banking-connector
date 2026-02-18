// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

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
    public DbProvider Provider { get; set; } = DbProvider.PostgreSql;

    /// <summary>
    ///     Connection strings for each database provider. Open Banking Connector only reads the one for the provider specified
    ///     by OpenBankingConnector.Database.Provider. The BankTests project, however, may use multiple connection strings to
    ///     test with multiple databases.
    /// </summary>
    public Dictionary<DbProvider, string> ConnectionStrings { get; set; } =
        new()
        {
            [DbProvider.Sqlite] = string.Empty,
            [DbProvider.PostgreSql] = string.Empty,
            [DbProvider.MongoDb] = string.Empty
        };

    /// <summary>
    ///     Database names (currently supported for MongoDB only).
    /// </summary>
    public Dictionary<DbProvider, string> Names { get; set; } =
        new()
        {
            [DbProvider.Sqlite] = string.Empty,
            [DbProvider.PostgreSql] = string.Empty,
            [DbProvider.MongoDb] = string.Empty
        };

    public Dictionary<DbProvider, SecretSource> PasswordSources { get; set; } =
        new()
        {
            [DbProvider.Sqlite] = SecretSource.Configuration,
            [DbProvider.PostgreSql] = SecretSource.Configuration,
            [DbProvider.MongoDb] = SecretSource.Configuration
        };

    public Dictionary<DbProvider, string> PasswordSettingNames { get; set; } =
        new();

    /// <summary>
    ///     At application start-up, ensure database is created if does not exist.
    /// </summary>
    public bool EnsureDatabaseCreated { get; set; } = false;

    /// <summary>
    ///     At application start-up, apply pending migrations. Only supported
    ///     for database providers with migration support (i.e. only PostgreSql at this time).
    /// </summary>
    public bool EnsureDatabaseMigrated { get; set; } = true;

    public string SettingsGroupName => "OpenBankingConnector:Database";

    public DatabaseSettings Validate()
    {
        // Ensure connection string provided
        if (string.IsNullOrEmpty(ConnectionStrings[Provider]))
        {
            throw new ArgumentException(
                "Configuration or key secrets error: " +
                $"No non-empty connection string provided for DB provider {Provider}.");
        }

        if (Provider is DbProvider.MongoDb)
        {
            if (string.IsNullOrEmpty(Names[Provider]))
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    $"No non-empty database name provided for DB provider {Provider}.");
            }
        }

        return this;
    }
}
