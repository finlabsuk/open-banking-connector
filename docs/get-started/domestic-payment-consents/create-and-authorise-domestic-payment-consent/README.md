# Create and authorise a domestic payment consent

To set up a domestic payment, you must create and then authorise a domestic payment consent.

You can then use the consent to make a payment.

## Create a domestic payment consent

A *domestic payment consent* describes the payment permission associated with a domestic payment. When *authorising* a
domestic payment consent,
an end-user will be asked to approve the payment.

A domestic payment consent suitable for paying another account in the same name (`TransferToSelf`) can be created for a
given bank registration by using the example request
below. This request is made to the [`POST /pisp/domestic-payment-consents`](../../../apis/payment-initiation/openapi.md) endpoint and will create a
domestic payment consent object at the bank. Note that in this example the creditor (payee)
account is specified but the debtor (payer) account is not. This allows the end-user to select a debtor account during
consent auth.

```http
# Create domestic payment consent for payment to account in same name (TransferToSelf)

# Note: placeholders like {{description}} should be replaced with appropriate values.

POST http://{{host}}/pisp/domestic-payment-consents
Content-Type: application/json

{
  "BankRegistrationId": "{{bank registration ID}}",
  "ExternalApiRequest": {
    "Data": {
      "ReadRefundAccount": "Yes",
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

Upon creation, Open Banking Connector will provide you with an ID for the domestic payment consent which can then be
used to
create an auth context (auth session) and, following auth, make a payment.

## Create an auth context

Create an auth context (auth session) for your domestic payment consent to begin end-user auth. You can do this using
the
example request below made to the [`POST /pisp/domestic-payment-consent-auth-contexts`](../../../apis/payment-initiation/openapi.md) endpoint.

```http
# Create domestic payment auth context

# Note: placeholders like {{description}} should be replaced with appropriate values.

POST http://{{host}}/pisp/domestic-payment-consent-auth-contexts
Content-Type: application/json

{
  "DomesticPaymentConsentId": "{{domestic payment consent ID}}}"
}
```

Upon creation, Open Banking Connector will provide you with an auth URL for end-user auth.

## Perform end-user auth

*Note: For demonstration purposes, here we will use manual browser end-user auth (i.e. paste the auth URL into a
browser) which is suitable for sandboxes such as the OBIE model bank. Supporting end-user auth with real banks requires
creating a suitable app -
see [here](../../../guide/README.md#open-banking-connector-supports-end-user-auth-in-your-app)
for more info.*

To start manual end-user auth (suitable for sandboxes), copy the auth URL into a browser and go through the bank's
authorisation process.

When this is complete you will be redirected back to your redirect URL, which by default will be the default fragment (
or query)
redirect URL from the software statement specified when creating the consent's bank registration. The bank
redirect data will be included in the fragment (or query) of the URL. Even if no web page exists at the URL, you can
still copy the
URL fragment (or query) from your browser to capture the bank redirect data.

To complete end-user auth, pass to Open Banking Connector the bank redirect data supplied by the bank in the fragment (
or query) of
the redirect URL. This needs to be done within 10 minutes of auth context creation or you will need to create a new auth
context and attempt auth again.

Upon receipt and successful validation of the bank redirect data, Open Banking Connector will obtain tokens for the
consent in preparation for future requests.
