# Contributor info

Open Banking Connector is an open-source project and welcomes contributions. Please contact us for more information about contributing to this project.

See the following sections for contributor guidance on:

- [documentation](./documentation/README.md)

## Environment selection

Open Banking Connector can be run using different [environments](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0) such as development, staging and production. In fact, its behaviour is only actually sensitive to whether it is run in the development environment.

By default, Open Banking Connector uses the production environment.

In the development environment, logging, error handling and configuration defaults etc are modified to suit the needs of developing and testing Open Banking Connector. The development environment should not be used in production.

Environment selection is not configured by Open Banking Connector settings but normally via the Microsoft `DOTNET_ENVIRONMENT` or `ASPNETCORE_ENVIRONMENT` environment variables.

Here is an example of how to run the web app container using environment variables to (a) use the development environment and (b) set the DB provider to `"Sqlite"`:
