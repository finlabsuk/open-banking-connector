// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using NSubstitute;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached.SoftwareStatementProfile;


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
                instrumentation: Substitute.For<IInstrumentationClient>(),
                keySecretReadOnlyProvider: Substitute.For<IKeySecretReadOnlyProvider>(),
                dbEntityRepositoryFactory: Substitute.For<IDbEntityRepositoryFactory>(),
                softwareStatementProfileCachedRepo: Substitute
                    .For<IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached>>(),
                entityMapper: Substitute.For<IEntityMapper>(),
                dbContextService: Substitute.For<IDbMultiEntityMethods>());
        }


        public static RequestBuilder CreateMockRequestBuilder()
        {
            return new RequestBuilder(
                timeProvider: Substitute.For<ITimeProvider>(),
                entityMapper: new EntityMapper(),
                dbContextService: Substitute.For<IDbMultiEntityMethods>(),
                logger: Substitute.For<IInstrumentationClient>(),
                keySecretReadOnlyProvider: Substitute.For<IKeySecretReadOnlyProvider>(),
                apiClient: Substitute.For<IApiClient>(),
                certificateReader: Substitute.For<ICertificateReader>(),
                softwareStatementProfileCachedRepo: Substitute
                    .For<IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached>>(),
                dbEntityRepositoryFactory: Substitute.For<IDbEntityRepositoryFactory>());
        }
    }
}
