// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        ///     Get key secret provider from configuration and add key secrets from this provider
        ///     to .NET Generic Host app configuration
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IHostBuilder ConfigureKeySecrets(
            this IHostBuilder builder,
            IDictionary<KeySecretProvider, IGenericHostKeySecretProvider> providers) =>
            builder.ConfigureAppConfiguration(
                (context, config) =>
                {
                    IConfigurationRoot builtConfig = config.Build();
                    KeySecretOptions? keySecretsOptions = builtConfig
                        .GetSection(new OpenBankingConnectorSettings().SettingsGroupName)
                        .Get<OpenBankingConnectorSettings>()
                        .Validate()
                        .KeySecrets;
                    if (keySecretsOptions is null &&
                        !context.HostingEnvironment.IsDevelopment())
                    {
                        throw new ArgumentException(
                            "No key secret provider specified (local secrets only valid in Development environment).");
                    }

                    if (!(keySecretsOptions is null))
                    {
                        bool success = providers.TryGetValue(
                            keySecretsOptions.ProcessedProvider,
                            out IGenericHostKeySecretProvider? provider);
                        if (success)
                        {
                            config.AddKeyVault(provider!, keySecretsOptions.VaultName);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException($"{keySecretsOptions.ProcessedProvider}");
                        }
                    }
                });
    }
}
