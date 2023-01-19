// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using Newtonsoft.Json;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class
    TransactionGet : IAccountAccessConsentExternalRead<TransactionsResponse, TransactionsReadParams>
{
    private readonly AccountAccessConsentCommon _accountAccessConsentCommon;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;

    public TransactionGet(
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        ConsentAccessTokenGet consentAccessTokenGet,
        AccountAccessConsentCommon accountAccessConsentCommon)
    {
        _instrumentationClient = instrumentationClient;
        _mapper = mapper;
        _consentAccessTokenGet = consentAccessTokenGet;
        _accountAccessConsentCommon = accountAccessConsentCommon;
    }

    public async Task<(TransactionsResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(TransactionsReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Get consent and associated data
        (AccountAccessConsentPersisted persistedConsent, string externalApiConsentId,
                AccountAndTransactionApiEntity accountAndTransactionApi, BankRegistrationPersisted bankRegistration,
                string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile, string
                    bankTokenIssuerClaim) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(readParams.ConsentId, true);

        // Get access token
        string accessToken =
            await _consentAccessTokenGet.GetAccessTokenAndUpdateConsent(
                persistedConsent,
                bankTokenIssuerClaim,
                "openid accounts",
                bankRegistration,
                persistedConsent.BankRegistrationNavigation.BankNavigation.TokenEndpoint,
                readParams.ModifiedBy);

        // Retrieve endpoint URL
        string urlString = (externalAccountId: readParams.ExternalApiAccountId,
                externalStatementId: readParams.ExternalApiStatementId) switch
            {
                (null, null) => $"{accountAndTransactionApi.BaseUrl}/transactions",
                ({ } extAccountId, null) =>
                    $"{accountAndTransactionApi.BaseUrl}/accounts/{extAccountId}/transactions",
                ({ } extAccountId, { } extStatementId) =>
                    $"{accountAndTransactionApi.BaseUrl}/accounts/{extAccountId}/statements/{extStatementId}/transactions",
                _ => throw new ArgumentOutOfRangeException()
            };
        Uri apiRequestUrl = new UriBuilder(urlString)
        {
            Query = readParams.QueryString ?? string.Empty
        }.Uri;

        // Get external object from bank API
        JsonSerializerSettings? jsonSerializerSettings = null;
        IApiGetRequests<OBReadTransaction6> apiRequests =
            accountAndTransactionApi.ApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                    OBReadTransaction6,
                    BankApiModels.UkObRw.V3p1p7.Aisp.Models.OBReadTransaction6>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.Version3p1p10 => new ApiGetRequests<
                    OBReadTransaction6,
                    OBReadTransaction6>(new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.ApiVersion} not supported.")
            };
        (OBReadTransaction6 apiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.GetAsync(
                apiRequestUrl,
                jsonSerializerSettings,
                processedSoftwareStatementProfile.ApiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Create response
        var validQueryParameters = new List<string>();

        // Get link queries
        var linksUrlOperations = new LinksUrlOperations(
            apiRequestUrl,
            readParams.PublicRequestUrlWithoutQuery,
            false,
            validQueryParameters);
        apiResponse.Links.Self = linksUrlOperations.ValidateAndTransformUrl(apiResponse.Links.Self);
        apiResponse.Links.First = linksUrlOperations.ValidateAndTransformUrl(apiResponse.Links.First);
        apiResponse.Links.Prev = linksUrlOperations.ValidateAndTransformUrl(apiResponse.Links.Prev);
        apiResponse.Links.Next = linksUrlOperations.ValidateAndTransformUrl(apiResponse.Links.Next);
        apiResponse.Links.Last = linksUrlOperations.ValidateAndTransformUrl(apiResponse.Links.Last);
        var response = new TransactionsResponse(apiResponse, null);

        return (response, nonErrorMessages);
    }
}
