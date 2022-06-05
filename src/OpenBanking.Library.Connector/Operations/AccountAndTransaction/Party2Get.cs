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
        AccountAndTransactionModelsPublic.OBReadParty3, ExternalEntityReadParams>
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

        protected override Uri GetApiRequestUrl(
            string baseUrl,
            ExternalEntityReadParams readParams)
        {
            string urlString = readParams.ExternalApiAccountId switch
            {
                null => throw new ArgumentOutOfRangeException(),
                { } extAccountId => $"{baseUrl}/accounts/{extAccountId}/parties"
            };
            return new UriBuilder(urlString)
            {
                Query = readParams.QueryString ?? string.Empty
            }.Uri;
        }

        protected override Parties2Response PublicGetResponse(
            AccountAndTransactionModelsPublic.OBReadParty3 apiResponse,
            Uri apiRequestUrl,
            ExternalEntityReadParams readParams)
        {
            var validQueryParameters = new List<string>();

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
