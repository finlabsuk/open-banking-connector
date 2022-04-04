FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/OpenBanking.WebApp.Connector/OpenBanking.WebApp.Connector.csproj", "src/OpenBanking.WebApp.Connector/"]
COPY ["src/OpenBanking.Library.Connector.Web/OpenBanking.Library.Connector.Web.csproj", "src/OpenBanking.Library.Connector.Web/"]
COPY ["src/OpenBanking.Library.Connector.GenericHost/OpenBanking.Library.Connector.GenericHost.csproj", "src/OpenBanking.Library.Connector.GenericHost/"]
COPY ["src/OpenBanking.Library.Connector/OpenBanking.Library.Connector.csproj", "src/OpenBanking.Library.Connector/"]
COPY ["src/OpenBanking.Library.BankApiModels/OpenBanking.Library.BankApiModels.csproj", "src/OpenBanking.Library.BankApiModels/"]
COPY ["src/OpenBanking.Library.Connector.GenericHost.Azure/OpenBanking.Library.Connector.GenericHost.Azure.csproj", "src/OpenBanking.Library.Connector.GenericHost.Azure/"]
RUN dotnet restore "src/OpenBanking.WebApp.Connector/OpenBanking.WebApp.Connector.csproj"
COPY . .
WORKDIR "/src/src/OpenBanking.WebApp.Connector"
RUN dotnet build "OpenBanking.WebApp.Connector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenBanking.WebApp.Connector.csproj" -c Release -o /app/publish
WORKDIR /certs
ADD https://openbanking.atlassian.net/wiki/download/attachments/252018873/OB_SandBox_PP_Root%20CA.cer?version=1&modificationDate=1525354123970&cacheVersion=1&api=v2 open-banking-sandbox-root-ca.cer
RUN openssl x509 -inform DER -in ./open-banking-sandbox-root-ca.cer -out ./open-banking-sandbox-root-ca.crt

FROM base AS final
COPY --from=publish /certs/open-banking-sandbox-root-ca.crt /usr/local/share/ca-certificates/open-banking-sandbox-root-ca.crt
RUN chmod 644 /usr/local/share/ca-certificates/open-banking-sandbox-root-ca.crt && update-ca-certificates
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinnovationLabs.OpenBanking.WebApp.Connector.dll"]