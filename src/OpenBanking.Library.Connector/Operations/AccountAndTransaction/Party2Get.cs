// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

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
            string? publicRequestUrlWithoutQuery,
            bool allowValidQueryParametersOnly,
            ExternalEntityReadParams readParams)
        {
            var validQueryParameters = new List<string>();

            // Get link queries
            var linksUrlOperations = new LinksUrlOperations(
                apiRequestUrl,
                publicRequestUrlWithoutQuery,
                allowValidQueryParametersOnly,
                validQueryParameters);
            apiResponse.Links.Self = linksUrlOperations.TransformLinksUrl(apiResponse.Links.Self);
            apiResponse.Links.First = linksUrlOperations.TransformLinksUrl(apiResponse.Links.First);
            apiResponse.Links.Prev = linksUrlOperations.TransformLinksUrl(apiResponse.Links.Prev);
            apiResponse.Links.Next = linksUrlOperations.TransformLinksUrl(apiResponse.Links.Next);
            apiResponse.Links.Last = linksUrlOperations.TransformLinksUrl(apiResponse.Links.Last);

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
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.Version3p1p10 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadParty3,
                    AccountAndTransactionModelsV3p1p10.OBReadParty3>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };
    }
}
