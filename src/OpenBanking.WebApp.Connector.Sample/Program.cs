// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.NetGenericHost.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args)
                .Build();
            host.Services.CheckDbExists();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder
                            .UseUrls("https://*:5001/", "http://*:5000/")
                            .ConfigureLogging(
                                (whbc, lb) =>
                                {
                                    lb.AddConfiguration(whbc.Configuration.GetSection("Logging"));
                                    lb.AddConsole();
                                })
                            .UseStartup<Startup>();
                    });
    }
}
