# OpenBanking.Service.Payments

A sample Azure Function project demonstrating OpenBankingConnector.Net. It is intended to guide users in the library's use on a local development machine. As such it is a simplified form and not intended to be production ready.

## Runtime

* Azure Function v2
* .Net core 2  (Core 3 once full support is applied)
* C# 7

## Endpoints

See [here](Suggested_API_endpoints.md)

## Dependencies
| Type | Dependency | Notes |
| -- | -- | -- | 
| Database | Cosmos DB | CosmosDB is the defacto elastic high availability database in Azure. 
| Instrumentation | Azure Application Insights | App Insights is the standard application instrumentation platform within Azure.
| Keystore | Azure Key Vault | Standard key storage platform within Azure.
