// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Web.HostedServices;
using Microsoft.Extensions.DependencyInjection;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebHostServices(
        this IServiceCollection services,
        string? serviceVersion = null)
    {
        // Startup tasks
        services.AddHostedService(
            sp => serviceVersion is not null
                ? ActivatorUtilities.CreateInstance<WebAppInformationHostedService>(sp, serviceVersion)
                : ActivatorUtilities.CreateInstance<WebAppInformationHostedService>(sp));

        services.AddHealthChecks();

        return services;
    }
}
