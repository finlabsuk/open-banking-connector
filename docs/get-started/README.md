# Get Started

Open Banking Connector is a collection of C# class libraries provided as packages in Nuget.

## Overview

In order to use Open Banking Connector in your C# Open Banking application you will need to:
- [Select a package from Nuget](#select-a-nuget-package) and include it in your application

The following additional setup steps are also required and might differ between development, staging and production environments:
- [Install the UK Open Banking root certificate](#install-the-uk-open-banking-root-certificate)
- [Configure settings](#configure-settings) for Open Banking Connector including a database connection string
- [Configure secrets](#configure-secrets) for Open Banking Connector including software statements and Open Banking certificates

## Select a Nuget package

The following Open Banking Connector packages are available and you should select one based on the type of app that will include it.

App Type | Nuget Package
--- | ---
"Plain" app (no .NET Generic Host) | [FinnovationLabs.OpenBanking.Library.Connector](https://www.nuget.org/packages/FinnovationLabs.OpenBanking.Library.Connector)
.NET Generic Host app | [FinnovationLabs.OpenBanking.Library.Connector.GenericHost](https://www.nuget.org/packages/FinnovationLabs.OpenBanking.Library.Connector.GenericHost)
.NET Generic Host app (Azure-hosted)
ASP.NET Core app | [FinnovationLabs.OpenBanking.Library.Connector.Web](https://www.nuget.org/packages/FinnovationLabs.OpenBanking.Library.Connector.Web)

You will also need to consider how you will handle bank authentication redirects. You can handle these externally to Open Banking Connector and provide authentication results via its Fluent interface. Or, if you use FinnovationLabs.OpenBanking.Library.Connector.Web in an ASP.NET Core app, Open Banking Connector provides web endpoints to absorb authentication redirects.

## Install the UK Open Banking root certificate

For a development environment, please follow instructions [here](./dev-environment/install-ob-root-cert.md).

For staging/production environment, please follow your internal procedures.
## Configure settings

Open Banking Connector is configured by settings which e.g. provide a database connection string and limits which SoftwareStatementProfiles are loaded.

More information is provided [here.](./configure-settings.md)
## Configure secrets

Open Banking Connector is also configured by secrets which provide sensitive information such as SoftwareStatementProfiles and ObCertificateProfiles.

More information is provided here.



