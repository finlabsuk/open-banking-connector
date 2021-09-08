## Create bank API information 

Annotated Sample code:
```csharp
// Create bank API information
BankApiInformation apiInformationRequest = bankProfile.BankApiInformationRequest(
    testNameUnique,
    bankId);

// Makes call to Open Banking Connector. Creates object at bank and stores in database.
IFluentResponse<BankApiInformationResponse> apiInformationResponse = await requestBuilder
    .ClientRegistration
    .BankApiInformationObjects
    .PostLocalAsync(apiInformationRequest);

// Response from Open Banking Connector.
Guid bankApiInformationId = apiInformationResponse.Data!.Id;
```

Refer to [sample app](./../../../../src/OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent/BankConfigurationMethods.cs#39)
to view implementation.