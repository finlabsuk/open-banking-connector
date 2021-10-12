# Make Domestic Payment

Before making a domestic payment you must have created and authorised a consent.

Then make a domestic payment, you must:

- create a `DomesticPayment`

- read the `DomesticPayment` to check its status.

# Example
Here is an example of how to make a domestic payment. We here create a domestic payment request object based on a previously created domestic payment consent request object (`domesticPaymentConsentRequest`) and a domestic payment consent ID (`domesticPaymentConsentId`).

```csharp
// Create domestic payment request
requestBuilder.Utility.Map(
    domesticPaymentConsentRequest.OBWriteDomesticConsent,
    out PaymentInitiationModelsPublic.OBWriteDomestic2 obWriteDomestic);    // maps Open Banking
                                                                            // request objects
DomesticPaymentRequest domesticPaymentRequest =
    new DomesticPaymentRequest
    {
        OBWriteDomestic = obWriteDomestic,
        DomesticPaymentConsentId = domesticPaymentConsentId,
        Name = "name"
    };

// POST domestic payment
IFluentResponse<DomesticPaymentResponse> domesticPaymentResp =
    await requestBuilder.PaymentInitiation.DomesticPayments
        .PostAsync(domesticPaymentRequest);
```