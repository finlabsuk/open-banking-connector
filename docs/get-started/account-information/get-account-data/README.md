# Get account data

You can get account data once you have created and authorised an account access consent. The ID of the account access consent must be supplied as a header so Open Banking Connector can supply/obtain the correct token for the bank request.

In this section, we first get a user's accounts.

Once their accounts (and, crucially, bank account IDs) are obtained, we are then able to get account-specific data.

We here show getting balances for first user account obtained as an example of how to get account-specific data. Transactions, standing orders, etc may be obtained similarly.

## Get accounts

We get a user's accounts by specifying the ID of the authorised consent.

The data returned will include the external (bank) account IDs which can be used subsequently to retrieve account-specific data.

### Example with Postman and OBIE Modelo model bank

![Alt text](get-accounts.png)

## Get balances

Using an external (bank) account ID obtained from user accounts, we can obtain the balances associated with an account.

### Example with Postman and OBIE Modelo model bank

![Alt text](get-account-balances.png)