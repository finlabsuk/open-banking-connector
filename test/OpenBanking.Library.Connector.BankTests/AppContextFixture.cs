// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;
using Xunit.Abstractions;
using ServiceCollectionExtensions =
    FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions.ServiceCollectionExtensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests;

public class AppContextFixture : ITestOutputHelperAccessor, IDisposable
{
    private readonly AsyncLocal<ITestOutputHelper?>
        _asyncLocalOutputHelper = new(); // to debug, can replace with "private ITestOutputHelper? _outputHelper" 

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

        // Update configuration
        builder.Host.AddGenericHostConfiguration(args);

        // Add services
        Assembly webHostAssembly = typeof(ServiceCollectionExtensions).Assembly;

        builder.Services
            // Add .NET generic host app services 
            .AddGenericHostServices(builder.Configuration)
            // Add .NET web host app services
            .AddWebHostServices(builder.Configuration)
            // Add bank testing services
            .AddBankTestingServices(builder.Configuration)
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

        // Add test logging
        builder.Host.ConfigureLogging(p => p.AddXUnit(this));

        // Build app
        WebApplication app = builder.Build();

        // Errors
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        // Add web host static files
        app.UseWebHostStaticFiles();

        // Add controller endpoints
        app.MapControllers();

        // Create and start .NET Generic Host
        app.Start();

        Host = app;
    }

    public IHost Host { get; }

    public void Dispose()
    {
        Host.Dispose();
    }

    public ITestOutputHelper? OutputHelper
    {
        get => _asyncLocalOutputHelper.Value;
        set => _asyncLocalOutputHelper.Value = value;
    }

    [CollectionDefinition("App context collection")]
    public class AppContextCollection : ICollectionFixture<AppContextFixture>
    {
        // Class solely for [CollectionDefinition] purpose.
    }
}
