// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.KeySecrets;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureKeySecrets(KeySecretProviders.Providers)
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder
                            .UseStartup<Startup>();
                    });
    }
}
