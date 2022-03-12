# Bank configuration

The `BankConfiguration` interface allows you to create and read configuration for a bank in Open Banking Connector.

The interface consists of three [object types](#interface-object-types) on which [various methods](#supported-methods) are supported. Created objects correspond to records in the Open Banking Connector database.

To set up a bank, you typically will create a `Bank `and at least one `BankRegistration` and one `BankApiInformationObject` object. More information is provided [here](./set-up-a-bank.md).

## Interface object types

The thee bank configuration object types are shown in the table below. Each corresponds to a table in the Open Banking Connector database. Two of the types are local-only and are not created externally at a bank. Objects of `BankRegistration` type, however, are created externally at a bank as well as having a local database record.

Object type | Parent type | Created at bank? | Description | 
--- | --- | --- | ---
Bank | N/A | No | Base object for a bank which includes its IssuerUrl and FinancialId
BankApiInformationObject | Bank| No | Object which describes a bank's functional APIs (e.g. PISP). Use multiple objects to allow access to multiple API versions supported by a bank (e.g. multiple versions of PISP), or to test new endpoints etc.
BankReistration | Bank| Yes | Object which describes a registration (i.e. OAuth2 client registration) with a bank based on a software statement. Use multiple objects to support multiple registrations with a bank or to test a new registration etc.

## Supported methods

The table below shows the methods supported for each object type as well as the main request and response types for these methods. Methods that include the word "Local" operate on the Open Banking Connector database but not at the external bank.

Object type | Methods | Request type| Response type
 --- | --- | ---| ---
Bank|GetLocalAsync <br/> PostLocalAsync <br/> DeleteLocalAsync <br/>|[Bank](../../../src/OpenBanking.Library.Connector/Models/Public/Request/Bank.cs#L23)|[Bank](../../../src/OpenBanking.Library.Connector/Models/Public/Response/BankResponse.cs#L18)
BankRegistration| PostAsync <br/> GetLocalAsync <br/> DeleteAsync |[BankRegistration](../../../src/OpenBanking.Library.Connector/Models/Public/Request/BankRegistration.cs#L17)|[BankRegistration](../../../src/OpenBanking.Library.Connector/Models/Public/Response/BankRegistrationResponse.cs#L22)
BankApiInformation| GetLocalAsyn <br/> PostLocalAsync <br/> DeleteLocalAsync |[BankApiInformation](../../../src/OpenBanking.Library.Connector/Models/Public/Request/BankApiSet.cs#L18)|[BankApiInformation](../../../src/OpenBanking.Library.Connector/Models/Public/Response/BankApiSetResponse.cs#L21)