# Server errors

## Overview

When an Open Banking Connector API endpoint cannot fulfil a request, it returns an error response using the
[Problem Details for HTTP APIs](https://www.rfc-editor.org/rfc/rfc9457) format (RFC 9457), served as
`application/problem+json`. Problem Details is a standard, self-describing error format: every response carries a
plain-English `title` and `detail` explaining what went wrong, alongside the HTTP `status`, in a shape that is instantly
recognisable to a developer and requires no bespoke parsing to read.

Open Banking Connector extends this base format so errors can also be handled programmatically. Every response carries a
`serverErrorType` field, which acts as a **discriminator**: its value both identifies which error occurred, for
`switch`/`case`-style handling, and identifies which schema of extension fields the response carries, so calling code
knows exactly which additional fields to expect and read once it has matched on a given `serverErrorType`
(for example the consent ID that could not be found, or the bank's HTTP response that triggered the failure).

## Response format

Every error response includes the standard Problem Details fields:

| Field    | Description                                                            |
|----------|------------------------------------------------------------------------|
| `title`  | A short, human-readable summary of the error type.                     |
| `detail` | A human-readable explanation specific to this occurrence of the error. |
| `status` | The HTTP status code, repeated in the body for convenience.            |

together with two Open Banking Connector-specific additions:

- **`serverErrorType`** &mdash; a stable, camelCase identifier for the specific kind of error (for example
  `consentNotFound`). Client code should key error handling off this field rather than `title` or `detail`, which are
  free text and may change wording over time.
- **Extension fields** &mdash; further properties specific to the error, letting a caller recover the exact context
  without re-parsing `detail`.

For example, requesting a consent that does not exist returns:

```json
{
  "title": "Consent not found",
  "detail": "No record found for account access consent with ID 4c9c9d6e-....",
  "status": 404,
  "serverErrorType": "consentNotFound",
  "consentType": "accountAccessConsent",
  "consentId": "4c9c9d6e-...."
}
```

## Server error types

The table below lists every `serverErrorType` Open Banking Connector can return, grouped by HTTP status code, along with
what triggers it and the extension fields it adds.

| Status | `serverErrorType`               | Description                                                                                                                                                                                         | Extension fields                                                                                                                                                   |
|--------|---------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 400    | `authContextNotFound`           | No auth context record found matching the given state value.                                                                                                                                        | `state`                                                                                                                                                            |
| 400    | `authContextStale`              | Auth context found but is more than ten minutes old, so the redirect will not be processed. A new auth context must be created and authentication repeated.                                         | `state`                                                                                                                                                            |
| 400    | `consentFromRequestNotFound`    | No consent record found for the given consent ID, where the ID was taken from the request header or body rather than the URL path.                                                                  | `consentType`, `consentId`, `consentIdSource`                                                                                                                      |
| 404    | `consentNotFound`               | No consent record found for the given consent ID (looked up via URL path).                                                                                                                          | `consentType`, `consentId`                                                                                                                                         |
| 502    | `idTokenValidationError`        | An ID token returned by the bank failed validation (expired, missing or mismatched nonce, ACR, issuer, audience, subject, or hashes). The specific reason is given in `idTokenValidationErrorType`. | `iss`, `iat`, `exp`, `acr`, `authTime`, `idTokenValidationErrorType`                                                                                               |
| 502    | `externalApiHttpRequestIoError` | An HTTP request to an external bank API endpoint failed with an I/O-level error (e.g. connection failure). The specific reason is given in `httpRequestError`.                                      | `requestUrl`, `requestHttpMethod`, `httpRequestError`                                                                                                              |
| 502    | `externalApiHttpRequestFailure` | An HTTP request to an external bank API endpoint returned an error response.                                                                                                                        | `requestUrl`, `requestHttpMethod`, `responseStatusCode`, `responseBody`, plus optionally `xFapiInteractionId`, `retryAfterSeconds`, `rateLimitPolicy`, `rateLimit` |
| 504    | `externalApiHttpRequestTimeout` | An HTTP request to an external bank API endpoint timed out.                                                                                                                                         | `requestUrl`, `requestHttpMethod`                                                                                                                                  |

### Extension field reference

Several `serverErrorType`s share the same extension fields (for example, most external-API errors carry
`requestUrl`/`requestHttpMethod`). Rather than repeat types and descriptions per error, every extension field used above
is defined once here:

| Field                        | Type                                  | Description                                                                                                                           |
|------------------------------|---------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------|
| `state`                      | string                                | OAuth2 `state` value identifying the auth context.                                                                                    |
| `consentType`                | string                                | Consent type, e.g. `accountAccessConsent`, `domesticPaymentConsent`, `domesticVrpConsent`.                                            |
| `consentId`                  | string (UUID)                         | ID of the consent record.                                                                                                             |
| `consentIdSource`            | string                                | Where the consent ID was read from: `requestHeader` or `requestBody`.                                                                 |
| `iss`                        | string                                | `iss` (issuer) claim from the bank's ID token.                                                                                        |
| `iat`                        | string (ISO 8601 date-time)           | `iat` (issued-at) claim from the bank's ID token.                                                                                     |
| `exp`                        | string (ISO 8601 date-time)           | `exp` (expiry) claim from the bank's ID token.                                                                                        |
| `acr`                        | string, nullable                      | `acr` (authentication context class reference) claim from the bank's ID token.                                                        |
| `authTime`                   | string (ISO 8601 date-time), nullable | `auth_time` claim from the bank's ID token.                                                                                           |
| `idTokenValidationErrorType` | string                                | Which specific ID token check failed, e.g. `nonceMismatch`, `idTokenExpired`.                                                         |
| `requestUrl`                 | string (URL)                          | URL of the external bank API request that failed.                                                                                     |
| `requestHttpMethod`          | string                                | HTTP method of the external bank API request that failed.                                                                             |
| `httpRequestError`           | string                                | .NET [`HttpRequestError`](https://learn.microsoft.com/dotnet/api/system.net.http.httprequesterror) classification of the I/O failure. |
| `responseStatusCode`         | integer                               | HTTP status code returned by the bank.                                                                                                |
| `responseBody`               | object                                | The bank's parsed error response body.                                                                                                |
| `xFapiInteractionId`         | string, optional                      | `x-fapi-interaction-id` header returned by the bank, if present.                                                                      |
| `retryAfterSeconds`          | integer, optional                     | Value of a `Retry-After` header returned by the bank, if present.                                                                     |
| `rateLimitPolicy`            | array of string, optional             | Value(s) of a `RateLimit-Policy` header returned by the bank, if present.                                                             |
| `rateLimit`                  | array of string, optional             | Value(s) of a `RateLimit` header returned by the bank, if present.                                                                    |

## Response deserialisation failures (not yet migrated)

One error case is not yet expressed through the `serverErrorType` model above: a `500` response returned when the
response body from an external bank API endpoint cannot be deserialised into the expected model. This is currently
raised as a hand-built `problem+json` response rather than going through the same internal error model as the errors
above, so it carries no `serverErrorType`. It does, however, follow the same base Problem Details fields, plus:

| Field                       | Type                       | Description                                                                                   |
|-----------------------------|----------------------------|-----------------------------------------------------------------------------------------------|
| `endpointHttpMethod`        | string                     | HTTP method of the request sent to the external bank API endpoint.                            |
| `endpointUrl`               | string (URL)               | URL of the external bank API endpoint.                                                        |
| `endpointFapiInteractionId` | string, optional           | The `x-fapi-interaction-id` header returned by the endpoint, if present.                      |
| `deserialisationError`      | string                     | The underlying deserialisation error message.                                                 |
| `endpointResponse`          | string or object, optional | The endpoint's raw response body, if exposing successful response bodies on error is enabled. |

This case is expected to be migrated to the `serverErrorType` model in a future release.
