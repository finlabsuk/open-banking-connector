// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBankProfileServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Get settings via IOptions (ensure no updates after app start) and add to service collection
            services
                .Configure<BankProfileSettings>(configuration.GetSection(new BankProfileSettings().SettingsSectionName))
                .AddOptions();
            services.AddSingleton<ISettingsProvider<BankProfileSettings>>(
                sp =>
                {
                    BankProfileSettings bankProfileSettings =
                        sp.GetRequiredService<IOptions<BankProfileSettings>>().Value;
                    return new DefaultSettingsProvider<BankProfileSettings>(bankProfileSettings);
                });

            // Set up bank profile definitions
            services.AddSingleton(
                sp =>
                {
                    BankProfileSettings bankProfileSettings =
                        sp.GetRequiredService<ISettingsProvider<BankProfileSettings>>().GetSettings();
                    return new BankProfileDefinitions(
                        DataFile.ReadFile<Dictionary<string, Dictionary<string, BankProfileHiddenProperties>>>(
                            bankProfileSettings.HiddenPropertiesFile,
                            new JsonSerializerSettings()).GetAwaiter().GetResult());
                });

            return services;
        }
    }
}
