// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebAppServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services;
        }
    }
}
