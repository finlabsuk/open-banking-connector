# Open Banking Connector

Open Banking Connector (OBC) is open-source software that manages connections to UK Open Banking APIs. It is currently
focussed on support for domestic payments in UK Open Banking but has been designed for future extension to all UK Open
Banking APIs.

OBC can be used:

* as a connection layer (library) allowing backend software to connect to UK banks
* as a standalone test suite. Its built-in bank tests test a bank's Open Banking APIs by POSTing a client, POSTing and
  authorising a domestic payment consent (via automated UI), GETing that domestic consent including funds confirmation,
  and then finally POSTing/GETing a domestic payment.

OBC takes care of the following:

* Appropriate handling of software statements and signing/transport keys (these are provided via key secret vault)
* Creation and management of bank OAuth2 clients ("bank registrations")
* Acquisition and management of access tokens for bank API access
* Mapping to/from and older UK Open Banking standard versions when making API calls to a bank using a previous standard
  version

OBC provides an intuitive, Fluent REST-inspired API based on UK Open Banking DCR and PISP standards. The idea is to have
a single, standardised interface that works with multiple banks.

To this end, OBC also provides public Bank Profiles showing the configuration required to connect to a given bank.
Anyone is invited to contribute and update these profiles to extend the bank coverage of OBC and make Open Banking API
compatibility and implementation status more transparent.

The main purpose of OBC is to save you the time and expense of building your own Open Banking connectivity stack and
learning the hard way all the tweaks necessary to connect to each bank.

For more information please contact Finnovation Labs. We would love to hear from you!

Developer documentation is [here](https://docs.openbankingconnector.io/).

Finnovation Labs wishes to thank NewDay Cards for support given to this project.
