# Developer setup

Open Banking Connector is an open-source project and the open-source repo contains files necessary for developers to
build and test Open Banking Connector.

The provided C# solution `<repo>\FinnovationLabs.OpenBanking.Library.Connector.sln` includes both the web app and a
number of
test projects. It will allow you to build and test Open Banking Connector locally.

To run Open Banking Connector tests on a local machine, you will need to first do the following:

- [Install Open Banking UK root certificates](install-obuk-root-certs.md)

## Environment selection

Open Banking Connector can be run using
different [environments](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-8.0)
such as `development` and `production`. In fact, its behaviour is only actually sensitive to whether it is run in the
`development` environment.

By default, Open Banking Connector uses the `production` environment which should be used for all production
deployments.

In the `development` environment, logging, error handling and configuration defaults etc are modified to suit the needs
of local development and testing. The `development` environment should not be used in production deployments.

Environment selection is not configured by Open Banking Connector settings but normally via the Microsoft
`DOTNET_ENVIRONMENT` or `ASPNETCORE_ENVIRONMENT` environment variables. These are used to configure the `development`
environment for local app running (in `<repo>\src\OpenBanking.WebApp.Connector\Properties\launchSettings.json`) and
local app testing (in `<repo>\.runsettings`).
