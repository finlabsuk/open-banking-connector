# Create and Authorise domestic vrp consent

To set up domestic vrp consent, you need to:

- create a `DomesticVrpConsent`

- [optional] check for funds confirmation

- create an `AuthContext` which generates an auth URL

- let the user authorise the consent using the URL which will generate a redirect

- either

    - (a) use Open Banking Connector to process the redirect

    - (b) use external software to process the redirect and pass the results to Open Banking Connector
    
# Example
Here is an example of how to set up the domestic vrp consent. The example is based on code in the [demo app](../../../src/OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent/DomesticPaymentConsentMethods.cs#39).

First, we create a domestic vrp consent object:
```csharp
// Create domestic VRP consent 
DomesticVrpConsent domesticVrpConsentRequest =
bankProfile.DomesticVrpConsentRequest(
bankRegistrationId,
bankApiSetId,
domesticVrpTypeEnum,
testNameUnique);

// Makes call to Open Banking Connector. Creates objectand stores in database.
IFluentResponse<DomesticVrpConsentResponse> domesticVrpConsentResponse =
await requestBuilder
.VariableRecurringPayments
.DomesticVrpConsents
.PostAsync(domesticVrpConsentRequest);

// Response from Open Banking Connector.
Guid domesticVrpConsentId = domesticVrpConsentResponse.Data!.Id;
```


Next, get the Domestic Vrp Consent Funds Confirmation:
```csharp
// GET consent funds confirmation
// Makes call to Open Banking Connector. Creates objectand stores in database.
IFluentResponse<DomesticVrpConsentResponse> domesticPaymentConsentResp4 =
      await requestBuilderNew.VariableRecurringPayments.DomesticVrpConsents
           .GetFundsConfirmationAsync(domesticVrpConsentId);
```

Then, create the Auth Context:
```csharp
 // POST auth context
            var authContextRequest = new DomesticVrpConsentAuthContext
            {
                DomesticVrpConsentId = domesticVrpConsentId,
                Name = testNameUnique
            };
            IFluentResponse<DomesticVrpConsentAuthContextPostResponse> authContextResponse =
                await requestBuilder.VariableRecurringPayments
                    .DomesticVrpConsents
                    .AuthContexts
                    .PostLocalAsync(authContextRequest);
```
<br />
Once a `DomesticPaymentConsent` has been created and authorised, a domestic payment [may be made](./make-domestic-vrps.md).

<br />
<br />
<br />

