// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebHostServiceCollectionExtensions =
    FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions.ServiceCollectionExtensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
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
            Assembly webHostAssembly = typeof(WebHostServiceCollectionExtensions).Assembly;

            services
                // Add .NET generic host app services 
                .AddGenericHostServices(Configuration)
                // Add .NET web host app services
                .AddWebHostServices(Configuration)
                // Add bank testing services
                .AddBankTestingServices(Configuration)
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

            // Add web host static files
            app.UseWebHostStaticFiles();

            // Add controller endpoints
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
