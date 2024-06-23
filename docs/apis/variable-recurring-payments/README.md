# Variable Recurring Payments API

The Variable Recurring Payments API provides access to UK Open Banking Variable Recurring Payments API functionality.

## OpenAPI Spec

An OpenAPI spec for the Variable Recurring Payments API showing the endpoints and data types is provided [here](./openapi.md). This is a static capture of the API at a given release (see grey box at top right of OpenAPI spec for which release).

For any running version of Open Banking Connector, the OpenAPI spec for the currently-running software is provided at `GET /swagger/index.html`.

## Relationship to UK Open Banking Variable Recurring Payments API

The UK Open Banking Variable Recurring Payments (VRP) API is defined [here](https://openbankinguk.github.io/read-write-api-site3/v3.1.11/profiles/vrp-profile.html).

Open Banking Connector provides replacement endpoints for the UK OB VRP endpoints as shown in the following table. The replacement endpoints, where relevant, use data types from UK OB VRP API version v3.1.11. When calling external (bank) APIs, Open Banking Connector performs request/response payload translation for any banks supporting earlier API versions.

| UK OB VRP endpoint                                         | Replacement Open Banking Connector endpoint                               | Requires header `x-obc-domestic-vrp-consent-id`?
|------------------------------------------------------------|---------------------------------------------------------------------------|-
| POST /domestic-vrp-consents                                | POST /vrp/domestic-vrp-consents                                           | :white_large_square:
| GET /domestic-vrp-consents/{ConsentId}                     | GET /vrp/domestic-vrp-consents/{domesticVrpConsentId}                     |  :white_large_square:
| DELETE /domestic-vrp-consents/{ConsentId}                  | DELETE /vrp/domestic-vrp-consents/{domesticVrpConsentId}                  | :white_large_square:
| POST /domestic-vrp-consents/{ConsentId}/funds-confirmation | POST /vrp/domestic-vrp-consents/{domesticVrpConsentId}/funds-confirmation | :white_large_square:
| POST /domestic-vrps                                        | POST /vrp/domestic-vrps                                                   | :white_large_square:
| GET /domestic-vrps/{DomesticVRPId}                         | GET /vrp/domestic-vrps/{externalApiId}                                    | :white_check_mark: 
| GET /domestic-vrps/{DomesticVRPId}/payment-details         | GET /vrp/domestic-vrps/{externalApiId}/payment-details                    |:white_check_mark: 

Behavioural variations between banks are handled by Open Banking Connector via *bank profiles*. However this does not extend to data normalisation leaving you in full control over what is sent to a bank and with full visibility of what is received.

GET endpoints for domestic VRPs require the header `x-obc-domestic-vrp-consent-id` to identify the related domestic VRP consent (use `Id` returned from [POST /vrp/domestic-vrp-consents](./openapi.md)).

The Open Banking Connector API provides two additional endpoints that are not UK OB replacements: [POST /vrp/domestic-vrp-consent-auth-contexts](./openapi.md) and [GET /vrp/domestic-vrp-consent-auth-contexts/{domesticVrpConsentAuthContextId}](./openapi.md). These respectively create and read an *auth context* which is a time-limited session for end-user authentication. Calling [POST /vrp/domestic-vrp-consent-auth-contexts](./openapi.md) creates an auth context and returns a URL which can be used for end-user auth.
