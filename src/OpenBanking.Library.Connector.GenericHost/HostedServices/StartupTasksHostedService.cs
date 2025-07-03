// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
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
    private readonly EncryptionSettings _encryptionSettings;

    private readonly HttpClientSettings _httpClientSettings;

    private readonly IInstrumentationClient _instrumentationClient;

    private readonly KeysSettings _keySettings;

    private readonly ILogger<StartupTasksHostedService> _logger;

    private readonly IMemoryCache _memoryCache;

    private readonly ISecretProvider _secretProvider;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly ITimeProvider _timeProvider;

    private readonly TppReportingMetrics _tppReportingMetrics;

    public StartupTasksHostedService(
        IBankProfileService bankProfileService,
        IConfiguration configuration,
        ISettingsProvider<DatabaseSettings> databaseSettingsProvider,
        EncryptionSettings encryptionSettings,
        ISettingsProvider<HttpClientSettings> httpClientSettingsProvider,
        IInstrumentationClient instrumentationClient,
        ILogger<StartupTasksHostedService> logger,
        IMemoryCache memoryCache,
        ISecretProvider secretProvider,
        IServiceScopeFactory serviceScopeFactory,
        TppReportingMetrics tppReportingMetrics,
        ISettingsProvider<KeysSettings> keySettingsProvider,
        ITimeProvider timeProvider)
    {
        _bankProfileService = bankProfileService ?? throw new ArgumentNullException(nameof(bankProfileService));
        _configurationRoot =
            (IConfigurationRoot) (configuration ?? throw new ArgumentNullException(nameof(configuration)));
        _databaseSettingsProvider = databaseSettingsProvider ??
                                    throw new ArgumentNullException(nameof(databaseSettingsProvider));
        _encryptionSettings = encryptionSettings ?? throw new ArgumentNullException(nameof(encryptionSettings));
        _httpClientSettings =
            (httpClientSettingsProvider ?? throw new ArgumentNullException(nameof(httpClientSettingsProvider)))
            .GetSettings();
        _instrumentationClient =
            instrumentationClient ?? throw new ArgumentNullException(nameof(instrumentationClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _secretProvider = secretProvider ?? throw new ArgumentNullException(nameof(secretProvider));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _tppReportingMetrics = tppReportingMetrics ?? throw new ArgumentNullException(nameof(tppReportingMetrics));
        _timeProvider = timeProvider;
        _keySettings = keySettingsProvider.GetSettings();
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
                case DbProvider.MongoDb:
                    var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbDbContext>();

                    if (!await mongoDbContext.Database.CanConnectAsync())
                    {
                        throw new ApplicationException();
                    }

                    if (true)
                    {
                        if (databaseSettings.EnsureDatabaseCreated)
                        {
                            // Create database
                            await mongoDbContext.Database.EnsureCreatedAsync();
                        }
                        else
                        {
                            throw new ApplicationException(
                                "No database found. Note: set \"Database:EnsureDatabaseCreated\" to \"true\" to create database at application start-up.");
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

            await new EncryptionKeyDescriptionCleanup()
                .Cleanup(
                    postgreSqlDbContext,
                    _secretProvider,
                    _keySettings,
                    _encryptionSettings,
                    _memoryCache,
                    _instrumentationClient,
                    _timeProvider,
                    cancellationToken);

            await new SoftwareStatementCleanup()
                .Cleanup(
                    postgreSqlDbContext,
                    _secretProvider,
                    _httpClientSettings,
                    _memoryCache,
                    _instrumentationClient,
                    _tppReportingMetrics);

            await new BankRegistrationCleanup()
                .Cleanup(
                    postgreSqlDbContext,
                    _logger);

            await new AccountAccessConsentCleanup()
                .Cleanup(
                    postgreSqlDbContext,
                    _logger);

            //postgreSqlDbContext.ChangeTracker.DetectChanges();

            await postgreSqlDbContext.SaveChangesAsync(cancellationToken);

            await new EncryptedObjectCleanup()
                .Cleanup(
                    postgreSqlDbContext,
                    _keySettings,
                    _instrumentationClient,
                    _timeProvider,
                    cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
