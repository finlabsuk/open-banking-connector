// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
//using System.Diagnostics;
using FinnovationLabs.OpenBanking.Library.Connector.NetGenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    public abstract class RecordBaseCmdlet : BaseCmdlet
    {
        protected readonly IHost _host;

        public RecordBaseCmdlet(string verbName, string nounName) : base(verbName: verbName, nounName: nounName)
        {
            //Debugger.Launch();

            IHostBuilder builder = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        config.AddJsonFile(
                            path:
                            "appsettings.json");
                        config.AddEnvironmentVariables();
                        config.AddUserSecrets<RecordBaseCmdlet>();
                    })
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        //services.AddOptions();
                        //services.Configure<AppConfig>(hostContext.Configuration.GetSection("AppConfig"));
                        services.AddOpenBankingConnector(hostContext.Configuration);
                        services.AddScoped<ICreateSoftwareStatementProfile, CreateSoftwareStatementProfile>();
                        services.AddScoped<ICreateBankClientProfile, CreateBankClientProfile>();
                        services.AddScoped<ICreatePaymentInitiationApiProfile, CreatePaymentInitiationApiProfile>();
                    })
                .UseConsoleLifetime()
                .ConfigureLogging(
                    (hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                    });

            _host = builder.Build();
            _host.Start();
        }

        protected abstract void ProcessRecordInner(IServiceProvider services);

        protected override void ProcessRecord()
        {
            using IServiceScope serviceScope = _host.Services.CreateScope();
            IServiceProvider services = serviceScope.ServiceProvider;
            ProcessRecordInner(services);
        }
    }
}
