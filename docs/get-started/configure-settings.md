# Configure Open Banking Connector settings
Settings are used to configure Open Bankinng Connector alongside secrets which provide sensitive configuration data.

They are organised into groups defined by C# classes. They can be provided in various forms depending on the type of app using Open Banking Connector.

Two groups of settings are currently defined:

Group | C# class | Description
--- | --- | ---
Open Banking Connector | [OpenBankingConnectorSettings](.././../src/OpenBanking.Library.Connector/Models/Configuration/OpenBankingConnectorSettings.cs#L54) | settings related to Core Open Banking functionality including the database connection string
Bank Profile Settings | [BankProfilesSettings](.././../src/OpenBanking.Library.Connector/Models/Configuration/BankProfilesSettings.cs#L30)| settings related to Bank Profiles which is an optional feature of Open Bnaking Connector providing configuration for UK bank sandboxes

## Available Settings

* Group (C# class): Open Banking Connector

 Setting | Example value | Description
 --- | --- | ---
 "SqliteDb<br/>ConnectionString": | "Data Source=../../../../OpenBanking.Library.<br/>Connector/sqliteTestDb.db" |This string specifies the type of connection string being used. <br/>The path to the .db file will be specified.
 "EnsureDbCreated": | "true"|This settings identifies whether the database has been created.

* Group (C# class): Bank Profile Settings

Setting | Example value | Description
 --- | --- | ---
 "DataDirectory":{ | "Windows":|Specified different OS types. 
 "Windows": | "C:/Repos/open-banking-connector-csharp-data/bankProfileData"|Displays the specific path to the data directory for each OS.


## Configure settings for a "Plain" app (app without .NET Generic Host)
* user has freedom to create settings in any way and use them when creating the Open Banking Connector request object.

* TODO: Need to set up setting providers for each settings group. (Bank Profile and Open Banking Connector)

## Configure settings for .NET Generic Host or ASP.NET Core app

Microsoft ASP.NET Core is configured using various sources. One configuration method is the use of settings files such as appsettings.json.
 
Please see [Microsoft link](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0) for more information.

The two setting groups can be used to create setting sections in appsettings.json or the settings can be set individually through the use of environment variables.

For example:

You could add the following to appsettings.json to configure the BankProfileSettings settings group.

```json
"BankProfiles": {
  "DataDirectory": {
    "Windows":"C:/Repos/open-banking-connector-csharp-data/bankProfileData",
    "MacOs": "~/Repos/open-banking-connector-csharp-data/bankProfileData",
    "Linux": "~/Repos/open-banking-connector-csharp-data/bankProfileData"
  }
}
```
Refer to [Microsoft link](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0) for more examples of how the C# code maps to the AppSettings file.
