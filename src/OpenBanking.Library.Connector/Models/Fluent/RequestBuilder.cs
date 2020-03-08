// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Security.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class RequestBuilder : IOpenBankingRequestBuilder
    {
        private readonly IApiClient _apiClient;
        private readonly BaseDbContext _baseDbContext;
        private readonly IApiProfileRepository _apiProfileRepository;
        private readonly ICertificateReader _certificateReader;
        private readonly IOpenBankingClientProfileRepository _clientProfileRepository;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IDomesticConsentRepository _domesticConsentRepo;
        private readonly IEntityMapper _entityMapper;
        private readonly IKeySecretProvider _keySecretProvider;
        private readonly IInstrumentationClient _logger;
        private readonly ISoftwareStatementProfileRepository _softwareStatementRepo;
        private readonly ITimeProvider _timeProvider;

        
        public RequestBuilder(IEntityMapper entityMapper,
            BaseDbContext baseDbContext,
            IConfigurationProvider configurationProvider,
            IInstrumentationClient logger, IKeySecretProvider keySecretProvider, IApiClient apiClient,
            ICertificateReader certificateReader,
            IOpenBankingClientProfileRepository clientProfileRepository,
            ISoftwareStatementProfileRepository softwareStatementProfileRepo,
            IDomesticConsentRepository domesticConsentRepo,
            IApiProfileRepository apiProfileRepository)
            : this(new TimeProvider(), entityMapper, baseDbContext, configurationProvider, logger, keySecretProvider, apiClient,
                certificateReader,
                clientProfileRepository, softwareStatementProfileRepo, domesticConsentRepo,
                apiProfileRepository)
        {
        }

        internal RequestBuilder(ITimeProvider timeProvider, IEntityMapper entityMapper,
            BaseDbContext baseDbContext,
            IConfigurationProvider configurationProvider,
            IInstrumentationClient logger, IKeySecretProvider keySecretProvider, IApiClient apiClient,
            ICertificateReader certificateReader,
            IOpenBankingClientProfileRepository clientProfileRepository,
            ISoftwareStatementProfileRepository softwareStatementProfileRepo,
            IDomesticConsentRepository domesticConsentRepo,
            IApiProfileRepository apiProfileRepository)
        {
            _certificateReader = certificateReader.ArgNotNull(nameof(certificateReader));
            _timeProvider = timeProvider.ArgNotNull(nameof(timeProvider));
            _entityMapper = entityMapper.ArgNotNull(nameof(entityMapper));
            _baseDbContext = baseDbContext;
            _configurationProvider = configurationProvider.ArgNotNull(nameof(configurationProvider));
            _logger = logger.ArgNotNull(nameof(logger));
            _keySecretProvider = keySecretProvider.ArgNotNull(nameof(keySecretProvider));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _clientProfileRepository = clientProfileRepository.ArgNotNull(nameof(clientProfileRepository));
            _softwareStatementRepo = softwareStatementProfileRepo.ArgNotNull(nameof(softwareStatementProfileRepo));
            _domesticConsentRepo = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
            _apiProfileRepository = apiProfileRepository.ArgNotNull(nameof(apiProfileRepository));
        }

        public SoftwareStatementProfileContext SoftwareStatementProfile()
        {
            var context = CreateContext();

            return new SoftwareStatementProfileContext(context);
        }

        public BankClientProfileContext BankClientProfile()
        {
            var context = CreateContext();

            return new BankClientProfileContext(context);
        }

        public DomesticPaymentConsentContext DomesticPaymentConsent(string openBankingClientProfileId)
        {
            openBankingClientProfileId.ArgNotNull(nameof(openBankingClientProfileId));

            var context = CreateContext();

            return new DomesticPaymentConsentContext(context)
            {
                OpenBankingClientProfileId = openBankingClientProfileId
            };
        }

        public DomesticPaymentContext DomesticPayment(string domesticConsentId)
        {
            domesticConsentId.ArgNotNull(nameof(domesticConsentId));

            var context = CreateContext();

            return new DomesticPaymentContext(context)
            {
                DomesticConsentId = domesticConsentId
            };
        }

        public AuthorisationCallbackContext AuthorisationCallback()
        {
            var context = CreateContext();

            return new AuthorisationCallbackContext(context);
        }

        public PaymentInitiationApiProfileContext PaymentInitiationApiProfile()
        {
            var context = CreateContext();
            return new PaymentInitiationApiProfileContext(context);
        }

        private ISharedContext CreateContext()
        {
            var context = new SharedContext(
                _baseDbContext,
                _certificateReader,
                _apiClient,
                _configurationProvider,
                _logger,
                _keySecretProvider, 
                _clientProfileRepository,
                _softwareStatementRepo, 
                _domesticConsentRepo,
                _entityMapper,
                _apiProfileRepository)
            {
                Created = _timeProvider.GetUtcNow()
            };

            return context;
        }
    }
}
