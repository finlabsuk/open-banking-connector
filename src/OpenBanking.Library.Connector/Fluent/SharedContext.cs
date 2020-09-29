// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using SoftwareStatementProfileRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    internal class SharedContext : ISharedContext
    {
        public SharedContext(
            ITimeProvider timeProvider,
            ICertificateReader certificateReader,
            IApiClient apiClient,
            IObcConfigurationProvider configurationProvider,
            IInstrumentationClient instrumentation,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider,
            IDbEntityRepositoryFactory dbEntityRepositoryFactory,
            IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> activeSrRepo,
            IKeySecretMultiItemReadRepository<SoftwareStatementProfileRequest> sReadOnlyRepo,
            IKeySecretMultiItemWriteRepository<SoftwareStatementProfileRequest> sRepo,
            IKeySecretReadRepository<ActiveSoftwareStatementProfiles> activeSReadOnlyRepo,
            ISoftwareStatementProfileService softwareStatementProfileService,
            IEntityMapper entityMapper,
            IDbMultiEntityMethods dbContextService)
        {
            TimeProvider = timeProvider;
            CertificateReader = certificateReader;
            ApiClient = apiClient;
            ConfigurationProvider = configurationProvider;
            Instrumentation = instrumentation;
            KeySecretReadOnlyProvider = keySecretReadOnlyProvider;
            DbEntityRepositoryFactory = dbEntityRepositoryFactory;
            ActiveSRRepo = activeSrRepo;
            SReadOnlyRepo = sReadOnlyRepo;
            SRepo = sRepo;
            ActiveSReadOnlyRepo = activeSReadOnlyRepo;
            SoftwareStatementProfileService = softwareStatementProfileService;
            EntityMapper = entityMapper;
            DbContextService = dbContextService;
        }

        public ITimeProvider TimeProvider { get; }
        public DateTimeOffset Created { get; set; }
        public ICertificateReader CertificateReader { get; }
        public IApiClient ApiClient { get; }
        public IObcConfigurationProvider ConfigurationProvider { get; }
        public IInstrumentationClient Instrumentation { get; }
        public IKeySecretReadOnlyProvider KeySecretReadOnlyProvider { get; }
        public IDbEntityRepositoryFactory DbEntityRepositoryFactory { get; }
        public IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> ActiveSRRepo { get; }
        public IKeySecretMultiItemReadRepository<SoftwareStatementProfileRequest> SReadOnlyRepo { get; }
        public IKeySecretMultiItemWriteRepository<SoftwareStatementProfileRequest> SRepo { get; }
        public IKeySecretReadRepository<ActiveSoftwareStatementProfiles> ActiveSReadOnlyRepo { get; }
        public ISoftwareStatementProfileService SoftwareStatementProfileService { get; }
        public IEntityMapper EntityMapper { get; }
        public IDbMultiEntityMethods DbContextService { get; }
    }
}
