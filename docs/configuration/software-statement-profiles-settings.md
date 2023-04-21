# Software statement profiles settings

Software statement profiles settings are used to configure software statement profiles for use by Open Banking Connector.

In UK Open Banking, a software statement identifies a third-party provider (TPP) that can create bank registrations (OAuth2 clients) with banks and can be seen as a kind of "passport". A TPP can create multiple software statements and this is fully supported by Open Banking Connector.

You should create the software statements you need in the [UK Open Banking Directory](https://www.openbanking.org.uk/directory/). From there, you can also generate a corresponding software statement assertion (SSA) for each. Each SSA can then be the basis of one or more software statement profiles in Open Banking Connector.

A *software statement profile* is a group of settings comprising

1. a software statement assertion (SSA)
2. associated information which will be used for bank registrations that reference the software statement profile:

    -  a [transport certificate profile](./transport-certificate-profiles-settings.md) specifying a transport key/certificate
    - a [signing certificate profile](./signing-certificate-profiles-settings.md) which specifying a signing key/certificate
    - a default redirect URL

Transport certificate profiles and signing certificate profiles can be re-used between software statement profiles, i.e. referenced by more than one software statement profile.

When creating a bank registration (OAuth2 client), a software statement profile is specified which configures the associated SSA, transport key-pair, signing key, etc.

No software statement profiles are configured by default and you will require at least one in order to create a bank registration. To create a software statement profile, simply decide upon an ID and configure minimally the settings lacking defaults [below](#settings).

As your needs evolve you can create multiple software statement profiles to allow use of multiple SSAs and certs in different combinations etc.

Software statement profiles are validated on application start-up so please be alert to warning and error messages related to these.

## Settings

Name | Valid Values | Default Value(s) | Description
--- | --- | --- | ---
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:Active <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | {`"true"`, `"false"`} | `"true"` | Whether profile is active or inactive (ignored by Open Banking Connector). This allows profiles to be "switched on and off" for testing etc.
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:SoftwareStatement <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | string | - | Software statement assertion (SSA) as string, i.e. "FirstPart.SecondPart.ThirdPart".
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:TransportCertificateProfileId <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | string | - | ID of [transport certificate profile](./transport-certificate-profiles-settings.md) to use for mutual TLS with this software statement profile.
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:SigningCertificateProfileId <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | string | - | ID of [signing certificate profile](./signing-certificate-profiles-settings.md) to use for signing JWTs etc with this software statement profile.
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:DefaultFragmentRedirectUrl <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | string | - | Default redirect URL for consent authorisations when OAuth2 `response_mode` = `fragment`.
