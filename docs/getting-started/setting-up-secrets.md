# Setting up a software statement profile in secrets storage

OBC uses key secrets to store sensitive data such as software statment profiles. A software statement profile consists of a software statement (created in the UK Open Banking directory) and associated information (keys, certificates, etc). Each software statement identifies a third-party entity that can create registrations (clients) with banks.

To use OBC, at least one software statement profile must be provided in secrets storage. This will allow the creation of bank registrations and is a pre-requisite for using OBC.

During development, it is suggested to use the [ASP.NET Core Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) as the OBC secrets store. This is a local ```secrets.json``` file stored outside of the project and well away from your Git repo (!).

This page explains how to add a software statement profile to the ASP.NET Core Secret Manager.

## Software statement profile secrets

The structure of a software statement profile is given and documented by the C# class `FinnovationLabs.OpenBanking.Library.Connector.Configuration.SoftwareStatementProfile`. Each property of this class is a string and corresponds to a single secret.

To specify a software statement profile via secrets, you will need to decide upon an ID for the profile and then create secrets using the key format `SoftwareStatementProfiles:<Profile ID>:<C# Property Name>`.

For example, here is JSON specifying secrets for a software statement profile with ID `All`:
```json
{
  "SoftwareStatementProfiles:All:CertificateType":"LegacyOB",
  "SoftwareStatementProfiles:All:SigningKeyId": "mySigningKeyId",
  "SoftwareStatementProfiles:All:SigningKey": "-----BEGIN PRIVATE KEY-----\nmyKeyLine1\nmyKeyLine2\n-----END PRIVATE KEY-----\n",
  "SoftwareStatementProfiles:All:SigningCertificate": "-----BEGIN CERTIFICATE-----\nmyCertLine1\nmyCertLine2\n-----END CERTIFICATE-----\n",
  "SoftwareStatementProfiles:All:TransportKey": "-----BEGIN PRIVATE KEY-----\nmyKeyLine1\nmyKeyLine2\n-----END PRIVATE KEY-----\n",
  "SoftwareStatementProfiles:All:TransportCertificate": "-----BEGIN CERTIFICATE-----\nmyCertLine1\nmyCertLine2\n-----END CERTIFICATE-----\n",
  "SoftwareStatementProfiles:All:SoftwareStatement": "mySoftwareStatementPart1.mySoftwareStatementPart2.mySoftwareStatementPart3",
  "SoftwareStatementProfiles:All:DefaultFragmentRedirectUrl": "https://example.com/auth/fragment-redirect"
}
```

## Adding a software statement profile to the ASP.NET Core Secret Manager

To add a software statement profile to the ASP.NET Core Secret Manager, first create a `secrets.json` file with JSON specifying your software statement profile secrets (see previous section).

Then place this file in a new secrets folder named the same as the `UserSecretsId`. The default `UserSecretsId` for OBC is currently specified in applicable `.csproj` files as:

```xml
    <UserSecretsId>aa921213-9461-4f9e-8fec-153624ec67ad</UserSecretsId>
```

This secrets folder should be created in the Microsoft `UserSecrets` folder appropriate for your platform.

(Note: Please also remember to supply the IDs of active software statement profiles to OBC via configuration.)