# Get started

On this page, we describe how to get started with Open Banking Connector.

The basic steps provided here will get you up and running with Open Banking Connector:

1. [Set up basic configuration](#set-up-basic-configuration)
2. [Run the Open Banking Connector container](#run-the-open-banking-connector-container)
3. [Add a software statement and certificates](#add-a-software-statement-and-certificates)
4. [Create a bank registration](#create-a-bank-registration)
5. [Use an account access consent to access account data](#use-an-account-access-consent-to-access-account-data)
6. [Use a domestic payment consent to make a payment](#use-a-domestic-payment-consent-to-make-a-payment)
7. [Use a domestic VRP consent to make a payment](#use-a-domestic-vrp-consent-to-make-a-payment)

## Set up basic configuration

To run Open Banking Connector, some basic configuration is required including database settings.
Please [set this up](./set-up-basic-configuration/README.md#set-up-basic-configuration).

## Run the Open Banking Connector container

Using the basic configuration from the previous step, you
can [pull and run the Open Banking Connector Docker container](./run-docker-container/README.md).

## Add a software statement and certificates

In order to communicate with banks, you will need:

- a software statement
- a signed OB WAC transport certificate and key (used for mutual TLS when communicating with banks)
- a signed OBSeal signing certificate and key (used to sign and validate JWTs sent to banks)

These can be created using the [UK Open Banking Directory](https://directory.openbanking.org.uk/s/login/) and following
the instructions there. (Note that the sandbox and production environments of the Open Banking Directory are separate so
please create these in the right environment and in both environments if connecting to both sandbox and production bank
APIs).

Please create these and [add them to Open Banking Connector](./add-software-statement-and-certificates/README.md).

## Create a bank registration

Before using a bank's APIs you need to create a relationship with a bank
by [creating a bank registration](./create-bank-registration/README.md).

## Use an account access consent to access account data

With a bank registration that supports account information retrieval, you
can [create and authorise an account access consent](./account-access-consents/create-and-authorise-account-access-consent/README.md).

Once an account access consent is authorised, you can use it
to [get account data](./account-access-consents/get-account-data/README.md).

## Use a domestic payment consent to make a payment

With a bank registration that supports payments, you
can [create and authorise a domestic payment consent](./domestic-payment-consents/create-and-authorise-domestic-payment-consent/README.md).

Once the domestic payment consent is authorised, you
can [confirm funds and make a payment](./domestic-payment-consents/confirm-funds-and-make-payment/README.md).

## Use a domestic VRP consent to make a payment

With a bank registration that supports payments, you
can [create and authorise a domestic VRP consent](./domestic-vrp-consents/create-and-authorise-domestic-vrp-consent/README.md).

Once the domestic VRP consent is authorised, you
can [confirm funds and make a payment](./domestic-vrp-consents/confirm-funds-and-make-payment/README.md).
