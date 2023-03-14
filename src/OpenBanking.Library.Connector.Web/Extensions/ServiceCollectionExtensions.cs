// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Web.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebHostServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Startup tasks
        services.AddHostedService<WebAppInformationHostedService>();

        return services;
    }
}
