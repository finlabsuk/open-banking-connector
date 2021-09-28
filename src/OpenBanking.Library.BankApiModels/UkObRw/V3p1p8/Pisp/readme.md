# AutoRest Config

This file contains the configuration for generating My API from the OpenAPI specification.

> see https://aka.ms/autorest
> 
``` yaml
input-file:
  - ./swagger.json
csharp:
  namespace: FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Pisp
  use-datetimeoffset: true
output-folder: .

directive:
  - from: swagger-document # do it globally 
    where: $.paths.*.* 
    transform:transform:
      $["operationId"] = `${$path[$path.length - 1]}  ${$.tags[0]}`;
  - from: swagger-document # do it globally 
    where: $..*[?(@.enum)]
    transform: >-
      let name = "";
      for (const pathElement of $path) {
        if (pathElement == "items")
        {
          name += "Item";
        }
        else if (pathElement && pathElement != "properties" && pathElement != "definitions" && pathElement != "components" && pathElement != "schemas")
        {
          name += pathElement;
        }
      }
      name += "Enum";
      $["x-ms-enum"] = { name: name, modelAsString: false };
```