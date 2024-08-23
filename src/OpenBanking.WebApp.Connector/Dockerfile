FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH

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

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Get UK Open Banking sandbox and prod root CA certs
WORKDIR /app/certs
## Sandbox cert from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/252018873/OB+Root+and+Issuing+Certificates+for+Sandbox
ADD "https://openbanking.atlassian.net/wiki/download/attachments/252018873/OB_SandBox_PP_Root%20CA.cer?version=1&modificationDate=1525354123970&cacheVersion=1&api=v2" open-banking-sandbox-root-ca.cer
## Prod cert from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/80544075/OB+Root+and+Issuing+Certificates+for+Production
ADD "https://openbanking.atlassian.net/wiki/download/attachments/80544075/OpenBankingRootCA.cer?version=1&modificationDate=1516021348170&cacheVersion=1&api=v2" open-banking-prod-root-ca.cer
## Check fingerprints
RUN if openssl x509 -inform DER -in ./open-banking-sandbox-root-ca.cer -noout -fingerprint -sha1 | grep -q '3C:97:AD:3F:63:9B:21:EF:00:F3:39:93:90:61:6C:8A:7D:0D:5F:03'; then echo "success"; else echo "failure"; return 1; fi
RUN if openssl x509 -inform PEM -in ./open-banking-prod-root-ca.cer -noout -fingerprint -sha1 | grep -q 'BD:D9:DA:6C:21:B9:11:32:F8:0E:8B:09:D7:2C:43:F0:34:6B:E4:1F'; then echo "success"; else echo "failure"; return 1; fi
# Convert certs to standard PEM format
RUN openssl x509 -inform DER -in ./open-banking-sandbox-root-ca.cer -out /usr/local/share/ca-certificates/open-banking-sandbox-root-ca.crt
RUN openssl x509 -inform PEM -in ./open-banking-prod-root-ca.cer -out /usr/local/share/ca-certificates/open-banking-prod-root-ca.crt
# Adjust permissions and install certs
WORKDIR /app
RUN rm -rf /app/certs
RUN chmod 644 /usr/local/share/ca-certificates/open-banking-sandbox-root-ca.crt
RUN chmod 644 /usr/local/share/ca-certificates/open-banking-prod-root-ca.crt
RUN update-ca-certificates

EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinnovationLabs.OpenBanking.WebApp.Connector.dll"]