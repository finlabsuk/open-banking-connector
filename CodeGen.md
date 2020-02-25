
## OpenAPI Code generation

This project uses code generated against Open Banking Swagger documentation.

The latest Open Banking OpenAPI specification are [referenced here](https://openbanking.atlassian.net/wiki/spaces/DZ/pages/1077805743/Payment+Initiation+API+Specification+-+v3.1.2).

Various on-line API stub generators exist, however they have been proved to be unstable. A local copy of the ```openapi-generator CLI``` is available at ```\tools\openapi-gen```. This requires JVM to be locally installed.

### Getting the latest CLI gen
Installation [notes are matained by OpenAPI](https://openapi-generator.tech/docs/installation)

### Generating 
Please refer to the [full usage notes](https://openapi-generator.tech/docs/usage)

```java -jar .\openapi-generator-cli.jar generate -g csharp -i https://raw.githubusercontent.com/OpenBankingUK/read-write-api-specs/v3.1.2-RC1/dist/payment-initiation-swagger.json -o <output directory>```

```https://raw.githubusercontent.com/OpenBankingUK/read-write-api-specs/v3.1.2-RC1/dist/payment-initiation-swagger.json```: the full URL of the OpenAPI specification

```<output directory>```: the new project's destination folder 


The generated tests are mostly stubs. Those that aren't (e.g. API tests) do not work due to singleton object entanglement.

```OpenBanking.Library.Connector.Model.csproj``` contains the generated Model folder, with namespaces adjusted to suit project & assembly naming.

