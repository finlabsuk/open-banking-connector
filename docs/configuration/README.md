# Configuration

Configuration for the Open Banking Connetor web app is read-only information that customises the behaviour of the app. This is in contrast to database access which is read-write.

This page describes configuration used by the app and how to provide it.

Configuration is normally provided by either a key secret vault (including the local [Microsoft secret manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows)) and/or environment variables.

## Structure

The Open Banking connector web app makes available a number of [settings](#settings-groups) to support app configuration.

Configuration then consists of *a set of key-value pairs which give values to available settings*.

Settings (and hence keys) have a hierarchical naming structure where the first "level" is always OpenBankingConnector and subsequent levels are separated by colons (:).

Values are always representable as strings (although in JSON files, where applicable and optionally, they may be represented in natural form as numbers and booleans).

Here is an example of a key-value pair that configures the Open Bnaking Connector database provider to be `"Sqlite"`:

Key (Setting Name) | Value
--- | ---
OpenBankingConnector:Database:Provider | `"Sqlite"`

## Sources

The app as a baseline uses the ASP.NET Core [Default Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#default-configuration) to collect configuration from a number of configuration sources in priority order.

In practice, this means default values for settings are provided by code defaults (see [ISettings](../../src/OpenBanking.Library.Connector/Configuration/ISettings.cs#L16) classes) and the two appsettings.json files for the app ([appsettings.json](../../src/OpenBanking.WebApp.Connector/appsettings.json) and [appsettings.Development.json](../../src/OpenBanking.WebApp.Connector/appsettings.Development.json)) included in the public repo.

Customisation (user configuration) is then provided either by a key secret vault and/or environment variables.

When building and running the web app from source code (rather than using a container) in the [development environment](#environment-selection), the local [Microsoft secret manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) may be used in place of a cloud-based key secret vault.

**Important**: Some settings values are very sensitive, for example keys, and should be very carefully and securely stored and managed. These should **never** be stored in-repo, for example in additional or modified appsettings.json files, due to the risk of disclosure. The Microsoft secret manager allows sensitive secrets to be stored far removed from the local repo during code development and testing.

## Environment selection

The Open Banking Connector web app allows use of different [environments](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0) such as development, staging and production.

In practice, app behaviour is only actually sensitive to whether it is run in the development environment. Other environments can be used as desired by the user but they should not affect app behaviour.

 In the development environment, logging, error handling and configuration defaults etc are modified to suit the needs of developing and testing the app. The development environment should not be used in production.

 Environment selection is not configured by Open Banking Connector web app settings but normally via the Microsoft DOTNET_ENVIRONMENT or ASPNETCORE_ENVIRONMENT environment variables. Please see [below](#using-environment-variables) for an example of how to set environemnt variables when running the app. The default environment is production (not development) in most situations including when running the web app container (see [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0) for more details). 

## Settings groups

Open Banking Connector web app configuration settings are collected into groups which are described in their own pages:

- [database settings](database-settings.md) which configure the database used by the app
- [software statement profiles settings](software-statement-profiles-settings.md) which configure software statement profiles used by the app
- [transport certificate profiles settings](transport-certificate-profiles-settings.md) which configure transport certificate profiles used by the app
- [signing certificate profiles settings](signing-certificate-profiles-settings.md) which configure signing certificate profiles used by the app

## Minimal required configuration

In the [development environment](#environment-selection), at least the following should be configured to allow a bank registration to be performed:

- a [software statement profile](software-statement-profiles-settings.md) with an associated [transport certificate profile](transport-certificate-profiles-settings.md) and [signing certificate profile](signing-certificate-profiles-settings.md).

When not in the development environment, additionally at least the following should be configured to allow a bank registration to be performed:

- a [database provider and connection string](database-settings.md)

##  Provide configuration

We here provide some guidance and examples of how to provide configuration to the Open Banking Connector web app from various sources.

### Using the Microsoft secret manager

To use the Microsoft secret manager, you will need a secrets.json file in the appropriate directory (if you do not have one already). The Microsoft documentation [here](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#how-the-secret-manager-tool-works) gives the path for this directory. Please note that the UserSecretsId for the app is `aa921213-9461-4f9e-8fec-153624ec67ad` as given [here](../../src/OpenBanking.WebApp.Connector/OpenBanking.WebApp.Connector.csproj).

You can then add key-value pairs to the secrets.json file to supply settings to the app.

Here is an example of what such a file might look like after configuring a software statement profile, transport certificate profile and signing certificate profile:

```json
{
  "OpenBankingConnector:TransportCertificateProfiles:New2:CertificateDnWithStringDottedDecimalAttributeValues": "CN=abc,2.5.4.97=abc,O=abc,C=GB",
  "OpenBankingConnector:TransportCertificateProfiles:New2:CertificateDnWithHexDottedDecimalAttributeValues": "CN=abc,2.5.4.97=#123,O=abc,C=GB",
  "OpenBankingConnector:TransportCertificateProfiles:New2:Certificate": "-----BEGIN CERTIFICATE-----\nabc\n-----END CERTIFICATE-----\n",
  "OpenBankingConnector:TransportCertificateProfiles:New2:AssociatedKey": "-----BEGIN PRIVATE KEY-----\nabc\n-----END PRIVATE KEY-----\n",
  "OpenBankingConnector:SoftwareStatementProfiles:All:TransportCertificateProfileId": "New2",
  "OpenBankingConnector:SoftwareStatementProfiles:All:SoftwareStatement": "a.b.c",
  "OpenBankingConnector:SoftwareStatementProfiles:All:SigningCertificateProfileId": "New2",
  "OpenBankingConnector:SoftwareStatementProfiles:All:DefaultFragmentRedirectUrl": "https://example.com/auth/fragment-redirect",
  "OpenBankingConnector:SigningCertificateProfiles:New2:Certificate": "-----BEGIN CERTIFICATE-----\nabc\n-----END CERTIFICATE-----\n",
  "OpenBankingConnector:SigningCertificateProfiles:New2:AssociatedKeyId": "abc",
  "OpenBankingConnector:SigningCertificateProfiles:New2:AssociatedKey": "-----BEGIN PRIVATE KEY-----\nabc\n-----END PRIVATE KEY-----\n"
}
```

### Using Environment Variables

When providing settings via environment variables, it is recommended to use double underscores (__) in place of colons (:) in the keys. So, for instance, the key OpenBankingConnector:Database:Provider would become OpenBankingConnector__Database__Provider. The reasoning for this is provided [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#environment-variables).

Here is an example of how to run the web app container using envirnoment variables to (a) use the development environment and (b) set the DB provider to `"Sqlite"`:

```powershell
docker run -dt -e "DOTNET_ENVIRONMENT=Development" -e "OpenBankingConnector__Database__Provider=Sqlite" -P --name OpenBanking.WebApp.Connector.v1.0.0-alpha03 ghcr.io/finlabsuk/open-banking-connector-web-app:1.0.0-alpha03
```