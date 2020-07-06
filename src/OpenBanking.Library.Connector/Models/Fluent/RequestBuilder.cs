// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using SoftwareStatementProfile =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class RequestBuilder : IOpenBankingRequestBuilder
    {
        private readonly IKeySecretReadRepository<ActiveSoftwareStatementProfiles> _activeSReadOnlyRepo;
        private readonly IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> _activeSRRepo;
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<ApiProfile> _apiProfileRepository;
        private readonly ICertificateReader _certificateReader;
        private readonly IDbEntityRepository<BankClientProfile> _clientProfileRepository;
        private readonly IObcConfigurationProvider _configurationProvider;
        private readonly IDbMultiEntityMethods _dbContextService;
        private readonly IDbEntityRepository<DomesticConsent> _domesticConsentRepo;
        private readonly IEntityMapper _entityMapper;
        private readonly IKeySecretReadOnlyProvider _keySecretReadOnlyProvider;
        private readonly IInstrumentationClient _logger;
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;
        private readonly IKeySecretMultiItemReadRepository<SoftwareStatementProfile> _sReadOnlyRepo;
        private readonly IKeySecretMultiItemWriteRepository<SoftwareStatementProfile> _sRepo;
        private readonly ITimeProvider _timeProvider;


        public RequestBuilder(
            IEntityMapper entityMapper,
            IDbMultiEntityMethods dbContextService,
            IObcConfigurationProvider configurationProvider,
            IInstrumentationClient logger,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider,
            IApiClient apiClient,
            ICertificateReader certificateReader,
            IDbEntityRepository<BankClientProfile> clientProfileRepository,
            IDbEntityRepository<DomesticConsent> domesticConsentRepo,
            IDbEntityRepository<ApiProfile> apiProfileRepository,
            IKeySecretReadRepository<ActiveSoftwareStatementProfiles> activeSReadOnlyRepo,
            IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> activeSrRepo,
            IKeySecretMultiItemReadRepository<SoftwareStatementProfile> sReadOnlyRepo,
            IKeySecretMultiItemWriteRepository<SoftwareStatementProfile> sRepo,
            ISoftwareStatementProfileService softwareStatementProfileService)
            : this(
                timeProvider: new TimeProvider(),
                entityMapper: entityMapper,
                dbContextService: dbContextService,
                configurationProvider: configurationProvider,
                logger: logger,
                keySecretReadOnlyProvider: keySecretReadOnlyProvider,
                apiClient: apiClient,
                certificateReader: certificateReader,
                clientProfileRepository: clientProfileRepository,
                domesticConsentRepo: domesticConsentRepo,
                apiProfileRepository: apiProfileRepository,
                activeSReadOnlyRepo: activeSReadOnlyRepo,
                activeSrRepo: activeSrRepo,
                sReadOnlyRepo: sReadOnlyRepo,
                sRepo: sRepo,
                softwareStatementProfileService: softwareStatementProfileService) { }

        internal RequestBuilder(
            ITimeProvider timeProvider,
            IEntityMapper entityMapper,
            IDbMultiEntityMethods dbContextService,
            IObcConfigurationProvider configurationProvider,
            IInstrumentationClient logger,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider,
            IApiClient apiClient,
            ICertificateReader certificateReader,
            IDbEntityRepository<BankClientProfile> clientProfileRepository,
            IDbEntityRepository<DomesticConsent> domesticConsentRepo,
            IDbEntityRepository<ApiProfile> apiProfileRepository,
            IKeySecretReadRepository<ActiveSoftwareStatementProfiles> activeSReadOnlyRepo,
            IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> activeSrRepo,
            IKeySecretMultiItemReadRepository<SoftwareStatementProfile> sReadOnlyRepo,
            IKeySecretMultiItemWriteRepository<SoftwareStatementProfile> sRepo,
            ISoftwareStatementProfileService softwareStatementProfileService)
        {
            _certificateReader = certificateReader.ArgNotNull(nameof(certificateReader));
            _timeProvider = timeProvider.ArgNotNull(nameof(timeProvider));
            _entityMapper = entityMapper.ArgNotNull(nameof(entityMapper));
            _dbContextService = dbContextService;
            _activeSReadOnlyRepo = activeSReadOnlyRepo;
            _activeSRRepo = activeSrRepo;
            _sReadOnlyRepo = sReadOnlyRepo;
            _sRepo = sRepo;
            _softwareStatementProfileService = softwareStatementProfileService;
            _configurationProvider = configurationProvider.ArgNotNull(nameof(configurationProvider));
            _logger = logger.ArgNotNull(nameof(logger));
            _keySecretReadOnlyProvider = keySecretReadOnlyProvider.ArgNotNull(nameof(keySecretReadOnlyProvider));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _clientProfileRepository = clientProfileRepository.ArgNotNull(nameof(clientProfileRepository));
            _domesticConsentRepo = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
            _apiProfileRepository = apiProfileRepository.ArgNotNull(nameof(apiProfileRepository));
        }

        public SoftwareStatementProfileContext SoftwareStatementProfile()
        {
            ISharedContext context = CreateContext();

            return new SoftwareStatementProfileContext(context);
        }

        public BankClientProfileContext BankClientProfile()
        {
            ISharedContext context = CreateContext();

            return new BankClientProfileContext(context);
        }

        public DomesticPaymentConsentContext DomesticPaymentConsent(string openBankingClientProfileId)
        {
            openBankingClientProfileId.ArgNotNull(nameof(openBankingClientProfileId));

            ISharedContext context = CreateContext();

            return new DomesticPaymentConsentContext(context)
            {
                ApiProfileId = openBankingClientProfileId
            };
        }

        public DomesticPaymentContext DomesticPayment()
        {
            ISharedContext context = CreateContext();
            return new DomesticPaymentContext(context);
        }

        public AuthorisationCallbackDataContext AuthorisationCallbackData()
        {
            ISharedContext context = CreateContext();

            return new AuthorisationCallbackDataContext(context);
        }

        public PaymentInitiationApiProfileContext PaymentInitiationApiProfile()
        {
            ISharedContext context = CreateContext();
            return new PaymentInitiationApiProfileContext(context);
        }

        private ISharedContext CreateContext()
        {
            SharedContext context = new SharedContext(
                certificateReader: _certificateReader,
                apiClient: _apiClient,
                configurationProvider: _configurationProvider,
                instrumentation: _logger,
                keySecretReadOnlyProvider: _keySecretReadOnlyProvider,
                clientProfileRepository: _clientProfileRepository,
                softwareStatementProfileService: _softwareStatementProfileService,
                domesticConsentRepository: _domesticConsentRepo,
                entityMapper: _entityMapper,
                apiProfileRepository: _apiProfileRepository,
                dbContextService: _dbContextService,
                activeSrRepo: _activeSRRepo,
                sReadOnlyRepo: _sReadOnlyRepo,
                sRepo: _sRepo,
                activeSReadOnlyRepo: _activeSReadOnlyRepo)
            {
                Created = _timeProvider.GetUtcNow()
            };
            return context;
        }
    }
}
