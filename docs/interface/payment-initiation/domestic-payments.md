# Domestic Payments

The Payment Initiation API supports creating and reading domestic payments.

## Interface object types

The three domestic payment object types are shown in the table below. Each corresponds to a table in the Open Banking Connector database.


Object type | Parent type(s) | Created at bank? | Description | 
--- | --- | --- | ---
DomesticPayment<br/>Consents |`BankRegistration`, `BankApiSet` | Yes | Object a user can "authorise" to consent to a payment authorisation a domestic payment.|
DomesticPayments | `DomesticPaymentConsent`| Yes | Payment object
AuthContexts| `DomesticPaymentConsent`| No | Object which represents a session allowing the user to authorise a consent

## Supported methods

The table below shows the methods supported for each object type as well as the main request and response types for these methods. Methods that include the word "Local" operate on the Open Banking Connector database but not at the external bank.

Object type | Methods | Request type| Response type
 --- | --- | ---| ---
DomesticPaymentConsents|GetLocalAsync <br/> PostAsync <br/> DeleteLocalAsync <br/> GetAsync <br/> GetFundsConfirmationAsync|[Consent](../../../src/OpenBanking.Library.Connector/Models/Public/PaymentInitiation/Request/DomesticPaymentConsent.cs)|[Consent](../../../src/OpenBanking.Library.Connector/Models/Public/PaymentInitiation/Response/DomesticPaymentConsentResponse.cs)
DomesticPayments| GetAsync<br/> PostAsync <br/> GetLocalAsync <br/> DeleteAsync |[Payment](../../../src/OpenBanking.Library.Connector/Models/Public/PaymentInitiation/Request/DomesticPayment.cs)|[Payment](../../../src/OpenBanking.Library.Connector/Models/Public/PaymentInitiation/Response/DomesticPaymentResponse.cs)
AuthContexts| |[Auth](../../../src/OpenBanking.Library.Connector/Models/Public/PaymentInitiation/Request/DomesticPaymentConsentAuthContext.cs)|[Auth](../../../src/OpenBanking.Library.Connector/Models/Public/PaymentInitiation/Response/DomesticPaymentConsentAuthContextResponse.cs)
