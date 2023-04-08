# Signing certificate profiles settings

Signing certificate profiles settings are used to configure signing certificate profiles used by Open Banking Connector.

A *signing certificate profile* is a group of settings comprising information related to a signing certificate. It is identified by a user-defined profile ID. It is referenced by one or more [software statement profiles](./software-statement-profiles-settings.md).

Signing certificates and associated keys are used to sign and validate JWTs sent to banks.

No signing certificate profiles are configured by default and you will require at least one in order to create a bank registration.


## Settings

| Name                                                                                                                                                                           | Valid Values               | Default Value(s) | Description                                                                                                                                                                                                   |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------|------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| OpenBankingConnector<wbr/>:SigningCertificateProfiles<wbr/>:{Id}<wbr/>:Active <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>                 | {`"true"`, `"false"`}      | `"true"`         | Whether profile is active or inactive (ignored by Open Banking Connector). This allows profiles to be "switched on and off" for testing etc.                                                                  |
| OpenBankingConnector<wbr/>:SigningCertificateProfiles<wbr/>:{Id}<wbr/>:SigningCertificateType <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p> | {`"OBLegacy"`, `"OBSeal"`} | `"OBSeal"`       | Type of UK Open Banking Directory certificate used.                                                                                                                                                           |
| OpenBankingConnector<wbr/>:SigningCertificateProfiles<wbr/>:{Id}<wbr/>:AssociatedKeyId <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>        | string                     | -                | Signing Key ID (from UK Open Banking Directory) as string. This is not the same as the user-definied profile ID for this profile.                                                                             |
| OpenBankingConnector<wbr/>:SigningCertificateProfiles<wbr/>:{Id}<wbr/>:AssociatedKey <p style="margin-top: 10px;"> *where string Id is user-defined profile ID*  </p>          | string                     | -                | Signing key (PKCS #8) as "stringified" PEM file with escaped newline characters ("`\n`") and PRIVATE KEY label.             Example: `"-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"`.       |
