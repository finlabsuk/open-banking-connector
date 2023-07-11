# Bank API models

This library contains bank API models generated using [AutoRest v3](https://github.com/Azure/autorest) from Swagger/Open
API source files.

## Source files

The Swagger/Open API source files come from the following two repos:

- https://github.com/OpenBankingUK/client-registration-api-specs
- https://github.com/OpenBankingUK/read-write-api-specs

whose copyright holder is the UK Open Banking Implementation Entity and which are made available under the MIT licence (
see footer of this [page](https://standards.openbanking.org.uk/api-specifications/)).

## Model generation

To generate models on a Windows machine,
- install Node.js and npm
- install AutoRest v3:
  ```powershell
  npm install -g autorest
  ```
- navigate to target current directory, e.g.
  ```powershell
  cd src/OpenBanking.Library.BankApiModels/UkObRw/V3p1p11/Aisp
  ```
- generate model files:
  ```powershell
  autorest --reset
  autorest --use=@autorest/csharp@3.0.0-beta.20220317.3
  ```





## Misc

The first version of the file `src\OpenBanking.Library.BankApiModels\UkObRw\V3p1p8\Pisp\swagger.json` was created by
converting https://raw.githubusercontent.com/OpenBankingUK/read-write-api-specs/v3.1.8r4/dist/openapi/payment-initiation-openapi.json
from Open API v3 to v2 using the https://github.com/LucyBot-Inc/api-spec-converter tool.
