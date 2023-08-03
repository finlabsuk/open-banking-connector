// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBankTestingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add settings groups
        services
            .AddSettingsGroup<BankTestSettings>();

        return services;
    }
}
