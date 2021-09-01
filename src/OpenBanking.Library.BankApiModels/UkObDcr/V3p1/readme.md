# AutoRest Config

This file contains the configuration for generating My API from the OpenAPI specification.

> see https://aka.ms/autorest

Before running AutoRest, please clone OpenBankingUK/read-write-api-specs from GitHub (note not DCR repo) and check out
tag "v3.1-RC1". Then adjust input file path below as appropriate and run "autorest --legacy" in this directory.

``` yaml
input-file:
  - C:\Repos\OBUK.read-write-api-specs\dist\client-registration-swagger.yaml
csharp:
  namespace: FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1
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