# Migrations

These allow tracking of changes in the database schema. Currently these are created and supported for DB provider `PostgreSql` only.

See the [EF Core docs](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli) for more infomration on EF Core migrations.

## Creating migrations

To create a migration, ensure .NET EF tools are installed as follows:

```powershell
dotnet tool install --global dotnet-ef
```

When we create a migration, we also store the SQL script associated with the update to allow for easy inspection of the changes to the DB schema.

### Initial migration

To create the first migration and associated SQL script, use the following code adjusted to the DB provider (this code is for provider `PostgreSql`)

```powershell
# Set working directory to core library (migrations are stored there)
cd src/OpenBanking.Library.Connector

# Create initial migration and specify output directory
dotnet ef migrations add InitialCreate --startup-project ..\OpenBanking.WebApp.Connector --context PostgreSqlDbContext --output-dir Migrations/PostgreSql -- --OpenBankingConnector:Database:Provider=PostgreSql

# Create associated SQL (adjust output file name to match migration name)
dotnet ef migrations script --startup-project ..\OpenBanking.WebApp.Connector --context PostgreSqlDbContext -o Migrations/PostgreSql/20220420094645_InitialCreate.sql -- --OpenBankingConnector:Database:Provider=PostgreSql
```

### Subsequent migrations

TODO: add instructions for creating subsequent migrations

## Applying migrations

There are multiple ways to apply a migration including using SQL scripts.

Example: if you have access to the source code, you can use the following `dotnet ef` command to update the database used by the Open Banking Connector Web App to the latest schema (or create it).

```powershell

# Set working directory to web app (startup project)
cd src/OpenBanking.WebApp.Connector

dotnet ef database update --context PostgreSqlDbContext -- --OpenBankingConnector:Database:Provider=PostgreSql
```

