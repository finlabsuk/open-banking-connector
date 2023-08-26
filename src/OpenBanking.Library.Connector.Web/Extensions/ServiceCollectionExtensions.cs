// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Web.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
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
        AddOpenTelemetry(services, serviceVersion, openTelemetrySettings);

        // Startup tasks
        services.AddHostedService<WebAppInformationHostedService>();

        return services;
    }

    private static void AddOpenTelemetry(
        IServiceCollection services,
        string? serviceVersion,
        OpenTelemetrySettings openTelemetrySettings)
    {
        Sdk.SetDefaultTextMapPropagator(
            new CompositeTextMapPropagator(
                Array.Empty<TextMapPropagator>())); // See https://github.com/dotnet/runtime/issues/90407

        string serviceName = openTelemetrySettings.ServiceName;
        string? otlpExporterUrl = openTelemetrySettings.Tracing.OtlpExporterUrl.Length > 0
            ? openTelemetrySettings.Tracing.OtlpExporterUrl
            : null;
        bool useConsoleExporter = openTelemetrySettings.Tracing.UseConsoleExporter;
        ProviderFilter providerFilter = openTelemetrySettings.Tracing.ProviderFilter;

        void ConfigureTracing(TracerProviderBuilder builder)
        {
            builder
                .AddSource(new ActivitySource(serviceName, serviceVersion).Name)
                .ConfigureResource(
                    resource => resource.AddService(
                        serviceName,
                        serviceVersion: serviceVersion));

            if (providerFilter.HasFlag(ProviderFilter.AspNetCore))
            {
                builder.AddAspNetCoreInstrumentation();
            }

            if (providerFilter.HasFlag(ProviderFilter.HttpClient))
            {
                builder.AddHttpClientInstrumentation();
            }

            if (providerFilter.HasFlag(ProviderFilter.EfCore))
            {
                builder.AddEntityFrameworkCoreInstrumentation();
            }

            if (useConsoleExporter)
            {
                builder.AddConsoleExporter();
            }

            if (otlpExporterUrl is not null)
            {
                builder.AddOtlpExporter(opt => opt.Endpoint = new Uri(otlpExporterUrl));
            }
        }

        if (otlpExporterUrl is not null || useConsoleExporter)
        {
            services.AddOpenTelemetry()
                .WithTracing(ConfigureTracing);
        }
    }
}
