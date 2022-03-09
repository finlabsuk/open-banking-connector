// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.HostedServices;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGenericHostServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add settings groups
            services
                .AddSettingsGroup<OpenBankingConnectorSettings>(configuration)
                .AddSettingsGroup<BankProfilesSettings>(configuration)
                .AddSettingsGroup<SoftwareStatementProfilesSettings>(configuration)
                .AddSettingsGroup<TransportCertificateProfilesSettings>(configuration)
                .AddSettingsGroup<SigningCertificateProfilesSettings>(configuration);

            // Set up bank profile definitions
            services.AddSingleton(
                sp =>
                {
                    BankProfilesSettings bankProfilesSettings =
                        sp.GetRequiredService<ISettingsProvider<BankProfilesSettings>>().GetSettings();
                    return new BankProfileDefinitions(
                        DataFile.ReadFile<Dictionary<string, Dictionary<string, BankProfileHiddenProperties>>>(
                            bankProfilesSettings.HiddenPropertiesFile,
                            new JsonSerializerSettings()).GetAwaiter().GetResult());
                });

            // Set up software statement store
            services
                .AddSingleton<IProcessedSoftwareStatementProfileStore,
                    ProcessedSoftwareStatementProfileStore>();

            // Set up time provider
            services.AddSingleton<ITimeProvider, TimeProvider>();

            // Set up logging
            services.AddSingleton<IInstrumentationClient, LoggerInstrumentationClient>();

            // Set up API client not associated with software statement profile
            services.AddSingleton<IApiClient>(
                sp => new ApiClient(
                    sp.GetRequiredService<IInstrumentationClient>(),
                    new HttpClient(
                        new HttpRequestBuilder()
                            .CreateMessageHandler()))); // IHttpClientFactory no longer needed as SocketsHttpHandler now used by default

            // Set up mapper for API variants (different Open Banking standards)
            services.AddSingleton<IApiVariantMapper, ApiVariantMapper>();

            // Configure DB
            DatabaseOptions databaseOptions = configuration
                .GetSection(new OpenBankingConnectorSettings().SettingsGroupName)
                .Get<OpenBankingConnectorSettings>()
                .Validate()
                .Database;
            string connectionString = configuration.GetConnectionString(databaseOptions.ConnectionStringName) ??
                                      throw new ArgumentException("Database connection string not found.");
            switch (databaseOptions.Provider)
            {
                case DbProvider.Sqlite:
                    services
                        // See e.g. https://jasonwatmore.com/post/2020/01/03/aspnet-core-ef-core-migrations-for-multiple-databases-sqlite-and-sql-server 
                        .AddDbContext<BaseDbContext, SqliteDbContext>(
                            options =>
                            {
                                options
                                    .UseSqlite(connectionString)
                                    .ConfigureWarnings(
                                        warnings =>
                                            // Suppress warnings relating to non-root auth context entities since can manually apply
                                            // unsupported global query filter to entity queries
                                            warnings.Ignore(
                                                CoreEventId
                                                    .PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
                            });
                    break;
                default:
                    throw new ArgumentException("Unknown DB provider", configuration["DbProvider"]);
            }

            // Configure DB service
            services.AddScoped<IDbService, DbService>();

            // Configure Request Builder
            services.AddScoped<IRequestBuilder, RequestBuilder>();

            // Startup tasks
            services.AddHostedService<StartupTasksHostedService>();

            return services;
        }

        public static IServiceCollection AddSettingsGroup<TSettings>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TSettings : class, ISettings<TSettings>, new()
        {
            // Get settings group from configuration and validate
            services
                .AddOptions<TSettings>()
                .Bind(configuration.GetSection(new TSettings().SettingsGroupName))
                .ValidateDataAnnotations()
                .Validate(
                    settings =>
                    {
                        settings.Validate();
                        return true;
                    })
                .ValidateOnStart();

            // Convert to ISettingsProvider which is independent of .NET Generic Host
            services
                .AddSingleton<ISettingsProvider<TSettings>,
                    ConfigurationSettingsProvider<TSettings>>();

            return services;
        }
    }
}
