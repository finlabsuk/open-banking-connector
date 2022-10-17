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
    internal class BalanceGet : AccountAccessConsentExternalObject<BalancesResponse,
        AccountAndTransactionModelsPublic.OBReadBalance1, ExternalEntityReadParams>
    {
        public BalanceGet(
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
                null => $"{baseUrl}/balances",
                { } extAccountId => $"{baseUrl}/accounts/{extAccountId}/balances",
            };
            return new UriBuilder(urlString)
            {
                Query = readParams.QueryString ?? string.Empty
            }.Uri;
        }

        protected override BalancesResponse PublicGetResponse(
            AccountAndTransactionModelsPublic.OBReadBalance1 apiResponse,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery,
            bool allowValidQueryParametersOnly,
            ExternalEntityReadParams readParams)
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

            return new BalancesResponse(apiResponse);
        }

        protected override IApiGetRequests<AccountAndTransactionModelsPublic.OBReadBalance1> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient) =>
            accountAndTransactionApi.AccountAndTransactionApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadBalance1,
                    AccountAndTransactionModelsV3p1p7.OBReadBalance1>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.Version3p1p10 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadBalance1,
                    AccountAndTransactionModelsV3p1p10.OBReadBalance1>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };
    }
}
