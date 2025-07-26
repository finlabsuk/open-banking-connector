# Set up basic configuration

Configuration in Open Banking Connector is described in detail [here](../../configuration/README.md).

To run and get started with Open Banking Connector, some [basic configuration settings](#basic-configuration-settings)
are required including database settings.

Configuration settings are generally specified via environment variables but can also be specified using command line
options. Here we use environment variables.

The simplest way to supply environment variables when running the Open Banking Connector container locally is to place
them in a `.env` file. This file can then be supplied as argument `--env-file` to `docker run`. For production, you
may have a different way to supply environment variables to a container.

Note that Open Banking Connector uses the .NET convention of replacing ":" with "__" when environment variables are
used. So the configuration setting `OpenBankingConnector:Database:Provider` is set by the environment variable
`OpenBankingConnector__Database__Provider`.

## Basic configuration settings

### Database settings

Database settings are documented [here](../../configuration/database-settings.md).

Open Banking Connector requires connection to a PostgreSQL database.

The following environment variables can be used to configure use of a PostgreSQL database:

```bash
# Database settings
OpenBankingConnector__Database__Provider=PostgreSql # currently unnecessary as this is the default but recommended for future-proofing
OpenBankingConnector__Database__EnsureDatabaseCreated=true # suggest initially true to allow Open Banking Connector to create database on first use, then later false
OpenBankingConnector__Database__ConnectionStrings__PostgreSql="Host=localhost;Database=test;Username=postgres" # substitute your connection string without password
OpenBankingConnector__Database__PasswordSettingNames__PostgreSql=OpenBankingConnector:Custom:PostgreSqlPassword # substitute name of environment variable providing database password
OpenBankingConnector__Custom__PostgreSqlPassword=placeholder # example arbitrary environment variable that supplies database password
```

### Bank profile settings

Almost all bank profile information is supplied via source code.

However, sometimes important information such as org IDs and URLs cannot be obtained from public sources and is only
available to those registered with the UK Open Banking Directory or from bank dev portals. In such cases, extra
information needs to be provided to Open Banking Connector via configuration to complete the bank profile definitions.

*Important: It is only necessary to provide information that relates to bank profiles you want to use.* When using a
bank profile for the first time, if any required information from configuration is not available, Open Banking Connector
will provide an error with details.

For reference, we list here configuration required to support all
current [bank integrations](../../bank-integrations.md).

Please contact us if you need any assistance populating these configuration values.

```bash
# Bank profile settings

# Bank group: Barclays
OpenBankingConnector__BankProfiles__Barclays__Default__FinancialId=xyz # substitute value

# Bank group: Co-operative
OpenBankingConnector__BankProfiles__Cooperative__Default__FinancialId=xyz # substitute value

# Bank group: HSBC
OpenBankingConnector__BankProfiles__Hsbc__Default__FinancialId=xyz # substitute value

# Bank group: Lloyds
OpenBankingConnector__BankProfiles__Lloyds__MbnaPersonal__FinancialId=xyz # substitute value
OpenBankingConnector__BankProfiles__Lloyds__LloydsPersonal__FinancialId=xyz # substitute value
OpenBankingConnector__BankProfiles__Lloyds__BankOfScotlandPersonal__FinancialId=xyz # substitute value

# Bank group: Monzo
OpenBankingConnector__BankProfiles__Monzo__Monzo__FinancialId=xyz # substitute value

# Bank group: Santander
OpenBankingConnector__BankProfiles__Santander__Default__FinancialId=xyz # substitute value

# Bank group: Starling
OpenBankingConnector__BankProfiles__Starling__Default__FinancialId=xyz # substitute value

# Sandbox settings (only required if using sandbox bank profiles)
OpenBankingConnector__BankProfiles__Lloyds__Sandbox__IssuerUrl=xyz # substitute value
OpenBankingConnector__BankProfiles__Lloyds__Sandbox__PaymentInitiationApi__BaseUrl=xyz # substitute value
OpenBankingConnector__BankProfiles__NatWest__RoyalBankOfScotlandSandbox__PaymentInitiationApi__BaseUrl=xyz # substitute value
OpenBankingConnector__BankProfiles__NatWest__RoyalBankOfScotlandSandbox__IssuerUrl=xyz # substitute value
OpenBankingConnector__BankProfiles__NatWest__RoyalBankOfScotlandSandbox__AccountAndTransactionApi__BaseUrl=xyz # substitute value
OpenBankingConnector__BankProfiles__NatWest__NatWestSandbox__PaymentInitiationApi__BaseUrl=xyz # substitute value
OpenBankingConnector__BankProfiles__NatWest__NatWestSandbox__IssuerUrl=xyz # substitute value
OpenBankingConnector__BankProfiles__NatWest__NatWestSandbox__AccountAndTransactionApi__BaseUrl=xyz # substitute value
```