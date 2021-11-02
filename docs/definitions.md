# Definitions

This page provides defintions for commonly used terms in Open Banking Connector.

## Software Statement Profile

In UK Open Banking, a software statement is used to identify the entity (the "TPP") connecting to a bank. A software statement may be created in the UK Open Banking directory.

A software statement and associated information are represented in Open Banking Connector as a *Software Statement Profile*.

Each bank registration is based on a software statment profile and so it is necessary to set one up before attempting to create a bank registration.

Software statement profiles are provided to Open Banking Connector as key secrets.

## OB Certificate Profile

UK Open Banking uses certificates for signing and transport.

In Open Banking Connector, information relating to a signing certificate and transport certificate pair is grouped together in an *OB Certificate Profile*.

Software statement profiles are provided to Open Banking Connector as key secrets.

## Payment Initiation

In UK Open Banking, the term Payment Initiation refers to an API which allows customers to authorise single payments directly from their bank accounts.

## Variable Recurring Payments

Variable Recurring Payments refers to an API which allows third party companies (such as money management apps) to authorise recurring payments from the customer's bank account to the merchants bank account.

