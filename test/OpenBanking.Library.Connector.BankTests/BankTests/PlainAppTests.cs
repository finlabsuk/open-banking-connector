// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
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
                   nameof(BankProfile.ClientRegistrationApiSettings.UseRegistrationScope) + "in bank profile")]
        [MemberData(
            nameof(TestedUnskippedBanksById),
            false)]
        public async Task TestAllNoConsentAuth(
            BankProfileEnum bank,
            SoftwareStatementProfileData softwareStatementProfile,
            RegistrationScopeEnum registrationScope)
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
                bank,
                softwareStatementProfile,
                registrationScope,
                () => new RequestBuilderContainer(
                    timeProvider,
                    apiVariantMapper,
                    instrumentationClient,
                    apiClient,
                    processedSoftwareStatementProfileStore,
                    GetDbContext()),
                false);
        }

        public IApiClient GetApiClient()
        {
            if (TestConfig.GetBooleanValue("mockHttp").GetValueOrDefault(false))
            {
                var mockHttp = new MockHttpMessageHandler();

                OpenIdConfiguration openIdConfigData = TestConfig.GetOpenBankingOpenIdConfiguration()!;
                string openIdConfig = JsonConvert.SerializeObject(openIdConfigData);


                mockHttp.When(HttpMethod.Get, "https://issuer.com/.well-known/openid-configuration")
                    .Respond("application/json", openIdConfig);

                mockHttp.When(HttpMethod.Get, "").Respond("application/json", "{}");
                mockHttp.When(HttpMethod.Post, "").Respond("application/json", "{}");

                var client = mockHttp.ToHttpClient();

                return new ApiClient(Substitute.For<IInstrumentationClient>(), client);
            }

            X509Certificate2 certificate = CertificateFactories.GetCertificate2FromPem(
                TestConfig.GetValue("transportcertificatekey")!,
                TestConfig.GetValue("transportCertificate")!)!;

            HttpMessageHandler handler = new HttpRequestBuilder()
                .SetClientCertificate(certificate)
                .CreateMessageHandler();

            return ApiClientFactory.CreateApiClient(handler);
        }
    }
}
