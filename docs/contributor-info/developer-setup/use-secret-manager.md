# Use Microsoft's secret manager

When running Open Banking Connector as a .NET application from source code, and when in the development environment,
Microsoft's secret manager can be used to provide configuration/secrets.

To use the secret manager, you will need a `secrets.json` file in the appropriate directory (if you do not have one
already). The Microsoft
documentation [here](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#how-the-secret-manager-tool-works)
gives the path for this directory. Please note that the UserSecretsId for the app is
`aa921213-9461-4f9e-8fec-153624ec67ad` as given in the web app `.csproj` file.

You can then add key-value pairs to the `secrets.json` file to supply configuration to Open Banking Connector.
See [configuration & secrets](../../configuration/README.md) for the settings groups available and how setting names map
to `secrets.json` keys.

Note that software statements and OBWAC/OBSeal certificates are no longer supplied via configuration/secrets; they are
created and managed at runtime via the [Management API](../../apis/management/README.md) (e.g.
`POST /manage/software-statements`, `POST /manage/obwac-certificates`, `POST /manage/obseal-certificates`) and stored in
the database instead.

Here is an example of what such a file might look like after configuring the database connection string and password
secret name:

```json
{
  "OpenBankingConnector:Database:Provider": "PostgreSql",
  "OpenBankingConnector:Database:ConnectionStrings:PostgreSql": "Host=localhost;Database=obc;Username=postgres",
  "OpenBankingConnector:Database:PasswordSettingNames:PostgreSql": "obcDatabasePassword",
  "obcDatabasePassword": "abc"
}
```

See [database settings](../../configuration/database-settings.md) for more information on database configuration,
including how the password secret is looked up and appended to the connection string.

**Important**: Some settings values are very sensitive, for example keys and passwords, and should be carefully and
securely stored and managed. They should **never** be stored in-repo, for example in additional or modified
`appsettings.json` files, due to the risk of disclosure. The secret manager is designed to store secrets out-of-repo
during code development and testing where other configuration providers may not be available.

