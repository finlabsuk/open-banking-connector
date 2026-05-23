// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBankTestingServices(this IServiceCollection services)
    {
        services
            .AddOptions<BankTestSettings>()
            .BindConfiguration(BankTestSettings.ConfigSectionName)
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<BankTestSettings>, BankTestSettingsValidator>();

        return services;
    }
}
