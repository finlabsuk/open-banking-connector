# AutoRest Config

This file contains the configuration for generating My API from the OpenAPI specification.

> see https://aka.ms/autorest

``` yaml
input-file:
  - C:\Repos\OBUK.read-write-api-specs\dist\swagger\payment-initiation-swagger.yaml
csharp:
  namespace: FinnovationLabs.OpenBanking.Library.Connector.UkRwApi.V3p1p6.PaymentInitiation
  use-datetimeoffset: true
output-folder: .

directive:
  - from: swagger-document # do it globally 
    where: $.paths.*.* 
    transform: $.operationId = `OBC2_${$.tags[0]}`
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