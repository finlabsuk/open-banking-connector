﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsentAuthoriserServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Get settings via IOptions (ensure no updates after app start) and add to service collection
            services
                .Configure<BankTestSettings>(configuration.GetSection(new BankTestSettings().SettingsSectionName))
                .AddOptions();
            services.AddSingleton<ISettingsProvider<BankTestSettings>>(
                sp =>
                {
                    BankTestSettings bankTestSettings =
                        sp.GetRequiredService<IOptions<BankTestSettings>>().Value;
                    return new DefaultSettingsProvider<BankTestSettings>(bankTestSettings);
                });

            // Set up Node JS services
            services.AddSingleton<IOptions<NodeJSProcessOptions>>(
                sp =>
                {
                    BankTestSettings bankTestSettings =
                        sp.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();
                    return Options.Create(
                        bankTestSettings.ConsentAuthoriser.NodeJS.GetProccessedNodeJSProcessOptions());
                });
            services.AddSingleton<IOptions<OutOfProcessNodeJSServiceOptions>>(
                sp =>
                {
                    BankTestSettings bankTestSettings =
                        sp.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();
                    return Options.Create(bankTestSettings.ConsentAuthoriser.NodeJS.OutOfProcessNodeJSServiceOptions);
                });
            services.AddNodeJS();

            return services;
        }
    }
}
