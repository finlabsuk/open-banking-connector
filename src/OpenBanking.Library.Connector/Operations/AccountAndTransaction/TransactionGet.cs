// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.Web;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction
{
    internal class
        TransactionGet : AccountAccessConsentExternalObject<TransactionsResponse,
            AccountAndTransactionModelsPublic.OBReadTransaction6, TransactionsReadParams>
    {
        public TransactionGet(
            IDbReadWriteEntityMethods<AccountAccessConsent> entityMethods,
            IInstrumentationClient instrumentationClient,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IApiVariantMapper mapper,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IGrantPost grantPost,
            AuthContextAccessTokenGet authContextAccessTokenGet) : base(
            entityMethods,
            instrumentationClient,
            softwareStatementProfileRepo,
            mapper,
            dbSaveChangesMethod,
            timeProvider,
            grantPost,
            authContextAccessTokenGet) { }

        protected override Uri GetApiRequestUrl(
            string baseUrl,
            TransactionsReadParams readParams)
        {
            string urlString = (externalAccountId: readParams.ExternalApiAccountId,
                    externalStatementId: readParams.ExternalApiStatementId) switch
                {
                    (null, null) => $"{baseUrl}/transactions",
                    ({ } extAccountId, null) => $"{baseUrl}/accounts/{extAccountId}/transactions",
                    ({ } extAccountId, { } extStatementId) =>
                        $"{baseUrl}/accounts/{extAccountId}/statements/{extStatementId}/transactions",
                    _ => throw new ArgumentOutOfRangeException()
                };
            return new UriBuilder(urlString)
            {
                Query =  ConstructedQuery(readParams.QueryString, readParams)
            }.Uri;
        }

        private string ConstructedQuery(string? queryString, TransactionsReadParams readParams)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(queryString ?? new UriBuilder().Query);
            if (!string.IsNullOrEmpty(readParams.FromBookingDateTime))
            {
                query["fromBookingDateTime"] = readParams.FromBookingDateTime;
            }

            if (!string.IsNullOrEmpty(readParams.ToBookingDateTime))
            {
                query["toBookingDateTime"] = readParams.ToBookingDateTime;
            }

            return query.ToString()!;
        }

        protected override TransactionsResponse PublicGetResponse(
            AccountAndTransactionModelsPublic.OBReadTransaction6 apiResponse,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery,
            bool allowValidQueryParametersOnly,
            TransactionsReadParams readParams)
        {
            var validQueryParameters = new List<string>();

            // Get link queries
            apiResponse.Links.Self = Helpers.TransformLinkUrl(
                apiResponse.Links.Self,
                apiRequestUrl,
                publicRequestUrlWithoutQuery,
                allowValidQueryParametersOnly,
                validQueryParameters);
            apiResponse.Links.First = Helpers.TransformLinkUrl(
                apiResponse.Links.First,
                apiRequestUrl,
                publicRequestUrlWithoutQuery,
                allowValidQueryParametersOnly,
                validQueryParameters);
            apiResponse.Links.Prev = Helpers.TransformLinkUrl(
                apiResponse.Links.Prev,
                apiRequestUrl,
                publicRequestUrlWithoutQuery,
                allowValidQueryParametersOnly,
                validQueryParameters);
            apiResponse.Links.Next = Helpers.TransformLinkUrl(
                apiResponse.Links.Next,
                apiRequestUrl,
                publicRequestUrlWithoutQuery,
                allowValidQueryParametersOnly,
                validQueryParameters);
            apiResponse.Links.Last = Helpers.TransformLinkUrl(
                apiResponse.Links.Last,
                apiRequestUrl,
                publicRequestUrlWithoutQuery,
                allowValidQueryParametersOnly,
                validQueryParameters);

            return new TransactionsResponse(apiResponse);
        }

        protected override IApiGetRequests<AccountAndTransactionModelsPublic.OBReadTransaction6> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient) =>
            accountAndTransactionApi.AccountAndTransactionApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadTransaction6,
                    AccountAndTransactionModelsV3p1p7.OBReadTransaction6>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.Version3p1p10 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadTransaction6,
                    AccountAndTransactionModelsV3p1p10.OBReadTransaction6>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };
    }
}
