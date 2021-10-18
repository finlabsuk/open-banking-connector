# Make Variable Recurring Payments

Before making a variable recurring payment you must have created and authorised a consent.

You can then make a variable recurring payment by:

- creating a `VariableRecurringPayment`

- reading the `VariableRecurringPayment` to check its status.

- using the `PostAsync` method on `requestBuilder.VariableRecurringPayment.DomesticVrpPayments`

# Example
Here is an example of how to make a variable recurring payment. We create a domestic vrp request object based on a previously created domestic vrp consent request object.

The required inputs are:
- `domesticVrpConsentRequest` : the consent request object
- `domesticVrpConsentId` : the consent ID
- `testNameUnique` :the name field for the payment

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




