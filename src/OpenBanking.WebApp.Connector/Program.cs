// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Web;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.WebApp.Connector.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;
using ServiceCollectionExtensionsWeb =
    FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions.ServiceCollectionExtensions;
using ServiceCollectionExtensionsGenericHost =
    FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions.ServiceCollectionExtensions;

// Create builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Update configuration
builder.Host.AddGenericHostConfiguration(args);

// Add services

// Service version is taken to be assembly file version without revision number (see https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/versioning#assembly-file-version for info on assembly file version)
string serviceVersion =
    Assembly
        .GetExecutingAssembly()
        .GetCustomAttribute<AssemblyFileVersionAttribute>()!
        .Version;
serviceVersion = serviceVersion.Remove(serviceVersion.LastIndexOf('.'));

builder.Services
    // Add .NET generic host app services 
    .AddGenericHostServices(builder.Configuration)
    // Add .NET web host app services
    .AddWebHostServices(builder.Configuration, serviceVersion)
    // Configure Swagger
    .AddSwaggerGen(
        options =>
        {
            options.SwaggerDoc(
                "manage",
                new OpenApiInfo
                {
                    Title = "Management API",
                    Version = serviceVersion,
                    Description = "Management API for Open Banking Connector Web App"
                });
            options.SwaggerDoc(
                "aisp",
                new OpenApiInfo
                {
                    Title = "Account and Transaction API",
                    Version = serviceVersion,
                    Description = "Account and Transaction API for Open Banking Connector Web App"
                });
            options.SwaggerDoc(
                "pisp",
                new OpenApiInfo
                {
                    Title = "Payment Initiation API",
                    Version = serviceVersion,
                    Description = "Payment Initiation API for Open Banking Connector Web App"
                });
            options.SwaggerDoc(
                "vrp",
                new OpenApiInfo
                {
                    Title = "Variable Recurring Payments API",
                    Version = serviceVersion,
                    Description = "Variable Recurring Payments API for Open Banking Connector Web App"
                });
            options.SwaggerDoc(
                "auth-contexts",
                new OpenApiInfo
                {
                    Title = "Auth Contexts API",
                    Version = serviceVersion,
                    Description = "Auth Contexts API for Open Banking Connector Web App"
                });
            options.SwaggerDoc(
                "test",
                new OpenApiInfo
                {
                    Title = "Testing (non-production) API",
                    Version = serviceVersion,
                    Description =
                        "Testing API for Open Banking Connector Web App. Endpoints should not be used in production."
                });

            // Add XML from this assembly
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            // Add XML from OpenBankingLibrary.Connector
            xmlFilename = $"{typeof(BaseDbContext).GetTypeInfo().Assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            // Add XML from OpenBankingLibrary.BankApiModels
            xmlFilename = $"{typeof(AccountAndTransactionModelsPublic.Meta).GetTypeInfo().Assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            // Add XML from OpenBankingLibrary.GenericHost
            xmlFilename = $"{typeof(ServiceCollectionExtensionsGenericHost).GetTypeInfo().Assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            // Add XML from OpenBankingLibrary.Web
            xmlFilename =
                $"{typeof(ServiceCollectionExtensionsWeb).GetTypeInfo().Assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        })
    .AddSwaggerGenNewtonsoftSupport()
    // Add memory cache
    .AddMemoryCache()
    // Add controllers
    .AddControllers(
        options =>
        {
            // Add filter
            options.Filters.Add<ExternalApiHttpErrorExceptionFilter>();
            options.Filters.Add<GlobalExceptionFilter>();
        })
    // Add JSON support
    .AddNewtonsoftJson(
        options =>
        {
            options.SerializerSettings.ContractResolver =
                new DefaultContractResolver(); // no to CamelCase
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        });

// Build app
WebApplication app = builder.Build();

// Errors
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Add default files
app.UseDefaultFilesLocal();

// Add local static files
app.UseStaticFiles();

// Add web host static files
app.UseWebHostStaticFiles();

// Add Swagger generation
app.UseSwagger();
app.UseSwaggerUI(
    c =>
    {
        c.SwaggerEndpoint("/swagger/manage/swagger.json", "Management API");
        c.SwaggerEndpoint("/swagger/aisp/swagger.json", "Account and Transaction API");
        c.SwaggerEndpoint("/swagger/pisp/swagger.json", "Payment Initiation API");
        c.SwaggerEndpoint("/swagger/vrp/swagger.json", "Variable Recurring Payments API");
        c.SwaggerEndpoint("/swagger/auth-contexts/swagger.json", "Auth Contexts API");
        c.SwaggerEndpoint("/swagger/test/swagger.json", "Testing (non-production) API");
    });

// Add controller endpoints
app.MapControllers();

// Run 
app.Run();
