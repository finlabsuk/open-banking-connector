# Payment Initiation API

The Payment Initiation API provides access to UK Open Banking Payment Initiation API functionality.

## OpenAPI Spec

An OpenAPI spec for the Payment Initiation API showing the endpoints and data types is provided [here](./openapi.md). This is a static capture of the API at a given release (see grey box at top right of OpenAPI spec for which release).

For any running version of Open Banking Connector, the OpenAPI spec for the currently-running software is provided at `GET /swagger/index.html`.

## Relationship to UK Open Banking Payment Initiation API

The UK Open Banking Payment Initiation (PISP) API is defined [here](https://openbankinguk.github.io/read-write-api-site3/v3.1.10/profiles/payment-initiation-api-profile.html).

Open Banking Connector provides replacement endpoints for the UK OB PISP endpoints as shown in the following table. The replacement endpoints, where relevant, use data types from UK OB PISP API version v3.1.6. When calling external (bank) APIs, Open Banking Connector performs request/response payload translation for any banks supporting earlier API versions.

| UK OB PISP endpoint                        | Replacement Open Banking Connector endpoint                    |
|--------------------------------------------|----------------------------------------------------------------|
| POST /domestic-payment-consents            | POST /pisp/domestic-payment-consents                           |
| GET /domestic-payment-consents/{ConsentId} | GET /pisp/domestic-payment-consents/{domesticPaymentConsentId} |
| POST /domestic-payments                    | POST /pisp/domestic-payments                                   |
| GET /domestic-payments/{DomesticPaymentId} | GET /pisp/domestic-payments/{externalApiId}                    |

All replacement Open Banking Connector endpoints handle behaviour and spec variations between banks meaning the same API calls can be used regardless of bank. Bank-specific differences are determined by a bank's *bank profile*.

In the UK OB PISP API, some resource endpoints such as `POST /domestic-payments` require a consent-specific bearer token supplied in the `Authorization` header. Corresponding Open Banking Connector endpoints instead require the `x-obc-domestic-payment-consent-id` header to be populated with the relevant *domestic payment consent* ID (`Id` returned from [POST /pisp/domestic-payment-consents](./openapi.md)).

The Open Banking Connector API provides two additional endpoints that are not UK OB replacements: [POST /pisp/domestic-payment-consent-auth-contexts](./openapi.md) and [GET /pisp/domestic-payment-consent-auth-contexts/{domesticPaymentConsentAuthContextId}](./openapi.md). These respectively create and read an *auth context* which is a time-limited session for end-user authentication. Calling [POST /pisp/domestic-payment-consent-auth-contexts](./openapi.md) creates an auth context and returns a URL which can be used for end-user auth.

## Mappings to database objects

The following endpoints create/read records in the domestic_payment_consent database table:

- [POST /pisp/domestic-payment-consents](./openapi.md)
- [GET /pisp/domestic-payment-consents/{domesticPaymentConsentId}](./openapi.md)

The following endpoints create/read records of kind `DomesticPaymentConsentAuthContext` in the auth_context table (these records are deleted automatically following successful auth):

- [POST /pisp/domestic-payment-consent-auth-contexts](./openapi.md)
- [GET /pisp/domestic-payment-consent-auth-contexts/{domesticPaymentConsentAuthContextId}](./openapi.md)

## External (bank) API operations

All replacement Open Banking Connector endpoints included in the [table above](#relationship-to-uk-open-banking-payment-initiation-api) perform a call to the relevant external (bank) API endpoint listed. Additionally, calls are made to the external API token endpoint as required.
