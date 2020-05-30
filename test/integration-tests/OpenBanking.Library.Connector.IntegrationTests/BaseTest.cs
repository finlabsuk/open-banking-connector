// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests
{
    public abstract class BaseTest : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<SqliteDbContext> _dbContextOptions;

        protected BaseTest()
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

        public IOpenBankingRequestBuilder CreateOpenBankingRequestBuilder() => CreateMockRequestBuilder();

        private RequestBuilder CreateMockRequestBuilder()
        {
            var _dB = new SqliteDbContext(_dbContextOptions);
            var _secretProvider = new MemoryKeySecretProvider();
            var _entityMapper = new EntityMapper();
            var requestBuilder = new RequestBuilder(
                entityMapper: _entityMapper,
                dbContextService: new DbMultiEntityMethods(_dB),
                configurationProvider: new DefaultConfigurationProvider(),
                logger: new ConsoleInstrumentationClient(),
                keySecretReadOnlyProvider: _secretProvider,
                apiClient: GetApiClient(TestConfig),
                certificateReader: new PemParsingCertificateReader(),
                clientProfileRepository: new DbEntityRepository<BankClientProfile>(_dB),
                softwareStatementProfileRepo: new DbEntityRepository<SoftwareStatementProfile>(_dB),
                domesticConsentRepo: new DbEntityRepository<DomesticConsent>(_dB),
                apiProfileRepository: new DbEntityRepository<ApiProfile>(_dB),
                activeSReadOnlyRepo: new KeySecretReadRepository<ActiveSoftwareStatementProfiles>(_secretProvider),
                activeSrRepo: new KeySecretWriteRepository<ActiveSoftwareStatementProfiles>(_secretProvider),
                sReadOnlyRepo: new KeySecretMultiItemReadRepository<Models.Public.Request.SoftwareStatementProfile>(
                    _secretProvider),
                sRepo: new KeySecretMultiItemWriteRepository<Models.Public.Request.SoftwareStatementProfile>(
                    _secretProvider),
                softwareStatementProfileService: new SoftwareStatementProfileService(
                    softwareStatementProfileRepo:
                    new KeySecretMultiItemReadRepository<Models.Public.Request.SoftwareStatementProfile>(
                        _secretProvider),
                    activeSoftwareStatementProfilesRepo: new KeySecretReadRepository<ActiveSoftwareStatementProfiles>(
                        _secretProvider),
                    mapper: _entityMapper));

            return requestBuilder;
        }

        private IApiClient GetApiClient(ITestConfigurationProvider testConfig)
        {
            if (TestConfig.GetBooleanValue("mockHttp").GetValueOrDefault(false))
            {
                var mockHttp = new MockHttpMessageHandler();

                var openIdConfigData = TestConfig.GetOpenBankingOpenIdConfiguration();
                var openIdConfig = JsonConvert.SerializeObject(openIdConfigData);


                mockHttp.When(method: HttpMethod.Get, url: "https://issuer.com/.well-known/openid-configuration")
                    .Respond(mediaType: "application/json", content: openIdConfig);

                mockHttp.When(method: HttpMethod.Get, url: "").Respond(mediaType: "application/json", content: "{}");
                mockHttp.When(method: HttpMethod.Post, url: "").Respond(mediaType: "application/json", content: "{}");

                var client = mockHttp.ToHttpClient();

                return new ApiClient(instrumentation: Substitute.For<IInstrumentationClient>(), httpClient: client);
            }

            var certificate = CertificateFactories.GetCertificate2FromPem(
                privateKey: TestConfig.GetValue("transportcertificatekey"),
                pem: TestConfig.GetValue("transportCertificate"));

            var handler = new HttpRequestBuilder()
                .SetClientCertificate(certificate)
                .CreateMessageHandler();

            return ApiClientFactory.CreateApiClient(handler);
        }
    }
}
