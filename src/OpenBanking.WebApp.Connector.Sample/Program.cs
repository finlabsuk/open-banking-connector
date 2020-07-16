// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.HostedServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Use common host builder
            IHostBuilder builder = Helpers.CreateHostBuilder(args);

            builder.ConfigureServices(
                (hostContext, services) =>
                {
                    // Startup tasks
                    services.AddHostedService<WebAppInformationHostedService>();
                });

            builder.ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder
                        .UseUrls("https://*:5001/", "http://*:5000/") // TODO: check if necessary.
                        .UseStartup<Startup>();
                });

            IHost host = builder
                .Build();
            host.Run();
        }
    }
}
