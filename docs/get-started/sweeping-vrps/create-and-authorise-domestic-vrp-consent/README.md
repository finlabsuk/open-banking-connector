# Create and authorise a domestic VRP consent for sweeping

A [sweeping variable recurring payment](https://assets.publishing.service.gov.uk/media/622ef71fd3bf7f5a86be8fa4/Sweeping_clarification_letter_to_be_sent_14_March_2022__.pdf) is a variable recurring payment (VRP) which supports transfers between accounts in the same name.

To set up a sweeping VRP, we must create and then authorise a domestic VRP consent.

We can then use the consent to make payments.

## Create a domestic VRP consent

A *domestic VRP consent* describes payment permissions associated with a VRP. When *authorising* a domestic VRP consent, an end-user will be asked to agree to these permissions.

A domestic VRP consent suitable for sweeping can be created for a given bank registration by using the example request below. This will create a domestic consent VRP object at the bank. Note that the creditor (payee) account is specified but the debtor (payer) account is not. This allows the end-user to select a debtor account during consent auth. Both individual and period maximum payment amounts are specified in this example. See the [Open Banking spec](https://openbankinguk.github.io/read-write-api-site3/v3.1.11/resources-and-data-models/vrp/domestic-vrp-consents.html#obdomesticvrpcontrolparameters) for other options for control parameters.

```http
# Create domestic VRP consent for sweeping

# Note: placeholders like {{description}} should be replaced with appropriate values.

POST http://{{host}}/vrp/domestic-vrp-consents
Content-Type: application/json

{
  "BankRegistrationId": "{{bank registration ID}}",
  "ExternalApiRequest": {
    "Data": {
      "ReadRefundAccount": "Yes",
      "ControlParameters": {
        "PSUAuthenticationMethods": [
          "UK.OBIE.SCANotRequired"
        ],
        "PSUInteractionTypes": [
          "OffSession"
        ],
        "VRPType": [
          "UK.OBIE.VRPType.Sweeping"
        ],
        "MaximumIndividualAmount": {
          "Amount": "{{amount limit for individual payments, e.g. 5.00}}",
          "Currency": "GBP"
        },
        "PeriodicLimits": [
          {
            "Amount": "{{amount limit across period, e.g. 50.00}}",
            "Currency": "GBP",
            "PeriodAlignment": "Consent",
            "PeriodType": "Month"
          }
        ]
      },
      "Initiation": {
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

Upon creation, Open Banking Connector will provide you with an ID for the domestic VRP consent which can then be used to create an auth context (auth session) and, following auth, make one or more payments.

## Create an auth context

Create an auth context (auth session) for your domestic VRP consent to begin end-user auth. You can do this using the example request below.

```http
# Create domestic VRP auth context

# Note: placeholders like {{description}} should be replaced with appropriate values.

POST http://{{host}}/vrp/domestic-vrp-consent-auth-contexts
Content-Type: application/json

{
  "DomesticVrpConsentId": "{{domestic VRP consent ID}}"
}
```

Upon creation, Open Banking Connector will provide you with an auth URL for end-user auth.

## Perform end-user auth

*Note: For demonstration purposes, here we will use manual browser end-user auth (i.e. paste the auth URL into a browser) which is suitable for sandboxes such as the OBIE model bank. Supporting end-user auth with real banks requires creating a suitable app - see [here](../../../guide/README.md#open-banking-connector-supports-end-user-auth-in-your-app) for more info.*

To start manual end-user auth (suitable for sandboxes), copy the auth URL into a browser and go through the bank's authorisation process.

When this is complete you will be redirected back to your redirect URL, which by default will be the default fragment redirect URL from the software statement object specified when creating the consent's bank registration. The bank redirect data will be included in the fragment of the URL. Even if no web page exists at the URL, you can still copy the URL fragment from your browser to capture the bank redirect data. 

To complete end-user auth, pass to Open Banking Connector the bank redirect data supplied by the bank  in the fragment of the redirect URL. This needs to be done within 10 minutes of auth context creation or you will need to create a new auth context and attempt auth again.

Upon receipt and successful validation of the bank redirect data, Open Banking Connector will obtain tokens for the consent in preparation for future requests.

