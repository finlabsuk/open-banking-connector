// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction
{
    /// <summary>
    ///     Read operations on entities (objects stored in external (i.e. bank) database and local database).
    /// </summary>
    /// <typeparam name="TPublicResponse"></typeparam>
    /// <typeparam name="TApiResponse"></typeparam>
    /// <typeparam name="TExternalEntityReadParams"></typeparam>
    internal abstract class
        AccountAccessConsentExternalObject<TPublicResponse, TApiResponse, TExternalEntityReadParams> :
            IAccountAccessConsentExternalRead<TPublicResponse, TExternalEntityReadParams>
        where TApiResponse : class, ISupportsValidation
        where TExternalEntityReadParams : ExternalEntityReadParams
    {
        private readonly AuthContextAccessTokenGet _authContextAccessTokenGet;
        private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _entityMethods;
        protected readonly IGrantPost _grantPost;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IApiVariantMapper _mapper;
        private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public AccountAccessConsentExternalObject(
            IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods,
            IInstrumentationClient instrumentationClient,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IApiVariantMapper mapper,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IGrantPost grantPost,
            AuthContextAccessTokenGet authContextAccessTokenGet)
        {
            _entityMethods = entityMethods;
            _instrumentationClient = instrumentationClient;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _mapper = mapper;
            _timeProvider = timeProvider;
            _grantPost = grantPost;
            _authContextAccessTokenGet = authContextAccessTokenGet;
        }

        public async Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ReadAsync(TExternalEntityReadParams readParams)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Get consent and associated data
            AccountAccessConsentPersisted persistedConsent =
                await _entityMethods
                    .DbSet
                    .Include(o => o.AccountAccessConsentAuthContextsNavigation)
                    .Include(o => o.AccountAndTransactionApiNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == readParams.ConsentId) ??
                throw new KeyNotFoundException(
                    $"No record found for Account Access Consent with ID {readParams.ConsentId}.");
            AccountAndTransactionApiEntity accountAndTransactionApiEntity =
                persistedConsent.AccountAndTransactionApiNavigation;
            var accountAndTransactionApi = new AccountAndTransactionApi
            {
                AccountAndTransactionApiVersion = accountAndTransactionApiEntity.ApiVersion,
                BaseUrl = accountAndTransactionApiEntity.BaseUrl
            };
            BankRegistrationPersisted bankRegistration = persistedConsent.BankRegistrationNavigation;
            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            // Get API client
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementProfileOverride);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get access token
            string? requestObjectAudClaim =
                persistedConsent.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                    ?.AccountAccessConsentAuthGet
                    ?.AudClaim;
            string bankIssuerUrl =
                requestObjectAudClaim ??
                bankRegistration.BankNavigation.IssuerUrl ??
                throw new Exception("Cannot determine issuer URL for bank");
            string accessToken =
                await _authContextAccessTokenGet.GetAccessTokenAndUpdateConsent(
                    persistedConsent,
                    bankIssuerUrl,
                    "openid accounts",
                    bankRegistration,
                    readParams.ModifiedBy);

            // Retrieve endpoint URL
            Uri apiRequestUrl = GetApiRequestUrl(
                accountAndTransactionApi.BaseUrl,
                readParams);

            // Get external object from bank API
            JsonSerializerSettings? jsonSerializerSettings = null;
            IApiGetRequests<TApiResponse> apiRequests = ApiRequests(
                accountAndTransactionApi,
                bankFinancialId,
                accessToken,
                _instrumentationClient);
            TApiResponse apiResponse;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            (apiResponse, newNonErrorMessages) =
                await apiRequests.GetAsync(
                    apiRequestUrl,
                    jsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response
            TPublicResponse response = PublicGetResponse(
                apiResponse,
                apiRequestUrl,
                readParams.PublicRequestUrlWithoutQuery,
                false, // always support all query parameters now
                readParams);

            return (response, nonErrorMessages);
        }

        protected abstract Uri GetApiRequestUrl(
            string baseUrl,
            TExternalEntityReadParams readParams);

        protected abstract TPublicResponse PublicGetResponse(
            TApiResponse apiResponse,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery,
            bool allowValidQueryParametersOnly,
            TExternalEntityReadParams readParams);

        protected abstract IApiGetRequests<TApiResponse> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient);
    }
}
