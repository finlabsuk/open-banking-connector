# Make Variable Recurring Payments

Before making a variable recurring payment you must have created and authorised a consent.

Then make a domestic payment, you must:

- create a `VariableRecurringPayment`

- read the `VariableRecurringPayment` to check its status.

# Example
Here is an example of how to make a variable recurring payment. We create a domestic vrp request object based on a previously created domestic vrp consent request object (`domesticVrpConsentRequest`) and a domestic vrp consent ID (`domesticVrpConsentId`)

```csharp
// Create domestic vrp request
requestBuilder.Utility.Map(
                domesticVrpConsentRequest.OBDomesticVRPConsentRequest,
                out VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest obDomesticVrpRequest); 

// maps Open Banking request objects
DomesticVrp domesticVrpRequest =
        new DomesticVrp
        {
            OBDomesticVRPRequest = obDomesticVrpRequest,
            Name = testNameUnique,
            DomesticVrpConsentId = domesticVrpConsentId,
        };

// POST domestic VRP 
IFluentResponse<DomesticVrpResponse> domesticVrpResponse =
                await requestBuilder
                    .VariableRecurringPayments
                    .DomesticVrps
                    .PostAsync(domesticVrpRequest);
                    
// Response from Open Banking Connector.
Guid domesticVrpId = domesticVrpResponse.Data!.Id;

```




