## Create bank
Annotated Sample code:
```csharp
// Create bank
Bank bankRequest = bankProfile.BankRequest(testNameUnique);

// Makes call to Open Banking Connector. Creates object at bank and stores in database.
IFluentResponse<BankResponse> bankResp = await requestBuilder
    .ClientRegistration
    .Banks
    .PostLocalAsync(bankRequest);

// Response from Open Banking Connector.
Guid bankId = bankResp.Data!.Id;
```
Refer to [sample app](./../../../../src/OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent/BankConfigurationMethods.cs#39)
to view implementation.