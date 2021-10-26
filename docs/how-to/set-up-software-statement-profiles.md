# How to: Set up software statement profiles

In UK Open Banking, a software statement is used to identify the entity (the "TPP") connecting to the bank. A software statement may for example may be created in the UK Open Banking directory.

A software statement and associated information (keys, certificates, etc) are represented in Open Banking Connector (OBC) as a *Software Statement Profile*. The data that forms a software statement profile is shown below.

Each bank registration is based on a software statment profile and so it is necessary to set one up before attempting to create a bank registration. Setting up a software statement profile is essentially a pre-requisite for use of OBC.

Software statement profiles are provided to OBC via a secret provider as they contain very sensitive information. Although multiple software statement profiles can be provided, currently only one can be *active* (in use concurrently). We hope to relax this requirement in future. 

## Development set-up and secrets structure

For development purposes Microsoft's Secret Manager (i.e. a ```secrets.json``` file) can be used (see Microsoft's [documentation](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) for their Secrets Manager). The default ```UserSecretsId``` for OBC is currently specified in applicable ```.csproj``` files as:
```xml
    <UserSecretsId>aa921213-9461-4f9e-8fec-153624ec67ad</UserSecretsId>
```
Basically to set things up with a ```secrets.json``` file you need to create a secrets folder with the name ```aa921213-9461-4f9e-8fec-153624ec67ad``` in the Microsoft ```UserSecrets``` folder for your platform and then place a ```secrets.json``` file for OBC there.

Secrets used by OBC are always strings. Software statement profiles should be specified as secrets as shown in this example ```secrets.json``` file:
```json
{
  "active-software-statement-profiles:profile-ids": "0",
  "software-statement-profile:0:signing-key-id": "mySigningKeyId",
  "software-statement-profile:0:signing-key": "-----BEGIN PRIVATE KEY-----\nmyKeyLine1\nmyKeyLine2\n-----END PRIVATE KEY-----\n",
  "software-statement-profile:0:signing-certificate": "-----BEGIN CERTIFICATE-----\nmyCertLine1\nmyCertLine2\n-----END CERTIFICATE-----\n",
  "software-statement-profile:0:transport-key": "-----BEGIN PRIVATE KEY-----\nmyKeyLine1\nmyKeyLine2\n-----END PRIVATE KEY-----\n",
  "software-statement-profile:0:transport-certificate": "-----BEGIN CERTIFICATE-----\nmyCertLine1\nmyCertLine2\n-----END CERTIFICATE-----\n",
  "software-statement-profile:0:software-statement": "mySoftwareStatementPart1.mySoftwareStatementPart2.mySoftwareStatementPart3",
  "software-statement-profile:0:default-fragment-redirect-url": "https://example.com/auth/fragment-redirect",
  "software-statement-profile:1:signing-key-id": "mySigningKeyId",
  "software-statement-profile:1:signing-key": "-----BEGIN PRIVATE KEY-----\nmyKeyLine1\nmyKeyLine2\n-----END PRIVATE KEY-----\n",
  "software-statement-profile:1:signing-certificate": "-----BEGIN CERTIFICATE-----\nmyCertLine1\nmyCertLine2\n-----END CERTIFICATE-----\n",
  "software-statement-profile:1:transport-key": "-----BEGIN PRIVATE KEY-----\nmyKeyLine1\nmyKeyLine2\n-----END PRIVATE KEY-----\n",
  "software-statement-profile:1:transport-certificate": "-----BEGIN CERTIFICATE-----\nmyCertLine1\nmyCertLine2\n-----END CERTIFICATE-----\n",
  "software-statement-profile:1:software-statement": "mySoftwareStatementPart1.mySoftwareStatementPart2.mySoftwareStatementPart3",
  "software-statement-profile:1:default-fragment-redirect-url": "https://example.com/auth/fragment-redirect"
}
```

Here two software statment profiles are specified (with IDs ```0``` and ```1```).

```active-software-statement-profiles:profile-ids``` must refer to the (for now *single*) ID of a specified software statement (i.e. either ```0``` or ```1``` in the above example) and specifies which software statement profile is active. Any attempt to (a) use a non-active software statement profile when creating a bank registration in OBC, or (b) do anything based on a bank registration created with a non-active software statement in OBC will result in an error.

# Production set-up

[TODO]