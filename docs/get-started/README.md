# Get started

In this section, we describe how to get started with Open Banking Connector.

We first show how to get Open Banking Connector up and running, then demonstrate how to access bank APIs using Open Banking Connector.

## Get Open Banking Connector up and running

To get Open Banking Connector up and running, you will need to do the following:
1. [set up configuration and secrets](./set-up-configuration-and-secrets/README.md)
2. [pull and run the Open Banking Connector Docker image](./run-docker-image/README.md)

## Use Open Banking Connector to access bank APIs

We here walk through the basics of common Open Banking operations and provide examples using the OBIE Modelo model bank.

### Create a bank registration

The first thing to do with Open Banking Connector is to create a relationship with a bank by [creating a bank registration](./create-bank-registration/README.md).

### Use an account access consent to access account data

With a bank registration you can [create and authorise an account access consent](./create-and-authorise-account-access-consent/README.md).

Once an account access consent is authorised, you can use it to [get account data](./get-account-data/README.md).
