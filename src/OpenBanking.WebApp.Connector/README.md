# Docker commands

```powershell
# Build image from repo root
docker build -f .\src\OpenBanking.WebApp.Connector\Dockerfile --force-rm -t open-banking-connector-web-app:new .

# Run container from repo root (two examples)
docker run -dt -e "DOTNET_ENVIRONMENT=Development" -P --name OpenBanking.WebApp.Connector.New open-banking-connector-web-app:new
docker run -dt -e "DOTNET_ENVIRONMENT=Development" -P --name OpenBanking.WebApp.Connector.v1.0.0-alpha02 ghcr.io/finlabsuk/open-banking-connector-web-app:1.0.0-alpha02
# NB: not sure if need to add to above: -e "ASPNETCORE_URLS=http://+:80"
```