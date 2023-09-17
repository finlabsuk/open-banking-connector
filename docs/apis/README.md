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

The Open Banking Connector APIs are *internal (i.e. non-internet-accessible) APIs* for consumption within your back-end infrastructure. They should definitely not (!) be exposed to the internet in any way. This is *very important* as these endpoints provide access to Open Banking APIs.

The APIs are provided as HTTP rather than HTTPS APIs as customers are expected to configure HTTPS via an outer wrapper (e.g. using Kubernetes or a reverse proxy) to enable easier certificate configuration and to set up features such as MTLS.

As internal APIs, they do not require tokens but should obviously be secured as appropriate, e.g. via MTLS, firewalls, AWS security groups etc.

Internally, Open Banking Connector acquires and uses tokens to access UK Open Banking APIs. As a security feature, these tokens are not retrievable via the Open Banking Connector APIs. So, for example, when reading (GETing) a consent such as an Account Access Consent, tokens will not be included in the consent payload. To inspect them (in encrypted form), you will need to query the database directly.

