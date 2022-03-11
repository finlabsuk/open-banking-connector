# Software statement profiles settings

Software statement profiles settings are used to configure software statement profiles used by Open Banking Connector.

A *software statement profile* is a set of settings comprising a software statement assertion (SSA) and information related to that SSA. It is identified by a user-defined profile ID. It references an associated [transport certificiate profile](./transport-certificate-profiles-settings.md) and an associated [signing certificate profile](./signing-certificate-profiles-settings.md). All three profiles may be created after e.g. getting a software statement assertion (SSA) and signed certificates using the [UK Open Banking Directory](https://www.openbanking.org.uk/directory/).

No software statement profiles are configured by default and you will require at least one in order to create a bank registration.

Software statement profiles settings are defined in the [SoftwareStatementProfilesSettings](../../src/OpenBanking.Library.Connector/Models/Configuration/SoftwareStatementProfilesSettings.cs#L103) class.


## Settings

Name | Valid Values | Default Value(s) | Description
--- | --- | --- | ---
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:Active <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | {`"true"`, `"false"`} | `"true"` | Whether profile is active or inactive (ignored by Open Banking Connector). This allows profiles to be "switched on and off" for testing etc.
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:SoftwareStatement <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | string | - | Software statement assertion (SSA) as string, i.e. "FirstPart.SecondPart.ThirdPart".
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:TransportCertificateProfileId <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | string | - | ID of [transport certificiate profile](./transport-certificate-profiles-settings.md) to use for mutual TLS with this software statement profile.
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:SigningCertificateProfileId <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | string | - | ID of [signing certificiate profile](./signing-certificate-profiles-settings.md) to use for signing JWTs etc with this software statement profile.
OpenBankingConnector<wbr/>:SoftwareStatementProfiles<wbr/>:{Id}<wbr/>:DefaultFragmentRedirectUrl <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | string | - | Default redirect URL for consent authorisations when OAuth2 `response_mode` = `fragment`.
