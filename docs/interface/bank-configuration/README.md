# Bank Configuration

The bank configuration interface allows you to set up a bank in Open Banking connector including its registration and functional (e.g. PISP) APIs.

The bank configuration interface allows creation and use of three types of persisted objects.

A Local only object type denotes the object stores the information in the local database.<br/>
If the object is not local this means the object communicates with the bank.

Object Type | Parent Type | Local only | Description | 
--- | --- | --- | ---
Bank | N/A | Yes |
BankApiInformationObject | Bank| Yes |
BankRegistration | Bank| No |


# Methods supported

Objects | Methods |Request Type|Response Type
 --- | --- | ---| ---
 Bank|GetLocalAsync <br/> PostLocalAsync <br/> DeleteLocalAsync <br/>|[Bank](./../../../src/OpenBanking.Library.Connector/Models/Public/Request/Bank.cs#L23)|[Bank](./../../../src/OpenBanking.Library.Connector/Models/Public/Response/BankResponse.cs#L18)
BankRegistration| PostAsync <br/> GetLocalAsync <br/> DeleteAsync |[BankRegistration](./../../../src/OpenBanking.Library.Connector/Models/Public/Request/BankRegistration.cs#L17)|[BankRegistration](./../../../src/OpenBanking.Library.Connector/Models/Public/Response/BankRegistrationResponse.cs#L22)
BankApiInformation| GetLocalAsyn <br/> PostLocalAsync <br/> DeleteLocalAsync |[BankApiInformation](./../../../src/OpenBanking.Library.Connector/Models/Public/Request/BankApiInformation.cs#L14)|[BankApiInformation](./../../../src/OpenBanking.Library.Connector/Models/Public/Response/BankApiInformationResponse.cs#L20)
