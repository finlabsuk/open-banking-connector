// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServiceCollectionExtensions =
    FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions.ServiceCollectionExtensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class AppContextFixture : IDisposable
{
    public AppContextFixture()
    {
        // Create builder
        string[] args = Array.Empty<string>();

        WebApplicationBuilder builder =
            WebApplication.CreateBuilder(
                new WebApplicationOptions
                {
                    Args = args,
                    // Ensure correct application name to allow loading of user secrets                
                    ApplicationName = typeof(AppContextFixture).GetTypeInfo().Assembly.GetName().Name
                });

        // Add services
        Assembly webHostAssembly = typeof(ServiceCollectionExtensions).Assembly;

        builder.Services
            // Add .NET web host app services
            .AddWebHostServices()
            // Add bank testing services
            .AddBankTestingServices()
            // Add memory cache
            .AddMemoryCache()
            // Add controllers
            .AddControllers()
            // Add controllers from web host library (explicit add apparently required since not using Microsoft.NET.Sdk.Web)
            .AddApplicationPart(webHostAssembly)
            // Add JSON support
            .AddNewtonsoftJson(
                options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver(); // no to CamelCase
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

        builder
            .Logging
            .ClearProviders()
            .AddConsole();

        // Build app
        WebApplication app = builder.Build();
        Host = app;

        // Errors: always suppress exception details (including in development)
        app.UseExceptionHandler("/error");

        app.UseHsts();

        // Add static files
        app.UseWebHostStaticFiles();

        // Add controller endpoints
        app.MapControllers();

        // Create and start .NET Generic Host
        app.StartAsync();
    }

    public IHost Host { get; }

    public void Dispose()
    {
        Host.Dispose();
    }
}
