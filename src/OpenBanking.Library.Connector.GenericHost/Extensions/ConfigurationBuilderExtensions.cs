// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;

public static class ConfigurationBuilderExtensions
{
    /// <summary>
    ///     Add Key Vault to .NET Generic Host app configuration.
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="keySecretProvider"></param>
    /// <param name="vaultName"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddKeyVault(
        this IConfigurationBuilder configurationBuilder,
        IGenericHostKeySecretProvider keySecretProvider,
        string vaultName) => keySecretProvider.AddKeyVaultToConfigurationInner(configurationBuilder, vaultName);

    public static IHostBuilder AddGenericHostConfiguration(this IHostBuilder hostBuilder, string[] args) =>
        hostBuilder.ConfigureAppConfiguration(
            (hostingContext, config) =>
            {
                // Extract configuration source choices from default configuration sources
                IConfigurationRoot defaultConfig = config.Build();

                // Get configuration sources settings
                var configurationSourcesSettings =
                    ServiceCollectionExtensions.GetSettings<ConfigurationSourcesSettings>(defaultConfig);
                bool useUserSecrets = configurationSourcesSettings.UseUserSecrets;
                string awsSsmParameterPrefix = configurationSourcesSettings.AwsSsmParameterPrefix;

                // Remove user secrets source if present and unwanted
                IHostEnvironment env = hostingContext.HostingEnvironment;
                bool userSecretsSourceAlreadyAddedWherePossible = env.IsDevelopment();
                if (userSecretsSourceAlreadyAddedWherePossible && !useUserSecrets)
                {
                    IConfigurationSource? userSecretsSource =
                        config.Sources
                            .SingleOrDefault(
                                x => x switch
                                {
                                    JsonConfigurationSource xx => xx.Path == "secrets.json",
                                    _ => false
                                }); // Find secrets source if present
                    if (userSecretsSource is not null)
                    {
                        config.Sources.Remove(userSecretsSource);
                    }
                }

                // Note, in below, we add rather than insert new configuration sources to avoid
                // expensive reloads in .NET 6 with ConfigurationManager.
                // See: https://andrewlock.net/exploring-dotnet-6-part-1-looking-inside-configurationmanager-in-dotnet-6/

                // Add user secrets source
                var configAdded = false;
                var reloadOnChange = false; // all configuration validated and determined at launch
                if (!userSecretsSourceAlreadyAddedWherePossible && useUserSecrets)
                {
                    // Compare below with HostingHostBuilderExtensions.ConfigureDefaults()
                    if (env.ApplicationName.Length > 0)
                    {
                        Assembly assembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        config.AddUserSecrets(assembly, true, reloadOnChange);
                        configAdded = true;
                    }
                }

                // Add AWS SSM parameter source
                if (awsSsmParameterPrefix is not "")
                {
                    config.AddSystemsManager(awsSsmParameterPrefix);
                    configAdded = true;
                }

                // If new config sources added, re-add env vars and command line sources to ensure correct priority
                if (configAdded)
                {
                    config.AddEnvironmentVariables();

                    if (args.Length > 0)
                    {
                        config.AddCommandLine(args);
                    }
                }
            });
}
