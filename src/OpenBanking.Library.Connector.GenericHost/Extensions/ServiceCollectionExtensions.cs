﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGenericHostServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add settings groups
        services
            .AddSettingsGroup<DatabaseSettings>()
            .AddSettingsGroup<BankProfilesSettings>()
            .AddSettingsGroup<SoftwareStatementProfilesSettings>()
            .AddSettingsGroup<TransportCertificateProfilesSettings>()
            .AddSettingsGroup<SigningCertificateProfilesSettings>()
            .AddSettingsGroup<KeysSettings>();

        // Set up software statement store
        services
            .AddSingleton<IProcessedSoftwareStatementProfileStore,
                ProcessedSoftwareStatementProfileStore>();

        // Set up encryption key
        services
            .AddSingleton<IEncryptionKeyInfo, EncryptionKeyInfo>();

        // Set up bank profile definitions
        services.AddSingleton<IBankProfileService, BankProfileService>();

        // Set up time provider
        services.AddSingleton<ITimeProvider, TimeProvider>();

        // Set up mapper for API variants (different Open Banking standards)
        services.AddSingleton<IApiVariantMapper, ApiVariantMapper>();

        // Get DB settings and determine connection string
        var databaseSettings = GetSettings<DatabaseSettings>(configuration);
        string connectionString = GetConnectionString(databaseSettings, configuration);

        // Configure DB
        switch (databaseSettings.Provider)
        {
            case DbProvider.Sqlite:
                services
                    // See e.g. https://jasonwatmore.com/post/2020/01/03/aspnet-core-ef-core-migrations-for-multiple-databases-sqlite-and-sql-server 
                    .AddDbContext<BaseDbContext, SqliteDbContext>(options => { options.UseSqlite(connectionString); });
                break;
            case DbProvider.PostgreSql:
                services.AddDbContext<BaseDbContext, PostgreSqlDbContext>(
                    options => { options.UseNpgsql(connectionString); });
                break;
            default:
                throw new ArgumentException("Unsupported DB provider", configuration["DbProvider"]);
        }

        // Configure DB service
        services.AddScoped<IDbService, DbService>();

        // Set up logging
        services.AddSingleton<IInstrumentationClient, LoggerInstrumentationClient<RequestBuilder>>();

        // Set up API client not associated with software statement profile
        services.AddSingleton<IApiClient>(
            sp => new ApiClient(
                sp.GetRequiredService<
                    IInstrumentationClient>())); // IHttpClientFactory no longer needed as SocketsHttpHandler now used by default

        // Configure Request Builder
        services.AddScoped<IRequestBuilder, RequestBuilder>();

        // Startup tasks
        services.AddHostedService<StartupTasksHostedService>();

        return services;
    }

    public static TSettings GetSettings<TSettings>(IConfiguration configuration)
        where TSettings : class, ISettings<TSettings>, new() =>
        configuration
            .GetSection(new TSettings().SettingsGroupName)
            .Get<TSettings>() ?? new TSettings() // default object created if necessary
            .Validate();

    public static IServiceCollection AddSettingsGroup<TSettings>(this IServiceCollection services)
        where TSettings : class, ISettings<TSettings>, new()
    {
        // Get settings group from configuration and validate
        services
            .AddOptions<TSettings>()
            .BindConfiguration(new TSettings().SettingsGroupName)
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

    public static string GetConnectionString(DatabaseSettings settings, IConfiguration configuration)
    {
        if (!settings.ConnectionStrings.TryGetValue(settings.Provider, out string? connectionString))
        {
            throw new ArgumentException($"No database connection string found for provider {settings.Provider}.");
        }

        string? password = null;
        if (settings.PasswordSettingNames.TryGetValue(settings.Provider, out string? passwordSettingName))
        {
            password = configuration.GetValue<string>(passwordSettingName, "");
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(
                    $"Cannot get non-empty password from specified setting {passwordSettingName}.");
            }
        }

        return settings.Provider switch
        {
            DbProvider.Sqlite => connectionString,
            DbProvider.PostgreSql => connectionString + (password is not null ? $";Password={password}" : ""),
            _ => throw new ArgumentOutOfRangeException(nameof(settings.Provider), settings.Provider, null)
        };
    }
}
