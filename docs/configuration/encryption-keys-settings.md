# Encryption keys settings

Encryption keys are used to encrypt sensitive information such as bank access and refresh tokens stored in the database at application level as part of defence-in-depth. It is assumed and recommended that database at-rest encryption is also used.

(Note that bank endpoints accepting tokens also authenticate the client with MTLS as a further level of protection so unencrypted bank tokens are not usable in isolation.)

Encryption is performed using the in-the-box .NET implementation of AES-256-GCM and keys should be 256 bits. Note that use of unique nonces is guaranteed at database level by the unique nonce field in the database encrypted_object table. Please do not hard-delete records in this table whose key_id field matches an encryption key still in use since it is very important for AES-GCM that nonces are never re-used for the same encryption key.

Multiple encryption keys can be provided each with its own ID. This allows decryption of objects encrypted with previously-used keys. New objects will be encrypted with the key specified by *OpenBankingConnector:Keys:CurrentEncryptionKeyId*. Keys are specified as a base64-encoded string.

It is up to the user to generate their own keys, but for example purposes the [Kubernetes docs](https://kubernetes.io/docs/tasks/administer-cluster/encrypt-data/#encrypting-your-data) suggest that on Linux a base64-encoded key can be generated via the command
```bash
head -c 32 /dev/urandom | base64
```

No encryption keys are configured by default and you will require at least one in order to use Open Banking Connector unless encryption is disabled. To create one, simply decide upon an ID and configure minimally the settings lacking defaults [below](#settings).

In the future we plan to support automated rotation of encryption keys which will automate re-encryption of objects and hard-deletion of object records corresponding to old keys.

## Settings

| Name                                                                                                                                                            | Valid Values          | Default Value(s) | Description                                                                                                                                                                                                                          |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------|------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| OpenBankingConnector<wbr/>:Keys<wbr/>:CurrentEncryptionKeyId                                                                                                    | string                | -                | Encryption key to use for encrypting new objects. Specified by key ID.                                                                                                                                                               |
| OpenBankingConnector<wbr/>:Keys<wbr/>:Encryption<wbr/>:{Id}<wbr/>:Value <p style="margin-top: 10px;"> *where string Id is user-defined encryption key ID*  </p> | string                | -                | Encryption key (256-bit) used for symmetric encryption (AES-256-GCM) of sensitive data in database such as bank tokens. Specified as a base64-encoded string. See [above](#encryption-keys-settings) for example of how to generate. |
| OpenBankingConnector<wbr/>:Keys<wbr/>:DisableEncryption                                                                                                         | {`"true"`, `"false"`} | `"false"`        | Disable encryption of new objects (not recommended).                                                                                                                                                                                 |
