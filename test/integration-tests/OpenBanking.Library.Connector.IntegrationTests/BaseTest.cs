// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests
{
    public abstract class BaseTest: IDisposable
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

        public IOpenBankingRequestBuilder CreateOpenBankingRequestBuilder() => CreateMockRequestBuilder();

        private RequestBuilder CreateMockRequestBuilder()
        {
            var _dB = new SqliteDbContext(_dbContextOptions);
            var requestBuilder = new RequestBuilder(
                new EntityMapper(),
                new DbMultiEntityMethods(_dB),
                new DefaultConfigurationProvider(),
                new ConsoleInstrumentationClient(),
                new MemoryKeySecretProvider(),
                GetApiClient(TestConfig),
                new PemParsingCertificateReader(),
                new DbEntityRepository<BankClientProfile>(_dB),
                new DbEntityRepository<SoftwareStatementProfile>(_dB),
                new DbEntityRepository<DomesticConsent>(_dB),
                new DbEntityRepository<ApiProfile>(_dB));

            return requestBuilder;
        }

        private IApiClient GetApiClient(ITestConfigurationProvider testConfig)
        {
            if (TestConfig.GetBooleanValue("mockHttp").GetValueOrDefault(false))
            {
                var mockHttp = new MockHttpMessageHandler();

                var openIdConfigData = TestConfig.GetOpenBankingOpenIdConfiguration();
                var openIdConfig = JsonConvert.SerializeObject(openIdConfigData);


                mockHttp.When(HttpMethod.Get, "https://issuer.com/.well-known/openid-configuration")
                    .Respond("application/json", openIdConfig);

                mockHttp.When(HttpMethod.Get, "").Respond("application/json", "{}");
                mockHttp.When(HttpMethod.Post, "").Respond("application/json", "{}");

                var client = mockHttp.ToHttpClient();

                return new ApiClient(Substitute.For<IInstrumentationClient>(), client);
            }

            var certificate = CertificateFactories.GetCertificate2FromPem(
                TestConfig.GetValue("transportcertificatekey"),
                TestConfig.GetValue("transportCertificate"));

            var handler = new HttpRequestBuilder()
                .SetClientCertificate(certificate)
                .CreateMessageHandler();

            return ApiClientFactory.CreateApiClient(handler);
        }

        public void Dispose()
        {
            _connection.Close(); // Deletes DB
        }
    }
}
