#### AutoRest Config

This file contains the configuration for generating My API from the OpenAPI specification.

> see https://aka.ms/autorest

``` yaml
input-file: C:\Repos\OBUK.client-registration-api-specs\dist\client-registration-swagger.yaml
csharp:
  namespace: FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2
  use-datetimeoffset: true
output-folder: .

directive:
  - from: swagger-document # do it globally 
    where: $.paths.*.* 
    transform: $.operationId = `OBC2_${$.tags[0]}`
``` 