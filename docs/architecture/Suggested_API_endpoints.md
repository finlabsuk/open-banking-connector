# API Endpoints

These are the suggested HTTP API endpoints for sample services. These will help drive implementations for sample & test services by showing the extent of future client-API interactions. Please note that our intention is to provide a library and *not* to provide a production-quality service: the service will use the library.

| Area | Name | Method | Path | Request | Response |
| --| -- | -- | -- | -- | -- | 
| General | Create Software Statement Profiles | POST | ```/software-statement-profiles``` | ```SoftwareStatementProfile``` | ```SoftwareStatementProfile``` |
| | Create OB Client PRofile | POST | ```/register``` | ```OBClientProfile``` | ```OBClientProfile``` |
| PISP API | Create Domestic Payment Consents | POST | ```/pisp/domestic-payment-constents``` | ```OBWriteDomesticConentPublic``` | ```OBWriteDomesticConsentResponsePublic``` 
| | Get Domestic Payment Consents | GET | ```/pisp/domestic-payment-consents/{consentId}``` | | ```OBWriteDomesticConsentResponsePublic``` |
| | Get Domestic Payment Consents (FC) | GET | ```/pisp/domestic-payment-consents/{consentId}/funds-confirmation``` | | ```OBWriteFundsConfirmationResponsePublic```
| | Create Domestic Payments | POST | ```/pisp/domestic-payments``` | ```OBWriteDomesticPublic``` | ```OBWriteDomesticResponsePublic```
| | Get Domestic Payments | GET | ```/pisp/domestic-payments/{domesticPaymentId}``` | | ```OBWriteDomesticConstentResponsePublic```

* "Public" types will eventually be denoted by their namespace. An API would not expose a type with that suffix.
* FC: Funds Confirmation
