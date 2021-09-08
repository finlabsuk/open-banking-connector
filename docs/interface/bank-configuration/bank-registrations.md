## Create a bank registration
Annotated Sample code:  
```csharp
// Create bank registration using Bank Profile
BankRegistration registrationRequest = bankProfile.BankRegistrationRequest(
    testNameUnique,
    bankId,
    softwareStatementProfileId,
    registrationScope);

// Makes call to Open Banking Connector. Creates object at bank and stores in database.
IFluentResponse<BankRegistrationResponse> registrationResp = await requestBuilder
    .ClientRegistration
    .BankRegistrations
    .PostAsync(registrationRequest);

// Response from Open Banking Connector.
// Returns object with Unique ID.
Guid bankRegistrationId = registrationResp.Data!.Id;
```
Refer to [sample app](./../../../../src/OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent/BankConfigurationMethods.cs#39)
to view implementation.