# Database settings

Database settings are used to configure the database used by Open Banking Connector to read and write data. In the [development environment](./README.md#environment-selection), default settings are provided in [appsettings.Development.json](../../src/OpenBanking.WebApp.Connector/appsettings.Development.json) to allow auto-creation and use of a local SQLite file as the databse.

In non-development envrionments, you will at least need to at least provide a connection string setting for the configured database provider.

Database settings are defined in the [DatabaseSettings](../../src/OpenBanking.Library.Connector/Models/Configuration/DatabaseSettings.cs#L16) class.

## Settings

Name | Valid Values | Default Value(s) | Description
--- | --- | --- | ---
OpenBankingConnector<wbr/>:Database<wbr/>:Provider | {`"Sqlite"`, `"PostgreSql"`} | `"PostgreSql"` | Determines which database provider Open Banking Connector uses.
OpenBankingConnector<wbr/>:Database<wbr/>:ConnectionStrings:{Provider} <p style="margin-top: 10px;"> *where Provider ∈ {Sqlite, PostgreSql}*  </p> | string | `"Data Source=./sqliteTestDb.db"` (development environment and Provider = `"Sqlite"`) <p style="margin-top: 10px;"> `"Host=localhost;Database=test;`<wbr/>`Username=postgres"` (development environment and Provider = `"PostgreSql"`) <p style="margin-top: 10px;"> - (otherwise) | Connection strings for each database provider. Open Banking Connector only reads the one for the provider specified by OpenBankingConnector:Database:Provider. The BankTests project, however, may use multiple connection strings to test with multiple databases. A list of connection string parameters for provider `PostgreSql` is given [here](https://www.npgsql.org/doc/connection-string-parameters.html).
OpenBankingConnector<wbr/>:Database<wbr/>:EnsureDatabaseCreated |{`"true"`, `"false"`} | `"false"` | At application start-up, ensure database is created if does not exist.
OpenBankingConnector<wbr/>:Database<wbr/>:EnsureDatabaseMigrated |{`"true"`, `"false"`} | `"false"` | At application start-up, apply pending migrations. Only supported for database providers with migration support (i.e. only PostgreSql at this time).
