﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class StandingOrderGet : IAccountAccessConsentExternalRead<StandingOrdersResponse, ExternalEntityReadParams>
{
    private readonly AccountAccessConsentCommon _accountAccessConsentCommon;
    private readonly IBankProfileService _bankProfileService;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;

    public StandingOrderGet(
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        ConsentAccessTokenGet consentAccessTokenGet,
        AccountAccessConsentCommon accountAccessConsentCommon,
        IBankProfileService bankProfileService)
    {
        _instrumentationClient = instrumentationClient;
        _mapper = mapper;
        _consentAccessTokenGet = consentAccessTokenGet;
        _accountAccessConsentCommon = accountAccessConsentCommon;
        _bankProfileService = bankProfileService;
    }

    public async Task<(StandingOrdersResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ExternalEntityReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Get consent and associated data
        (AccountAccessConsent persistedConsent, BankRegistrationEntity bankRegistration,
                AccountAccessConsentAccessToken? storedAccessToken,
                AccountAccessConsentRefreshToken? storedRefreshToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(readParams.ConsentId, true);

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi();
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        string bankFinancialId = bankProfile.FinancialId;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;

        // Get access token
        string bankTokenIssuerClaim = AccountAccessConsentCommon.GetBankTokenIssuerClaim(
            customBehaviour,
            issuerUrl); // Get bank token issuer ("iss") claim
        string accessToken =
            await _consentAccessTokenGet.GetAccessTokenAndUpdateConsent(
                persistedConsent,
                bankTokenIssuerClaim,
                "openid accounts",
                bankRegistration,
                storedAccessToken,
                storedRefreshToken,
                tokenEndpointAuthMethod,
                persistedConsent.BankRegistrationNavigation.TokenEndpoint,
                supportsSca,
                idTokenSubClaimType,
                customBehaviour?.RefreshTokenGrantPost,
                customBehaviour?.JwksGet,
                readParams.ModifiedBy);

        // Retrieve endpoint URL
        string urlString = readParams.ExternalApiAccountId switch
        {
            null => $"{accountAndTransactionApi.BaseUrl}/standing-orders",
            { } extAccountId => $"{accountAndTransactionApi.BaseUrl}/accounts/{extAccountId}/standing-orders"
        };
        Uri apiRequestUrl = new UriBuilder(urlString) { Query = readParams.QueryString ?? string.Empty }.Uri;

        // Get external object from bank API
        JsonSerializerSettings jsonSerializerSettings = ApiClient.GetDefaultJsonSerializerSettings;
        IApiGetRequests<AccountAndTransactionModelsPublic.OBReadStandingOrder6> apiRequests =
            accountAndTransactionApi.ApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadStandingOrder6,
                    AccountAndTransactionModelsV3p1p7.OBReadStandingOrder6>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.VersionPublic => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadStandingOrder6,
                    AccountAndTransactionModelsPublic.OBReadStandingOrder6>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.ApiVersion} not supported.")
            };
        (AccountAndTransactionModelsPublic.OBReadStandingOrder6 apiResponse,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.GetAsync(
                apiRequestUrl,
                jsonSerializerSettings,
                processedSoftwareStatementProfile.ApiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Create response
        var validQueryParameters = new List<string>();

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
        var response = new StandingOrdersResponse(apiResponse, null);

        return (response, nonErrorMessages);
    }
}
