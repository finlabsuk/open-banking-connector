# Migrations

These allow tracking of changes in the database schema. Currently these are created and supported for DB provider `PostgreSql` only.

See the [EF Core docs](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli) for more infomration on EF Core migrations.

To create or update migrations from the repo, please first ensure .NET EF tools are installed as follows:

```powershell
dotnet tool install --global dotnet-ef
```

## Create a migration

A migration can be created from the current source code data models in the repo.

To create a migration, use the following code adjusted to the DB provider (this code is for provider `PostgreSql`):

```powershell
# Set working directory to core library (migrations are stored there)
cd src/OpenBanking.Library.Connector

# Create migration specifying output directory
dotnet ef migrations add MigrationName --startup-project ..\OpenBanking.WebApp.Connector --context PostgreSqlDbContext --output-dir Migrations/PostgreSql -- --OpenBankingConnector:Database:Provider=PostgreSql

```

## Apply the latest migration

Generally, you can use Open Banking Connector to automatically apply the latest migration using the [OpenBankingConnector:Database:EnsureDatabaseMigrated](../../../docs/configuration/database-settings.md) setting.

You can also apply a migration manually (or create the database) from the repo as follows:

```powershell

# Set working directory to web app (startup project)
cd src/OpenBanking.WebApp.Connector

# Create database or update to latest migration
dotnet ef database update --context PostgreSqlDbContext -- --OpenBankingConnector:Database:Provider=PostgreSql
```

