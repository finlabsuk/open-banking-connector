// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NSubstitute;
using RichardSzalay.MockHttp;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    [Collection("App context collection")]
    public partial class PlainAppTests : AppTests, IDisposable
    {
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
            var softwareStatementAndCertificateProfileOverridesSettings =
                AppConfiguration.GetSettings<SoftwareStatementAndCertificateProfileOverridesSettings>();
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
            var obcSettingsProvider2 =
                new DefaultSettingsProvider<SoftwareStatementAndCertificateProfileOverridesSettings>(
                    softwareStatementAndCertificateProfileOverridesSettings);
            var softwareStatementProfilesSettingsProvider =
                new DefaultSettingsProvider<SoftwareStatementProfilesSettings>(softwareStatementProfilesSettings);
            var obTransportCertificateProfilesSettingsProvider =
                new DefaultSettingsProvider<TransportCertificateProfilesSettings>(
                    obTransportCertificateProfilesSettings);
            var obSigningCertificateProfilesSettingsProvider =
                new DefaultSettingsProvider<SigningCertificateProfilesSettings>(obSigningCertificateProfilesSettings);
            var bankProfilesSettingsProvider =
                new DefaultSettingsProvider<BankProfilesSettings>(bankProfilesSettings);

            // Create stores
            var timeProvider = new TimeProvider();
            var instrumentationClient = new TestInstrumentationClient(_outputHelper, timeProvider);
            var processedSoftwareStatementProfileStore = new ProcessedSoftwareStatementProfileStore(
                obcSettingsProvider2,
                softwareStatementProfilesSettingsProvider,
                obTransportCertificateProfilesSettingsProvider,
                obSigningCertificateProfilesSettingsProvider,
                instrumentationClient);

            // Run test            
            var apiVariantMapper = new ApiVariantMapper();
            var apiClient = new ApiClient(instrumentationClient, new HttpClient());
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
                    new BankProfileDefinitions(bankProfilesSettingsProvider)),
                false);
        }
    }
}
