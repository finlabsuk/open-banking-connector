# Introduction

## Overview

Welcome to the documentation for Open Banking Connector. This documentation is available both in the GitHub repo and on the [docs website](https://docs.openbankingconnector.io/).

The documentation is divided into the following sections:

- [Getting Started](getting-started)
- [Architecture](architecture)
- [Client Interface](client_interface)
- [How-to](how-to)

## Compatibility

Open Banking Connector (OBC) currently targets .NET Standard 2.1. .NET standard 2.1 is supported by .NET Core 3.1 which is the current LTS release.

Going forward, we plan to evolve the `master` branch of OBC (where active development takes place) to track the current LTS version of .NET approximately six months after release. Hence the next planned update is to .NET 6 approximately six months after its release.

OBC is designed to run in the cloud as part of two kinds of .NET app:
- Apps based on [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host) (which may or may not be web apps but utilise features such as DI and Configuration)
- Apps not based on .NET Generic Host ("plain apps")

It requires both a relational database and key secret vault. We use EF Core for database access and test locally using SQLite. We use the local [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows) as a key vault for local testing.

We have designed OBC to enable DB and key secret support on all main cloud platforms and are currently preparing to test on Azure using Azure Key Vault and Azure SQL.

## Open Banking Support

Open Banking Connector is currently focussed on supporting UK Open Banking payments. We later hope to extend the library to support account information and other Open Banking APIs.

We currently support creation and storage of client registrations with banks and functionality related to making domestic payments. This functionality requires banks to support Open Banking APIs.

OBC currently requires bank support of the following standards:

- [UK Open Banking DCR API](https://openbankinguk.github.io/dcr-docs-pub/)
    - v3.2 or v3.3
- [UK Open Banking Read-Write API](https://openbankinguk.github.io/read-write-api-site3/)
    - Payment Initation API v3.1.4 or v3.1.6 (v3.1.2 models are also included so v3.1.2 support can be easily added if there is demand)
    
## Bank Profiles

Open Banking Connector contains a set of bank profiles which represent the configuration necessary to connect to a bank. These are stored in 


## Bank Tests


## Website Generation

To generate the docs website locally based on current edits, please ensure you have Python 3 installed and then open a terminal and `cd` to the OBC repo root. Run the following commands:
```powershell
pip install mkdocs-material # if mkdocs-material not installed
mkdocs serve
```

If you have permissions you can update the public website on Git Pages using
```powershell
mkdocs gh-deploy -m "Test deployment" -r upstream # adjust commit message and remote as required
```








