// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using NSubstitute;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests
{
    internal static class TestDataFactory
    {
        public static SharedContext CreateMockOpenBankingContext()
        {
            return new SharedContext(
                timeProvider: Substitute.For<ITimeProvider>(),
                certificateReader: Substitute.For<ICertificateReader>(),
                apiClient: Substitute.For<IApiClient>(),
                configurationProvider: Substitute.For<IObcConfigurationProvider>(),
                instrumentation: Substitute.For<IInstrumentationClient>(),
                keySecretReadOnlyProvider: Substitute.For<IKeySecretReadOnlyProvider>(),
                softwareStatementProfileService: Substitute.For<ISoftwareStatementProfileService>(),
                entityMapper: Substitute.For<IEntityMapper>(),
                dbContextService: Substitute.For<IDbMultiEntityMethods>(),
                activeSrRepo: Substitute.For<IKeySecretWriteRepository<ActiveSoftwareStatementProfiles>>(),
                sReadOnlyRepo: Substitute
                    .For<IKeySecretMultiItemReadRepository<SoftwareStatementProfile>>(),
                sRepo: Substitute.For<IKeySecretMultiItemWriteRepository<SoftwareStatementProfile>>(),
                activeSReadOnlyRepo: Substitute.For<IKeySecretReadRepository<ActiveSoftwareStatementProfiles>>(),
                dbEntityRepositoryFactory: Substitute.For<IDbEntityRepositoryFactory>());
        }


        public static RequestBuilder CreateMockRequestBuilder()
        {
            return new RequestBuilder(
                timeProvider: Substitute.For<ITimeProvider>(),
                entityMapper: new EntityMapper(),
                dbContextService: Substitute.For<IDbMultiEntityMethods>(),
                configurationProvider: new DefaultConfigurationProvider(),
                logger: Substitute.For<IInstrumentationClient>(),
                keySecretReadOnlyProvider: Substitute.For<IKeySecretReadOnlyProvider>(),
                apiClient: Substitute.For<IApiClient>(),
                certificateReader: Substitute.For<ICertificateReader>(),
                activeSReadOnlyRepo: Substitute.For<IKeySecretReadRepository<ActiveSoftwareStatementProfiles>>(),
                activeSrRepo: Substitute.For<IKeySecretWriteRepository<ActiveSoftwareStatementProfiles>>(),
                sReadOnlyRepo: Substitute
                    .For<IKeySecretMultiItemReadRepository<SoftwareStatementProfile>>(),
                sRepo: Substitute.For<IKeySecretMultiItemWriteRepository<SoftwareStatementProfile>>(),
                softwareStatementProfileService: Substitute.For<ISoftwareStatementProfileService>(),
                dbEntityRepositoryFactory: Substitute.For<IDbEntityRepositoryFactory>());
        }
    }
}
