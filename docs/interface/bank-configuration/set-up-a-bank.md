# Set up a bank

To set up a bank, you will need to create a bank registration (OAuth2 client) which establishes a relationship with the bank.

To do this, first determine the *bank profile* which is used to identify the bank in Open Banking Connector. Bank profiles are listed in the [bank integration matrix](../../bank-integration-matrix.md).

Each bank registration requires a [software statement profile](../../configuration/software-statement-profiles-settings.md) (includes an *SSA* which identifies the third-party provider (TPP) to the bank) and so it is necessary for one of these to be set up before attempting to create a bank registration.

To create the registration, call [POST /config/bank-registrations](./openapi.md). This will either create a new OAuth2 client at the bank or re-use an existing one as appropriate. The following call can be used:
```http
POST /config/bank-registrations HTTP/1.1
# standard headers omitted
{
  "BankProfile": "MyBankProfileName",
  "SoftwareStatementProfileId": "MySoftwareStatementProfileId"
}
```

Once a bank registration has been set up, you can then use functional APIs such as the Account and Transaction API to create an account access consent etc.