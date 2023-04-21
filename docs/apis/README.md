# APIs

## Overview

Open Banking Connector provides a set of HTTP APIs. Mostly these act as *outer APIs* providing replacement endpoints for endpoints provided by [UK Open Banking APIs](https://standards.openbanking.org.uk/api-specifications/). These replacement endpoints wrap calls to *UK Open Banking API* endpoints as well as extra calls for token acquisition etc. Aside from the Bank Configuration and Auth Context APIs, Open Banking Connector APIs aim to mirror their UK Open Banking equivalents as much as possible.

The APIs provided are as follows:

| API | Description
| - | -
| [Bank Configuration](./bank-configuration/README.md) | This API provides endpoints for bank setup including replacement endpoints for endpoints provided in the UK Open Banking [Dynamic Client Registration API](https://openbankinguk.github.io/dcr-docs-pub/v3.3/dynamic-client-registration.html)
| [Account and Transaction](./account-and-transaction/README.md) | This API aims to mirror the UK Open Banking [Account and Transaction API](https://openbankinguk.github.io/read-write-api-site3/v3.1.10/profiles/account-and-transaction-api-profile.html).
| [Payment Initiation](./payment-initiation/README.md) | This API aims to mirror the UK Open Banking [Payment Initiation API](https://openbankinguk.github.io/read-write-api-site3/v3.1.10/profiles/payment-initiation-api-profile.html).
| [Variable Recurring Payments](./variable-recurring-payments/README.md) | This API aims to mirror the UK Open Banking [Variable Recurring Payments API](https://openbankinguk.github.io/read-write-api-site3/v3.1.10/profiles/vrp-profile.html).
| [Auth Contexts API](./auth-contexts/README.md) | This API provides endpoints for handling bank OAuth2 redirects that occur following end-user auth

## Security

The Open Banking Connector API is intended to be used as an *internal (i.e. non-internet-accessible) API* within your backend infrastructure. Please see [below](#bank-redirects) for info about handling bank redirects.

It is provided as an HTTP rather than HTTPS API as customers are expected to configure HTTPS via an outer wrapper (e.g. using Kubernetes or a reverse proxy) to enable easier certificate configuration and to set up features such as MTLS.

As an internal API, no tokens are required to access the API. It should obviously be secured as appropriate, e.g. via MTLS, firewalls, AWS security groups etc.

Internally, Open Banking Connector acquires and uses tokens to access UK Open Banking APIs. As a security feature, these tokens are not retrievable via the HTTP API. So, for example, when reading (GETing) a consent such as an Account Access Consent tokens will not be included in the consent payload. To inspect them, you will need to query the database directly.

Open Banking Connector stores sensitive data including tokens in the database. So the database provided to Open Banking Connector should therefore be tightly secured including using at-rest encryption and not allowing access from any other application. Controls on human access to the database should also be implemented.

## Bank Redirects

It is expected that the customer implements their own secure internet-facing endpoint to handle bank redirects that occur following end-user auth.

This endpoint can then forward data to Open Banking Connector via the [POST /auth/redirect-delegate](./auth-contexts/openapi.md) endpoint. Please contact us if you need any help with creating an internet-facing redirect endpoint.
