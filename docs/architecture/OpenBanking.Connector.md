# OpenBanking.Library.Connector.Payments

OpenBanking.Library.Connector.Payments is the main deliverable assembly for OBC.NET.

## Features:

* .Net Standard library for direct integration into .Net Core/Xamarin applications and services
* Heterogenous Bank PISP APIs are abstracted to a canonical model
* Fluent, domain-driven client interface
* Interface abstractions for external services (DB, key store, instrumentation, etc)
* Dependency injection following ASP.NET Core DI patterns

## Implementation:

* .NET Standard 2 
* C# 7 
* We intend to upgrade to C# 8 in the near future.

## Namespaces:

The main package's root namespace is ```FinnovationLabs.OpenBanking.Library.Connector```.

| Namespace | Purpose |
| -- | --- | 
| .Configuration | Run-time configuration | 
| .Http | HTTP utilities | 
| .Instrumentation | Instrumentation abstractions | 
| .Json | JSON utilities | 
| .Model | Entity declarations. These describe the structure of messages, not their behaviour. | 
| .Model.Public | All client-facing entity types. These are canonical models that will cover versions | 
| .Model.Payments | Internal, auto-generated entity code. These are all sourced from OpenBanking PISP model OpenAPIs | 
| .Model.Payments.V3_1_0 | OpenBanking OpenAPI v3.1.0 | 
| .Model.Payments.V3_1_1 | OpenBanking OpenAPI v3.1.1 | 
| .Model.Payments.V3_1_2 | OpenBanking OpenAPI v3.1.2 | 
| .Payments | Payments behaviour implementation | 
| .Security | JWT/JWS utilities |  

## Usage
[See here](client_interface/README.md)

