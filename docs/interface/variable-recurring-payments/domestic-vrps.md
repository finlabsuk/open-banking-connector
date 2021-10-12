# Variable Recurring Payments
## Payment object types

The thee payment configuration object types are shown in the table below. Each corresponds to a table in the Open Banking Connector database.

Object type | Parent type(s) | Created at bank? | Description | 
--- | --- | --- | ---
DomesticVrp<br/>Consents |`BankRegistration`, `BankApiSet` | Yes | Object a user can "authorise" to consent to a payment authorisation a variable recurring payment.|
DomesticVrp | `DomesticVrpConsent`| Yes | Payment object
AuthContexts| `DomesticVrpConsent`| No | Object which represents a session allowing the user to authorise a consent

## Supported methods

The table below shows the methods supported for each object type as well as the main request and response types for these methods. Methods that include the word "Local" operate on the Open Banking Connector database but not at the external bank.

Object type | Methods | Request type| Response type
 --- | --- | ---| ---
DomesticVrpConsents|GetLocalAsync <br/> PostAsync <br/> DeleteLocalAsync <br/> GetAsync <br/> GetFundsConfirmationAsync|[Consent](./../../../src/OpenBanking.Library.Connector/Models/Public/VariableRecurringPayments/Request/DomesticVrpConsent.cs)|[Consent](./../../../src/OpenBanking.Library.Connector/Models/Public/VariableRecurringPayments/Response/DomesticVrpConsentResponse.cs)
DomesticPayments| GetAsync<br/> PostAsync <br/> GetLocalAsync <br/> DeleteAsync |[Payment](./../../../src/OpenBanking.Library.Connector/Models/Public/VariableRecurringPayments/Request/DomesticVrp.cs)|[Payment](./../../../src/OpenBanking.Library.Connector/Models/Public/VariableRecurringPayments/Response/DomesticVrpResponse.cs)
AuthContexts| |[Auth](./../../../src/OpenBanking.Library.Connector/Models/Public/VariableRecurringPayments/Request/DomesticVrpConsentAuthContext.cs)|[Auth](./../../../src/OpenBanking.Library.Connector/Models/Public/VariableRecurringPayments/Response/DomesticVrpConsentAuthContextResponse.cs)
