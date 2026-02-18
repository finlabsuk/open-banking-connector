# Configuration & secrets

Configuration and secrets are read-only settings that customise the behaviour of Open Banking Connector. This is in
contrast to database access which is read-write.

Secrets provide sensitive configuration but, from Open Banking Connector's perspective, *configuration and secrets are
unified* in that any setting can be assigned a value by any configuration provider. (This does not mean the two should
be stored together or supplied from the same source though - see [secrets and security](#secrets-and-security) below).

Here we give an overview of configuration in Open Banking Connector.

## Structure

Configuration consists of *settings* which can be assigned values.

These settings are organised by area into [settings groups](#settings-groups).

Settings have a hierarchical naming structure where the first "level" is always OpenBankingConnector (essentially a
namespace) and subsequent levels are separated by colons ( : ).

Values are always representable as strings (although in JSON files, where applicable and optionally, they may be
represented in natural form as numbers and booleans where applicable).

Below is an example of a setting and value that configures the Open Banking Connector database setting
*EnsureDatabaseCreated* to be
`"true"`:

| Setting                                             | Value    |
|-----------------------------------------------------|----------|
| OpenBankingConnector:Database:EnsureDatabaseCreated | `"true"` |

## Settings groups

Open Banking Connector configuration settings are collected into groups which are described on their own pages:

- [database settings](./database-settings.md) configure the database used by Open Banking Connector
- [open telemetry settings](./open-telemetry-settings.md) configure Open Telemetry settings used by Open Banking
  Connector

## Sources

Open Banking Connector is written in .NET and by default uses ASP.NET
Core [default application configuration sources](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-9.0#default-application-configuration-sources)
to collect configuration from a number of configuration providers in priority order.

These include the command-line and environment variable providers as highest-priority providers.

## Secrets and security

Some settings values are very sensitive, for example keys, and should be carefully and securely stored and managed.

It is recommended to provide sensitive configuration (secrets) and standard configuration from different sources and not
to store them together.

For example, standard configuration could be supplied via environment variables and secrets from a cloud secrets
provider.

Even if environment variables are the vehicle for providing both (to decouple configuration and application), the
secrets environment variables could e.g. be dynamically produced during application/pod startup and not stored with
standard configuration.

## Environment Variables

When supplying configuration via environment variables, you should use double underscores ( __ ) in place of colons ( : ) in
settings names. So, for instance, the setting *OpenBankingConnector:Database:Provider* would be configured using
environment variable `OpenBankingConnector__Database__Provider`. This is a .NET convention and
explained [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#environment-variables).

