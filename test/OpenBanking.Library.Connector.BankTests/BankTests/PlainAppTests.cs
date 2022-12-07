// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    [Collection("App context collection")]
    public partial class PlainAppTests : AppTests, IDisposable
    {
        private readonly AppContextFixture _appContextFixture;
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<SqliteDbContext> _dbContextOptions;

        public PlainAppTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture) : base(
            outputHelper,
            appContextFixture)
        {
            TestConfig = new TestConfigurationProvider();
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open(); // Creates DB
            _dbContextOptions = new DbContextOptionsBuilder<SqliteDbContext>()
                .UseSqlite(_connection)
                .Options;
            using var context = new SqliteDbContext(_dbContextOptions);
            context.Database.EnsureCreated(); // Initialise DB with schema
            _appContextFixture = appContextFixture;
        }

        public ITestConfigurationProvider TestConfig { get; }

        public void Dispose()
        {
            _connection.Close(); // Deletes DB
        }

        private BaseDbContext GetDbContext() => new SqliteDbContext(_dbContextOptions);

        [Theory]
        [MemberData(
            nameof(TestedSkippedBanksById),
            false,
            Skip = "Bank skipped due to setting of" +
                   nameof(BankProfile.BankConfigurationApiSettings.RegistrationScopeIsValid) + "in bank profile")]
        [MemberData(
            nameof(TestedUnskippedBanksById),
            false)]
        public async Task TestAllNoConsentAuth(
            BankTestData1 testGroup, // name chosen to customise label in test runner
            BankTestData2 bankProfile) // name chosen to customise label in test runner
        {
            // Collect settings from configuration (to ensure common settings with Generic Host tests;
            // a "plain app" might get settings from environment variables or a custom system;
            // see comment in next section).
            var softwareStatementProfilesSettings =
                AppConfiguration.GetSettings<SoftwareStatementProfilesSettings>();
            var obTransportCertificateProfilesSettings =
                AppConfiguration.GetSettings<TransportCertificateProfilesSettings>();
            var obSigningCertificateProfilesSettings =
                AppConfiguration.GetSettings<SigningCertificateProfilesSettings>();
            var bankProfilesSettings =
                AppConfiguration.GetSettings<BankProfilesSettings>();

            // Create providers from settings
            // TODO: update to write settings to environment variables and then use EnvironmentVariablesSettingsProvider to get
            // settings as might be done in a "plain app".
            var softwareStatementProfilesSettingsProvider =
                new DefaultSettingsProvider<SoftwareStatementProfilesSettings>(softwareStatementProfilesSettings);
            var obTransportCertificateProfilesSettingsProvider =
                new DefaultSettingsProvider<TransportCertificateProfilesSettings>(
                    obTransportCertificateProfilesSettings);
            var obSigningCertificateProfilesSettingsProvider =
                new DefaultSettingsProvider<SigningCertificateProfilesSettings>(obSigningCertificateProfilesSettings);
            var bankProfilesSettingsProvider =
                new DefaultSettingsProvider<BankProfilesSettings>(bankProfilesSettings);

            // Set up time provider
            var timeProvider = new TimeProvider();

            // Set up logging
            // ILoggerFactory? factory = LoggerFactory.Create(
            //     b =>
            //         b.AddConsole().AddXUnit(_appContextFixture));
            // ILogger<object> logger = factory.CreateLogger<object>();
            var logger = _serviceProvider.GetRequiredService<ILogger<RequestBuilder>>();
            var instrumentationClient = new LoggerInstrumentationClient<RequestBuilder>(logger);

            // Connect output to logging
            SetTestLogging();

            // Set up software statement store
            var processedSoftwareStatementProfileStore = new ProcessedSoftwareStatementProfileStore(
                softwareStatementProfilesSettingsProvider,
                obTransportCertificateProfilesSettingsProvider,
                obSigningCertificateProfilesSettingsProvider,
                instrumentationClient);

            var apiVariantMapper = new ApiVariantMapper();
            var apiClient = new ApiClient(instrumentationClient, new HttpClient());

            // Run test            
            await TestAllInner(
                testGroup,
                bankProfile,
                () => new RequestBuilderContainer(
                    timeProvider,
                    apiVariantMapper,
                    instrumentationClient,
                    apiClient,
                    processedSoftwareStatementProfileStore,
                    GetDbContext(),
                    new BankProfileService(bankProfilesSettingsProvider)),
                false);

            UnsetTestLogging();
        }
    }
}
