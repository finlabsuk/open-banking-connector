# AutoRest Config

This file contains the configuration for generating My API from the OpenAPI specification.

> see https://aka.ms/autorest

``` yaml
input-file:
  - C:\Repos\OBUK.client-registration-api-specs\dist\client-registration-swagger.yaml
csharp:
  namespace: FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3
  use-datetimeoffset: true
output-folder: .

directive:
  - from: swagger-document # do it globally 
    where: $.paths.*.* 
    transform: $.operationId = `OBC_${$.tags[0]}`
  - from: swagger-document # do it globally 
    where: $..*[?(@.enum)]
    transform: >-
      let name = "";
      for (const pathElement of $path) {
        if (pathElement == "items")
        {
          name += "Item";
        }
        else if (pathElement && pathElement != "properties" && pathElement != "definitions")
        {
          name += pathElement;
        }
      }
      name += "Enum";
      $["x-ms-enum"] = { name: name, modelAsString: false };
``` 