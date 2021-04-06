﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Extensions;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
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
                // Add .NET generic host app services 
                .AddGenericHostServices(Configuration)
                // Add .NET web host app services
                .AddWebHostServices(Configuration)
                // Configure Swagger
                .AddSwaggerGen(
                    c =>
                    {
                        c.SwaggerDoc(
                            "v1",
                            new OpenApiInfo
                            {
                                Title = "Open Banking Connector",
                                Version = "V1"
                            });
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenBankingConnector"); });

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
