# AutoRest Config

This file contains configuration for generating My API from the OpenAPI specification.

> see https://aka.ms/autorest

Please run `autorest --legacy` in this directory to generate model files.

``` yaml
input-file:
  - ./vrp-swagger.json
csharp:
  namespace: FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp
  use-datetimeoffset: true
output-folder: .

directive:
  - from: swagger-document # do it globally 
    where: $.paths.*.* 
    transform:
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