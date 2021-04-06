// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using RichardSzalay.MockHttp;
using Xunit;
using Xunit.Abstractions;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;

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

        public IRequestBuilder CreateOpenBankingRequestBuilder(
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfilesRepository,
            ITimeProvider timeProvider,
            IInstrumentationClient instrumentationClient) =>
            CreateMockRequestBuilder(softwareStatementProfilesRepository, timeProvider, instrumentationClient);

        private RequestBuilder CreateMockRequestBuilder(
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfilesRepository,
            ITimeProvider timeProvider,
            IInstrumentationClient instrumentationClient)
        {
            SqliteDbContext dB = new SqliteDbContext(_dbContextOptions);
            ApiVariantMapper apiVariantMapper = new ApiVariantMapper();
            RequestBuilder requestBuilder = new RequestBuilder(
                timeProvider,
                apiVariantMapper,
                instrumentationClient,
                new ApiClient(instrumentationClient, new HttpClient()),
                softwareStatementProfilesRepository,
                new DbService(dB));

            return requestBuilder;
        }

        public IApiClient GetApiClient()
        {
            if (TestConfig.GetBooleanValue("mockHttp").GetValueOrDefault(false))
            {
                MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();

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

        public async Task TestAll(
            string bankEnumString,
            RegistrationScope registrationScope)
        {
            // Create test data processor
            TestDataProcessor testDataProcessor = new TestDataProcessor(
                _dataFolder,
                $"{RegistrationScopeApiSetHelper.AbbreviatedName(registrationScope)}_{bankEnumString}");

            // Dereference bank
            BankProfileEnum bankProfileEnum = BankProfileEnumHelper.GetBankEnum(bankEnumString);
            BankProfile bankProfile = BankProfileEnumHelper.GetBank(bankProfileEnum);

            // Set up logging
            var timeProvider = new TimeProvider();
            var instrumentationClient = new TestInstrumentationClient(_outputHelper, timeProvider);

            // Get OBC settings and software statement profiles   
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets(typeof(PlainAppTests).GetTypeInfo().Assembly)
                .AddEnvironmentVariables()
                .Build();
            OpenBankingConnectorSettings obcSettings = configuration
                .GetSection(new OpenBankingConnectorSettings().SettingsSectionName)
                .Get<OpenBankingConnectorSettings>();
            SoftwareStatementProfilesSettings softwareStatementProfilesSettings = configuration
                .GetSection(new SoftwareStatementProfilesSettings().SettingsSectionName)
                .Get<SoftwareStatementProfilesSettings>();
            var obcSettingsProvider =
                new DefaultSettingsProvider<OpenBankingConnectorSettings>(obcSettings);
            var softwareStatementProfilesSettingsProvider =
                new DefaultSettingsProvider<SoftwareStatementProfilesSettings>(softwareStatementProfilesSettings);

            // Set up request builder
            var softwareStatementProfilesRepository = new SoftwareStatementProfileCache(
                obcSettingsProvider,
                softwareStatementProfilesSettingsProvider,
                instrumentationClient);
            IRequestBuilder requestBuilder = CreateOpenBankingRequestBuilder(
                softwareStatementProfilesRepository,
                timeProvider,
                instrumentationClient);

            // softwareStatementProfileResp.Should().NotBeNull();
            // softwareStatementProfileResp.Messages.Should().BeEmpty();
            // softwareStatementProfileResp.Data.Should().NotBeNull();
            // softwareStatementProfileResp.Data.Id.Should().NotBeNullOrWhiteSpace();

            // Where you see TestConfig.GetValue( .. )
            // these are injecting test data values. Here they're from test data, but can be anything else: database queries, Azure Key Vault configuration, etc.
        }
    }
}
