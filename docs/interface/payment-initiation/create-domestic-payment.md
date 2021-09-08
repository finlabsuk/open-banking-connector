# Create Domestic Payments

# Example
Here is an example of how to set up the domestic payment consent. The example is based on code in the [demo app](../../../src/OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent/DomesticPaymentConsentMethods.cs#39).

sample code:
```csharp
// POST domestic payment
DomesticPaymentRequest domesticPaymentRequest =
    new DomesticPaymentRequest
        { DomesticPaymentConsentId = default };
await testDataProcessorFluentRequestLogging
    .AppendToPath("domesticPayment")
    .AppendToPath("postRequest")
    .WriteFile(domesticPaymentRequest);
domesticPaymentRequest.DomesticPaymentConsentId = domesticPaymentConsentId;
domesticPaymentRequest.Name = testNameUnique;

// Makes call to Open Banking Connector. Creates objectand stores in database.
IFluentResponse<DomesticPaymentResponse> domesticPaymentResp =
    await requestBuilderNew.PaymentInitiation.DomesticPayments
        .PostAsync(domesticPaymentRequest);

```