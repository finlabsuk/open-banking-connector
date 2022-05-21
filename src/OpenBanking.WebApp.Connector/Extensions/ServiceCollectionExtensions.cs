// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebAppServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Set up bank profile definitions
            services.AddSingleton<IBankProfileDefinitions>(sp => new BankProfileDefinitionsStub());

            return services;
        }
    }
}
