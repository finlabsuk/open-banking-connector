// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.AspNetCore;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
                .AddOpenBankingConnector(Configuration)
                .AddControllers()
                //.AddJsonOptions(options =>
                // {
                //     options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                //     options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                //     options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                // });
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver());

            // Configure Swagger
            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Open Banking Connector",
                        Version = "V1"
                    });
                })
                .AddSwaggerGenNewtonsoftSupport();

            // Configure DB
            switch (Configuration["DbProvider"])
            {
                case "Sqlite":
                    services
                        // See e.g. https://jasonwatmore.com/post/2020/01/03/aspnet-core-ef-core-migrations-for-multiple-databases-sqlite-and-sql-server 
                        .AddDbContext<BaseDbContext, SqliteDbContext>(options =>
                        {
                            var connectionString = Configuration.GetConnectionString("SqliteDbContext");
                            options.UseSqlite(
                                connectionString
                            );
                        });
                    break;
                default:
                    throw new ArgumentException("Unknown DB provider", Configuration["DbProvider"]);
            }
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

            //app.UseHttpsRedirection();

            AddStaticFiles(app);

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenBankingConnector.NET"); });

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void AddStaticFiles(IApplicationBuilder app)
        {
            var fileOptions = new DefaultFilesOptions();
            fileOptions.DefaultFileNames.Clear();
            app.UseDefaultFiles(fileOptions);
            // var options = new RewriteOptions()
            //     .AddRewrite("^$", "index.html", true)
            //     .AddRewrite(@"^(.+)$", "$1.html", skipRemainingRules: true);
            // app.UseRewriter(options);
            app.UseStaticFiles();
        }
    }
}
