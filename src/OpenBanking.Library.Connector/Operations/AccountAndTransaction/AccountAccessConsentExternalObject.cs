// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
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
using BankApiSetPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiSet;
using BankRegistrationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction
{
    /// <summary>
    ///     Read operations on entities (objects stored in external (i.e. bank) database and local database).
    /// </summary>
    /// <typeparam name="TPublicResponse"></typeparam>
    /// <typeparam name="TApiResponse"></typeparam>
    internal abstract class
        AccountAccessConsentExternalObject<TPublicResponse, TApiResponse> :
            IObjectRead2<TPublicResponse>
        where TApiResponse : class, ISupportsValidation
    {
        private readonly AuthContextAccessTokenGet _authContextAccessTokenGet;
        private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _entityMethods;
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
            ITimeProvider timeProvider)
        {
            _entityMethods = entityMethods;
            _instrumentationClient = instrumentationClient;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _mapper = mapper;
            _timeProvider = timeProvider;
            _authContextAccessTokenGet = new AuthContextAccessTokenGet(
                softwareStatementProfileRepo,
                dbSaveChangesMethod,
                timeProvider);
        }

        public async Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ReadAsync(
                Guid consentId,
                string? externalApiAccountId,
                string? externalApiStatementId,
                string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Get consent and associated data
            AccountAccessConsentPersisted persistedConsent =
                await _entityMethods
                    .DbSet
                    .Include(o => o.AccountAccessConsentAuthContextsNavigation)
                    .Include(o => o.BankApiSetNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == consentId) ??
                throw new KeyNotFoundException($"No record found for Account Access Consent with ID {consentId}.");
            BankApiSetPersisted bankApiSet = persistedConsent.BankApiSetNavigation;
            AccountAndTransactionApi accountAndTransactionApi =
                bankApiSet.AccountAndTransactionApi ??
                throw new InvalidOperationException("Bank API Set has no Account and Transaction API specified.");
            BankRegistrationPersisted bankRegistration = persistedConsent.BankRegistrationNavigation;
            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            // Get API client
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get access token
            string accessToken =
                await _authContextAccessTokenGet.GetAccessToken(
                    persistedConsent.AccountAccessConsentAuthContextsNavigation,
                    bankRegistration,
                    modifiedBy);

            // Retrieve endpoint URL
            Uri endpointUrl = RetrieveGetUrl(
                accountAndTransactionApi.BaseUrl,
                externalApiAccountId,
                externalApiStatementId);

            // Get external object from bank API
            JsonSerializerSettings? jsonSerializerSettings = null;
            IApiGetRequests<TApiResponse> apiRequests = ApiRequests(
                bankApiSet.AccountAndTransactionApi,
                bankFinancialId,
                accessToken,
                _instrumentationClient);
            TApiResponse apiResponse;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            (apiResponse, newNonErrorMessages) =
                await apiRequests.GetAsync(
                    endpointUrl,
                    jsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response
            TPublicResponse response = PublicGetResponse(apiResponse);

            return (response, nonErrorMessages);
        }

        protected abstract Uri RetrieveGetUrl(
            string baseUrl,
            string? externalApiAccountId,
            string? externalApiStatementId);

        protected abstract TPublicResponse PublicGetResponse(TApiResponse apiResponse);

        protected abstract IApiGetRequests<TApiResponse> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient);
    }
}
