FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1
ENV NUGET_XMLDOC_MODE=skip

# Restore Open Banking Connector
WORKDIR /src
COPY src/OpenBanking.WebApp.Connector/OpenBanking.WebApp.Connector.csproj OpenBanking.WebApp.Connector/
COPY src/OpenBanking.Library.Connector.Web/OpenBanking.Library.Connector.Web.csproj OpenBanking.Library.Connector.Web/
COPY src/OpenBanking.Library.Connector.GenericHost/OpenBanking.Library.Connector.GenericHost.csproj OpenBanking.Library.Connector.GenericHost/
COPY src/OpenBanking.Library.Connector/OpenBanking.Library.Connector.csproj OpenBanking.Library.Connector/
COPY src/OpenBanking.Library.BankApiModels/OpenBanking.Library.BankApiModels.csproj OpenBanking.Library.BankApiModels/
RUN dotnet restore -a $TARGETARCH "OpenBanking.WebApp.Connector/OpenBanking.WebApp.Connector.csproj"

# Publish Open Banking Connector
COPY src/. ./
RUN dotnet publish -a $TARGETARCH "OpenBanking.WebApp.Connector/OpenBanking.WebApp.Connector.csproj" -c Release --no-restore -o /app/publish

# UK Open Banking sandbox and prod root/issuing CA certs.
# Discarded stage: only the converted .crt files below reach the final image.
# Architecture-independent (just cert downloads + openssl conversion), so build natively
# once via --platform=$BUILDPLATFORM rather than once per target platform under emulation.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0 AS certs
WORKDIR /certs
## Sandbox certs from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/252018873/OB+Root+and+Issuing+Certificates+for+Sandbox
ADD "https://openbanking.atlassian.net/wiki/download/attachments/252018873/OB_SandBox_PP_Root%20CA.cer?version=1&modificationDate=1525354123970&cacheVersion=1&api=v2" open-banking-sandbox-root-ca.cer
ADD "https://openbanking.atlassian.net/wiki/download/attachments/252018873/OB_SandBox_PP_Issuing%20CA.cer?version=1&modificationDate=1525354091771&cacheVersion=1&api=v2" open-banking-sandbox-issuing-ca.cer
## Prod certs from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/80544075/OB+Root+and+Issuing+Certificates+for+Production
ADD "https://openbanking.atlassian.net/wiki/download/attachments/80544075/OpenBankingRootCA.cer?version=1&modificationDate=1516021348170&cacheVersion=1&api=v2" open-banking-prod-root-ca.cer
ADD "https://openbanking.atlassian.net/wiki/download/attachments/80544075/OpenBankingIssuingCA.cer?version=1&modificationDate=1516021349129&cacheVersion=1&api=v2" open-banking-prod-issuing-ca.cer
## Verify fingerprints before trusting any cert (substring match: tolerates openssl's varying "Fingerprint=" prefix)
RUN verify() { \
      actual=$(openssl x509 -inform "$1" -in "$2" -noout -fingerprint -sha1); \
      case "$actual" in \
        *"$3"*) echo "OK: $2 ($actual)" ;; \
        *) echo "FINGERPRINT MISMATCH $2: got '$actual', expected '$3'" >&2; exit 1 ;; \
      esac; \
    }; \
    verify DER open-banking-sandbox-root-ca.cer    3C:97:AD:3F:63:9B:21:EF:00:F3:39:93:90:61:6C:8A:7D:0D:5F:03; \
    verify DER open-banking-sandbox-issuing-ca.cer 06:A0:6C:4C:45:77:A7:87:81:16:B1:CF:39:3E:7D:4F:9A:D7:B6:8E; \
    verify PEM open-banking-prod-root-ca.cer       BD:D9:DA:6C:21:B9:11:32:F8:0E:8B:09:D7:2C:43:F0:34:6B:E4:1F; \
    verify PEM open-banking-prod-issuing-ca.cer    58:BE:4E:D3:45:FD:59:8F:89:02:80:57:04:2A:98:33:17:44:BB:F5
# Convert certs to standard PEM format
RUN convert() { openssl x509 -inform "$1" -in "$2" -out "/usr/local/share/ca-certificates/${2%.cer}.crt"; }; \
    convert DER open-banking-sandbox-root-ca.cer; \
    convert DER open-banking-sandbox-issuing-ca.cer; \
    convert PEM open-banking-prod-root-ca.cer; \
    convert PEM open-banking-prod-issuing-ca.cer; \
    chmod 644 /usr/local/share/ca-certificates/*.crt

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Install the certs verified above
COPY --from=certs /usr/local/share/ca-certificates/*.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "FinnovationLabs.OpenBanking.WebApp.Connector.dll"]
