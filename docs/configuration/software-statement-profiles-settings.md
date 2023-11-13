# Software statement profiles settings

Software statement profiles settings are used to configure software statement profiles for use by Open Banking Connector.

In UK Open Banking, a software statement identifies a third-party provider (TPP) that can create bank registrations (OAuth2 clients) with banks and can be seen as a kind of "passport". A TPP can create multiple software statements and this is fully supported by Open Banking Connector.

You should create the software statements you need in the [UK Open Banking Directory](https://www.openbanking.org.uk/directory/). You can then create profiles allowing you to use these software statements in Open Banking Connector.

A *software statement profile* is a group of settings comprising:

1. essential details of a software statement including its identifier ("software ID")
2. associated information which will be used for bank registrations that reference the software statement profile:

    - a [transport certificate profile](./transport-certificate-profiles-settings.md) specifying a transport key/certificate
    - a [signing certificate profile](./signing-certificate-profiles-settings.md) which specifying a signing key/certificate
    - a default redirect URL

Transport certificate profiles and signing certificate profiles can be re-used between software statement profiles, i.e. referenced by more than one software statement profile.

When creating a bank registration (OAuth2 client), a software statement profile is specified which configures the associated software statement, transport key-pair, signing key, etc.

No software statement profiles are configured by default and you will require at least one in order to create a bank registration. To create a software statement profile, simply decide upon an ID and configure minimally the settings lacking defaults [below](#settings).

As your needs evolve you can create multiple software statement profiles.

Software statement profiles are validated on application start-up so please be alert to warning and error messages related to these.

## Settings

| Name                                                                                                                                                                                 | Valid Values          | Default Value(s) | Description                                                                                                                                       |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------|------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|
| OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:Active <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>                        | {`"true"`, `"false"`} | `"true"`         | Whether profile is active or inactive (ignored by Open Banking Connector). This allows profiles to be "switched on and off" for testing etc.      |
| OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:OrganisationId <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>                | string                | -                | Organisation ID from UK Open Banking directory as string.                                                                                         |
| OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:SoftwareId <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>                    | string                | -                | Software statement ID from UK Open Banking directory as string.                                                                                   |
| OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:SandboxEnvironment <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>            | string                | `"false"`        | When true, denotes software statement is defined in UK OB directory sandbox (not production) environment.                                         |
| OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:TransportCertificateProfileId <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | string                | -                | ID of [transport certificate profile](./transport-certificate-profiles-settings.md) to use for mutual TLS with this software statement profile.   |
| OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:SigningCertificateProfileId <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>   | string                | -                | ID of [signing certificate profile](./signing-certificate-profiles-settings.md) to use for signing JWTs etc with this software statement profile. |
| OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:DefaultFragmentRedirectUrl <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>    | string                | -                | Default redirect URL for consent authorisations when OAuth2 `response_mode` = `fragment`.                                                         |
| OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:DefaultQueryRedirectUrl <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>       | string                | -                | Default query URL for consent authorisations when OAuth2 `response_mode` = `query`.                                                               |

