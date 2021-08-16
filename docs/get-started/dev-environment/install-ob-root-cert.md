# Install Open Banking Root Certificate

An Open Banking UK root certificate needs to be installed to check remote certs from banks when creating SSL connections.

## Obtain the Root Certificate

The UK Open Baking Sandbox root certificate can be obtained [here](https://openbanking.atlassian.net/wiki/spaces/DZ/pages/252018873/OB+Root+and+Issuing+Certificates+for+Sandbox).

Currently only the Open Banking UK root certificate (and not the issuing certificate) needs to be installed.

## Install the Root Certificate

On Windows, open the certificate manager (certlm.msc) and right-click "Trusted Root Certification Authorities / Certificates" and choose to import the certificate.

On macOS (Big Sur), double-click the cert to add to the System Keychain and right-click to enable "Always Trust".
