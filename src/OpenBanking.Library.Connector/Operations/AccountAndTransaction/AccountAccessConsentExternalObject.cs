// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.Web;
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
                string? fromBookingDateTime,
                string? toBookingDateTime,
                string? page,
                string? modifiedBy,
                string? publicRequestUrlWithoutQuery,
                string? queryString)
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
                    .SingleOrDefaultAsync(x => x.Id == consentId) ??
                throw new KeyNotFoundException($"No record found for Account Access Consent with ID {consentId}.");
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
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get access token
            string accessToken =
                await _authContextAccessTokenGet.GetAccessTokenAndUpdateConsent(
                    persistedConsent,
                    bankRegistration,
                    modifiedBy);

            // Retrieve endpoint URL
            string apiRequestBaseUrl = GetApiBaseRequestUrl(
                accountAndTransactionApi.BaseUrl,
                externalApiAccountId,
                externalApiStatementId);
            string apiRequestQueryString = GetQueryString(
                fromBookingDateTime,
                toBookingDateTime,
                page,
                queryString);
            Uri apiRequestUrl = new UriBuilder(apiRequestBaseUrl)
            {
                Query = apiRequestQueryString
            }.Uri;

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
            var validQueryParameters = new List<string>();
            if (fromBookingDateTime is not null)
            {
                validQueryParameters.Add("fromBookingDateTime");
            }

            if (toBookingDateTime is not null)
            {
                validQueryParameters.Add("toBookingDateTime");
            }

            if (page is not null)
            {
                validQueryParameters.Add("page");
            }

            TPublicResponse response = PublicGetResponse(
                apiResponse,
                apiRequestUrl,
                publicRequestUrlWithoutQuery,
                queryString is not null,
                validQueryParameters);

            return (response, nonErrorMessages);
        }

        private string GetQueryString(
            string? fromBookingDateTime,
            string? toBookingDateTime,
            string? page,
            string? queryString)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(new UriBuilder().Query);
            if (!string.IsNullOrEmpty(fromBookingDateTime))
            {
                query["fromBookingDateTime"] = fromBookingDateTime;
            }

            if (!string.IsNullOrEmpty(toBookingDateTime))
            {
                query["toBookingDateTime"] = toBookingDateTime;
            }

            if (!string.IsNullOrEmpty(page))
            {
                query["page"] = page;
            }

            if (queryString is null)
            {
                return query.ToString()!;
            }

            return queryString!;
        }

        protected abstract string GetApiBaseRequestUrl(
            string baseUrl,
            string? externalApiAccountId,
            string? externalApiStatementId);

        protected abstract TPublicResponse PublicGetResponse(
            TApiResponse apiResponse,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery,
            bool supportAllQueryParameters,
            IList<string> validQueryParameters);

        protected abstract IApiGetRequests<TApiResponse> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient);

        protected string? GetLinkUrlQuery(
            string? linkUrlString,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery,
            bool supportAllQueryParameters,
            IList<string> validQueryParameters)
        {
            if (linkUrlString is null)
            {
                return null;
            }

            var linkUrl = new Uri(linkUrlString);

            int urlsMatch = Uri.Compare(
                linkUrl,
                apiRequestUrl,
                UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path,
                UriFormat.Unescaped,
                StringComparison.InvariantCulture);

            // Check URLs without fragment and query parameters match
            if (urlsMatch != 0)
            {
                throw new InvalidOperationException(
                    $"Request URL {apiRequestUrl} and link URL {linkUrl} have different base URLs!");
            }

            // Check there are no fragment parameters
            if (!string.IsNullOrEmpty(linkUrl.Fragment))
            {
                throw new InvalidOperationException($"Link URL {linkUrl} has unexpected fragment.");
            }

            // Check query parameters are valid
            if (!string.IsNullOrEmpty(linkUrl.Query) &&
                !supportAllQueryParameters)
            {
                string[] linkUrlQueryParameterPairs = linkUrl.Query.TrimStart('?').Split('&');
                foreach (string queryParameterPair in linkUrlQueryParameterPairs)
                {
                    string queryParameterName = queryParameterPair.Split('=')[0];
                    if (!validQueryParameters.Contains(queryParameterName))
                    {
                        throw new InvalidOperationException(
                            $"External API returned link URL with query parameter {queryParameterName} which is unexpected.");
                    }
                }
            }

            // Return relative URL
            if (publicRequestUrlWithoutQuery is null)
            {
                return linkUrl.Query;
            }

            // Return absolute URL
            var uriBuilder = new UriBuilder(publicRequestUrlWithoutQuery);
            uriBuilder.Query = linkUrl.Query;
            Uri fullUri = uriBuilder.Uri;
            return fullUri.ToString();
        }
    }
}
