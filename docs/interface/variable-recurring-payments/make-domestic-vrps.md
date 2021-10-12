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
            out VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest obDomesticVrpRequest);                                                                                     // request objects
DomesticVrpRequest domesticVrpRequest =
    new DomesticVrpRequest
    {
        OBDomesticVRPRequest = obDomesticVrpRequest,
        DomesticVrpConsentId = domesticVrpConsentId,
        Name = "name"
    };
// POST domestic payment
IFluentResponse<DomesticVrpResponse> domesticVrpResp =
                await requestBuilder.VariableRecurringPayments.DomesticVrp
                    .PostAsync(domesticVrpRequest);

```





