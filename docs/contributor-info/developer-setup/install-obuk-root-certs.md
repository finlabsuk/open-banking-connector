# Install Open Banking UK root certificates

When running Open Banking Connector as a local .NET application, Open Banking UK root certificates must be installed to check remote certs from banks when creating TLS connections.

OB UK root certificates are available for both sandbox and production environments. Please install one or both root certificates according to the intended use case.

Each OB UK root certificate also has a corresponding issuing certificate but installing the root certificate alone is sufficient.

## Download root certificates

Root certificates should first be downloaded from OB UK.

Sandbox root certificate:
```bash
# Download sandbox cert from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/252018873/OB+Root+and+Issuing+Certificates+for+Sandbox
wget -O open-banking-sandbox-root-ca.cer "https://openbanking.atlassian.net/wiki/download/attachments/252018873/OB_SandBox_PP_Root%20CA.cer?version=1&modificationDate=1525354123970&cacheVersion=1&api=v2"
# Check fingerprint
if openssl x509 -inform DER -in ./open-banking-sandbox-root-ca.cer -noout -fingerprint -sha1 | grep -q '3C:97:AD:3F:63:9B:21:EF:00:F3:39:93:90:61:6C:8A:7D:0D:5F:03'; then echo "success"; else echo "failure"; return 1; fi
```

Production root certificate:
```bash
# Download production cert from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/80544075/OB+Root+and+Issuing+Certificates+for+Production
wget -O open-banking-prod-root-ca.cer "https://openbanking.atlassian.net/wiki/download/attachments/80544075/OpenBankingRootCA.cer?version=1&modificationDate=1516021348170&cacheVersion=1&api=v2"
# Check fingerprint
if openssl x509 -inform PEM -in ./open-banking-prod-root-ca.cer -noout -fingerprint -sha1 | grep -q 'BD:D9:DA:6C:21:B9:11:32:F8:0E:8B:09:D7:2C:43:F0:34:6B:E4:1F'; then echo "success"; else echo "failure"; return 1; fi
```

## Install root certificates

## Windows

On Windows, open the certificate manager (certlm.msc) and right-click "Trusted Root Certification Authorities / Certificates" and choose to import the certificate.

## macOS

On macOS (Big Sur), double-click the cert to add to the System Keychain and right-click to enable "Always Trust".

## Ubuntu Linux (server and WSL2)

```bash
# Convert and move certs
sudo openssl x509 -inform DER -in ./open-banking-sandbox-root-ca.cer -out /usr/local/share/ca-certificates/open-banking-sandbox-root-ca.crt
sudo openssl x509 -inform PEM -in ./open-banking-prod-root-ca.cer -out /usr/local/share/ca-certificates/open-banking-prod-root-ca.crt
# Adjust permissions
sudo chmod 644 /usr/local/share/ca-certificates/open-banking-sandbox-root-ca.crt
sudo chmod 644 /usr/local/share/ca-certificates/open-banking-prod-root-ca.crt
# Install
sudo update-ca-certificates
```
