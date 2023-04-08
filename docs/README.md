# Overview

## Introduction

Open Banking Connector is a *fully open-source*, *free-to-use* UK Open Banking client (connection layer) that simplifies connections to UK Open Banking APIs.

It is self-deployed (runs as a component in your backend infrastructure) and released via a Docker image built entirely from the public source code. Because it runs locally, it has great performance (including low latency) as well as privacy/security benefits because PII data is not passed through another organisation. And fully-transparent source code allows you to see what the code is doing and even build it yourself using the included Dockerfile.

It provides an HTTP API based on the [UK Open Banking API](https://standards.openbanking.org.uk/specifications/) but with two major simplifications:

1. *bank profiles* allow Open Banking Connector to support differing bank implementations and behaviours including non-uniform spec versions and quirks/unexpected behaviours. This means you see a *single, multi-bank API* with these issues taken care of saving the considerable effort required for individual bank integrations.
2. security profile (auth) concerns including access token acquisition and management are taken care of transparently (invisibly) isolating these from the rest of your backend and saving you the effort of implementing these.

It is designed for use in the cloud and has been tested in production with AWS Elastic Kubernetes Service (EKS).

## Uses

Open Banking Connector can be used:

- as a connectivity layer enabling your Open Banking application to connect to UK bank APIs
- as a tool for creating and managing bank registrations (OAuth2 clients) for use by other software (perhaps your own).

The Open Banking Connector repo also contains a .NET project for bank tests. This is used in development of Open Banking Connector as a regression test suite. Please contact us if you are interested in using this.

## Requirements

To access UK Open Banking APIs, you will need to have an appropriate authorisation/registration with the FCA which gives you access to the UK Open Banking directory. To use bank sandboxes, you simply need to register with OBIE to use the UK Open Banking directory sandbox.

To host Open Banking Connector in your backend infrastructure, you will need:

- a host (e.g. VM or Kubernetes cluster) to run the Open Banking Connector Docker image
- a relational database. Currently PosgreSQL is supported but, as Open Banking Connector uses EF Core (Microsoft's .NET ORM), other database integrations should be relatively easy to add (for example, SQLite is supported for development).
- a means of supplying configuration/secrets to Open Banking Connector container. Environment variables can be used and, in the case of secrets, could be e.g. dynamically supplied from a key secret vault. Parameters from AWS SSM Parameter Store are supported and we are open to adding other cloud configuration sources as required. [ASP.NET Core user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#secret-manager) are supported for development. 

## Philosophy and business model

Open Banking Connector was conceived following difficulties encountered when integrating with different bank APIs. It seemed pointless that everyone should have the burden of individually integrating with each bank - why couldn't we all contribute to a community effort instead?

In particular, *bank profiles* (which capture bank-specific behaviours) were created so that new ones could be added and existing ones updated by the community, perhaps even by the banks themselves.

The product is fully free-to-use but we ask customers to fund new features and bank integrations either by direct code contributions (please contact us) or by contracting us (Finnovation Labs) to do the work.

Finnovation Labs also offers paid support, training and consultancy services.

## Documentation

This documentation is published on the [docs website](https://docs.openbankingconnector.io/) based on Markdown source code in the [docs directory of the GitHub repo](https://github.com/finlabsuk/open-banking-connector/tree/master/docs).

Please raise a [GitHub issue](https://github.com/finlabsuk/open-banking-connector/issues) if you experience any problems or have any feedback.
