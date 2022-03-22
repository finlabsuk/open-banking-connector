// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    /// <summary>
    ///     Read operations on entities (objects stored in external (i.e. bank) database and local database).
    /// </summary>
    /// <typeparam name="TPublicResponse"></typeparam>
    /// <typeparam name="TApiResponse"></typeparam>
    internal abstract class
        ApiEntityGet<TPublicResponse, TApiResponse> :
            IObjectRead2<TPublicResponse>
        where TApiResponse : class, ISupportsValidation
    {
        private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _entityMethods;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IApiVariantMapper _mapper;
        private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;


        public ApiEntityGet(
            IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods,
            IInstrumentationClient instrumentationClient,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IApiVariantMapper mapper)
        {
            _entityMethods = entityMethods;
            _instrumentationClient = instrumentationClient;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _mapper = mapper;
        }

        protected abstract string RelativePath { get; }

        protected abstract string RelativePath2 { get; }

        public async Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ReadAsync(
                Guid consentId,
                string? externalAccountId,
                string? externalStatementId)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // READ from bank API

            // Load object
            AccountAccessConsentPersisted persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.AccountAccessConsentAuthContextsNavigation)
                    .Include(o => o.BankApiSetNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == consentId) ??
                throw new KeyNotFoundException($"No record found for Account Access Consent with ID {consentId}.");
            string bankApiId = persistedObject.ExternalApiId;
            BankApiSet bankApiSet = persistedObject.BankApiSetNavigation;
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

            // Get token
            List<AccountAccessConsentAuthContext> authContextsWithToken =
                persistedObject.AccountAccessConsentAuthContextsNavigation
                    .Where(x => x.TokenEndpointResponse.Data != null)
                    .ToList();

            TokenEndpointResponse userTokenEndpointResponse =
                authContextsWithToken.Any()
                    ? authContextsWithToken
                        .OrderByDescending(x => x.TokenEndpointResponse.Modified)
                        .Select(x => x.TokenEndpointResponse.Data)
                        .First()! // We already filtered out null entries above
                    : throw new InvalidOperationException("No token is available for Account Access Consent.");

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get client credentials grant token if necessary
            TokenEndpointResponse tokenEndpointResponse =
                userTokenEndpointResponse ??
                await PostTokenRequest.PostClientCredentialsGrantAsync(
                    "payments",
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    null,
                    apiClient,
                    _instrumentationClient);

            // Determine endpoint URL
            string baseUrl =
                bankApiSet.AccountAndTransactionApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Account and Transaction API.");
            Uri endpointUrl = (externalAccountId, externalStatementId) switch
            {
                (null, null) => new Uri(baseUrl + RelativePath),
                ({ } extAccountId, null) => new Uri(baseUrl + $"/accounts/{extAccountId}" + RelativePath2),
                ({ } extAccountId, { } extStatementId) => new Uri(
                    baseUrl + $"/accounts/{extAccountId}" + $"/statements/{extStatementId}" + RelativePath2),
                _ => throw new ArgumentOutOfRangeException()
            };

            // Create new Open Banking object by posting JWT
            JsonSerializerSettings? jsonSerializerSettings = null;
            IApiGetRequests<TApiResponse> apiRequests = ApiRequests(
                bankApiSet.AccountAndTransactionApi,
                bankFinancialId,
                tokenEndpointResponse,
                processedSoftwareStatementProfile,
                _instrumentationClient);

            // L1
            TApiResponse apiResponse;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            (apiResponse, newNonErrorMessages) =
                await apiRequests.GetAsync(
                    endpointUrl,
                    jsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response (may involve additional processing based on entity)
            TPublicResponse response = PublicGetResponse(apiResponse);

            return (response, nonErrorMessages);
        }

        protected abstract TPublicResponse PublicGetResponse(TApiResponse apiResponse);

        protected abstract IApiGetRequests<TApiResponse> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient);
    }
}
