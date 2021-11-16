// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Utility;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IRequestBuilder
    {
        /// <summary>
        ///     API for setting up banks in Open Banking Connector including OAuth2 clients and functional APIs.
        /// </summary>
        IBankConfigurationContext BankConfiguration { get; }

        /// <summary>
        ///     API corresponding to UK Open Banking Payment Initiation functional API.
        /// </summary>
        IPaymentInitiationContext PaymentInitiation { get; }

        /// <summary>
        ///     API corresponding to UK Open Banking Account and Transaction functional API.
        /// </summary>
        IAccountAndTransactionContext AccountAndTransaction { get; }

        /// <summary>
        ///     API corresponding to UK Open Banking Variable Recurring Payments functional API.
        /// </summary>
        IVariableRecurringPaymentsContext VariableRecurringPayments { get; }

        /// <summary>
        ///     API-independent methods for auth contexts. Method for passing back auth result obtained via bank redirect is
        ///     included.
        /// </summary>
        IAuthContextsContext AuthContexts { get; }

        /// <summary>
        ///     API containing utility methods.
        /// </summary>
        IUtilityContext Utility { get; }
    }

    public class RequestBuilder : IRequestBuilder
    {
        private readonly IApiClient _apiClient;
        private readonly IApiVariantMapper _apiVariantMapper;
        private readonly IDbService _dbService;
        private readonly IInstrumentationClient _logger;
        private readonly IReadOnlyRepository<ProcessedSoftwareStatementProfile> _softwareStatementProfileCachedRepo;
        private readonly ITimeProvider _timeProvider;

        public RequestBuilder(
            ITimeProvider timeProvider,
            IApiVariantMapper apiVariantMapper,
            IInstrumentationClient logger,
            IApiClient apiClient,
            IReadOnlyRepository<ProcessedSoftwareStatementProfile> softwareStatementProfileCachedRepo,
            IDbService dbService)
        {
            _timeProvider = timeProvider.ArgNotNull(nameof(timeProvider));
            _apiVariantMapper = apiVariantMapper.ArgNotNull(nameof(apiVariantMapper));
            _softwareStatementProfileCachedRepo = softwareStatementProfileCachedRepo;
            _dbService = dbService;
            _logger = logger.ArgNotNull(nameof(logger));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
        }


        public IBankConfigurationContext BankConfiguration =>
            new BankConfigurationContext(CreateContext());

        public IAccountAndTransactionContext AccountAndTransaction =>
            new AccountAndTransactionContext(CreateContext());

        public IPaymentInitiationContext PaymentInitiation =>
            new PaymentInitiationContext(CreateContext());

        public IVariableRecurringPaymentsContext VariableRecurringPayments =>
            new VariableRecurringPaymentsContext(CreateContext());

        public IAuthContextsContext AuthContexts =>
            new AuthContextsContext(CreateContext());

        public IUtilityContext Utility =>
            new UtilityContext(CreateContext());

        private ISharedContext CreateContext()
        {
            var context = new SharedContext(
                _timeProvider,
                _apiClient,
                _logger,
                _dbService,
                _softwareStatementProfileCachedRepo,
                _apiVariantMapper)
            {
                Created = _timeProvider.GetUtcNow()
            };
            return context;
        }
    }
}
