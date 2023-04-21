# Account and Transaction API

The Account and Transaction API provides access to UK Open Banking Account and Transaction API functionality.

## OpenAPI Spec

An OpenAPI spec for the Account and Transaction API showing the endpoints and data types is provided [here](./openapi.md). This is a static capture of the API at a given release (see grey box at top right of OpenAPI spec for which release).

For any running version of Open Banking Connector, the OpenAPI spec for the currently-running software is provided at `GET /swagger/index.html`.

## Relationship to UK Open Banking Account and Transaction API

The UK Open Banking Account and Transaction (AISP) API is defined [here](https://openbankinguk.github.io/read-write-api-site3/v3.1.10/profiles/account-and-transaction-api-profile.html).

Open Banking Connector provides replacement endpoints for the UK OB AISP endpoints as shown in the following table. The replacement endpoints, where relevant, use data types from UK OB AISP API version v3.1.10. When calling external (bank) APIs, Open Banking Connector performs request/response payload translation for any banks supporting earlier API versions.

| UK OB AISP endpoint                                             | Replacement Open Banking Connector endpoint                                                |
|-----------------------------------------------------------------|--------------------------------------------------------------------------------------------|
| POST /account-access-consents                                   | POST /aisp/account-access-consents                                                         |
| GET /account-access-consents/{ConsentId}                        | GET /aisp/account-access-consents/{accountAccessConsentId}                                 |
| DELETE /account-access-consents/{ConsentId}                     | DELETE /aisp/account-access-consents/{accountAccessConsentId}                              |
| GET /accounts                                                   | GET /aisp/accounts                                                                         |
| GET /accounts/{AccountId}                                       | GET /aisp/accounts/{externalApiAccountId}                                                  |
| GET /balances                                                   | GET /aisp/balances                                                                         |
| GET /accounts/{AccountId}/balances                              | GET /aisp/accounts/{externalApiAccountId}/balances                                         |
| GET /direct-debits                                              | GET /aisp/direct-debits                                                                    |
| GET /accounts/{AccountId}/direct-debits                         | GET /aisp/accounts/{externalApiAccountId}/direct-debits                                    |
| GET /party                                                      | GET /aisp/party                                                                            |
| GET /accounts/{AccountId}/party                                 | GET /aisp/accounts/{externalApiAccountId}/party                                            |
| GET /accounts/{AccountId}/parties                               | GET /aisp/accounts/{externalApiAccountId}/parties                                          |
| GET /standing-orders                                            | GET /aisp/standing-orders                                                                  |
| GET /accounts/{AccountId}/standing-orders                       | GET /aisp/accounts/{externalApiAccountId}/standing-orders                                  |
| GET /transactions                                               | GET /aisp/transactions                                                                     |
| GET /accounts/{AccountId}/transactions                          | GET /aisp/accounts/{externalApiAccountId}/transactions                                     |
| GET /accounts/{AccountId}/statements/{StatementId}/transactions | GET /aisp/accounts/{externalApiAccountId}/statements/{externalApiStatementId}/transactions |

All replacement Open Banking Connector endpoints handle behaviour and spec variations between banks meaning the same API calls can be used regardless of bank. Bank-specific differences are determined by a bank's *bank profile*.

In the UK OB AISP API, resource endpoints such as `GET /accounts` require a consent-specific bearer token supplied in the `Authorization` header. Corresponding Open Banking Connector endpoints instead require the `x-obc-account-access-consent-id` header to be populated with the relevant *account access consent* ID (`Id` returned from [POST /aisp/account-access-consents](./openapi.md)).

The Open Banking Connector API provides two additional endpoints that are not UK OB replacements: [POST /aisp/account-access-consent-auth-contexts](./openapi.md) and [GET /aisp/account-access-consent-auth-contexts/{accountAccessConsentAuthContextId}](./openapi.md). These respectively create and read an *auth context* which is a time-limited session for end-user authentication. Calling [POST /aisp/account-access-consent-auth-contexts](./openapi.md) creates an auth context and returns a URL which can be used for end-user auth.

## Mappings to database objects

The following endpoints create/read/delete records in the account_access_consent database table:

- [POST /aisp/account-access-consents](./openapi.md)
- [GET/aisp/account-access-consents/{accountAccessConsentId}](./openapi.md)
- [DELETE /aisp/account-access-consents/{accountAccessConsentId}](./openapi.md)

The following endpoints create/read records of kind `AccountAccessConsentAuthContext` in the auth_context table (these records are deleted automatically following successful auth):

- [POST /aisp/account-access-consent-auth-contexts](./openapi.md)
- [GET /aisp/account-access-consent-auth-contexts/{accountAccessConsentAuthContextId}](./openapi.md)

## External (bank) API operations

All replacement Open Banking Connector endpoints included in the [table above](#relationship-to-uk-open-banking-account-and-transaction-api) perform a call to the relevant external (bank) API endpoint listed. Additionally, calls are made to the external API token endpoint as required.

In the case of [DELETE /aisp/account-access-consents/{accountAccessConsentId}](./openapi.md), the header `x-obc-include-external-api-operation` can be used to avoid deleting an account access consent at the external API (the default behaviour). When `x-obc-include-external-api-operation` is `false`, only the local database object will be deleted. 
