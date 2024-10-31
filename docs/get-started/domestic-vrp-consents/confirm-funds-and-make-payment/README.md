# Confirm funds and make a payment

Before making a payment, you may optionally perform a confirmation of funds (CoF) check.

You can then make the payment.

After making a payment, you can check the payment status by "reading" the payment.

## Confirm funds

You can confirm availability of end-user funds for a given payment amount by "creating" a domestic VRP consent funds
confirmation. This request will be passed to the bank and the bank will respond indicating if funds are available. You
can do this using the example request below which is made to the [`POST /vrp/domestic-vrp-consents/{{domestic VRP consent ID}}/funds-confirmation`](../../../apis/variable-recurring-payments/openapi.md)
endpoint.

Note that `"{{reference text}}"` should match that used when creating the domestic VRP consent.

```http
# Create domestic VRP consent funds confirmation

# Note: placeholders like {{description}} should be replaced with appropriate values.

POST http://{{host}}/vrp/domestic-vrp-consents/{{domestic VRP consent ID}}/funds-confirmation
Content-Type: application/json

{
  "ExternalApiRequest": {
    "Data": {
      "ConsentId": "",
      "Reference": "{{reference text}}",
      "InstructedAmount": {
        "Amount": "{{payment amount, e.g. 5.00}}",
        "Currency": "GBP"
      }
    }
  }
}
```

Upon creation, Open Banking Connector will provide you with a response including
*ExternalApiResponse.Data.FundsAvailableResult.FundsAvailable* which will equal `"Available"` if funds are available.

## Make a payment

You can make a payment by "creating" a domestic VRP. You can do this using the example request below which is made to
the [`POST /vrp/domestic-vrps`](../../../apis/variable-recurring-payments/openapi.md)
endpoint. It is important that the "Initiation" and "Risk" sections match those of the corresponding consent.

```http
# Create domestic VRP

# Note: placeholders like {{description}} should be replaced with appropriate values.

# Note 2: the parameter ExternalApiRequest.Data.ConsentId, if left empty (which is recommended), will be auto-populated by Open Banking Connector.

POST http://{{host}}/vrp/domestic-vrps
Content-Type: application/json

{
  "DomesticVrpConsentId": "{{domestic VRP consent ID}}",
  "ExternalApiRequest": {
    "Data": {
      "ConsentId": "",
      "PSUAuthenticationMethod": "UK.OBIE.SCANotRequired",
      "PSUInteractionType": "OffSession",
        "VRPType": "UK.OBIE.VRPType.Sweeping",
      "Initiation": {
        "CreditorAccount": {
          "SchemeName": "UK.OBIE.SortCodeAccountNumber",
          "Identification": "{{sort code followed by account number as 14-digit number}}",
          "Name": "{{account holder's name, e.g. John Smith}}"
        },
        "RemittanceInformation": {
          "Reference": "{{reference text}}"
        }
      },
      "Instruction": {
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
*ExternalApiResponse.Data.DomesticVRPId* which is the external API (bank) ID of the domestic VRP.

## Check payment status by reading payment

The status of a payment usually changes with time from a pending/unsettled status to a settled or rejected status. You
can check the status of a payment by "reading" the domestic VRP. You can do this using the example request below which
is made to the [
`GET /vrp/domestic-vrps/{{domestic VRP external API ID}}`](../../../apis/variable-recurring-payments/openapi.md)
endpoint.

```http
# Read domestic VRP

# Note: placeholders like {{description}} should be replaced with appropriate values.

GET http://{{host}}/vrp/domestic-vrps/{{domestic VRP external API ID}}
x-obc-domestic-vrp-consent-id: {{domestic VRP consent ID}}
```

Upon reading, Open Banking Connector will provide you with a response including *ExternalApiResponse.Data.Status* which
is the status of the domestic VRP (payment).