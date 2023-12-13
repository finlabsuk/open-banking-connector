# Get started

In this section, we describe how to get started with Open Banking Connector.

We cover:

1. how to run the Open Banking Connector container
2. how to add a software statement and certificates
3. how to create a bank registration
4. how to use an account access consent to access account data

## Run the Open Banking Connector container

Before running the Open Banking Connector container, please [set up basic configuration](./set-up-basic-configuration/README.md)

You can then [pull and run the Open Banking Connector Docker image](./run-docker-image/README.md)

## Add a software statement and certificates

You will need to create:

- a software statement
- a signed OBWAC transport certificate and key
- a signed OBSeal signing certificate and key
 
using the [UK Open Banking Directory](https://directory.openbanking.org.uk/s/login/) and following the instructions there.

Once you have created these, please [add them to Open Banking Connector](./add-software-statement-and-certificates/README.md).

## Create a bank registration

Before using a bank's APIs you need to create a relationship with a bank by [creating a bank registration](./create-bank-registration/README.md).

## Use an account access consent to access account data

With a bank registration you can [create and authorise an account access consent](./create-and-authorise-account-access-consent/README.md).

Once an account access consent is authorised, you can use it to [get account data](./get-account-data/README.md).
