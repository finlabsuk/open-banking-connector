# Management API

The Management API allows you to create, read, update and delete management (setup) objects in Open Banking Connector.

## OpenAPI Spec

An OpenAPI spec for the Management API showing the endpoints and data types is provided [here](./openapi.md). This is a
static capture of the API at a given release (see grey box at top right of OpenAPI spec for which release).

For any running version of Open Banking Connector, the OpenAPI spec for the currently-running software is provided at
`GET /swagger/index.html`.

## Bank Registration endpoints

The three endpoints

- [POST /manage/bank-registrations](./openapi.md)
- [GET /manage/bank-registrations/{bankRegistrationId}](./openapi.md)
- [DELETE /manage/bank-registrations/{bankRegistrationId}](./openapi.md)

create, read and delete `BankRegistration` records in the database `bank_registration` table. A `BankRegistration`
record corresponds to and describes an OAuth2 client created at a
bank. The endpoints, where possible and required, create, read and delete OAuth2 clients on the bank side via calls
to the external (bank) API (more information is provided on this below) and act as replacements to those in the UK Open
Banking Dynamic Client Registration API.

### Relationship to UK Open Banking Dynamic Client Registration API

The UK Open Banking Dynamic Client Registration (DCR) API is
defined [here](https://openbankinguk.github.io/dcr-docs-pub/).

Open Banking Connector provides replacement endpoints for the UK OB DCR endpoints as shown below. The replacement
endpoints, where relevant, use data types from the latest UK OB DCR v3.3 API. When calling external (bank) APIs, Open
Banking Connector performs request/response payload translation for any banks supporting earlier API versions.

| UK OB DCR endpoint          | Replacement Open Banking Connector endpoint            |
|-----------------------------|--------------------------------------------------------|
| POST /register              | POST /manage/bank-registrations                        |
| GET /register/{ClientId}    | GET /manage/bank-registrations/{bankRegistrationId}    |
| DELETE /register/{ClientId} | DELETE /manage/bank-registrations/{bankRegistrationId} |

All the replacement Open Banking Connector endpoints handle behaviour and spec variations between banks meaning the same
API calls can be used regardless of bank. Bank-specific differences are determined by a bank's *bank profile*.

There are some further major differences when creating a `BankRegistration`/OAuth2 client
using [POST /manage/bank-registrations](./openapi.md) compared to using a bank's POST /register endpoint.

Firstly, Open Banking Connector can infer most of the payload to send to the bank during DCR from only a few request
properties. This means the creation of a new `BankRegistration`/OAuth2 client is usually as simple as:

```http
POST /manage/bank-registrations HTTP/1.1
# standard headers omitted
{
  "BankProfile": "{{bankProfile}}",
  "SoftwareStatementId": "{{softwareStatementId}}"
}
```

where only the bank's *bank profile* and ID of the `SoftwareStatement` record are required. Other optional properties
are available but generally not necessary to use.

Secondly, Open Banking Connector knows when to "re-use" an existing external (bank) API OAuth2 client rather than create
a new one meaning the user can happily call `POST /manage/bank-registrations` for each bank they want to use without any
further knowledge [^1]. This client re-use is completely transparent to the user.

Finally, Open Banking Connector allows the external (bank) ID of an existing OAuth2 client created externally to be
passed in using the
`ExternalApiId` property. This is useful when DCR is not supported by the bank or an OAuth2 client has been created
elsewhere. When `ExternalApiId` is non-null, DCR is not performed.

### External (bank) API operations

When [POST /manage/bank-registrations](./openapi.md) causes DCR to be performed, a call will be made to the external
(bank) POST /register endpoint and the property `ExternalApiResponse` will be included in the response. Otherwise
just the local database object will be returned.

Where supported and specified by the bank profile (and not overridden via header
`x-obc-exclude-external-api-operation`), [GET /manage/bank-registrations/{bankRegistrationId}](./openapi.md) will
perform a call to the external (bank) GET /register endpoint and the property `ExternalApiResponse` will be
included in the response. Otherwise just the local database object will be returned.

Where supported and specified by the bank profile (and not overridden via header
`x-obc-exclude-external-api-operation`), [DELETE /manage/bank-registrations/{bankRegistrationId}](./openapi.md) will
perform a call to the external (bank) DELETE /register/{ClientId} endpoint. Otherwise just the local database
object will be deleted.

Note that only a few banks support GET /register/{ClientId} and DELETE /register/{ClientId} and so in most
cases [GET /manage/bank-registrations/{bankRegistrationId}](./openapi.md)
and [DELETE /manage/bank-registrations/{bankRegistrationId}](./openapi.md) will not result in external (bank) API calls.

[^1]: Why is this re-use necessary or desirable? Some *bank groups* (e.g. `NatWest`) expect/require the same OAuth2
client to re-used between sets of banks (e.g. `NatWest_RoyalBankOfScotland` and `NatWest_VirginOne`). Also performing
DCR twice for the same bank might, depending on bank behaviour, result in re-generating the previous OAuth2 client (with
same ID). For at least one bank, though, using the same client for both AISP and PISP is not allowed. Open Banking
Connector has an internal concept of *bank registration groups*. These are essentially rules which determine when OAuth2
clients can be re-used (shared) by default. These are designed to ensure `POST /manage/bank-registrations` can be called
safely for all banks and only creates new OAuth2 clients where necessary.

## Software Statement endpoints

The five endpoints

- [POST /manage/software-statements](./openapi.md)
- [GET /manage/software-statements](./openapi.md)
- [GET /manage/software-statements/{softwareStatementId}](./openapi.md)
- [PUT /manage/software-statements/{softwareStatementId}](./openapi.md)
- [DELETE /manage/software-statements/{softwareStatementId}](./openapi.md)

create, read all, read, update and delete `SoftwareStatement` records in the database `software_statement` table. A
`SoftwareStatement` record corresponds to and describes a software statement created in the UK Open Banking Directory.

In UK Open Banking, a software statement identifies a third-party provider (TPP) application which can create
OAuth2 clients (bank registrations) with banks. A TPP can create and use multiple software statements (i.e. have
multiple applications) and this is fully
supported by Open Banking Connector. Before creating a bank registration, you will need to add at least one software
statement to Open Banking Connector using the [POST /manage/software-statements](./openapi.md) endpoint.

Each software statement must refer to both an *OBWAC transport certificate* and an *OBSeal signing certificate* - these
will then be used in conjunction with that software statement. These should be added to Open Banking Connector before
adding the software statement that refers to them. Each certificate (OBWAC or OBSeal) can be used by more than one
software statement.

Follow the instructions in the [UK Open Banking Directory](https://directory.openbanking.org.uk/s/login/) to create
required software statements, signed OBWAC certificates (including keys) and signed OBSeal certificates (including
keys). Note that the sandbox and production environments of the Open Banking Directory are separate so please create
things in the right environment depending on intended use. You can then add them to Open Banking Connector.

Note that software statements are validated on application start-up so please be alert to warning and error messages
related to these.

## Encryption Key Description endpoints

The three endpoints

- [POST /manage/encryption-key-descriptions](./openapi.md)
- [GET /manage/encryption-key-descriptions/{encryptionKeyDescriptionId}](./openapi.md)
- [DELETE /manage/encryption-key-descriptions/{encryptionKeyDescriptionId}](./openapi.md)

create, read and delete `EncryptionKeyDescription` records in the database `encryption_key_description` table. An
`EncryptionKeyDescription` record describes (but does not include) an encryption key used for application-level
encryption of sensitive data (such as bank access and refresh tokens) stored in the database.

In Open Banking Connector, encryption is performed using the in-the-box .NET implementation of AES-256-GCM. Note that
use of unique nonces is guaranteed at database level by the unique nonce field in the database encrypted_object table.
Please do not hard-delete records in this table whose `encryption_key_description_id` field matches an encryption key
still in use since it is
very important for AES-GCM that nonces are never re-used for the same encryption key.

Multiple encryption key descriptions can be added to Open Banking Connector using
the [POST /manage/encryption-key-descriptions](./openapi.md) endpoint. This allows decryption of objects encrypted with
previously-used keys. New objects will be encrypted with the key specified in the database settings table by
field `current_encryption_key_description_id`. This can be updated when adding a key.

Encryption keys are described by both a `Source` and `Name`. Supported sources at time of writing are environment
variables (not generally recommended) and AWS SSM secrets. Additional sources supporting other key vaults can be added
in future as required. Each encryption key should be 256 bits and specified as a base64-encoded string.
