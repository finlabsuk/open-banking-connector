To create a new migration, ensure .NET EF tools are installed via
```zsh
dotnet tool install --global dotnet-ef
```

Then create a new migration for a DB provider via
```zsh
cd src/OpenBanking.Library.Connector
dotnet ef migrations add InitialCreate --startup-project ../OpenBanking.WebApp.Connector.Sample --context SqliteDbContext --output-dir Migrations/SqliteMigrations
```

Use the following ```dotnet ef``` command to update an application database to the latest schema (or create it).

```zsh
cd src/OpenBanking.WebApp.Connector.Sample
dotnet ef database update --context SqliteDbContext 
```

See [here](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli) for more info.