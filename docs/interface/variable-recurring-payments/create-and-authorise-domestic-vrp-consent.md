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

## First, we create a domestic vrp consent object:
sample code: <br />
```csharp
// Create domestic vrp consent
DomesticVrpConsentRequest domesticVrpConsentRequest =
    bankProfile.DomesticVrpConsentRequest(
        Guid.Empty,
        Guid.Empty,
        DomesticVrpSubtestHelper.DomesticVrpType(subtestEnum),
        "placeholder: random GUID",
        "placeholder: random GUID",
        null);
// Makes call to Open Banking Connector. Creates objectand stores in database.
IFluentResponse<DomesticVrpConsentResponse> domesticVrpConsentResp =
await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
.PostAsync(domesticVrpConsentRequest);
// Response from Open Banking Connector.
Guid domesticVrpConsentId = domesticVrpConsentResp.Data!.Id;
```

## Next, get the Domestic Vrp Consent Funds Confirmation
sample code:
```csharp
// GET consent funds confirmation
// Makes call to Open Banking Connector. Creates objectand stores in database.
IFluentResponse<DomesticVrpConsentResponse> domesticPaymentConsentResp4 =
      await requestBuilderNew.VariableRecurringPayments.DomesticVrpConsents
           .GetFundsConfirmationAsync(domesticVrpConsentId);
```

## Then, create the Auth Context
sample code:
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
Once a `DomesticPaymentConsent` has been created and authorised, a domestic payment [may be made](./make-domestic-payment.md).

<br />
<br />
<br />

