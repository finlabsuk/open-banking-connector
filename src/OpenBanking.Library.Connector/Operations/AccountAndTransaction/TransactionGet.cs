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
using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;
using AccountAndTransactionModelsV3p1p7 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models;

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
                Query = readParams.QueryString ?? ConstructedQuery(readParams)
            }.Uri;
        }

        private string ConstructedQuery(TransactionsReadParams readParams)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(new UriBuilder().Query);
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
            TransactionsReadParams readParams)
        {
            var validQueryParameters = new List<string>();
            if (readParams.FromBookingDateTime is not null)
            {
                validQueryParameters.Add("fromBookingDateTime");
            }

            if (readParams.ToBookingDateTime is not null)
            {
                validQueryParameters.Add("toBookingDateTime");
            }

            // Get link queries
            apiResponse.Links.Self = GetLinkUrlQuery(
                apiResponse.Links.Self,
                apiRequestUrl,
                readParams,
                validQueryParameters);
            apiResponse.Links.First = GetLinkUrlQuery(
                apiResponse.Links.First,
                apiRequestUrl,
                readParams,
                validQueryParameters);
            apiResponse.Links.Prev = GetLinkUrlQuery(
                apiResponse.Links.Prev,
                apiRequestUrl,
                readParams,
                validQueryParameters);
            apiResponse.Links.Next = GetLinkUrlQuery(
                apiResponse.Links.Next,
                apiRequestUrl,
                readParams,
                validQueryParameters);
            apiResponse.Links.Last = GetLinkUrlQuery(
                apiResponse.Links.Last,
                apiRequestUrl,
                readParams,
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
                AccountAndTransactionApiVersion.Version3p1p9 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadTransaction6,
                    AccountAndTransactionModelsPublic.OBReadTransaction6>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };
    }
}
