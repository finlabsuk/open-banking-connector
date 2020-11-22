// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.HostedServices;
using Jering.Javascript.NodeJS;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public class AppContextFixture : ITestOutputHelperAccessor, IDisposable
    {
        private readonly AsyncLocal<ITestOutputHelper?> _asyncLocalOutputHelper = new AsyncLocal<ITestOutputHelper?>();
        private ITestOutputHelper? _outputHelper;
        private bool _useAsyncLocalOutputHelper;

        public AppContextFixture()
        {
            // Set up Generic Host (i.e. DI container) per test due to no possibility for scoped iLogger: https://github.com/aspnet/Logging/issues/678#issuecomment-322680083
            IHostBuilder builder = Helpers.CreateHostBuilder(new string[0]);

            // Since content root used to set base path for IConfigurationBuilder (which is used when loading "appsettings.json"),
            // we must make sure this points to project directory.
            // Since can't control current directory for XUnit test runner, use assembly directory to set content root
            string assemblyLocation = typeof(BaseTests).GetTypeInfo().Assembly.Location;
            DirectoryInfo file = new DirectoryInfo(assemblyLocation);
            DirectoryInfo projectRoot = file.Parent?.Parent?.Parent?.Parent ??
                                        throw new Exception("Can't determine project root from assembly location.");
            ProjectRootPath = projectRoot.ToString();
            string consentAuthoriserDirectory = Path.Combine(
                path1: ProjectRootPath,
                path2: "../BankTestsConsentAuthoriser/.build");

            builder.UseContentRoot(ProjectRootPath);

            string fileName = "FinnovationLabs.OpenBanking.Library.Connector.BankTests";
            builder = builder.ConfigureHostConfiguration(
                configBuilder =>
                {
                    configBuilder.AddInMemoryCollection(
                        new[]
                        {
                            new KeyValuePair<string, string>(
                                key: HostDefaults.ApplicationKey,
                                value: fileName ?? throw new ArgumentNullException(nameof(fileName)))
                        });
                });

            // Per-test logging
            builder.ConfigureLogging(p => p.AddXUnit(this));

            // Web host services
            builder.ConfigureServices(
                (hostContext, services) =>
                {
                    // Startup tasks
                    services.AddHostedService<WebAppInformationHostedService>();
                    services.AddNodeJS();
                    bool nodeJsInspectBrk = hostContext.Configuration["NodeJsInspectBrk"] == "true";
                    services.Configure<NodeJSProcessOptions>(
                        options =>
                        {
                            options.ProjectPath = consentAuthoriserDirectory;
                            if (nodeJsInspectBrk)
                            {
                                options.NodeAndV8Options = "--inspect-brk";
                            }
                        });
                    services.Configure<OutOfProcessNodeJSServiceOptions>(
                        options =>
                        {
                            if (nodeJsInspectBrk)
                            {
                                options.TimeoutMS = -1;
                            }
                        });

                    // Use of AsyncLocal for logging
                    _useAsyncLocalOutputHelper = hostContext.Configuration["AsyncLocalXunitLogging"] == "true";
                });

            // Web host configuration
            builder.ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder
                        .UseUrls("https://*:5001/", "http://*:5000/") // TODO: check if necessary.
                        .UseStartup<Startup>();
                });

            Host = builder
                .Build();
            Host.Start();
        }

        public IHost Host { get; }

        public string ProjectRootPath { get; }

        public void Dispose()
        {
            Host.Dispose();
        }

        public ITestOutputHelper? OutputHelper
        {
            get => _useAsyncLocalOutputHelper ? _asyncLocalOutputHelper.Value : _outputHelper;
            set
            {
                if (_useAsyncLocalOutputHelper)
                {
                    _asyncLocalOutputHelper.Value = value;
                }
                else
                {
                    _outputHelper = value;
                }
            }
        }
    }

    [CollectionDefinition("App context collection")]
    public class AppContextCollection : ICollectionFixture<AppContextFixture>
    {
        // Class solely for [CollectionDefinition] purpose.
    }
}
