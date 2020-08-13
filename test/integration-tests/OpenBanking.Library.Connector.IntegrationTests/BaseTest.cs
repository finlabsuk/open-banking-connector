// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
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
using SoftwareStatementProfile =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.SoftwareStatementProfile;

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
            using SqliteDbContext? context = new SqliteDbContext(_dbContextOptions);
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
            SqliteDbContext? _dB = new SqliteDbContext(_dbContextOptions);
            MemoryKeySecretProvider? _secretProvider = new MemoryKeySecretProvider();
            EntityMapper? _entityMapper = new EntityMapper();
            RequestBuilder? requestBuilder = new RequestBuilder(
                entityMapper: _entityMapper,
                dbContextService: new DbMultiEntityMethods(_dB),
                configurationProvider: new DefaultConfigurationProvider(),
                logger: new ConsoleInstrumentationClient(),
                keySecretReadOnlyProvider: _secretProvider,
                apiClient: GetApiClient(TestConfig),
                certificateReader: new PemParsingCertificateReader(),
                clientProfileRepository: new DbEntityRepository<BankClientProfile>(_dB),
                domesticConsentRepo: new DbEntityRepository<DomesticConsent>(_dB),
                apiProfileRepository: new DbEntityRepository<ApiProfile>(_dB),
                activeSReadOnlyRepo: new KeySecretReadRepository<ActiveSoftwareStatementProfiles>(_secretProvider),
                activeSrRepo: new KeySecretWriteRepository<ActiveSoftwareStatementProfiles>(_secretProvider),
                sReadOnlyRepo: new KeySecretMultiItemReadRepository<SoftwareStatementProfile>(_secretProvider),
                sRepo: new KeySecretMultiItemWriteRepository<SoftwareStatementProfile>(_secretProvider),
                softwareStatementProfileService: new SoftwareStatementProfileService(
                    softwareStatementProfileRepo:
                    new KeySecretMultiItemReadRepository<SoftwareStatementProfile>(_secretProvider),
                    activeSoftwareStatementProfilesRepo: new KeySecretReadRepository<ActiveSoftwareStatementProfiles>(
                        _secretProvider),
                    mapper: _entityMapper));

            return requestBuilder;
        }

        private IApiClient GetApiClient(ITestConfigurationProvider testConfig)
        {
            if (TestConfig.GetBooleanValue("mockHttp").GetValueOrDefault(false))
            {
                MockHttpMessageHandler? mockHttp = new MockHttpMessageHandler();

                OpenIdConfiguration? openIdConfigData = TestConfig.GetOpenBankingOpenIdConfiguration();
                string? openIdConfig = JsonConvert.SerializeObject(openIdConfigData);


                mockHttp.When(method: HttpMethod.Get, url: "https://issuer.com/.well-known/openid-configuration")
                    .Respond(mediaType: "application/json", content: openIdConfig);

                mockHttp.When(method: HttpMethod.Get, url: "").Respond(mediaType: "application/json", content: "{}");
                mockHttp.When(method: HttpMethod.Post, url: "").Respond(mediaType: "application/json", content: "{}");

                HttpClient? client = mockHttp.ToHttpClient();

                return new ApiClient(instrumentation: Substitute.For<IInstrumentationClient>(), httpClient: client);
            }

            X509Certificate2? certificate = CertificateFactories.GetCertificate2FromPem(
                privateKey: TestConfig.GetValue("transportcertificatekey"),
                pem: TestConfig.GetValue("transportCertificate"));

            HttpMessageHandler? handler = new HttpRequestBuilder()
                .SetClientCertificate(certificate)
                .CreateMessageHandler();

            return ApiClientFactory.CreateApiClient(handler);
        }
    }
}
