# Set up configuration and secrets

Configuration and secrets are read-only settings supplied to Open Banking Connector that configure its behaviour and supply sensitive information such as transport and signing keys.

They can be supplied via environment variables, command line options, or via a supported cloud secrets provider (or any of these in combination).

Using environment variables is an attractive option in production because it completely decouples your configuration/secrets from the application. Further, it is often desirable to separate *configuration* (less sensitive settings) and *secrets* (more sensitive settings). To achieve this, you could use static environment variables for configuration and for secrets use environment variables dynamically obtained from a secrets vault.

In this section, we show how to set up minimal configuration and secrets using environment variables. Note that Open Banking Connector uses the .NET convention of replacing ":" with "__" when environment variables are used. So the configuration setting `OpenBankingConnector:Database:Provider` is set by the environment variable `OpenBankingConnector__Database__Provider`.

### 1. Configure software statement assertions, transport/signing/encryption keys

Software statement assertions (SSAs), transport keys and certs, and signing keys are supplied (alongside related data) to Open Banking Connector by means of:

- [software statement profiles](../../configuration/software-statement-profiles-settings.md)
- [transport certificate profiles](../../configuration/transport-certificate-profiles-settings.md)
- [signing certificate profiles](../../configuration/signing-certificate-profiles-settings.md)

Encryption keys are supplied by means of:

- [encryption keys](../../configuration/encryption-keys-settings.md)

Below are environment variables to set up of one of each of these (the minimum necessary to create a bank registration). You will need to substitute your own values into some of these as noted in the comments. To allow this you will need to create an SSA, a signed OBWAC transport cert and key, and a signed OBSeal signing cert and key using the [UK Open Banking Directory](https://directory.openbanking.org.uk/s/login/) and following the instructions there. You will also need to generate a 256-bit encryption key (example code is given [here](../../configuration/encryption-keys-settings.md#encryption-keys-settings)).

```bash
# Software statement profile with ID "All"
OpenBankingConnector__SoftwareStatementProfiles__All__TransportCertificateProfileId=All # instruction to use transport certificate profile with ID "All"
OpenBankingConnector__SoftwareStatementProfiles__All__SoftwareStatement=a.b.c # substitute your software statement assertion (SSA)
OpenBankingConnector__SoftwareStatementProfiles__All__SigningCertificateProfileId=All # instruction to use signing certificate profile with ID "All"
OpenBankingConnector__SoftwareStatementProfiles__All__DefaultFragmentRedirectUrl=https://example.com/auth/fragment-redirect # substitute your default redirect URL (must match one in your included in your SSA)

# Transport certificate profile with ID "All"
OpenBankingConnector__TransportCertificateProfiles__All__Certificate="-----BEGIN CERTIFICATE-----\nline1\nline2\n-----END CERTIFICATE-----\n" # substitute your OBWAC transport certificate
OpenBankingConnector__TransportCertificateProfiles__All__AssociatedKey="-----BEGIN PRIVATE KEY-----\nline1\nline2\n-----END PRIVATE KEY-----\n" # substitute the key associated with your OBWAC transport certificate

# Signing certificate profile with ID "All"
OpenBankingConnector__SigningCertificateProfiles__All__AssociatedKeyId=xyz # substitute the key ID (kid) associated with your OBSeal signing certificate to allow certificate lookup (obtain the key ID from UK Open Banking Directory)
OpenBankingConnector__SigningCertificateProfiles__All__AssociatedKey="-----BEGIN PRIVATE KEY-----\nline1\nline2\n-----END PRIVATE KEY-----\n" # substitute the key associated with your OBSeal signing certificate

# Encryption key with ID "MyKey"
OpenBankingConnector__Keys__CurrentEncryptionKeyId=MyKey
OpenBankingConnector__Keys__Encryption__MyKey__Value=Q95hOua2S5wXK9W5q/j0+1xIThOSbGhUl2Wano2uTc4= # substitute your base64-encoded 256-bit encryption key (see text for link to example generation code)
```

### 2. Configure database connection

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

### 3. Configure non-public bank profile information

Almost all bank profile information is supplied via source code.

However, some key information such as org IDs and URLs is only available to those registered with the UK Open Banking Directory. This information needs to be provided via configuration to complete the bank profile definitions.

When using a bank profile for the first time, if any required information from configuration is not available, Open Banking Connector will provide an error with details.

For reference, we list here currently required configuration for various bank groups. *Important: You only need to provide information that relates to bank profiles you want to use.*

```bash
# Bank group: NatWest (only affects sandbox bank profiles)
OpenBankingConnector__BankProfiles__NatWest__RoyalBankOfScotlandSandbox__PaymentInitiationApi__BaseUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__RoyalBankOfScotlandSandbox__IssuerUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__RoyalBankOfScotlandSandbox__AccountAndTransactionApi__BaseUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__NatWestSandbox__PaymentInitiationApi__BaseUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__NatWestSandbox__IssuerUrl=xx # substitute value
OpenBankingConnector__BankProfiles__NatWest__NatWestSandbox__AccountAndTransactionApi__BaseUrl=xx # substitute value

# Bank group: Monzo (only affects sandbox bank profiles)
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