// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Web.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebHostServices(
        this IServiceCollection services,
        IConfiguration configuration,
        string? serviceVersion)
    {
        var openTelemetrySettings =
            GenericHost.Extensions.ServiceCollectionExtensions.GetSettings<OpenTelemetrySettings>(configuration);
        services.AddOpenTelemetry(serviceVersion, openTelemetrySettings);

        // Startup tasks
        services.AddHostedService<WebAppInformationHostedService>();

        return services;
    }

    public static ILoggingBuilder AddWebHostLogging(
        this ILoggingBuilder loggingBuilder,
        IConfiguration configuration,
        string? serviceVersion)
    {
        var openTelemetrySettings =
            GenericHost.Extensions.ServiceCollectionExtensions.GetSettings<OpenTelemetrySettings>(configuration);
        loggingBuilder.AddOpenTelemetry(serviceVersion, openTelemetrySettings);

        return loggingBuilder;
    }

    private static void AddOpenTelemetry(
        this ILoggingBuilder loggingBuilder,
        string? serviceVersion,
        OpenTelemetrySettings openTelemetrySettings)
    {
        string? otlpExporterUrl = openTelemetrySettings.Logging.OtlpExporterUrl.Length > 0
            ? openTelemetrySettings.Logging.OtlpExporterUrl
            : null;

        if (otlpExporterUrl is not null)
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
                        .AddOtlpExporter(otlpExporterOptions => otlpExporterOptions.Endpoint = new Uri(otlpExporterUrl));
                });
        }
    }

    private static void AddOpenTelemetry(
        this IServiceCollection services,
        string? serviceVersion,
        OpenTelemetrySettings openTelemetrySettings)
    {
        Sdk.SetDefaultTextMapPropagator(
            new CompositeTextMapPropagator(
                Array.Empty<TextMapPropagator>())); // See https://github.com/dotnet/runtime/issues/90407
        
        bool useConsoleExporter = openTelemetrySettings.UseConsoleExporter;

        ProviderFilter tracingProviderFilter = openTelemetrySettings.Tracing.ProviderFilter;

        string? tracingOtlpExporterUrl = openTelemetrySettings.Tracing.OtlpExporterUrl.Length > 0
            ? openTelemetrySettings.Tracing.OtlpExporterUrl
            : null;

        string? metricsOtlpExporterUrl = openTelemetrySettings.Metrics.OtlpExporterUrl.Length > 0
            ? openTelemetrySettings.Metrics.OtlpExporterUrl
            : null;

        void ConfigureTracing(TracerProviderBuilder builder)
        {
            if (tracingProviderFilter.HasFlag(ProviderFilter.AspNetCore))
            {
                builder.AddAspNetCoreInstrumentation();
            }

            if (tracingProviderFilter.HasFlag(ProviderFilter.HttpClient))
            {
                builder.AddHttpClientInstrumentation();
            }

            if (tracingProviderFilter.HasFlag(ProviderFilter.EfCore))
            {
                builder.AddEntityFrameworkCoreInstrumentation();
            }

            if (useConsoleExporter)
            {
                builder.AddConsoleExporter();
            }

            if (tracingOtlpExporterUrl is not null)
            {
                builder.AddOtlpExporter(opt => opt.Endpoint = new Uri(tracingOtlpExporterUrl));
            }
        }

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

            if (metricsOtlpExporterUrl is not null)
            {
                builder.AddOtlpExporter(
                    (otlpExporterOptions, metricReaderOptions) =>
                    {
                        otlpExporterOptions.Endpoint = new Uri(metricsOtlpExporterUrl);
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

        bool useTracing = tracingOtlpExporterUrl is not null || useConsoleExporter;
        bool useMetrics = metricsOtlpExporterUrl is not null || useConsoleExporter;

        if (useTracing || useMetrics)
        {
            OpenTelemetryBuilder builder = services.AddOpenTelemetry()
                .ConfigureResource(
                    resource => resource.AddService(
                        openTelemetrySettings.ServiceName,
                        openTelemetrySettings.ServiceNamespace,
                        serviceVersion,
                        false,
                        openTelemetrySettings.ServiceInstanceId));
            if (useTracing)
            {
                builder.WithTracing(ConfigureTracing);
            }
            if (useMetrics)
            {
                builder.WithMetrics(ConfigureMetrics);
            }
        }
    }
}
