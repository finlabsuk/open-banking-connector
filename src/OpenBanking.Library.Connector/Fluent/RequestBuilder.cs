// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class RequestBuilder : IRequestBuilder
    {
        private readonly IKeySecretReadRepository<ActiveSoftwareStatementProfiles> _activeSReadOnlyRepo;
        private readonly IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> _activeSRRepo;
        private readonly IApiClient _apiClient;
        private readonly ICertificateReader _certificateReader;
        private readonly IObcConfigurationProvider _configurationProvider;
        private readonly IDbMultiEntityMethods _dbContextService;
        private readonly IDbEntityRepositoryFactory _dbEntityRepositoryFactory;
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
            IKeySecretReadRepository<ActiveSoftwareStatementProfiles> activeSReadOnlyRepo,
            IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> activeSrRepo,
            IKeySecretMultiItemReadRepository<SoftwareStatementProfile> sReadOnlyRepo,
            IKeySecretMultiItemWriteRepository<SoftwareStatementProfile> sRepo,
            ISoftwareStatementProfileService softwareStatementProfileService,
            IDbEntityRepositoryFactory dbEntityRepositoryFactory)
            : this(
                timeProvider: new TimeProvider(),
                entityMapper: entityMapper,
                dbContextService: dbContextService,
                configurationProvider: configurationProvider,
                logger: logger,
                keySecretReadOnlyProvider: keySecretReadOnlyProvider,
                apiClient: apiClient,
                certificateReader: certificateReader,
                activeSReadOnlyRepo: activeSReadOnlyRepo,
                activeSrRepo: activeSrRepo,
                sReadOnlyRepo: sReadOnlyRepo,
                sRepo: sRepo,
                softwareStatementProfileService: softwareStatementProfileService,
                dbEntityRepositoryFactory: dbEntityRepositoryFactory) { }

        internal RequestBuilder(
            ITimeProvider timeProvider,
            IEntityMapper entityMapper,
            IDbMultiEntityMethods dbContextService,
            IObcConfigurationProvider configurationProvider,
            IInstrumentationClient logger,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider,
            IApiClient apiClient,
            ICertificateReader certificateReader,
            IKeySecretReadRepository<ActiveSoftwareStatementProfiles> activeSReadOnlyRepo,
            IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> activeSrRepo,
            IKeySecretMultiItemReadRepository<SoftwareStatementProfile> sReadOnlyRepo,
            IKeySecretMultiItemWriteRepository<SoftwareStatementProfile> sRepo,
            ISoftwareStatementProfileService softwareStatementProfileService,
            IDbEntityRepositoryFactory dbEntityRepositoryFactory)
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
            _dbEntityRepositoryFactory = dbEntityRepositoryFactory;
            _configurationProvider = configurationProvider.ArgNotNull(nameof(configurationProvider));
            _logger = logger.ArgNotNull(nameof(logger));
            _keySecretReadOnlyProvider = keySecretReadOnlyProvider.ArgNotNull(nameof(keySecretReadOnlyProvider));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
        }

        public IFluentContextLocalEntity<DomesticPaymentConsent, DomesticPaymentConsentResponse,
            IDomesticPaymentConsentPublicQuery> DomesticPaymentConsents =>
            new FluentContext<Models.Persistent.PaymentInitiation.DomesticPaymentConsent, DomesticPaymentConsent,
                DomesticPaymentConsentResponse, IDomesticPaymentConsentPublicQuery>(CreateContext());

        public IFluentContextLocalEntity<DomesticPayment, DomesticPaymentResponse, IDomesticPaymentPublicQuery>
            DomesticPayments => new FluentContext<Models.Persistent.PaymentInitiation.DomesticPayment, DomesticPayment,
            DomesticPaymentResponse, IDomesticPaymentPublicQuery>(CreateContext());

        public SoftwareStatementProfileContext SoftwareStatementProfile()
        {
            ISharedContext context = CreateContext();

            return new SoftwareStatementProfileContext(context);
        }

        public IFluentContextLocalEntity<Bank, BankResponse, IBankPublicQuery> Banks =>
            new FluentContext<Models.Persistent.Bank, Bank, BankResponse, IBankPublicQuery>(CreateContext());

        public IFluentContextLocalEntity<BankRegistration, BankRegistrationResponse, IBankRegistrationPublicQuery>
            BankRegistrations =>
            new FluentContext<Models.Persistent.BankRegistration, BankRegistration, BankRegistrationResponse,
                IBankRegistrationPublicQuery>(CreateContext());

        public IFluentContextLocalEntity<BankProfile, BankProfileResponse, IBankProfilePublicQuery> BankProfiles =>
            new FluentContext<Models.Persistent.BankProfile, BankProfile, BankProfileResponse, IBankProfilePublicQuery>(
                CreateContext());

        public IFluentContextPostOnlyEntity<AuthorisationRedirectObject, AuthorisationRedirectObjectResponse>
            AuthorisationRedirectObjects =>
            new FluentContextPostOnlyEntity<Models.PostOnly.AuthorisationRedirectObject, AuthorisationRedirectObject,
                AuthorisationRedirectObjectResponse>(CreateContext());


        private ISharedContext CreateContext()
        {
            SharedContext context = new SharedContext(
                timeProvider: _timeProvider,
                certificateReader: _certificateReader,
                apiClient: _apiClient,
                configurationProvider: _configurationProvider,
                instrumentation: _logger,
                keySecretReadOnlyProvider: _keySecretReadOnlyProvider,
                dbEntityRepositoryFactory: _dbEntityRepositoryFactory,
                activeSrRepo: _activeSRRepo,
                sReadOnlyRepo: _sReadOnlyRepo,
                sRepo: _sRepo,
                activeSReadOnlyRepo: _activeSReadOnlyRepo,
                softwareStatementProfileService: _softwareStatementProfileService,
                entityMapper: _entityMapper,
                dbContextService: _dbContextService)
            {
                Created = _timeProvider.GetUtcNow()
            };
            return context;
        }
    }
}
