// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                //.AddJsonOptions(options =>
                // {
                //     options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                //     options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                //     options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                // });
                .AddNewtonsoftJson(
                    options =>
                        options.SerializerSettings.ContractResolver =
                            new CamelCasePropertyNamesContractResolver());

            // Configure Swagger
            services
                .AddSwaggerGen(
                    c =>
                    {
                        c.SwaggerDoc(
                            name: "v1",
                            info: new OpenApiInfo
                            {
                                Title = "Open Banking Connector",
                                Version = "V1"
                            });
                    })
                .AddSwaggerGenNewtonsoftSupport();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            string? pathToWebHostProjectRoot = Path.Combine(
                path1: env.ContentRootPath,
                path2: "../OpenBanking.Library.Connector.WebHost");

            Helpers.AddStaticFiles(app: app, pathToWebHostProjectRoot: pathToWebHostProjectRoot);

            app.UseSwagger();
            app.UseSwaggerUI(
                c => { c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "OpenBankingConnector"); });

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
