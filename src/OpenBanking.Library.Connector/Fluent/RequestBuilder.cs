// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public class RequestBuilder : IRequestBuilder
    {
        private readonly IApiClient _apiClient;
        private readonly ICertificateReader _certificateReader;
        private readonly IDbMultiEntityMethods _dbContextService;
        private readonly IDbEntityRepositoryFactory _dbEntityRepositoryFactory;
        private readonly IEntityMapper _entityMapper;
        private readonly IKeySecretReadOnlyProvider _keySecretReadOnlyProvider;
        private readonly IInstrumentationClient _logger;

        private readonly IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached>
            _softwareStatementProfileCachedRepo;

        private readonly ITimeProvider _timeProvider;


        public RequestBuilder(
            IEntityMapper entityMapper,
            IDbMultiEntityMethods dbContextService,
            IInstrumentationClient logger,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider,
            IApiClient apiClient,
            ICertificateReader certificateReader,
            IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> softwareStatementProfileCachedRepo,
            IDbEntityRepositoryFactory dbEntityRepositoryFactory)
            : this(
                timeProvider: new TimeProvider(),
                entityMapper: entityMapper,
                dbContextService: dbContextService,
                logger: logger,
                keySecretReadOnlyProvider: keySecretReadOnlyProvider,
                apiClient: apiClient,
                certificateReader: certificateReader,
                softwareStatementProfileCachedRepo: softwareStatementProfileCachedRepo,
                dbEntityRepositoryFactory: dbEntityRepositoryFactory) { }

        internal RequestBuilder(
            ITimeProvider timeProvider,
            IEntityMapper entityMapper,
            IDbMultiEntityMethods dbContextService,
            IInstrumentationClient logger,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider,
            IApiClient apiClient,
            ICertificateReader certificateReader,
            IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> softwareStatementProfileCachedRepo,
            IDbEntityRepositoryFactory dbEntityRepositoryFactory)
        {
            _certificateReader = certificateReader.ArgNotNull(nameof(certificateReader));
            _timeProvider = timeProvider.ArgNotNull(nameof(timeProvider));
            _entityMapper = entityMapper.ArgNotNull(nameof(entityMapper));
            _dbContextService = dbContextService;
            _softwareStatementProfileCachedRepo = softwareStatementProfileCachedRepo;
            _dbEntityRepositoryFactory = dbEntityRepositoryFactory;
            _logger = logger.ArgNotNull(nameof(logger));
            _keySecretReadOnlyProvider = keySecretReadOnlyProvider.ArgNotNull(nameof(keySecretReadOnlyProvider));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
        }


        public IClientRegistration ClientRegistration => new ClientRegistration(CreateContext());
        public IPaymentInitiation PaymentInitiation => new PaymentInitiation(CreateContext());

        public IAccountAndTransaction AccountAndTransaction =>
            new AccountAndTransaction(CreateContext());

        private ISharedContext CreateContext()
        {
            SharedContext context = new SharedContext(
                timeProvider: _timeProvider,
                certificateReader: _certificateReader,
                apiClient: _apiClient,
                instrumentation: _logger,
                keySecretReadOnlyProvider: _keySecretReadOnlyProvider,
                dbEntityRepositoryFactory: _dbEntityRepositoryFactory,
                softwareStatementProfileCachedRepo: _softwareStatementProfileCachedRepo,
                entityMapper: _entityMapper,
                dbContextService: _dbContextService)
            {
                Created = _timeProvider.GetUtcNow()
            };
            return context;
        }
    }
}
