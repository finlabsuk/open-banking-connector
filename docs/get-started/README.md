# Get started

Here we will describe how to get Open Banking Connector up and running.

The main steps are:

1. [set up configuration and secrets](#set-up-configuration-and-secrets)
2. [set up bank redirect handling](#set-up-bank-redirect-handling)
3. [run the Open Banking Connector Docker image](#run-the-docker-image)

You can then use Open Banking Connector APIs to:

1. [set up a bank](../apis/bank-configuration/set-up-a-bank.md)
2. connect to the bank using APIs such as the [Account and Transaction API](../apis/account-and-transaction/README.md)

## Set up configuration and secrets

Configuration and secrets are read-only data supplied to Open Banking Connector that configure its behaviour and supply sensitive information such as transport and signing keys.

They can be supplied via environment variables, command line options, or via a supported cloud secrets provider (or any of these in combination). You may wish to decouple your configuration sources from Open Banking Connector as an application by combining static *configuration environment variables* with dynamically-generated *secrets environment variables*.

In this section, we will set up minimal configuration and secrets using environment variables. (Note that Open Banking Connector uses the .NET convention of replacing ":" with "__" when environment variables are used. So the configuration setting `OpenBankingConnector:Database:Provider` is set by the environment variable `OpenBankingConnector__Database__Provider`.)

### 1. Set up software statement, transport/signing/encryption keys

Software statements (SSAs), transport keys and certs, and signing keys are supplied (alongside related data) to Open Banking Connector by means of:

- [software statement profiles](../configuration/software-statement-profiles-settings.md)
- [transport certificate profiles](../configuration/transport-certificate-profiles-settings.md)
- [signing certificate profiles](../configuration/signing-certificate-profiles-settings.md)

Encryption keys are supplied using:

- [encryption keys](../configuration/encryption-keys-settings.md)

Below are environment variables to set up of one of each of these (the minimum necessary to create a bank registration). You will need to substitute your own values into some of these as noted in the comments. To allow this you will need to generate an SSA, a OBWAC transport key with signed cert, and an OBSeal signing key with signed cert using the [UK Open Banking Directory](https://directory.openbanking.org.uk/s/login/) and following the instructions there. You will also need to generate a 256-bit encryption key (example code is given [here](../configuration/encryption-keys-settings.md#encryption-keys-settings)).

```bash
# Software statement profile with ID "All"
OpenBankingConnector__SoftwareStatementProfiles__All__TransportCertificateProfileId=All # instruction to use transport certificate profile with ID "All"
OpenBankingConnector__SoftwareStatementProfiles__All__SoftwareStatement=a.b.c # substitute your software statement assertion (SSA)
OpenBankingConnector__SoftwareStatementProfiles__All__SigningCertificateProfileId=All # instruction to use signing certificate profile with ID "All"
OpenBankingConnector__SoftwareStatementProfiles__All__DefaultFragmentRedirectUrl=https://example.com/auth/fragment-redirect # substitute your default redirect URL (must match one in your included in your SSA)

# Transport certificate profile with ID "All"
OpenBankingConnector__TransportCertificateProfiles__All__CertificateDnWithHexDottedDecimalAttributeValues="CN=00158,2.5.4.97=#1,O=MyCompany,C=GB" # substitute your transport certificate DN
OpenBankingConnector__TransportCertificateProfiles__All__Certificate="-----BEGIN CERTIFICATE-----\nline1\nline2\n-----END CERTIFICATE-----\n" # substitute your OBWAC transport certificate
OpenBankingConnector__TransportCertificateProfiles__All__AssociatedKey="-----BEGIN PRIVATE KEY-----\nline1\nline2\n-----END PRIVATE KEY-----\n" # substitute your transport key

# Signing certificate profile with ID "All"
OpenBankingConnector__SigningCertificateProfiles__All__AssociatedKeyId=xyz # substitute your signing key ID (kid) to allow certificate lookup (obtain from UK Open Banking Directory)
OpenBankingConnector__SigningCertificateProfiles__All__AssociatedKey="-----BEGIN PRIVATE KEY-----\nline1\nline2\n-----END PRIVATE KEY-----\n" # substitute your signing key (that associated with OBSeal signing certificate)

# Encryption key with ID "MyKey"
OpenBankingConnector__Keys__CurrentEncryptionKeyId=MyKey
OpenBankingConnector__Keys__Encryption__MyKey__Value=Q95hOua2S5wXK9W5q/j0+1xIThOSbGhUl2Wano2uTc4= # substitute your base64-encoded 256-bit encryption key (see text for link to example generation code)
```

### 2. Set up database connection

Database settings are described [here](../configuration/database-settings.md).

Open Banking Connector supports PostgreSQL for production usage.

Here are environment variables to configure use of PostgreSQL including providing a connection string.

```bash
OpenBankingConnector__Database__Provider=PostgreSql # strictly unnecessary as this is the default
OpenBankingConnector__Database__EnsureDatabaseCreated=true # suggest temporarily true to allow Open Banking Connector to create database on first use
OpenBankingConnector__Database__ConnectionStrings__PostgreSql="Host=localhost;Database=test;Username=postgres" # substitute your connection string without password
OpenBankingConnector__Database__PasswordSettingNames__PostgreSql=OpenBankingConnector:Custom:PostgreSqlPassword # substitute name of environment variable providing database password
OpenBankingConnector__Custom__PostgreSqlPassword=placeholder # substitute name/value with database password environment variable
```

### 3. Provide bank profile information

Almost all bank profile information is supplied via source code.

However, some key information such as org IDs and URLs is only available to those registered with the UK Open Banking Directory. This information needs to be provided via configuration to fill gaps in the bank profile definitions.

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
OpenBankingConnector__BankProfiles__Hsbc__Default__AccountAndTransactionApi__ApiVersion=VersionZZ # substitute value

# Bank group: Barclays
OpenBankingConnector__BankProfiles__Barclays__Default__FinancialId=yy # substitute value
```

## Set up bank redirect handling

For production, you will also need to setup a redirect endpoint (e.g. simple web page) to handle the bank redirects that follow end-user auth. Your redirect endpoint needs to match one of the redirect URLs included in in your SSA and you will probably want to customise the branding and message shown.

Open Banking Connector is designed as an internal API and its endpoints should be secured (see [here](../apis/README.md#security)) and definitely not exposed to the internet in any way. This is *very important* as endpoints provide access to Open Banking APIs.

To give an idea of what is required, Open Banking Connector does provide an example redirect endpoint at `/auth/fragment-redirect` which is a simple web page with "Open Banking Connector" branding.

## Run the Docker image

Open Banking Connector Docker images (including the latest) are available [here](https://github.com/finlabsuk/open-banking-connector/pkgs/container/open-banking-connector-web-app). The Docker images are produced from the Dockerfile in the source code repo. Git tags are used so you can see the exact source code used to create each Docker image.

For test purposes, you can pull and run a desired Docker image using a command such as:
```bash
# Substitute env file location and image version in this command
docker run -dt --env-file "<path>/docker.env" -p 50000:80 --name "open_banking_connector" ghcr.io/finlabsuk/open-banking-connector-web-app:x.y.z
```
where an *env file* is used to supply environment variables.
