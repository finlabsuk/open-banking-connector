﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.HostedServices
{
    public class StartupTasksHostedService : IHostedService
    {
        private readonly ISettingsProvider<OpenBankingConnectorSettings> _obcSettingsProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public StartupTasksHostedService(
            ISettingsProvider<OpenBankingConnectorSettings> obcSettingsProvider,
            IServiceScopeFactory serviceScopeFactory)
        {
            _obcSettingsProvider = obcSettingsProvider ??
                                   throw new ArgumentNullException(nameof(obcSettingsProvider));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Load Open Banking Connector configuration options
            OpenBankingConnectorSettings obcSettings = _obcSettingsProvider.GetSettings();

            // Ensure DB exists
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            using BaseDbContext context = scope.ServiceProvider.GetRequiredService<BaseDbContext>();
            if (obcSettings.Database.ProcessedEnsureDbCreated)
            {
                // Create DB if configured to do so and DB doesn't exist
                context.Database.EnsureCreated();
            }
            else
            {
                // Throw exception if DB doesn't exist
                IRelationalDatabaseCreator creator = context.Database.GetService<IRelationalDatabaseCreator>();
                if (!creator.Exists())
                {
                    throw new ApplicationException(
                        "No database found. Run 'dotnet ef database update' in OpenBanking.WebApp.Connector.Sample root folder to create test DB.");
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
