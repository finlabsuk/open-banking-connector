# OpenBanking.Library.Connector.IntegrationTests

OpenBanking.Library.Connector.IntegrationTests is a single assembly containing all integration tests. These tests will cover:

* integration testing against our internal sandbox mock APIs 
* integration testing against PISP partners' sandbox APIs
* QoS checks against PISP partner sandbox APIs & optionally live PISP APIs

## Dependencies:
* PISP sandbox mocks running getsandbox mocks
* PISP Partner sandboxes
* PISP partner sandboxes

## Runtime:
* Continual testing against our internal sandbox mock APIs - isolated tests
* On-demand execution against PISP Partner sandbox & live APIs

## Implementation:
* .NET Core 2 / C# 7
* XUnit/BDD tests 

The tests' first facade is against the package’s C# code and not HTTP endpoints. 

Specflow is rules out: it has licence requirements, and we want to make our source as widely usable as possible.

## Configuration:
* SSAs
* Certs
* Target API URLs
* Retained within the test project itself

## Namespaces

| Name | Purpose |
| -- | -- | 
| LocalMockTests | Tests against internal mocks |
| PispTests | Tests against PISP Sandboxes
| PispHealthTests | Tests against PISP Sandbox & API to gauge health and discover API changes
