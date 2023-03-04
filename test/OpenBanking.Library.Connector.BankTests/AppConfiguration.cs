// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    /// <summary>
    ///     Get app configuration outside of .NET Generic Host dependency injection.
    ///     This is used for test discovery in static context and when
    ///     configuring "Plain" app tests.
    /// </summary>
    public static class AppConfiguration
    {
        static AppConfiguration()
        {
            // Build dummy host to extract configuration etc
            _ = Host.CreateDefaultBuilder(new string[0])
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder
                            //.UseStartup<Startup>()
                            // Ensure correct application name to allow loading of user secrets
                            .UseSetting(
                                WebHostDefaults.ApplicationKey,
                                typeof(AppContextFixture).GetTypeInfo().Assembly.GetName().Name);
                    })
                .ConfigureAppConfiguration(
                    (context, builder) =>
                    {
                        EnvironmentName = context.HostingEnvironment.EnvironmentName;
                        //var appAssembly = Assembly.Load(new AssemblyName(context.HostingEnvironment.ApplicationName));
                        Configuration = builder.Build();
                    })
                .Build();
        }

        public static string EnvironmentName { get; set; } = null!;

        private static IConfiguration Configuration { get; set; } = null!;

        public static TSettings GetSettings<TSettings>()
            where TSettings : class, ISettings<TSettings>, new() =>
            ServiceCollectionExtensions.GetSettings<TSettings>(Configuration);
    }
}
