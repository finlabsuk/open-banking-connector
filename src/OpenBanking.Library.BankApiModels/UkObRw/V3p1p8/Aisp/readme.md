# Bank API Model Generation Info

This file contains configuration for generating bank API models.

Please install [AutoRest](https://github.com/Azure/autorest) and run command `autorest` in this directory to generate model files.

``` yaml
input-file:
  - ./account-info-openapi.yaml
csharp:
  namespace: FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp
  use-datetimeoffset: true
output-folder: .

directive:
  - from: openapi-document # do it globally 
    where: $..*[?(@.enum)]
    debug: true
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
      $lib.log($name);     
      $["x-ms-enum"] = { name: name, modelAsString: false };
  - from: openapi-document
    where: $..*[?(@.type == 'object')]
    debug: true
    transform: >-
      let lengthCount = 0;
      let lastElement = "";
      for (const pathElement of $path) {
        lengthCount += pathElement.length;
        if (pathElement != "items")
        {
          lastElement = pathElement;
        }
      }
      $lib.log(`Next (${$path.length}):`);
      $lib.log($path.join("__"));
      $lib.log($path.length);
      let name = "X" + lastElement + Math.floor(Math.random()*1001);
      if (lengthCount > 100) {
        name +=  "long";
      }
      /*$["x-ms-client-name"] = name;
      $lib.log($["x-ms-client-name"]);*/
``` 