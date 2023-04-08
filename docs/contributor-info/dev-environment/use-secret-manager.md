# Use Microsoft's secret manager

When running Open Banking Connector as a .NET application from source code, and when in the development environment, Microsoft's secret manager can be used to provide configuration/secrets.

To use the secret manager, you will need a `secrets.json` file in the appropriate directory (if you do not have one already). The Microsoft documentation [here](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#how-the-secret-manager-tool-works) gives the path for this directory. Please note that the UserSecretsId for the app is `aa921213-9461-4f9e-8fec-153624ec67ad` as given [here](../../../src/OpenBanking.WebApp.Connector/OpenBanking.WebApp.Connector.csproj).

You can then add key-value pairs to the `secrets.json` file to supply configuration to Open Banking Connector.

Here is an example of what such a file might look like after configuring a software statement profile, transport certificate profile and signing certificate profile:

```json
{
  "OpenBankingConnector:TransportCertificateProfiles:New2:CertificateDnWithStringDottedDecimalAttributeValues": "CN=abc,2.5.4.97=abc,O=abc,C=GB",
  "OpenBankingConnector:TransportCertificateProfiles:New2:CertificateDnWithHexDottedDecimalAttributeValues": "CN=abc,2.5.4.97=#123,O=abc,C=GB",
  "OpenBankingConnector:TransportCertificateProfiles:New2:Certificate": "-----BEGIN CERTIFICATE-----\nabc\n-----END CERTIFICATE-----\n",
  "OpenBankingConnector:TransportCertificateProfiles:New2:AssociatedKey": "-----BEGIN PRIVATE KEY-----\nabc\n-----END PRIVATE KEY-----\n",
  "OpenBankingConnector:SoftwareStatementProfiles:All:TransportCertificateProfileId": "New2",
  "OpenBankingConnector:SoftwareStatementProfiles:All:SoftwareStatement": "a.b.c",
  "OpenBankingConnector:SoftwareStatementProfiles:All:SigningCertificateProfileId": "New2",
  "OpenBankingConnector:SoftwareStatementProfiles:All:DefaultFragmentRedirectUrl": "https://example.com/auth/fragment-redirect",
  "OpenBankingConnector:SigningCertificateProfiles:New2:AssociatedKeyId": "abc",
  "OpenBankingConnector:SigningCertificateProfiles:New2:AssociatedKey": "-----BEGIN PRIVATE KEY-----\nabc\n-----END PRIVATE KEY-----\n"
}
```

**Important**: Some settings values are very sensitive, for example keys, and should be carefully and securely stored and managed.
 They should **never** be stored in-repo, for example in additional or modified `appsettings.json` files, due to the risk of disclosure. The secret manager is designed to store secrets out-of-repo during code development and testing where other configuration providers may not be available.

