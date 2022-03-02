// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.WebApp.Connector.Extensions;
using FinnovationLabs.OpenBanking.WebApp.Connector.KeySecrets;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
                "v1",
                new OpenApiInfo
                {
                    Title = "Open Banking Connector Web App API",
                    Version = "V1",
                    Description = "API for Web App version of Open Banking Connector"
                });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        })
    .AddSwaggerGenNewtonsoftSupport()
    // Add controllers
    .AddControllers()
    // Add JSON support
    .AddNewtonsoftJson(
        options =>
        {
            options.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
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
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenBankingConnector"); });

// Add controller endpoints
app.MapControllers();

// Run 
app.Run();
