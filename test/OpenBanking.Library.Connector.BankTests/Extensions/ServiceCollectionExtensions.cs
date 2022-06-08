// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBankTestingServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add settings groups
            services
                .AddSettingsGroup<BankTestSettings>(configuration);

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

            // Set up bank users
            services.AddSingleton(
                sp =>
                {
                    BankTestSettings bankTestSettings =
                        sp.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();
                    string bankUsersFile = Path.Combine(
                        bankTestSettings.GetDataDirectoryForCurrentOs(),
                        "bankUsers.json");
                    var bankUsers = new BankUserStore(
                        DataFile.ReadFile<BankUserDictionary>(
                            bankUsersFile,
                            new JsonSerializerSettings()).GetAwaiter().GetResult());
                    return bankUsers;
                });

            return services;
        }
    }
}
