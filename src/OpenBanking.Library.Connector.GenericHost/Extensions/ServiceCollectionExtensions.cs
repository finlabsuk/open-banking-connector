// Licensed to Finnovation Labs Limited under one or more agreements.
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
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TimeProvider = FinnovationLabs.OpenBanking.Library.Connector.Services.TimeProvider;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGenericHostServices(
        this IServiceCollection services,
        IConfiguration configuration,
        string? serviceVersion = null,
        Action<TracerProviderBuilder>? addAspNetCoreTracingInstrumentation = null)
    {
        // Add settings groups
        services
            .AddSettingsGroup<DatabaseSettings>()
            .AddSettingsGroup<BankProfilesSettings>()
            .AddSettingsGroup<HttpClientSettings>();

        // Add secret provider
        services.AddSingleton<ISecretProvider, SecretProvider>();

        // Add settings service (caches database settings record)
        services.AddSingleton<ISettingsService, SettingsService>();

        // Set up bank profile definitions
        services.AddSingleton<IBankProfileService, BankProfileService>();

        // Set up time provider
        services.AddSingleton<ITimeProvider, TimeProvider>();

        // Set up mapper for API variants (different Open Banking standards)
        services.AddSingleton<IApiVariantMapper, ApiVariantMapper>();

        // Get DB settings and determine connection string
        var databaseSettings = GetSettings<DatabaseSettings>(configuration);
        services.AddSingleton<IDbConnectionString, DbConnectionString>(
            sp =>
            {
                var secretProvider = sp.GetRequiredService<ISecretProvider>();
                return new DbConnectionString(databaseSettings, secretProvider);
            });

        // Configure DB
        switch (databaseSettings.Provider)
        {
            case DbProvider.Sqlite:
                services
                    // See e.g. https://jasonwatmore.com/post/2020/01/03/aspnet-core-ef-core-migrations-for-multiple-databases-sqlite-and-sql-server 
                    .AddDbContext<BaseDbContext, SqliteDbContext>(
                        (sp, optionsBuilder) =>
                        {
                            var connectionStringService = sp.GetRequiredService<IDbConnectionString>();
                            optionsBuilder.UseSqlite(connectionStringService.GetConnectionString());
                        });
                break;
            case DbProvider.PostgreSql:
                services.AddDbContext<BaseDbContext, PostgreSqlDbContext>(
                    (sp, optionsBuilder) =>
                    {
                        var connectionStringService = sp.GetRequiredService<IDbConnectionString>();
                        optionsBuilder.UseNpgsql(
                            connectionStringService.GetConnectionString(),
                            options => options.EnableRetryOnFailure());
                    });
                break;
            case DbProvider.MongoDb:
                string databaseName = databaseSettings.Names[DbProvider.MongoDb];
                services.AddSingleton<IMongoClient>(
                    sp =>
                    {
                        var connectionStringService = sp.GetRequiredService<IDbConnectionString>();
                        return new MongoClient(connectionStringService.GetConnectionString());
                    });
                services.AddSingleton<IMongoDatabase>(
                    sp =>
                    {
                        var mongoClient = sp.GetRequiredService<IMongoClient>();
                        return mongoClient.GetDatabase(databaseName);
                    });
                services.AddDbContext<BaseDbContext, MongoDbDbContext>(
                    (sp, optionsBuilder) =>
                    {
                        var mongoClient = sp.GetRequiredService<IMongoClient>();
                        optionsBuilder.UseMongoDB(mongoClient, databaseName);
                    });
                break;
            default:
                throw new ArgumentException("Unsupported DB provider", configuration["DbProvider"]);
        }

        // Configure DB service
        services.AddScoped<IDbService, DbService>();

        // Set up logging
        services.AddSingleton<IInstrumentationClient, LoggerInstrumentationClient<RequestBuilder>>();

        // Set up TPP reporting metrics
        services.AddSingleton<TppReportingMetrics>();

        // Set up API client not associated with software statement profile
        var httpClientSettings = GetSettings<HttpClientSettings>(configuration);
        services.AddSingleton<IApiClient>(
            sp => new ApiClient(
                sp.GetRequiredService<
                    IInstrumentationClient>(),
                httpClientSettings
                    .PooledConnectionLifetimeSeconds,
                sp.GetRequiredService<TppReportingMetrics>()));

        // Configure Request Builder
        services.AddScoped<IRequestBuilder, RequestBuilder>();

        // Startup tasks
        services.AddHostedService<StartupTasksHostedService>();

        // Add OpenTelemetry services
        var openTelemetrySettings = GetSettings<OpenTelemetrySettings>(configuration);
        services.AddOpenTelemetryServices(serviceVersion, openTelemetrySettings, addAspNetCoreTracingInstrumentation);

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

    public static ILoggingBuilder AddGenericHostLogging(
        this ILoggingBuilder loggingBuilder,
        IConfiguration configuration,
        string? serviceVersion)
    {
        var openTelemetrySettings = GetSettings<OpenTelemetrySettings>(configuration);
        loggingBuilder.AddOpenTelemetryLogging(serviceVersion, openTelemetrySettings);
        return loggingBuilder;
    }

    private static void AddOpenTelemetryServices(
        this IServiceCollection services,
        string? serviceVersion,
        OpenTelemetrySettings openTelemetrySettings,
        Action<TracerProviderBuilder>? addAspNetCoreTracingInstrumentation = null)
    {
        bool useConsoleExporter = openTelemetrySettings.UseConsoleExporter;

        ProviderFilter tracingProviderFilter = openTelemetrySettings.Tracing.ProviderFilter;

        string? tracingOtlpExporterUrlProcessed = openTelemetrySettings.Tracing.OtlpExporterUrlProcessed;

        void ConfigureTracing(TracerProviderBuilder builder)
        {
            if (tracingProviderFilter.HasFlag(ProviderFilter.HttpClient))
            {
                builder.AddHttpClientInstrumentation();
            }

            if (tracingProviderFilter.HasFlag(ProviderFilter.EfCore))
            {
                builder.AddEntityFrameworkCoreInstrumentation();
            }

            if (tracingProviderFilter.HasFlag(ProviderFilter.AspNetCore) &&
                addAspNetCoreTracingInstrumentation is not null)
            {
                addAspNetCoreTracingInstrumentation(builder);
            }

            if (useConsoleExporter)
            {
                builder.AddConsoleExporter();
            }

            if (tracingOtlpExporterUrlProcessed is not null)
            {
                builder.AddOtlpExporter(opt => opt.Endpoint = new Uri(tracingOtlpExporterUrlProcessed));
            }
        }

        string? metricsOtlpExporterUrlProcessed = openTelemetrySettings.Metrics.OtlpExporterUrlProcessed;

        void ConfigureMetrics(MeterProviderBuilder builder)
        {
            //builder.AddMeter("System.Net.Http");
            builder.AddMeter("TppReportingMetrics");

            if (useConsoleExporter)
            {
                builder.AddConsoleExporter(
                    (_, metricReaderOptions) =>
                    {
                        metricReaderOptions.PeriodicExportingMetricReaderOptions =
                            new PeriodicExportingMetricReaderOptions
                            {
                                ExportIntervalMilliseconds = openTelemetrySettings.Metrics
                                    .MetricReaderExportIntervalMilliseconds
                            };
                        metricReaderOptions.TemporalityPreference =
                            openTelemetrySettings.Metrics.MetricReaderTemporality;
                    });
            }

            if (metricsOtlpExporterUrlProcessed is not null)
            {
                builder.AddOtlpExporter(
                    (otlpExporterOptions, metricReaderOptions) =>
                    {
                        otlpExporterOptions.Endpoint = new Uri(metricsOtlpExporterUrlProcessed);
                        metricReaderOptions.PeriodicExportingMetricReaderOptions =
                            new PeriodicExportingMetricReaderOptions
                            {
                                ExportIntervalMilliseconds = openTelemetrySettings.Metrics
                                    .MetricReaderExportIntervalMilliseconds
                            };
                        metricReaderOptions.TemporalityPreference =
                            openTelemetrySettings.Metrics.MetricReaderTemporality;
                    });
            }
        }


        Sdk.SetDefaultTextMapPropagator(
            new CompositeTextMapPropagator(
                Array.Empty<TextMapPropagator>())); // See https://github.com/dotnet/runtime/issues/90407

        bool anyTracingExporter = openTelemetrySettings.HasAnyTracingExporter;
        bool anyMetricsExporter = openTelemetrySettings.HasAnyMetricsExporter;

        if (anyTracingExporter || anyMetricsExporter)
        {
            OpenTelemetryBuilder otelBuilder = services.AddOpenTelemetry()
                .ConfigureResource(
                    resource => resource.AddService(
                        openTelemetrySettings.ServiceName,
                        openTelemetrySettings.ServiceNamespace,
                        serviceVersion,
                        false,
                        openTelemetrySettings.ServiceInstanceId));

            if (anyTracingExporter)
            {
                otelBuilder.WithTracing(ConfigureTracing);
            }

            if (anyMetricsExporter)
            {
                otelBuilder.WithMetrics(ConfigureMetrics);
            }
        }
    }

    private static void AddOpenTelemetryLogging(
        this ILoggingBuilder loggingBuilder,
        string? serviceVersion,
        OpenTelemetrySettings openTelemetrySettings)
    {
        if (openTelemetrySettings.Logging.OtlpExporterUrlProcessed is not null)
        {
            loggingBuilder.AddOpenTelemetry(
                options =>
                {
                    options
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(
                                    openTelemetrySettings.ServiceName,
                                    openTelemetrySettings.ServiceNamespace,
                                    serviceVersion,
                                    false,
                                    openTelemetrySettings.ServiceInstanceId))
                        .AddOtlpExporter(
                            otlpExporterOptions => otlpExporterOptions.Endpoint =
                                new Uri(openTelemetrySettings.Logging.OtlpExporterUrlProcessed));
                });
        }
    }
}
