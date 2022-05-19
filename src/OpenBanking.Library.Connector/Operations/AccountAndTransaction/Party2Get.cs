// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    internal class Party2Get : AccountAccessConsentExternalObject<Parties2Response,
        AccountAndTransactionModelsPublic.OBReadParty3>
    {
        public Party2Get(
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

        protected string RelativePath => "/parties";
        protected string RelativePath2 => "/parties";

        protected override Uri GetRequestUrl(
            string baseUrl,
            string? externalApiAccountId,
            string? externalApiStatementId,
            string? fromBookingDateTime,
            string? toBookingDateTime,
            string? page)
        {
            Uri endpointUrl =
                (externalAccountId: externalApiAccountId, externalStatementId: externalApiStatementId) switch
                {
                    (null, null) => new Uri(baseUrl + RelativePath),
                    ({ } extAccountId, null) => new Uri(baseUrl + $"/accounts/{extAccountId}" + RelativePath2),
                    ({ } extAccountId, { } extStatementId) => new Uri(
                        baseUrl + $"/accounts/{extAccountId}" + $"/statements/{extStatementId}" + RelativePath2),
                    _ => throw new ArgumentOutOfRangeException()
                };
            return endpointUrl;
        }

        protected override Parties2Response PublicGetResponse(
            AccountAndTransactionModelsPublic.OBReadParty3 apiResponse,
            Uri apiRequestUrl,
            string? requestUrlWithoutQuery)
        {
            // Get link queries
            apiResponse.Links.Self = GetLinkUrlQuery(apiResponse.Links.Self, apiRequestUrl, requestUrlWithoutQuery);
            apiResponse.Links.First = GetLinkUrlQuery(apiResponse.Links.First, apiRequestUrl, requestUrlWithoutQuery);
            apiResponse.Links.Prev = GetLinkUrlQuery(apiResponse.Links.Prev, apiRequestUrl, requestUrlWithoutQuery);
            apiResponse.Links.Next = GetLinkUrlQuery(apiResponse.Links.Next, apiRequestUrl, requestUrlWithoutQuery);
            apiResponse.Links.Last = GetLinkUrlQuery(apiResponse.Links.Last, apiRequestUrl, requestUrlWithoutQuery);

            return new Parties2Response(apiResponse);
        }

        protected override IApiGetRequests<AccountAndTransactionModelsPublic.OBReadParty3> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient) =>
            accountAndTransactionApi.AccountAndTransactionApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadParty3,
                    AccountAndTransactionModelsV3p1p7.OBReadParty3>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.Version3p1p9 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadParty3,
                    AccountAndTransactionModelsPublic.OBReadParty3>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };
    }
}
