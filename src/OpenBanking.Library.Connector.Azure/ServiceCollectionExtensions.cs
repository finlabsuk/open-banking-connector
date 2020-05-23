// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnovationLabs.OpenBanking.Library.Connector.Azure
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Add OpenBankingConnector's Azure dependencies. Execute this after other OBC dependency injections.
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddOpenBankingConnectorAzureDependencies(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddSingleton<IKeySecretReadOnlyProvider, AzureKeySecretReadOnlyProvider>();

            return services;
        }
    }
}
