// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    internal class SharedContext : ISharedContext
    {
        public SharedContext(
            ITimeProvider timeProvider,
            ICertificateReader certificateReader,
            IApiClient apiClient,
            IInstrumentationClient instrumentation,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider,
            IDbEntityRepositoryFactory dbEntityRepositoryFactory,
            IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> softwareStatementProfileCachedRepo,
            IEntityMapper entityMapper,
            IDbMultiEntityMethods dbContextService)
        {
            TimeProvider = timeProvider;
            CertificateReader = certificateReader;
            ApiClient = apiClient;
            Instrumentation = instrumentation;
            KeySecretReadOnlyProvider = keySecretReadOnlyProvider;
            DbEntityRepositoryFactory = dbEntityRepositoryFactory;
            SoftwareStatementProfileCachedRepo = softwareStatementProfileCachedRepo;
            EntityMapper = entityMapper;
            DbContextService = dbContextService;
        }

        public ITimeProvider TimeProvider { get; }
        public DateTimeOffset Created { get; set; }
        public ICertificateReader CertificateReader { get; }
        public IApiClient ApiClient { get; }
        public IInstrumentationClient Instrumentation { get; }
        public IKeySecretReadOnlyProvider KeySecretReadOnlyProvider { get; }
        public IDbEntityRepositoryFactory DbEntityRepositoryFactory { get; }

        public IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> SoftwareStatementProfileCachedRepo
        {
            get;
        }

        public IEntityMapper EntityMapper { get; }
        public IDbMultiEntityMethods DbContextService { get; }
    }
}
