# Set up basic configuration

Configuration settings are read-only settings supplied to Open Banking Connector that configure its behaviour.

They are generally supplied via environment variables but can also be supplied using command line options.

In this section, we show how to set up minimal configuration required to run Open Banking Connector.

Note that Open Banking Connector uses the .NET convention of replacing ":" with "__" when environment variables are used. So the configuration setting `OpenBankingConnector:Database:Provider` is set by the environment variable `OpenBankingConnector__Database__Provider`.

### 1. Configure database connection

Database settings are described [here](../../configuration/database-settings.md).

Open Banking Connector supports PostgreSQL for production usage.

Here are environment variables to configure use of PostgreSQL including specification of a connection string.

```bash
OpenBankingConnector__Database__Provider=PostgreSql # strictly unnecessary as this is the default
OpenBankingConnector__Database__EnsureDatabaseCreated=true # suggest temporarily true to allow Open Banking Connector to create database on first use
OpenBankingConnector__Database__ConnectionStrings__PostgreSql="Host=localhost;Database=test;Username=postgres" # substitute your connection string without password
OpenBankingConnector__Database__PasswordSettingNames__PostgreSql=OpenBankingConnector:Custom:PostgreSqlPassword # substitute name of environment variable providing database password
OpenBankingConnector__Custom__PostgreSqlPassword=placeholder # example arbitrary environment variable that supplies database password
```

### 2. Configure an encryption key

Encryption keys are supplied by means of:

- [encryption keys](../../configuration/encryption-keys-settings.md)

Below are the environment variables necessary to configure a 256-bit encryption key which can be generated using example code [here](../../configuration/encryption-keys-settings.md#encryption-keys-settings)).

```bash
# Encryption key with ID "MyKey"
OpenBankingConnector__Keys__CurrentEncryptionKeyId=MyKey
OpenBankingConnector__Keys__Encryption__MyKey__Value=Q95hOua2S5wXK9W5q/j0+1xIThOSbGhUl2Wano2uTc4= # substitute your base64-encoded 256-bit encryption key (see text for link to example generation code)
```

### 3. Configure non-public bank profile information

Almost all bank profile information is supplied via source code.

However sometimes key information such as org IDs and URLs cannot be obtained from public sources and is only available to those registered with the UK Open Banking Directory or from bank dev portals. This information needs to be provided via configuration to complete the bank profile definitions.

*Important: It is only necessary to provide information that relates to bank profiles you want to use.* When using a bank profile for the first time, if any required information from configuration is not available, Open Banking Connector will provide an error with details.

For reference, we list here currently required configuration for supported bank groups.

Please contact us if you need any assistance populating these configuration values.

```bash
# Bank group: NatWest (only required if using sandbox bank profiles)
OpenBankingConnector__BankProfiles__NatWest__RoyalBankOfScotlandSandbox__PaymentInitiationApi__BaseUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__RoyalBankOfScotlandSandbox__IssuerUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__RoyalBankOfScotlandSandbox__AccountAndTransactionApi__BaseUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__NatWestSandbox__PaymentInitiationApi__BaseUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__NatWestSandbox__IssuerUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__NatWestSandbox__AccountAndTransactionApi__BaseUrl=xx # substitute value

# Bank group: Monzo (only required if using sandbox bank profiles)
OpenBankingConnector__BankProfiles__Monzo__Monzo__PaymentInitiationApi__ApiVersion=VersionZZ # substitute value
OpenBankingConnector__BankProfiles__Monzo__Monzo__FinancialId=yy # substitute value

# Bank group: Lloyds
OpenBankingConnector__BankProfiles__Lloyds__MbnaPersonal__FinancialId=yy # substitute value
OpenBankingConnector__BankProfiles__Lloyds__LloydsPersonal__FinancialId=yy # substitute value
OpenBankingConnector__BankProfiles__Lloyds__BankOfScotlandPersonal__FinancialId=yy # substitute value

# Bank group: HSBC
OpenBankingConnector__BankProfiles__Hsbc__Default__FinancialId=yy # substitute value

# Bank group: Barclays
OpenBankingConnector__BankProfiles__Barclays__Default__FinancialId=yy # substitute value
```