# Create a bank registration

To access Open Banking APIs provided by a bank, you need to first create a bank registration (OAuth2 client) which
establishes
a relationship between you and the bank.

To do this, first identify the *bank profile* for the bank you wish to connect to. Bank profiles are listed in
the [bank integrations table](../../bank-integrations.md).

Each bank registration requires the ID of a
previously-added [software statement](../add-software-statement-etc/README.md#add-a-software-statement-to-open-banking-connector).
By default, the software statement will be used to determine the scope (e.g. AISP, PISP or both) of the bank
registration.

You can use the bank profile and software statement ID to create a new bank registration.

Upon creation, Open Banking Connector will provide you with an ID for the bank registration which you can then use to
create consents.

If you receive SSL errors and are running Open banking connector in your localhost, you will need to [install open banking root certificates](../../contributor-info/developer-setup/install-obuk-root-certs/) .  

## Example with Postman and OBIE Modelo model bank

![Alt text](bank_reg.png)
