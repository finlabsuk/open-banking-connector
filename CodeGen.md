
## OpenAPI C# Model Code Generation

We use C# model code generated from OpenAPI files describing UK Open Banking APIs.

### OpenAPI file sources

OpenAPI files for UK Open Banking Read/Write Data APIs may be obtained here: https://github.com/OpenBankingUK/read-write-api-specs. Tags are used to denote releases.

OpenAPI files for UK Open Banking Dynamic Client Registration APIs may be obtained here: https://github.com/OpenBankingUK/client-registration-api-specs. Tags are used to denote releases.

### Code Generation

We use [OpenAPI Generator](https://openapi-generator.tech/) to generate C# model code from OpenAPI files.

#### Installing OpenAPI Generator


It is suggested to install OpenAPI Generator locally as online versions have proved unreliable. See [latest installation instructions](https://openapi-generator.tech/docs/installation).

Example install command (PowerShell):
```PowerShell
Invoke-WebRequest -OutFile openapi-generator-cli.jar https://repo1.maven.org/maven2/org/openapitools/openapi-generator-cli/4.3.1/openapi-generator-cli-4.3.1.jar
```

You may need to install Java (e.g. OpenJDK) to enable it to run.

#### Generation of API Model Code

Please refer to the [full usage notes](https://openapi-generator.tech/docs/usage) for OpenAPI Generator.

For a given API, we auto-generate a client then extract the model code.

Here is some example PowerShell code for generating a Payment Initiation API client:

```PowerShell
$specVersion = "PaymentInitiation.V3p1p1"
$specFile = "C:\Repos\OBUK.read-write-api-specs\dist\payment-initiation-openapi.yaml"
mkdir $specVersion
java.exe -jar openapi-generator-cli.jar generate `
-g csharp-netcore `
-i $specFile `
-p "packageName=OpenBanking.Library.Connector.ObModels.$specVersion,netCoreProjectFile=true,targetFramework=netstandard2.1,useDateTimeOffset=true" `
-o ./$specVersion
```

We can then copy the contents of ```.\$specVersion\src\OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1\Model``` into the project.
