// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using NSubstitute;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests
{
    internal static class TestDataFactory
    {
        public static SharedContext CreateMockOpenBankingContext()
        {
            return new SharedContext(
                certificateReader: Substitute.For<ICertificateReader>(),
                apiClient: Substitute.For<IApiClient>(),
                configurationProvider: Substitute.For<IConfigurationProvider>(),
                instrumentation: Substitute.For<IInstrumentationClient>(),
                keySecretProvider: Substitute.For<IKeySecretProvider>(),
                clientProfileRepository: Substitute.For<IDbEntityRepository<BankClientProfile>>(),
                softwareStatementRepository: Substitute.For<IDbEntityRepository<SoftwareStatementProfile>>(),
                domesticConsentRepository: Substitute.For<IDbEntityRepository<DomesticConsent>>(),
                entityMapper: Substitute.For<IEntityMapper>(),
                apiProfileRepository: Substitute.For<IDbEntityRepository<ApiProfile>>(),
                dbContextService: Substitute.For<IDbMultiEntityMethods>());
        }


        public static RequestBuilder CreateMockRequestBuilder()
        {
            return new RequestBuilder(
                timeProvider: Substitute.For<ITimeProvider>(),
                entityMapper: new EntityMapper(),
                dbContextService: Substitute.For<IDbMultiEntityMethods>(),
                configurationProvider: new DefaultConfigurationProvider(),
                logger: Substitute.For<IInstrumentationClient>(),
                keySecretProvider: Substitute.For<IKeySecretProvider>(),
                apiClient: Substitute.For<IApiClient>(),
                certificateReader: Substitute.For<ICertificateReader>(),
                clientProfileRepository: Substitute.For<IDbEntityRepository<BankClientProfile>>(),
                softwareStatementProfileRepo: Substitute.For<IDbEntityRepository<SoftwareStatementProfile>>(),
                domesticConsentRepo: Substitute.For<IDbEntityRepository<DomesticConsent>>(),
                apiProfileRepository: Substitute.For<IDbEntityRepository<ApiProfile>>());
        }
    }
}
