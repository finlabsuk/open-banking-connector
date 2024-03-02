﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.HostedServices;

public class StartupTasksHostedService : IHostedService
{
    // Ensures this set up at application start-up
    private readonly IBankProfileService _bankProfileService;

    private readonly IConfigurationRoot _configurationRoot;

    private readonly ISettingsProvider<DatabaseSettings> _databaseSettingsProvider;

    // Ensures this set up at application start-up
    private readonly IEncryptionKeyInfo _encryptionKeyInfo;

    private readonly HttpClientSettings _httpClientSettings;

    private readonly IInstrumentationClient _instrumentationClient;

    private readonly ILogger<StartupTasksHostedService> _logger;

    private readonly IMemoryCache _memoryCache;
    private readonly TppReportingMetrics _tppReportingMetrics;

    // Ensures this set up at application start-up
    private readonly IProcessedSoftwareStatementProfileStore _processedSoftwareStatementProfileStore;

    private readonly ISecretProvider _secretProvider;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public StartupTasksHostedService(
        ISettingsProvider<DatabaseSettings> databaseSettingsProvider,
        IServiceScopeFactory serviceScopeFactory,
        IProcessedSoftwareStatementProfileStore processedSoftwareStatementProfileStore,
        IBankProfileService bankProfileService,
        ILogger<StartupTasksHostedService> logger,
        IEncryptionKeyInfo encryptionKeyInfo,
        IConfiguration configuration,
        ISettingsProvider<HttpClientSettings> httpClientSettingsProvider,
        IInstrumentationClient instrumentationClient,
        ISecretProvider secretProvider,
        IMemoryCache memoryCache, TppReportingMetrics tppReportingMetrics)
    {
        _databaseSettingsProvider =
            databaseSettingsProvider ??
            throw new ArgumentNullException(nameof(databaseSettingsProvider));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _processedSoftwareStatementProfileStore = processedSoftwareStatementProfileStore;
        _bankProfileService = bankProfileService;
        _logger = logger;
        _encryptionKeyInfo = encryptionKeyInfo;
        _httpClientSettings = httpClientSettingsProvider.GetSettings();
        _instrumentationClient = instrumentationClient;
        _secretProvider = secretProvider;
        _memoryCache = memoryCache;
        _tppReportingMetrics = tppReportingMetrics;
        _configurationRoot = (IConfigurationRoot) configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string x = _configurationRoot.GetDebugView();

        // Load database settings
        DatabaseSettings databaseSettings = _databaseSettingsProvider.GetSettings();

        {
            // Get scope
            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            // Database startup tasks
            switch (databaseSettings.Provider)
            {
                case DbProvider.Sqlite:
                    var sqliteDbContext = scope.ServiceProvider.GetRequiredService<SqliteDbContext>();
                    bool sqliteDbExists =
                        sqliteDbContext.Database.GetService<IRelationalDatabaseCreator>().Exists();
                    if (!sqliteDbExists)
                    {
                        if (databaseSettings.EnsureDatabaseCreated)
                        {
                            // Create database
                            sqliteDbContext.Database.EnsureCreated();
                        }
                        else
                        {
                            throw new ApplicationException(
                                "No database found. Note: set \"Database:EnsureDatabaseCreated\" to \"true\" to create database at application start-up.");
                        }
                    }

                    break;
                case DbProvider.PostgreSql:
                    var postgreSqlDbContext = scope.ServiceProvider.GetRequiredService<PostgreSqlDbContext>();
                    bool postgreSqlExists =
                        postgreSqlDbContext.Database.GetService<IRelationalDatabaseCreator>().Exists();
                    if (!postgreSqlExists)
                    {
                        if (databaseSettings.EnsureDatabaseCreated)
                        {
                            // Create database
                            postgreSqlDbContext.Database.Migrate();
                        }
                        else
                        {
                            throw new ApplicationException(
                                "No database found. Note: set \"Database:EnsureDatabaseCreated\" to \"true\" to create database at application start-up.");
                        }
                    }
                    else if (postgreSqlDbContext.Database.GetPendingMigrations().Any())
                    {
                        if (databaseSettings.EnsureDatabaseMigrated)
                        {
                            // Apply pending migrations
                            postgreSqlDbContext.Database.Migrate();
                        }
                        else
                        {
                            throw new ApplicationException(
                                "Pending migrations have not been applied. Note: set \"Database:EnsureDatabaseMigrated\" to \"true\" to apply pending migrations at application start-up.");
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Database checks and cleanup
        if (databaseSettings.Provider is DbProvider.PostgreSql)
        {
            // Get scope
            using IServiceScope scope2 = _serviceScopeFactory.CreateScope();

            var postgreSqlDbContext = scope2.ServiceProvider.GetRequiredService<PostgreSqlDbContext>();

            await new BankRegistrationCleanup()
                .Cleanup(
                    postgreSqlDbContext,
                    _processedSoftwareStatementProfileStore,
                    _logger);

            await new AccountAccessConsentCleanup()
                .Cleanup(
                    postgreSqlDbContext,
                    _processedSoftwareStatementProfileStore,
                    _logger);

            //postgreSqlDbContext.ChangeTracker.DetectChanges();

            await postgreSqlDbContext.SaveChangesAsync(cancellationToken);

            await new SoftwareStatementCleanup()
                .Cleanup(
                    postgreSqlDbContext,
                    _processedSoftwareStatementProfileStore,
                    _secretProvider,
                    _httpClientSettings,
                    _memoryCache,
                    _instrumentationClient, _tppReportingMetrics);

            await postgreSqlDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
