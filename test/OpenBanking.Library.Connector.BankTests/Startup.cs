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
using Newtonsoft.Json.Serialization;

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
            Assembly assembly = typeof(RedirectController).Assembly;

            services.AddControllers()
                .AddApplicationPart(assembly)
                .AddNewtonsoftJson(
                    options =>
                        options.SerializerSettings.ContractResolver =
                            new CamelCasePropertyNamesContractResolver());
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

            string pathToWebHostProjectRoot = Path.Combine(
                path1: env.ContentRootPath,
                path2: "../../../open-banking-connector-csharp/src/OpenBanking.Library.Connector.WebHost");

            Helpers.AddStaticFiles(app: app, pathToWebHostProjectRoot: pathToWebHostProjectRoot);

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
