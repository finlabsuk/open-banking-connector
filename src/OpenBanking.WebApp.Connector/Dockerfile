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

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinnovationLabs.OpenBanking.WebApp.Connector.dll"]