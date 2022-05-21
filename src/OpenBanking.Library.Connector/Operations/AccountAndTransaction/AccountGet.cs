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
    internal class AccountGet : AccountAccessConsentExternalObject<AccountsResponse,
        AccountAndTransactionModelsPublic.OBReadAccount6>
    {
        public AccountGet(
            IDbReadWriteEntityMethods<AccountAccessConsent> entityMethods,
            IInstrumentationClient instrumentationClient,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IApiVariantMapper mapper,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider) : base(
            entityMethods,
            instrumentationClient,
            softwareStatementProfileRepo,
            mapper,
            dbSaveChangesMethod,
            timeProvider) { }

        protected string RelativePath => "/accounts";
        protected string RelativePath2 => "";

        protected override Uri GetRequestUrl(
            string baseUrl,
            string? externalApiAccountId,
            string? externalApiStatementId,
            string? fromBookingDateTime,
            string? toBookingDateTime,
            string? page)
        {
            string endpointUrlBase =
                (externalAccountId: externalApiAccountId, externalStatementId: externalApiStatementId) switch
                {
                    (null, null) => baseUrl + RelativePath,
                    ({ } extAccountId, null) => baseUrl + $"/accounts/{extAccountId}" + RelativePath2,
                    ({ } extAccountId, { } extStatementId) =>
                        baseUrl + $"/accounts/{extAccountId}" + $"/statements/{extStatementId}" + RelativePath2,
                    _ => throw new ArgumentOutOfRangeException()
                };
            var uriBuilder = new UriBuilder(endpointUrlBase);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (page != null)
            {
                query["page"] = page;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        protected override AccountsResponse PublicGetResponse(
            AccountAndTransactionModelsPublic.OBReadAccount6 apiResponse,
            Uri apiRequestUrl,
            string? requestUrlWithoutQuery)
        {
            // Get link queries
            apiResponse.Links.Self = GetLinkUrlQuery(apiResponse.Links.Self, apiRequestUrl, requestUrlWithoutQuery);
            apiResponse.Links.First = GetLinkUrlQuery(apiResponse.Links.First, apiRequestUrl, requestUrlWithoutQuery);
            apiResponse.Links.Prev = GetLinkUrlQuery(apiResponse.Links.Prev, apiRequestUrl, requestUrlWithoutQuery);
            apiResponse.Links.Next = GetLinkUrlQuery(apiResponse.Links.Next, apiRequestUrl, requestUrlWithoutQuery);
            apiResponse.Links.Last = GetLinkUrlQuery(apiResponse.Links.Last, apiRequestUrl, requestUrlWithoutQuery);

            return new AccountsResponse(apiResponse);
        }

        protected override IApiGetRequests<AccountAndTransactionModelsPublic.OBReadAccount6> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient) =>
            accountAndTransactionApi.AccountAndTransactionApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadAccount6,
                    AccountAndTransactionModelsV3p1p7.OBReadAccount6>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.Version3p1p9 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadAccount6,
                    AccountAndTransactionModelsPublic.OBReadAccount6>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };
    }
}
