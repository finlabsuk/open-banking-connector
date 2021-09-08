# Create and Authorise domestic payment consent

To set up domestic payment consent, you need to:
* create domestic payment consent
* get domestic payment consent funds confirmation
* post auth context 
* ADDITIONAL STEP REQUIRED: ASK MARK
<br />

# Example
Here is an example of how to set up the domestic payment consent. The example is based on code in the [demo app](../../../src/OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent/DomesticPaymentConsentMethods.cs#39).

## First, we create a domenstic payment consent object:
sample code: <br />
```csharp
// Create domestic payment consent
DomesticPaymentConsent domesticPaymentConsentRequest =
    bankProfile.DomesticPaymentConsentRequest(
        bankRegistrationId,
        bankApiInformationId,
        DomesticPaymentTypeEnum.PersonToMerchant,
        Guid.NewGuid().ToString("N"),
        Guid.NewGuid().ToString("N"),
        testNameUnique);
// Makes call to Open Banking Connector. Creates objectand stores in database.
IFluentResponse<DomesticPaymentConsentResponse> domesticPaymentConsentResp =
    await requestBuilder.PaymentInitiation.DomesticPaymentConsents
        .PostAsync(domesticPaymentConsentRequest);

// Response from Open Banking Connector.
Guid domesticPaymentConsentId = domesticPaymentConsentResp.Data!.Id;
```


## Next, get the Domestic Payment Consent Funds Confirmation 
sample code:
```csharp
// GET consent funds confirmation
// Makes call to Open Banking Connector. Creates objectand stores in database.
IFluentResponse<DomesticPaymentConsentResponse> domesticPaymentConsentResp4 =
    await requestBuilderNew.PaymentInitiation.DomesticPaymentConsents
        .GetFundsConfirmationAsync(domesticPaymentConsentId);
```

<br />

## Then, create the Auth Context
sample code:
```csharp
// POST auth context
var authContextRequest = new DomesticPaymentConsentAuthContext
        {
            DomesticPaymentConsentId = domesticPaymentConsentId,
            Name = testNameUnique
        };
// Makes call to Open Banking Connector. Creates objectand stores in database.
IFluentResponse<DomesticPaymentConsentAuthContextPostResponse> authContextResponse =
    await requestBuilder.PaymentInitiation
        .DomesticPaymentConsents
        .AuthContexts
        .PostLocalAsync(authContextRequest);
```

Once this is complete, the next step is to create the domestic payment. [See here](./create-domestic-payment.md)

