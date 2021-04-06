// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public class AppContextFixture : ITestOutputHelperAccessor, IDisposable
    {
        private readonly AsyncLocal<ITestOutputHelper?> _asyncLocalOutputHelper =
            new AsyncLocal<ITestOutputHelper?>(); // to debug, can replace with "private ITestOutputHelper? _outputHelper" 

        public AppContextFixture()
        {
            // Create/identify data folder for reference test data logging.
            // Folder is subfolder of project folder to allow Git repo storage of reference test data.
            string projectRoot = FolderPaths.ProjectRoot;
            DataFolder = Path.Join(projectRoot, "data");
            Directory.CreateDirectory(DataFolder); // Create if doesn't exist

            // Create and start .NET Generic Host
            Host = CreateHostBuilder(new string[0]).Build();
            Host.Start();
        }

        public IHost Host { get; }

        public string DataFolder { get; }

        public void Dispose()
        {
            Host.Dispose();
        }

        public ITestOutputHelper? OutputHelper
        {
            get => _asyncLocalOutputHelper.Value;
            set => _asyncLocalOutputHelper.Value = value;
        }

        private IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureKeySecrets(KeySecretProviders.Providers)
                .ConfigureLogging(p => p.AddXUnit(this))
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder
                            .UseStartup<Startup>();
                    });

        [CollectionDefinition("App context collection")]
        public class AppContextCollection : ICollectionFixture<AppContextFixture>
        {
            // Class solely for [CollectionDefinition] purpose.
        }
    }
}
