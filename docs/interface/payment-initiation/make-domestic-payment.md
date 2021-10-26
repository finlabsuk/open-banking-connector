# Make Domestic Payment

Before making a domestic payment you need to create and authorise a domestic payment consent.

You can then make a domestic payment by:

- using the `PostAsync` method on `requestBuilder.PaymentInitiation.DomesticPayments`

# Example

This is an example of how to make a domestic payment. We here create a request object for the domestic payment based on the previously created request object for the *domestic payment consent*.

The required inputs are:

- `domesticPaymentConsentRequest`: the consent request object

- `domesticPaymentConsentId`: the consent ID

- `testNameUnique`: the name field for the payment

```csharp
// Create domestic payment request
requestBuilder.Utility.Map(
    domesticPaymentConsentRequest.OBWriteDomesticConsent,
    out PaymentInitiationModelsPublic.OBWriteDomestic2 obWriteDomestic); // maps Open Banking request objects
DomesticPayment domesticPaymentRequest =
    new DomesticPayment
    {
        OBWriteDomestic = obWriteDomestic,
        DomesticPaymentConsentId = domesticPaymentConsentId,
        Name = testNameUnique
    };

// POST domestic payment
IFluentResponse<DomesticPaymentResponse> domesticPaymentResponse =
    await requestBuilder
        .PaymentInitiation
        .DomesticPayments
        .PostAsync(domesticPaymentRequest);
Guid domesticPaymentId = domesticPaymentResponse.Data!.Id;
```