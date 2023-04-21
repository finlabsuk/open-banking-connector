# Bank Configuration API

The Bank Configuration API allows you to create, read, and delete configuration (setup) objects for a bank in Open Banking Connector.

To set up a bank, see instructions [here](./set-up-a-bank.md). 

## OpenAPI Spec

An OpenAPI spec for the Bank Configuration API showing the endpoints and data types is provided [here](./openapi.md). This is a static capture of the API at a given release (see grey box at top right of OpenAPI spec for which release).

For any running version of Open Banking Connector, the OpenAPI spec for the currently-running software is provided at `GET /swagger/index.html`.

## Relationship to UK Open Banking Dynamic Client Registration API

The UK Open Banking Dynamic Client Registration (DCR) API is defined [here](https://openbankinguk.github.io/dcr-docs-pub/).

Open Banking Connector provides replacement endpoints for the UK OB DCR endpoints as shown below. The replacement endpoints, where relevant, use data types from the latest UK OB DCR v3.3 API. When calling external (bank) APIs, Open Banking Connector performs request/response payload translation for any banks supporting earlier API versions.

| UK OB DCR endpoint          | Replacement Open Banking Connector endpoint            |
|-----------------------------|--------------------------------------------------------|
| POST /register              | POST /config/bank-registrations                        |
| GET /register/{ClientId}    | GET /config/bank-registrations/{bankRegistrationId}    |
| DELETE /register/{ClientId} | DELETE /config/bank-registrations/{bankRegistrationId} |

All the replacement Open Banking Connector endpoints handle behaviour and spec variations between banks meaning the same API calls can be used regardless of bank. Bank-specific differences are determined by a bank's *bank profile*.

There are some further major differences when creating a bank registration/OAuth2 client using [POST /config/bank-registrations](./openapi.md).

Firstly, Open Banking Connector can infer most of the payload to send to the bank during DCR from only a few request properties. This means the creation of a new BankRegistration (OAuth2 client) is usually as simple as:
```http
POST /config/bank-registrations HTTP/1.1
# standard headers omitted
{
  "BankProfile": "Obie_Modelo",
  "SoftwareStatementProfileId": "All"
}
```
where only the bank's *bank profile* and ID of the *software statement profile* are required. Other optional properties are available but generally not necessary to use. 

Secondly, Open Banking Connector knows when to "re-use" an existing external (bank) API OAuth2 client rather than create a new one meaning the user can happily call `POST /config/bank-registrations` for each bank they want to use without any further knowledge [^1]. This client re-use is completely transparent to the user. It can be overridden using the `ForceDynamicClientRegistration` property.

Finally, Open Banking Connector allows an existing OAuth2 client created externally to be passed in using the `ExternalApiObject` property. This is useful when DCR is not supported by the bank or an OAuth2 client has been created elsewhere. When `ExternalApiObject` is non-null, DCR is not performed.

## Mappings to database objects

A successful call to [`POST /config/bank-registrations`](./openapi.md) always creates a record in the database `bank_registration` table even when DCR is not performed (i.e. an existing client has been supplied or re-used).

Likewise, [GET /config/bank-registrations/{bankRegistrationId}](./openapi.md) and [DELETE /config/bank-registrations/{bankRegistrationId}](./openapi.md) can be used to read and delete database `bank_registration` records. 

## External (bank) API operations

When [`POST /config/bank-registrations`](./openapi.md) causes DCR to be performed, a call will be made to the external (bank) API `POST /register` endpoint and the property `ExternalApiResponse` will be included in the response. Otherwise just the local database object will be returned.

Where supported and specified by the bank profile (or forced via header `x-obc-include-external-api-operation`), [GET /config/bank-registrations/{bankRegistrationId}](./openapi.md) will perform a call to the external (bank) API `GET /register` endpoint and the property `ExternalApiResponse` will be included in the response. Otherwise just the local database object will be returned.

Where supported and specified by the bank profile (or forced via header `x-obc-include-external-api-operation`), [DELETE /config/bank-registrations/{bankRegistrationId}](./openapi.md) will perform a call to the external (bank) API `DELETE /register/{ClientId}` endpoint. Otherwise just the local database object will be deleted.

Note that only a few banks support `GET /register/{ClientId}` and `DELETE /register/{ClientId}` and so in most cases [GET /config/bank-registrations/{bankRegistrationId}](./openapi.md) and [DELETE /config/bank-registrations/{bankRegistrationId}](./openapi.md) will not result in external (bank) API calls.

[^1]: Why is this re-use necessary or desirable? Some *bank groups* (e.g. `NatWest`) expect/require the same OAuth2 client to re-used between sets of banks (e.g. `NatWest_RoyalBankOfScotland` and `NatWest_VirginOne`). Also performing DCR twice for the same bank might, depending on bank behaviour, result in re-generating the previous OAuth2 client (with same ID). For at least one bank, though, using the same client for both AISP and PISP is not allowed. Open Banking Connector has an internal concept of *bank registration groups*. These are essentially rules which determine when OAuth2 clients can be re-used (shared) by default. These are designed to ensure `POST /config/bank-registrations` can be called safely for all banks and only creates new OAuth2 clients where necessary.

