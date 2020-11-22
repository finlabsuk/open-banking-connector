# Open Banking Connector

Open Banking Connector (OBC) is open-source software that manages your connections to UK Open Banking APIs.

OBC can be used as a connection layer (library) by backend software to connect to UK banks or standalone as a system for testing "liveness" of bank APIs.

Specifically the software:
* Handles creation and management of bank clients
* Manages user tokens associated with consents
* Supports API mapping for banks using older Open Banking API versions
* Supports overrides and specialised behaviour for compatibility with banks

The main purpose of OBC is to save you the time and expense of building your own Open Banking connectivity stack.

A key part of OBC are bank profiles which allow creation of OBC request objects for OBC suited to particular UK banks.

OBC also includes bank tests which test real bank API endpoints using OBC. 

Please note that this software is currently in development and our initial focus is the UK Open Banking Payment Initiation API.

For more information please contact Finnovation Labs. We would love to hear from you!

Developer documentation is [here](docs/README.md).

Finnovation Labs wishes to thank NewDay Cards for support given to this project.
