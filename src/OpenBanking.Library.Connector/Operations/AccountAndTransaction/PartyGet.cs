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
    internal class PartyGet : AccountAccessConsentExternalObject<PartiesResponse,
        AccountAndTransactionModelsPublic.OBReadParty2>
    {
        public PartyGet(
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

        protected override string GetApiBaseRequestUrl(
            string baseUrl,
            string? externalApiAccountId,
            string? externalApiStatementId) =>
            (externalAccountId: externalApiAccountId, externalStatementId: externalApiStatementId) switch
            {
                (null, null) => baseUrl + "/party",
                ({ } extAccountId, null) => baseUrl + $"/accounts/{extAccountId}" + "/party",
                ({ } extAccountId, { } extStatementId) =>
                    baseUrl + $"/accounts/{extAccountId}" + $"/statements/{extStatementId}" + "/party",
                _ => throw new ArgumentOutOfRangeException()
            };

        protected override PartiesResponse PublicGetResponse(
            AccountAndTransactionModelsPublic.OBReadParty2 apiResponse,
            Uri apiRequestUrl,
            string? requestUrlWithoutQuery,
            bool supportAllQueryParameters,
            IList<string> validQueryParameters)
        {
            // Get link queries
            apiResponse.Links.Self = GetLinkUrlQuery(
                apiResponse.Links.Self,
                apiRequestUrl,
                requestUrlWithoutQuery,
                supportAllQueryParameters,
                validQueryParameters);
            apiResponse.Links.First = GetLinkUrlQuery(
                apiResponse.Links.First,
                apiRequestUrl,
                requestUrlWithoutQuery,
                supportAllQueryParameters,
                validQueryParameters);
            apiResponse.Links.Prev = GetLinkUrlQuery(
                apiResponse.Links.Prev,
                apiRequestUrl,
                requestUrlWithoutQuery,
                supportAllQueryParameters,
                validQueryParameters);
            apiResponse.Links.Next = GetLinkUrlQuery(
                apiResponse.Links.Next,
                apiRequestUrl,
                requestUrlWithoutQuery,
                supportAllQueryParameters,
                validQueryParameters);
            apiResponse.Links.Last = GetLinkUrlQuery(
                apiResponse.Links.Last,
                apiRequestUrl,
                requestUrlWithoutQuery,
                supportAllQueryParameters,
                validQueryParameters);

            return new PartiesResponse(apiResponse);
        }

        protected override IApiGetRequests<AccountAndTransactionModelsPublic.OBReadParty2> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient) =>
            accountAndTransactionApi.AccountAndTransactionApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadParty2,
                    AccountAndTransactionModelsV3p1p7.OBReadParty2>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.Version3p1p9 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadParty2,
                    AccountAndTransactionModelsPublic.OBReadParty2>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };
    }
}
