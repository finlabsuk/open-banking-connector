// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.HostedServices;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached.SoftwareStatementProfile;
using SoftwareStatementProfileKeySecretsItem =
    FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.SoftwareStatementProfile;


namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenBankingConnector(
            this IServiceCollection services,
            IConfiguration configuration,
            bool loadSecretsFromConfig)
        {
            // Get configuration
            NetGenericHostObcConfigurationProvider configurationProvider =
                new NetGenericHostObcConfigurationProvider(configuration);
            ObcConfiguration obcConfig = configurationProvider.GetObcConfiguration();
            IEnumerable<string> activeSoftwareStatementProfileIds = obcConfig.ProcessedSoftwareStatementProfileIds;

            // Prepare key secret provider
            // TODO: handle cloud key secret provider case
            KeySecretBuilder builder = new KeySecretBuilder();
            IList<KeySecret> secrets = builder.GetKeySecretProvider(
                config: configuration,
                profileIds: activeSoftwareStatementProfileIds);
            IKeySecretProvider keySecretProvider = new MemoryKeySecretProvider(secrets);

            // Assemble dictionary of software statement profiles
            ReadOnlyKeySecretItemRepository<SoftwareStatementProfileKeySecretsItem> itemRepo =
                new ReadOnlyKeySecretItemRepository<SoftwareStatementProfileKeySecretsItem>(keySecretProvider);
            ConcurrentDictionary<string, SoftwareStatementProfileKeySecretsItem> softwareStatementProfiles =
                new ConcurrentDictionary<string, SoftwareStatementProfileKeySecretsItem>();
            foreach (string id in activeSoftwareStatementProfileIds)
            {
                SoftwareStatementProfileKeySecretsItem item = itemRepo.GetAsync(id).GetAwaiter().GetResult();
                softwareStatementProfiles.TryAdd(key: item.Id, value: item);
                string cert = item.TransportCertificate; // TODO: check not-null
                string key = item.TransportKey; // TODO: check non-null
                X509Certificate2 certificate = CertificateFactories.GetCertificate2FromPem(privateKey: key, pem: cert);
                List<X509Certificate2> certificates = new List<X509Certificate2> { certificate };
                services.AddHttpClient(item.HttpClientName())
                    .ConfigurePrimaryHttpMessageHandler(
                        sp =>
                        {
                            HttpMessageHandler handler = new HttpRequestBuilder()
                                .SetClientCertificates(certificates)
                                .CreateMessageHandler();

                            return handler;
                        });
            }

            services.AddSingleton<IObcConfigurationProvider>(sp => new DefaultConfigurationProvider(obcConfig));
            services.AddSingleton<IInstrumentationClient, LoggerInstrumentationClient>();
            services.AddSingleton(x => keySecretProvider);
            services.AddSingleton(x => (IKeySecretReadOnlyProvider) x.GetRequiredService<IKeySecretProvider>());

            // Create API client not associated with software statement profile
            string defaultHttpClientName = "default";
            services.AddHttpClient(defaultHttpClientName)
                .ConfigurePrimaryHttpMessageHandler(
                    sp =>
                    {
                        HttpMessageHandler handler = new HttpRequestBuilder()
                            .CreateMessageHandler();

                        return handler;
                    });
            services.AddSingleton<IApiClient>(
                sp =>
                {
                    IHttpClientFactory hcf = sp.GetRequiredService<IHttpClientFactory>();
                    HttpClient client = hcf.CreateClient(defaultHttpClientName);

                    return new ApiClient(
                        instrumentation: sp.GetRequiredService<IInstrumentationClient>(),
                        httpClient: client);
                });
            services.AddSingleton<ICertificateReader, PemParsingCertificateReader>();
            services.AddSingleton<IEntityMapper, EntityMapper>();

            // Configure DB
            switch (obcConfig.ProcessedDbProvider)
            {
                case DbProvider.Sqlite:
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
            services.AddScoped<IDbEntityRepositoryFactory, DbEntityRepositoryFactory>();
            services.AddScoped(
                serviceType: typeof(IDbEntityRepository<>),
                implementationType: typeof(DbEntityRepository<>));
            services
                .AddSingleton<IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached>,
                    ReadOnlyKeySecretItemCache<SoftwareStatementProfileCached>>(
                    sp =>
                    {
                        IInstrumentationClient instrumentationClient = sp.GetRequiredService<IInstrumentationClient>();
                        IHttpClientFactory httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                        ConcurrentDictionary<string, SoftwareStatementProfileCached> z =
                            new ConcurrentDictionary<string, SoftwareStatementProfileCached>();
                        foreach (string id in activeSoftwareStatementProfileIds)
                        {
                            softwareStatementProfiles.TryGetValue(
                                key: id,
                                value: out SoftwareStatementProfileKeySecretsItem value);
                            HttpClient client = httpClientFactory.CreateClient(value.HttpClientName());
                            ApiClient apiClient = new ApiClient(
                                instrumentation: instrumentationClient,
                                httpClient: client);

                            SoftwareStatementProfileCached ss2 = new SoftwareStatementProfileCached(
                                profileKeySecrets: value,
                                apiClient: apiClient);
                            z.TryAdd(key: ss2.Id, value: ss2);
                        }

                        return new ReadOnlyKeySecretItemCache<SoftwareStatementProfileCached>(z);
                    });
            services.AddScoped<IRequestBuilder, RequestBuilder>();

            // Startup tasks
            services.AddHostedService<StartupTasksHostedService>();

            return services;
        }
    }
}
