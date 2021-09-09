# Open Banking Connector

## Introduction

Open Banking Connector is open-source software that manages and simplifies connections to UK Open Banking APIs. Its core is a collection of C# class libraries provided as [packages in Nuget](./get-started/README.md#select-a-nuget-package). It is designed for use with modern C# applications including "plain" apps (those without a [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host)) and .NET Generic Host-based/ASP.NET Core apps.

Its main purposes are:
- to manage security aspects of Open Banking API accesses including registrations and tokens
- to absorb bank differences including API version differences and behavioural/implementation differences so as much as possible the *same API calls may be used for all banks*

It is currently focussed on support for domestic payments in UK Open Banking but has been designed for future extension to all UK Open Banking APIs.

## Uses

Open Banking Connector can be used:

* as a connection layer allowing .NET backend software to connect to UK banks
* as a reference platform for creating and managing bank registrations used by other software.
* as a standalone bank test suite. Its built-in bank tests test Open Banking APIs in UK bank sandboxes and include a consent authoriser that automates web page user consent.

To support the above, Open Banking Connector contains Bank Profiles for UK banks which provide reference configurations as used in the bank tests. Anyone is invited to contribute and update these profiles to extend the bank coverage of OBC and increase the value of this resource.

## Compatibility

Open Banking Connector is a set of .NET Core libraries. They either target .NET Standard 2.1 or .NET Core 3.1, i.e. the current .NET Core LTS release.

Going forward, we plan to evolve the `master` branch of Open Banking Connector (where active development takes place) to track the current LTS version of .NET approximately six months after release. Hence the next planned update is to .NET 6 approximately six months after its release.

Open Banking Connector is designed to run in the cloud as part of two kinds of .NET app:
- Apps based on [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host) (which may or may not be web apps but utilise features such as DI and Configuration)
- Apps not based on .NET Generic Host ("plain apps")

It requires both a relational database and key secret vault. We use EF Core for database access and test locally using SQLite. We use the local [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows) as a key vault for local testing.

We have designed OBC to enable DB and key secret support on all main cloud platforms and are currently preparing to test on Azure using Azure Key Vault and Azure SQL.
## Interface

Open Banking Connector provides an intuitive, Fluent REST-inspired interface. The idea is to have
a single, standardised interface that works with multiple banks. More information may be found [here](./interface/README.md).

## Open Banking API support

Open Banking Connector supports multiple Open Banking APIs (depends on bank support) and provides a Fluent interface based on the latest supported version. More information may be found [here](./supported-open-banking-apis.md).

## Documentation

This documentation is available both in the GitHub repo and on the [docs website](https://docs.openbankingconnector.io/).

The documentation is divided into the following sections:

- [Getting Started](get-started)
- [Architecture](architecture)
- [Client Interface](interface)
- [How-to](how-to)


To generate the docs website based on the contents of a local open-banking-connector repo (i.e. latest edits), you will need to install Python 3 and Material for MkDocs (mkdocs-material).

Assuming you have installed Python, you can install Material for MkDocs as follows:
```powershell
pip install mkdocs-material # if mkdocs-material not installed
```

To deploy the website locally for inspection and testing, please `cd` to the OBC repo root and run the following command:
```powershell
mkdocs serve
```
This will return the URL you can use to see the website.

If you have permissions, you can update the public website on GitHub Pages using
```powershell
mkdocs gh-deploy -m "Test deployment" -r publicRemote # adjust commit message and remote as required
```
