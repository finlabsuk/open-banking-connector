# Database settings

Database settings are used to configure the database used by Open Banking Connector to read and write data. In the [development environment](./README.md#environment-selection), default settings are provided in [appsettings.Development.json](../../src/OpenBanking.WebApp.Connector/appsettings.Development.json) to allow auto-creation and use of a local SQLite file as the databse.

In non-development envrionments, you will at least need to at least provide a connection string setting for the configured database provider.

Database settings are defined in the [DatabaseSettings](../../src/OpenBanking.Library.Connector/Models/Configuration/DatabaseSettings.cs#L16) class.

## Settings

Name | Valid Values | Default Value(s) | Description
--- | --- | --- | ---
OpenBankingConnector<wbr/>:Database<wbr/>:Provider | {`"Sqlite"`, `"Postgres"`} | `"Sqlite"` | Determines which database provider Open Banking Connector uses.
OpenBankingConnector<wbr/>:Database<wbr/>:ConnectionStrings:{Provider} <p style="margin-top: 10px;"> *where Provider ∈ {Sqlite, Postgres}*  </p> | string | `"Data Source=./sqliteTestDb.db"` (development environment and Provider = `"Sqlite"`) <p style="margin-top: 10px;"> - (otherwise) | Connection strings for each database provider. Open Banking Connector only reads the one for the provider specified by OpenBankingConnector:Database:Provider. The BankTests project, however, may use multiple connection strings to test with multiple databases.
OpenBankingConnector<wbr/>:Database<wbr/>:EnsureDbCreated |{`"true"`, `"false"`} |`"true"` (development environment) <p style="margin-top: 10px;"> `"false"` (otherwise) | Ensures database is created if does not exist. Intended for use in Development environment only.

