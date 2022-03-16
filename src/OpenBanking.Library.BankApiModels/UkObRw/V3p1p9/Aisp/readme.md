# Bank API Model Generation Info

This file contains configuration for generating bank API models using AutoRest.

> see https://aka.ms/autorest

Please install AutoRest v3 and run `autorest --reset` then `autorest --latest` in this directory to generate model files.

``` yaml
input-file:
  - ./account-info-openapi-modified.yaml # modified AISP v3.1.9r5 spec
csharp:
  namespace: FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp
  use-datetimeoffset: true
output-folder: .

directive:
  - from: openapi-document # do it globally 
    where: $..*[?(@.enum)]
    debug: true
    transform: >-
      /* Strip out verbosity from name and append "Enum" */
      let name = "";
      for (const pathElement of $path) {
        if (pathElement && pathElement != "properties" && pathElement != "definitions" && pathElement != "components" && pathElement != "schemas" && pathElement != "items")
        {
          name += pathElement;
        }
      }
      name += "Enum";
      $lib.log(name);
      /* Use real C# Enums in model as well as updated name */     
      $["x-ms-enum"] = { name: name, modelAsString: false };

``` 