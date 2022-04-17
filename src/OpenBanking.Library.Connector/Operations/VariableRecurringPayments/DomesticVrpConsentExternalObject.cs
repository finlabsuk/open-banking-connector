// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    /// <summary>
    ///     Read operations on entities (objects stored in external (i.e. bank) database and local database).
    /// </summary>
    /// <typeparam name="TPublicResponse"></typeparam>
    /// <typeparam name="TApiResponse"></typeparam>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TApiRequest"></typeparam>
    internal abstract class
        DomesticVrpConsentExternalObject<TPublicRequest, TPublicResponse, TApiRequest, TApiResponse> :
            IObjectRead3<TPublicResponse>, IObjectCreate2<TPublicRequest, TPublicResponse>
        where TApiResponse : class, ISupportsValidation
        where TApiRequest : class, ISupportsValidation
    {
        private readonly AuthContextAccessTokenGet _authContextAccessTokenGet;
        private readonly IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> _entityMethods;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IApiVariantMapper _mapper;
        private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        protected readonly ITimeProvider _timeProvider;


        public DomesticVrpConsentExternalObject(
            IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
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

        protected abstract string ClientCredentialsGrantScope { get; }


        public async Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            CreateAsync(TPublicRequest request, Guid consentId, string? createdBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Get consent and associated data
            DomesticVrpConsentPersisted persistedConsent =
                await _entityMethods
                    .DbSet
                    .Include(o => o.DomesticVrpConsentAuthContextsNavigation)
                    .Include(o => o.VariableRecurringPaymentsApiNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == consentId) ??
                throw new KeyNotFoundException($"No record found for Domestic Vrp Consent with ID {consentId}.");
            VariableRecurringPaymentsApiEntity variableRecurringPaymentsApiEntity =
                persistedConsent.VariableRecurringPaymentsApiNavigation;
            var variableRecurringPaymentsApi = new VariableRecurringPaymentsApi
            {
                VariableRecurringPaymentsApiVersion = variableRecurringPaymentsApiEntity.ApiVersion,
                BaseUrl = variableRecurringPaymentsApiEntity.BaseUrl
            };
            BankRegistrationPersisted bankRegistration = persistedConsent.BankRegistrationNavigation;
            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            // Get request
            string externalApiConsentId = persistedConsent.ExternalApiId;
            TApiRequest apiRequest = GetApiRequest(request, externalApiConsentId);

            // Get API client
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get access token
            string accessToken =
                await _authContextAccessTokenGet.GetAccessTokenAndUpdateConsent(
                    persistedConsent,
                    bankRegistration,
                    createdBy);

            // Retrieve endpoint URL
            Uri endpointUrl = RetrievePostUrl(variableRecurringPaymentsApi.BaseUrl);

            // Create external object at bank API
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiPostRequests<TApiRequest, TApiResponse> apiRequests =
                ApiPostRequests(
                    variableRecurringPaymentsApi,
                    bankFinancialId,
                    accessToken,
                    processedSoftwareStatementProfile,
                    _instrumentationClient);
            TApiResponse apiResponse;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            (apiResponse, newNonErrorMessages) =
                await apiRequests.PostAsync(
                    endpointUrl,
                    apiRequest,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response
            TPublicResponse response = PublicGetResponse(apiResponse);

            return (response, nonErrorMessages);
        }

        public async Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ReadAsync(
                string externalId,
                Guid consentId,
                string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Get consent and associated data
            DomesticVrpConsentPersisted persistedConsent =
                await _entityMethods
                    .DbSetNoTracking
                    .Include(o => o.DomesticVrpConsentAuthContextsNavigation)
                    .Include(o => o.VariableRecurringPaymentsApiNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == consentId) ??
                throw new KeyNotFoundException($"No record found for Domestic Vrp Consent with ID {consentId}.");
            VariableRecurringPaymentsApiEntity variableRecurringPaymentsApiEntity =
                persistedConsent.VariableRecurringPaymentsApiNavigation;
            var variableRecurringPaymentsApi = new VariableRecurringPaymentsApi
            {
                VariableRecurringPaymentsApiVersion = variableRecurringPaymentsApiEntity.ApiVersion,
                BaseUrl = variableRecurringPaymentsApiEntity.BaseUrl
            };
            BankRegistrationPersisted bankRegistration = persistedConsent.BankRegistrationNavigation;
            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            // Get API client
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get access token
            JsonSerializerSettings? jsonSerializerSettings1 = null;
            string accessToken =
                (await PostTokenRequest.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    jsonSerializerSettings1,
                    apiClient,
                    _instrumentationClient))
                .AccessToken;

            // Retrieve endpoint URL
            Uri endpointUrl = RetrieveGetUrl(
                variableRecurringPaymentsApi.BaseUrl,
                externalId);

            // Get external object from bank API
            JsonSerializerSettings? jsonSerializerSettings2 = null;
            IApiGetRequests<TApiResponse> apiRequests = ApiGetRequests(
                variableRecurringPaymentsApi,
                bankFinancialId,
                accessToken,
                _instrumentationClient);
            TApiResponse apiResponse;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            (apiResponse, newNonErrorMessages) =
                await apiRequests.GetAsync(
                    endpointUrl,
                    jsonSerializerSettings2,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response
            TPublicResponse response = PublicGetResponse(apiResponse);

            return (response, nonErrorMessages);
        }

        protected abstract TApiRequest GetApiRequest(TPublicRequest request, string externalApiConsentId);

        protected abstract Uri RetrieveGetUrl(
            string baseUrl,
            string externalId);

        protected abstract Uri RetrievePostUrl(string baseUrl);

        protected abstract TPublicResponse PublicGetResponse(TApiResponse apiResponse);

        protected abstract IApiGetRequests<TApiResponse> ApiGetRequests(
            VariableRecurringPaymentsApi variableRecurringPaymentsApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient);

        protected abstract IApiPostRequests<TApiRequest, TApiResponse> ApiPostRequests(
            VariableRecurringPaymentsApi variableRecurringPaymentsApi,
            string bankFinancialId,
            string accessToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient);
    }
}
