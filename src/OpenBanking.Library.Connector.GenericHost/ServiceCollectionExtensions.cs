// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.HostedServices;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftwareStatementProfile =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenBankingConnector(
            this IServiceCollection services,
            IConfiguration configuration,
            bool loadSecretsFromConfig)
        {
            const string httpClientName = "OBC";

            services.AddHttpClient(httpClientName)
                .ConfigurePrimaryHttpMessageHandler(
                    sp =>
                    {
                        ISoftwareStatementProfileService profileService =
                            sp.GetRequiredService<ISoftwareStatementProfileService>();

                        // This can be removed in .NET 5 when SoftwareStatementProfileHostedService
                        // and SocketsHttpHandler used.
                        profileService.SetSoftwareStatementProfileFromSecretsSync();

                        HttpMessageHandler handler = new HttpRequestBuilder()
                            .SetClientCertificates(profileService.GetCertificates())
                            .CreateMessageHandler();

                        return handler;
                    });

            services.AddSingleton<IObcConfigurationProvider>(
                sp => new NetGenericHostObcConfigurationProvider(configuration));
            services.AddSingleton<IInstrumentationClient, LoggerInstrumentationClient>();
            if (loadSecretsFromConfig)
            {
                services.AddSingleton(
                    sp =>
                    {
                        IObcConfigurationProvider configProvider = sp.GetService<IObcConfigurationProvider>();

                        KeySecretBuilder builder = new KeySecretBuilder();
                        return builder.GetKeySecretProvider(
                            config: configuration,
                            obcConfig: configProvider.GetObcConfiguration());
                    });
            }
            else
            {
                services.AddSingleton<IKeySecretProvider, MemoryKeySecretProvider>();
            }

            services.AddSingleton(x => (IKeySecretReadOnlyProvider) x.GetRequiredService<IKeySecretProvider>());
            services.AddSingleton<IApiClient>(
                sp =>
                {
                    IHttpClientFactory hcf = sp.GetService<IHttpClientFactory>();

                    HttpClient client = hcf.CreateClient(httpClientName);

                    return new ApiClient(instrumentation: sp.GetService<IInstrumentationClient>(), httpClient: client);
                });
            services.AddSingleton<ICertificateReader, PemParsingCertificateReader>();
            services.AddSingleton<IEntityMapper, EntityMapper>();

            // Configure DB
            switch (configuration["DbProvider"])
            {
                case "Sqlite":
                    services
                        // See e.g. https://jasonwatmore.com/post/2020/01/03/aspnet-core-ef-core-migrations-for-multiple-databases-sqlite-and-sql-server 
                        .AddDbContext<BaseDbContext, SqliteDbContext>(
                            options =>
                            {
                                string connectionString = configuration.GetConnectionString("SqliteDbContext");
                                options.UseSqlite(connectionString);
                            });
                    break;
                default:
                    throw new ArgumentException(message: "Unknown DB provider", paramName: configuration["DbProvider"]);
            }

            services.AddScoped<IDbMultiEntityMethods,
                DbMultiEntityMethods>();
            services.AddScoped<IDbEntityRepository<BankClientProfile>,
                DbEntityRepository<BankClientProfile>>();
            services.AddScoped<IDbEntityRepository<ApiProfile>,
                DbEntityRepository<ApiProfile>>();
            services.AddScoped<IDbEntityRepository<DomesticConsent>,
                DbEntityRepository<DomesticConsent>>();
            services.AddSingleton<IKeySecretReadRepository<ActiveSoftwareStatementProfiles>,
                KeySecretReadRepository<ActiveSoftwareStatementProfiles>>();
            services.AddSingleton<IKeySecretWriteRepository<ActiveSoftwareStatementProfiles>,
                KeySecretWriteRepository<ActiveSoftwareStatementProfiles>>();
            services.AddSingleton<IKeySecretMultiItemWriteRepository<SoftwareStatementProfile>,
                KeySecretMultiItemWriteRepository<SoftwareStatementProfile>>();
            services.AddSingleton<IKeySecretMultiItemReadRepository<SoftwareStatementProfile>,
                KeySecretMultiItemReadRepository<SoftwareStatementProfile>>();
            services.AddSingleton<ISoftwareStatementProfileService, SoftwareStatementProfileService>();
            services.AddScoped<IOpenBankingRequestBuilder, RequestBuilder>();

            // Startup tasks
            services.AddHostedService<StartupTasksHostedService>();

            // This can be enabled in .NET 5 when SoftwareStatementProfileHostedService
            // and SocketsHttpHandler used.
            // services.AddHostedService<SoftwareStatementProfileHostedService>();

            return services;
        }
    }
}
