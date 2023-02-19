# Configuration sources settings

*Configuration sources* are an ASP.NET Core concept and are used in Open Banking Connector to provide configuration settings including secrets.

*Configuration sources settings* are settings which adjust configuration sources used from the defaults given [here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#default-application-configuration-sources). Any new configuration sources added will be added with lower priority than the default environment variable and command line sources.

These settings should only be modified from a default configuration source, e.g. environemnt variables, so that they can be picked up. For example, specifying use of the AWS SSM parameter source via an AWS SSM parameter will not work.

## Settings

Name | Valid Values | Default Value(s) | Description
--- | --- | --- | ---
OpenBankingConnector<wbr/>:ConfigurationSources<wbr/>:UseUserSecrets | {`"true"`, `"false"`} | `"false"` (development environment) <p style="margin-top: 10px;"> `"true"` (otherwise) | User secrets are a ASP.NET Core configuration source intended for use in development (see [here](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows)). This setting allows user secrets to be explicitly included or excluded as desired.
OpenBankingConnector<wbr/>:ConfigurationSources<wbr/>:AwsSsmParameterPrefix | string | `""` | A non-empty value specifies that AWS SSM parameters with the specified prefix will be used as a configuration source in Open Banking Connector. The specified prefix functions as a namespace, so for example an AWS SSM parameter `/<specifiedPrefix>/OpenBankingConnector/Database/EnsureDatabaseCreated` will map to Open Banking Connector setting `OpenBankingConnector:Database:EnsureDatabaseCreated`. When this configuration source is used, please ensure Open Banking Connector is run in an envrionment with an AWS IAM role/user which contains a suitable permissions policy[^1].

[^1]: Exmple AWS permission policy:
    ```json
    {
        "Version": "2012-10-17",
        "Statement": [
            {
                "Sid": "SSMPermissionStatement",
                "Effect": "Allow",
                "Action": "ssm:GetParametersByPath",
                "Resource": "arn:aws:ssm:eu-west-2:<awsAccountId>:parameter/<specifiedPrefix>/*"
            }
        ]
    }
    ```


