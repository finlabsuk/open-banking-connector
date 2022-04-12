// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.WebApp.Connector;
using FinnovationLabs.OpenBanking.WebApp.Connector.Extensions;
using FinnovationLabs.OpenBanking.WebApp.Connector.KeySecrets;
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

// Add services
builder.Services
    // Add .NET generic host app services 
    .AddGenericHostServices(builder.Configuration)
    // Add .NET web host app services
    .AddWebHostServices(builder.Configuration)
    // Configure Swagger
    .AddSwaggerGen(
        options =>
        {
            options.SwaggerDoc(
                "config",
                new OpenApiInfo
                {
                    Title = "Bank Configuration API",
                    Version = "code-generated",
                    Description = "Bank Configuration API for Open Banking Connector Web App"
                });
            options.SwaggerDoc(
                "aisp",
                new OpenApiInfo
                {
                    Title = "Account and Transaction API",
                    Version = "code-generated",
                    Description = "Account and Transaction API for Open Banking Connector Web App"
                });
            options.SwaggerDoc(
                "pisp",
                new OpenApiInfo
                {
                    Title = "Payment Initiation API",
                    Version = "code-generated",
                    Description = "Payment Initiation API for Open Banking Connector Web App"
                });
            options.SwaggerDoc(
                "vrp",
                new OpenApiInfo
                {
                    Title = "Variable Recurring Payments API",
                    Version = "code-generated",
                    Description = "Variable Recurring Payments API for Open Banking Connector Web App"
                });
            options.SwaggerDoc(
                "auth-contexts",
                new OpenApiInfo
                {
                    Title = "Auth Contexts API",
                    Version = "code-generated",
                    Description = "Auth Contexts API for Open Banking Connector Web App"
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
    // Add controllers
    .AddControllers(options =>
        {
            // Add filter
            options.Filters.Add<GlobalExceptionFilter>();
        })
        // Add JSON support
    .AddNewtonsoftJson(
        options =>
        {
            options.SerializerSettings.ContractResolver = new DefaultContractResolver(); // no to CamelCase
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            // options.SerializerSettings.Error = (o, eventArgs) =>
            // {
            //     if (eventArgs.ErrorContext.Error is ArgumentNullException x)
            //     {
            //         throw new ValidationException(x.Message);
            //     }
            //     Console.WriteLine("hi");
            // };
        });

// Add key secrets providers
builder.Host.ConfigureKeySecrets(KeySecretProviders.Providers);

// Build app
WebApplication app = builder.Build();

// Errors
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
        c.SwaggerEndpoint("/swagger/config/swagger.json", "Bank Configuration API");
        c.SwaggerEndpoint("/swagger/aisp/swagger.json", "Account and Transaction API");
        c.SwaggerEndpoint("/swagger/pisp/swagger.json", "Payment Initiation API");
        c.SwaggerEndpoint("/swagger/vrp/swagger.json", "Variable Recurring Payments API");
        c.SwaggerEndpoint("/swagger/auth-contexts/swagger.json", "Auth Contexts API");
    });

// Add controller endpoints
app.MapControllers();

// Run 
app.Run();
