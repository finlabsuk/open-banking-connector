# Add software statement and certificates

Before creating a bank registration, you will need to add at least one software statement to Open Banking Connector. In
UK Open Banking, a software statement identifies a third-party provider (TPP) application which can create
registrations (OAuth2 clients) with banks. A TPP can have multiple software statements (applications) and this is fully
supported by Open Banking Connector.

Each software statement must refer to both an *OBWAC transport certificate* and an *OBSeal signing certificate* - these
will then be used in conjunction with that software statement. These should be added to Open Banking Connector before
adding the software statement that refers to them. Each certificate (OBWAC or OBSeal) can be used by more than one
software statement.

Note that the OBWAC and OBSeal certificate objects added to Open Banking Connector must each include a key
SecretSource (supported sources at time of writing are environment variables (not recommended) and AWS SSM secrets,
others can be easily added as required).

Follow the instructions in the [UK Open Banking Directory](https://directory.openbanking.org.uk/s/login/) to create
required software statements, signed OBWAC certificates (including keys) and signed OBSeal certificates (including
keys). Note that the sandbox and production environments of the Open Banking Directory are separate so please create
things in the right environment depending on intended use. You can then add them to Open Banking Connector.

Below are instructions on how to add an OBWAC certificate, OBSeal certificate and software statement to Open Banking
Connector.

## Add an OBWAC transport certificate

An OBWAC transport certificate is used for mutual TLS when communicating with banks.

You can add an OBWAC transport certificate using the [
`POST /manage/obwac-certificates`](../../apis/management/openapi.md) endpoint.

Upon adding, Open Banking Connector will provide you with an ID for the OBWAC transport certificate which you can then
use when adding a software statement.

### Example Postman request

![Alt text](add-obwac.png)

## Add an OBSeal signing certificate

An OBSeal signing certificate and its associated key is used to sign and validate JWTs sent to banks.

You can add an OBSeal signing certificate using the [
`POST /manage/obseal-certificates`](../../apis/management/openapi.md) endpoint.

Upon adding, Open Banking Connector will provide you with an ID for the OBSeal signing certificate which you can then
use when adding a software statement.

### Example Postman request

![Alt text](add-obseal.png)

## Add a software statement

In UK Open Banking, a software statement identifies a third-party provider (TPP) application that can create
registrations (OAuth2
clients) with banks. A TPP can create and use multiple software statements and this is fully
supported by Open Banking Connector.

You can add a software statement using the [`POST /manage/software-statements`](../../apis/management/openapi.md)
endpoint.

Upon adding, Open Banking Connector will provide you with an ID for the software statement which you can then use when
creating a bank registration.

Note that software statements are validated on application start-up so please be alert to warning and error messages
related to these.

### Example Postman request

![Alt text](add-software-statement.png)
