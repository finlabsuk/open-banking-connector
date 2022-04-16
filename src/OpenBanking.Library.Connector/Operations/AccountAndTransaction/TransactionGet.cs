// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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


namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction
{
    internal class
        TransactionGet : AccountAccessConsentExternalObject<TransactionsResponse,
            AccountAndTransactionModelsPublic.OBReadTransaction6>
    {
        public TransactionGet(
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

        protected override Uri RetrieveGetUrl(
            string baseUrl,
            string? externalApiAccountId,
            string? externalApiStatementId,
            string? fromBookingDateTime,
            string? toBookingDateTime)
        {
            string endpointUrlBase =
                (externalAccountId: externalApiAccountId, externalStatementId: externalApiStatementId) switch
                {
                    (null, null) => baseUrl + "/transactions",
                    ({ } extAccountId, null) => baseUrl + $"/accounts/{extAccountId}" + "/transactions",
                    ({ } extAccountId, { } extStatementId) =>
                        baseUrl + $"/accounts/{extAccountId}" + $"/statements/{extStatementId}" + "/transactions",
                    _ => throw new ArgumentOutOfRangeException()
                };
            var uriBuilder = new UriBuilder(endpointUrlBase);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (fromBookingDateTime != null)
            {
                query["fromBookingDateTime"] = fromBookingDateTime;
            }

            if (toBookingDateTime != null)
            {
                query["toBookingDateTime"] = toBookingDateTime;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        protected override TransactionsResponse PublicGetResponse(
            AccountAndTransactionModelsPublic.OBReadTransaction6 apiResponse)
        {
            int index = apiResponse.Links.Self.LastIndexOf("aisp", StringComparison.InvariantCulture);
            string newString = apiResponse.Links.Self.Substring(index);
            apiResponse.Links.Self = newString;
            return new TransactionsResponse(apiResponse);
        }

        protected override IApiGetRequests<AccountAndTransactionModelsPublic.OBReadTransaction6> ApiRequests(
            AccountAndTransactionApi accountAndTransactionApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient) =>
            accountAndTransactionApi?.AccountAndTransactionApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p9 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadTransaction6,
                    AccountAndTransactionModelsPublic.OBReadTransaction6
                >(new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                null => throw new NullReferenceException("No AISP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };
    }
}
