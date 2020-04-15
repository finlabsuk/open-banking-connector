// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IConfigurationProvider = FinnovationLabs.OpenBanking.Library.Connector.Configuration.IConfigurationProvider;

namespace FinnovationLabs.OpenBanking.Library.Connector.AspNetCore
{
    public static class DbMethods
    {
        public static IHost CheckDbExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<BaseDbContext>();
            
            // Delete/Create DB as required (should normally be commented out)
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            
            // Check DB exists
            var creator = context.Database.GetService<IRelationalDatabaseCreator>();
            if (!creator.Exists())
            {
                throw new ApplicationException(
                    "No database found. Run 'dotnet ef database update' in OpenBanking.WebApp.Connector.Sample root folder to create test DB.");
            }
            
            return host;
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenBankingConnector(this IServiceCollection services,
            IConfiguration configuration)
        {
            const string httpClientName = "OBC";

            services.AddHttpClient(httpClientName)
                .ConfigurePrimaryHttpMessageHandler(sp =>
                {
                    var secrets = sp.GetService<IKeySecretProvider>();

                    var handler = new HttpRequestBuilder()
                        .SetClientCertificates(CertificateFactories.GetCertificates(secrets))
                        .CreateMessageHandler();

                    return handler;
                });

            services.AddSingleton<IConfigurationProvider>(sp => new AppsettingsConfigurationProvider(configuration));
            services.AddSingleton<IInstrumentationClient, InstrumentationClient>();
            services.AddSingleton(sp =>
            {
                var builder = new KeySecretBuilder();
                var configProvider = sp.GetService<IConfigurationProvider>();

                return builder.GetKeySecretProvider(configuration, configProvider.GetRuntimeConfiguration());
            });
            services.AddSingleton<IApiClient>(sp =>
            {
                var hcf = sp.GetService<IHttpClientFactory>();

                var client = hcf.CreateClient(httpClientName);

                return new ApiClient(sp.GetService<IInstrumentationClient>(), client);
            });
            services.AddSingleton<ICertificateReader, PemParsingCertificateReader>();
            services.AddSingleton<IEntityMapper, EntityMapper>();
            
            // Configure DB
            switch (configuration["DbProvider"])
            {
                case "Sqlite":
                    services
                        // See e.g. https://jasonwatmore.com/post/2020/01/03/aspnet-core-ef-core-migrations-for-multiple-databases-sqlite-and-sql-server 
                        .AddDbContext<BaseDbContext, SqliteDbContext>(options =>
                        {
                            var connectionString = configuration.GetConnectionString("SqliteDbContext");
                            options.UseSqlite(
                                connectionString
                            );
                        });
                    break;
                default:
                    throw new ArgumentException("Unknown DB provider", configuration["DbProvider"]);
            }
            services.AddScoped<IDbEntityRepository<SoftwareStatementProfile>,
                DbEntityRepository<SoftwareStatementProfile>>();
            services.AddScoped<IDbEntityRepository<BankClientProfile>,
                DbEntityRepository<BankClientProfile>>();
            services.AddScoped<IDbEntityRepository<ApiProfile>,
                DbEntityRepository<ApiProfile>>();
            services.AddScoped<IDbEntityRepository<DomesticConsent>,
                DbEntityRepository<DomesticConsent>>();
            services.AddScoped<IOpenBankingRequestBuilder, RequestBuilder>();

            return services;
        }
    }
}
