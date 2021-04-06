﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IRequestBuilder
    {
        /// <summary>
        ///     API for setting up banks in OBC including OAuth2 clients and functional APIs
        /// </summary>
        IClientRegistrationContext ClientRegistration { get; }

        /// <summary>
        ///     API corresponding to UK Open Banking Payment Initiation functional API
        /// </summary>
        IPaymentInitiationContext PaymentInitiation { get; }

        /// <summary>
        ///     API corresponding to UK Open Banking Account and Transaction functional API
        /// </summary>
        IAccountAndTransactionContext AccountAndTransaction { get; }
    }

    public class RequestBuilder : IRequestBuilder
    {
        private readonly IApiClient _apiClient;
        private readonly IApiVariantMapper _apiVariantMapper;
        private readonly IDbService _dbService;
        private readonly IInstrumentationClient _logger;
        private readonly IReadOnlyRepository<SoftwareStatementProfileCached> _softwareStatementProfileCachedRepo;
        private readonly ITimeProvider _timeProvider;

        public RequestBuilder(
            ITimeProvider timeProvider,
            IApiVariantMapper apiVariantMapper,
            IInstrumentationClient logger,
            IApiClient apiClient,
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfileCachedRepo,
            IDbService dbService)
        {
            _timeProvider = timeProvider.ArgNotNull(nameof(timeProvider));
            _apiVariantMapper = apiVariantMapper.ArgNotNull(nameof(apiVariantMapper));
            _softwareStatementProfileCachedRepo = softwareStatementProfileCachedRepo;
            _dbService = dbService;
            _logger = logger.ArgNotNull(nameof(logger));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
        }

        public IClientRegistrationContext ClientRegistration => new ClientRegistrationContext(CreateContext());
        public IPaymentInitiationContext PaymentInitiation => new PaymentInitiationContext(CreateContext());

        public IAccountAndTransactionContext AccountAndTransaction =>
            new AccountAndTransactionContext(CreateContext());

        private ISharedContext CreateContext()
        {
            SharedContext context = new SharedContext(
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
