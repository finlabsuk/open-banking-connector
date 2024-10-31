# Confirm funds and make a payment

Before making a payment, you may optionally perform a confirmation of funds (CoF) check.

You can then make the payment.

After making a payment, you can check the payment status by "reading" the payment.

## Confirm funds

You can confirm availability of end-user funds for a given payment amount by "reading" a domestic payment consent funds
confirmation. This request will be passed to the bank and the bank will respond indicating if funds are available. You
can do this using the example request below which is made to the [`GET /pisp/domestic-payment-consents/{{domestic payment consent ID}}/funds-confirmation`](../../../apis/payment-initiation/openapi.md)
endpoint.

```http
# Read domestic payment consent funds confirmation

# Note: placeholders like {{description}} should be replaced with appropriate values.

GET http://{{host}}/pisp/domestic-payment-consents/{{domestic payment consent ID}}/funds-confirmation
Content-Type: application/json
```

Upon creation, Open Banking Connector will provide you with a response which will include
*ExternalApiResponse.Data.FundsAvailableResult.FundsAvailable* (assuming that this is provided by the bank). This will
equal `true` if funds
are available.

## Make a payment

You can make a payment by "creating" a domestic payment. You can do this using the example request below which is made
to
the [`POST /pisp/domestic-payments`](../../../apis/payment-initiation/openapi.md) endpoint. It is important that
the "Initiation" and "Risk" sections match those of the corresponding consent.

```http
# Create domestic payment

# Note: placeholders like {{description}} should be replaced with appropriate values.

# Note 2: the parameter ExternalApiRequest.Data.ConsentId, if left empty (which is recommended), will be auto-populated by Open Banking Connector.

POST http://{{host}}/pisp/domestic-payments
Content-Type: application/json

{
  "DomesticPaymentConsentId": "{{domestic payment consent ID}}",
  "ExternalApiRequest": {
    "Data": {
      "ConsentId": "",
      "Initiation": {
        "InstructionIdentification": "{{instruction ID text}}",
        "EndToEndIdentification": "{{end-to-end ID text}}",
        "InstructedAmount": {
          "Amount": "{{payment amount, e.g. 5.00}}",
          "Currency": "GBP"
        },
        "CreditorAccount": {
          "SchemeName": "UK.OBIE.SortCodeAccountNumber",
          "Identification": "{{sort code followed by account number as 14-digit number}}",
          "Name": "{{account holder's name, e.g. John Smith}}"
        },
        "RemittanceInformation": {
          "Reference": "{{reference text}}"
        }
      }
    },
    "Risk": {
      "PaymentContextCode": "TransferToSelf",
      "ContractPresentIndicator": "true"
    }
  }
}
```

Upon creation, Open Banking Connector will provide you with a response including
*ExternalApiResponse.Data.DomesticPaymentId* which is the external API (bank) ID of the domestic payment.

## Check payment status by reading payment

The status of a payment usually changes with time from a pending/unsettled status to a settled or rejected status. You
can check the status of a payment by "reading" the domestic payment. You can do this using the example request below
which is made to the [`GET /pisp/domestic-payments/{{domestic payment external API ID}}`](../../../apis/payment-initiation/openapi.md)
endpoint.

```http
# Read domestic payment

# Note: placeholders like {{description}} should be replaced with appropriate values.

GET http://{{host}}/pisp/domestic-payments/{{domestic payment external API ID}}
x-obc-domestic-payment-consent-id: {{domestic payment consent ID}}
```

Upon reading, Open Banking Connector will provide you with a response including *ExternalApiResponse.Data.Status* which
is the status of the domestic payment.