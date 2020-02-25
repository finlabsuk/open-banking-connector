// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample
{
    public class Program
    {
        private static void CheckDbExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<BaseDbContext>();
            var creator = context.Database.GetService<IRelationalDatabaseCreator>();

            if (!creator.Exists())
            {
                throw new ApplicationException(
                    "No database found. Run 'dotnet ef database update' in OpenBanking.WebApp.Connector root folder to create test DB.");
            }
        }

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            CheckDbExists(host);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls("https://*:5001/", "http://*:5000/")
                        .ConfigureLogging((whbc, lb) =>
                        {
                            lb.AddConfiguration(whbc.Configuration.GetSection("Logging"));
                            lb.AddConsole();
                        })
                        .UseStartup<Startup>();
                });
    }
}
