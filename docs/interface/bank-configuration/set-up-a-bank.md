# Set up a bank

To set up a bank, you need to create:

* a `Bank` object

* at least one `BankRegistration` object

* at least one `BankApiInformationObject`

You can take advantage of Bank Profiles to generate request objects to use when creating these. Or you can use completely custom request objects.

# Example

Here is an example of how to set up a bank using a Bank Profile and is based on code in the [demo app](../../../../src/OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent/BankConfigurationMethods.cs#39).

First we create a `Bank` object:
```csharp
// Create Bank request object from Bank Profile
Bank bankRequest = bankProfile.BankRequest(testNameUnique);

// Makes call to Open Banking Connector. Creates objectand stores in database.
IFluentResponse<BankResponse> bankResp = await requestBuilder
    .ClientRegistration
    .Banks
    .PostLocalAsync(bankRequest);

// Response from Open Banking Connector.
Guid bankId = bankResp.Data!.Id;
```

Then we create a `BankApiInfromationObject`:
```csharp
// Create BankApiInformationObject request object from Bank Profile
BankApiInformation apiInformationRequest = bankProfile.BankApiInformationRequest(
    testNameUnique,
    bankId);

// Makes call to Open Banking Connector. Creates object and stores in database.
IFluentResponse<BankApiInformationResponse> apiInformationResponse = await requestBuilder
    .ClientRegistration
    .BankApiInformationObjects
    .PostLocalAsync(apiInformationRequest);

// Response from Open Banking Connector.
Guid bankApiInformationId = apiInformationResponse.Data!.Id;
```

Finally, we create a `BankRegistration` object:
```csharp
// Create BankRegistration request object from Bank Profile
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

We now have set up a bank and are ready to access functional APIs (e.g. PISP).