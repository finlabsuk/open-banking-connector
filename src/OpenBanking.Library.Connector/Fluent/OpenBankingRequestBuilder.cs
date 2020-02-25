// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class OpenBankingRequestBuilder : IOpenBankingRequestBuilder
    {
        private readonly IApiClient _apiClient;
        private readonly ICertificateReader _certificateReader;
        private readonly IOpenBankingClientProfileRepository _clientProfileRepository;
        private readonly IOpenBankingClientRepository _clientRepository;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IDomesticConsentRepository _domesticConsentRepo;
        private readonly IEntityMapper _entityMapper;
        private readonly IKeySecretProvider _keySecretProvider;
        private readonly IInstrumentationClient _logger;
        private readonly ISoftwareStatementProfileRepository _softwareStatementRepo;
        private readonly ITimeProvider _timeProvider;

        public OpenBankingRequestBuilder(IEntityMapper entityMapper,
            IConfigurationProvider configurationProvider,
            IInstrumentationClient logger, IKeySecretProvider keySecretProvider, IApiClient apiClient,
            ICertificateReader certificateReader,
            IOpenBankingClientRepository clientRepository, IOpenBankingClientProfileRepository clientProfileRepository,
            ISoftwareStatementProfileRepository softwareStatementProfileRepo,
            IDomesticConsentRepository domesticConsentRepo)
            : this(new TimeProvider(), entityMapper, configurationProvider, logger, keySecretProvider, apiClient,
                certificateReader,
                clientRepository, clientProfileRepository, softwareStatementProfileRepo, domesticConsentRepo)
        {
        }

        internal OpenBankingRequestBuilder(ITimeProvider timeProvider, IEntityMapper entityMapper,
            IConfigurationProvider configurationProvider,
            IInstrumentationClient logger, IKeySecretProvider keySecretProvider, IApiClient apiClient,
            ICertificateReader certificateReader,
            IOpenBankingClientRepository clientRepository, IOpenBankingClientProfileRepository clientProfileRepository,
            ISoftwareStatementProfileRepository softwareStatementProfileRepo,
            IDomesticConsentRepository domesticConsentRepo)
        {
            _clientRepository = clientRepository.ArgNotNull(nameof(clientRepository));
            _certificateReader = certificateReader.ArgNotNull(nameof(certificateReader));
            _timeProvider = timeProvider.ArgNotNull(nameof(timeProvider));
            _entityMapper = entityMapper.ArgNotNull(nameof(entityMapper));
            _configurationProvider = configurationProvider.ArgNotNull(nameof(configurationProvider));
            _logger = logger.ArgNotNull(nameof(logger));
            _keySecretProvider = keySecretProvider.ArgNotNull(nameof(keySecretProvider));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _clientProfileRepository = clientProfileRepository.ArgNotNull(nameof(clientProfileRepository));
            _softwareStatementRepo = softwareStatementProfileRepo.ArgNotNull(nameof(softwareStatementProfileRepo));
            _domesticConsentRepo = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
        }

        public SoftwareStatementProfileContext SoftwareStatementProfile()
        {
            var context = CreateContext();

            return new SoftwareStatementProfileContext(context);
        }

        public ClientProfileContext ClientProfile(string softwareStatementProfileId)
        {
            softwareStatementProfileId.ArgNotNull(nameof(softwareStatementProfileId));
            var context = CreateContext();

            return new ClientProfileContext(context)
            {
                SoftwareStatementProfileId = softwareStatementProfileId
            };
        }

        public ClientContext Client()
        {
            var context = CreateContext();

            return new ClientContext(context);
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

        private OpenBankingContext CreateContext()
        {
            var context = new OpenBankingContext(_configurationProvider, _logger, _keySecretProvider, _apiClient,
                _certificateReader,
                _clientProfileRepository, _clientRepository, _softwareStatementRepo, _entityMapper,
                _domesticConsentRepo)
            {
                Created = _timeProvider.GetUtcNow()
            };

            return context;
        }
    }
}
