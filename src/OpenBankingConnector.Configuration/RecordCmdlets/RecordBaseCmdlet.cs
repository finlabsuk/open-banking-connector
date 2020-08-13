// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

//using System.Diagnostics;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    public abstract class RecordBaseCmdlet : BaseCmdlet
    {
        protected readonly IHost _host;

        public RecordBaseCmdlet(
            string verbName,
            string nounName,
            bool deleteAndRecreateDb) : base(verbName: verbName, nounName: nounName)
        {
            //Debugger.Launch();

            IHostBuilder builder = Helpers.CreateHostBuilder(null);

            string fileName = "OpenBankingConnector.Configuration";
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
            
            // Use console lifetime
            builder.UseConsoleLifetime();

            _host = builder
                .Build();
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
