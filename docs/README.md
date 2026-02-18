# Overview

## Introduction

Open Banking Connector is a *fully open-source*, *free-to-use* UK Open Banking client (connection layer) that simplifies
connections to UK Open Banking APIs.

It is self-deployed (runs as a component in your backend infrastructure) and released via a Docker image built entirely
from the public source code. Because it runs locally, it has great performance (including low latency) as well as
privacy/security benefits because bank data is not passed through another organisation. And fully-transparent source
code allows you to see what the code is doing and even build it yourself using the included Dockerfile.

It provides internal HTTP [APIs](./apis/README.md) based on
the [UK Open Banking APIs](https://standards.openbanking.org.uk/specifications/) but with two major simplifications:

1. *bank profiles* allow Open Banking Connector to support differing bank implementations and behaviours including
   non-uniform spec versions and quirks/unexpected API behaviours. This means you see a *single, unified multi-bank API*
   with these issues taken care of saving the considerable effort required for individual bank integrations. (Note: we
   do not however adjust or normalise bank data itself in order to preserve maximum processing flexibility.)
2. security profile (auth) concerns including access token acquisition and management are taken care of transparently (
   invisibly) isolating these from the rest of your backend and saving you the effort of implementing these.

It is designed for use in the cloud and is ideally suited for deployment in a Kubernetes cluster.

## Uses

Open Banking Connector can be used:

- as a connectivity layer enabling your Open Banking application to connect to UK bank APIs
- as a tool for creating and managing bank registrations (OAuth2 clients) for use by other software (perhaps your own).

The Open Banking Connector repo also contains a .NET project for bank tests. This is used in the development of Open
Banking Connector as a regression test suite. Please contact us if you are interested in using this.

## Requirements

To access UK Open Banking APIs, you will need to have an appropriate authorisation/registration with the FCA which gives
you access to the UK Open Banking directory. To use bank sandboxes, you simply need to register with OBIE to use the UK
Open Banking directory sandbox.

Open Banking Connector needs to be hosted in your back-end and requires access to a database and configuration. Please
see [infrastructure requirements](./guide/README.md#what-are-open-banking-connectors-infrastructure-requirements) for
more information.

## Philosophy and business model

Open Banking Connector was conceived following difficulties encountered when integrating with different bank APIs. It
seemed pointless that everyone should have the burden of individually integrating with each bank - why couldn't we all
contribute to a community effort instead?

In particular, *bank profiles* (which capture bank-specific behaviours) were created so that new ones could be added and
existing ones updated by the community, perhaps even by the banks themselves.

The product is fully free-to-use but we ask customers to fund new features and bank integrations either by direct code
contributions (please contact us) or by contracting us (Finnovation Labs) to do the work.

We also offer a support and maintenance product for customers running Open Banking Connector in production as well as a sponsorship programme for those wishing to further support Open Banking Connector.
