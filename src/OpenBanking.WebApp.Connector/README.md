# Docker commands

```powershell
# Build Docker image (assumes current directory set to repo root)
docker build -f .\src\OpenBanking.WebApp.Connector\Dockerfile --force-rm -t open-banking-connector-web-app:dev .

# Run Docker container (assumes configuration and secrets set up in C:\env.txt and API to be made available at http://localhost:50000)
## Example 1: local container in Dev environment
docker run -dt -e "DOTNET_ENVIRONMENT=Development" -e "Logging__Console__FormatterName=Simple" --env-file C:\env.txt -p 50000:80 -P --name OpenBanking.WebApp.Connector.Dev open-banking-connector-web-app:dev
## Example 2: released container in Dev environment
docker run -dt -e "DOTNET_ENVIRONMENT=Development" -e "Logging__Console__FormatterName=Simple" --env-file C:\env.txt -p 50000:80 -P --name OpenBanking.WebApp.Connector.1.0.0-alpha12.Dev ghcr.io/finlabsuk/open-banking-connector-web-app:1.0.0-alpha12
## Example 2: released container in Prod environment
docker run -dt -e "DOTNET_ENVIRONMENT=Production" --env-file C:\env.txt -p 50000:80 -P --name OpenBanking.WebApp.Connector.1.0.0-alpha12.Prod ghcr.io/finlabsuk/open-banking-connector-web-app:1.0.0-alpha12
```
