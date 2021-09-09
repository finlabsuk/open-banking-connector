# Supported Open Banking APIs

Open Banking Connector supports connections to banks using Open Banking APIs. This page details what APIs are supported.

Generally the Open Banking Connector Fluent interface is based on the latest supported version of an API and older API versions are supported by request/response type mapping. For example, to send a DCR v3.1 request to a bank the request would first be mapped to a DCR v3.1 request and then the response mapped back to a v3.3 response.

## UK Open Banking Dynamic Client Registration API

The UK Open Banking Dynamic Client Registration API is described [here](https://openbankinguk.github.io/dcr-docs-pub/).

Open Banking Connector supports the following versions of this API.

API Version | Type Mapping | Comment
 --- | --- | ---
v3.3 | No type mapping is used | Fluent interface based on this version
v3.2 | v3.3 request/repsonse types are mapped to v3.2
v3.1 | v3.3 request/response types are mapped to v3.1

Open Banking Connector supports the following endpoints for this API.

 API Endpoint | Fluent Interface Method
 --- | ---
POST /register | `requestBuilder.BankConfiguration.BankRegistrations.PostAsync()`
DELETE /register/{ClientId} | `requestBuilder.BankConfiguration.BankRegistrations.DeleteAsync()`

## UK Open Banking Read-Write Payment Initation API

The UK Open Banking Read-Write Payment Initiation API is described [here](https://openbankinguk.github.io/read-write-api-site3/).

Open Banking Connector supports the following versions of this API.

API Version | Type Mapping | Comment
 --- | --- | ---
v3.1.6 | No type mapping is used | Fluent interface based on this version
v3.1.4 | v3.1.6 request/repsonse types are mapped to v3.1.4
[v3.1.2] | | Types present but mapping not yet implemented

Open Banking Connector supports the following endpoints for this API.

 API Endpoint | Fluent Interface Method
 --- | ---
POST ​/domestic-payment-consents | <pre>```requestBuilder```<br/>```  .PaymentInitiation```<br/>```  .DomesticPaymentConsents```<br/>```  .PostAsync()```
GET ​/domestic-payment-consents​/{ConsentId} | `requestBuilder.PaymentInitiation.DomesticPaymentConsents.GetAsync()`
GET ​/domestic-payment-consents​/{ConsentId}​/funds-confirmation | `requestBuilder.PaymentInitiation.DomesticPaymentConsents.GetFundsConfirmationAsync()`
POST ​/domestic-payments | `requestBuilder.PaymentInitiation.DomesticPayments.PostAsync()`
GET ​/domestic-payments​/{DomesticPaymentId} | `requestBuilder.PaymentInitiation.DomesticPayments.GetAsync()`
