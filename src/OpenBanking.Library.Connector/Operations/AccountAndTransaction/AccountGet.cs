// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using Newtonsoft.Json;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class AccountGet : IAccountAccessConsentExternalRead<AccountsResponse, AccountAccessConsentExternalReadParams>
{
    private readonly AccountAccessConsentCommon _accountAccessConsentCommon;
    private readonly IBankProfileService _bankProfileService;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;

    public AccountGet(
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        ConsentAccessTokenGet consentAccessTokenGet,
        AccountAccessConsentCommon accountAccessConsentCommon,
        IBankProfileService bankProfileService,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods)
    {
        _instrumentationClient = instrumentationClient;
        _mapper = mapper;
        _consentAccessTokenGet = consentAccessTokenGet;
        _accountAccessConsentCommon = accountAccessConsentCommon;
        _bankProfileService = bankProfileService;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
    }

    public async Task<(AccountsResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(AccountAccessConsentExternalReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Get consent and associated data
        (AccountAccessConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(readParams.ConsentId, true);

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        bool aispUseV4 = bankRegistration.AispUseV4;
        AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi(aispUseV4);
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        ReadWriteGetCustomBehaviour?
            readWriteGetCustomBehaviour = customBehaviour?.AccountGet;
        string bankFinancialId = bankProfile.FinancialId;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;

        // Get IApiClient
        IApiClient apiClient = bankRegistration.UseSimulatedBank
            ? bankProfile.ReplayApiClient
            : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Get access token
        string bankTokenIssuerClaim = AccountAccessConsentCommon.GetBankTokenIssuerClaim(
            customBehaviour,
            issuerUrl); // Get bank token issuer ("iss") claim
        string accessToken =
            await _consentAccessTokenGet.GetAccessTokenAndUpdateConsent(
                persistedConsent,
                bankTokenIssuerClaim,
                "accounts",
                bankRegistration,
                _accountAccessConsentCommon.GetAccessToken,
                _accountAccessConsentCommon.GetRefreshToken,
                externalApiSecret,
                bankProfile.UseOpenIdConnect,
                apiClient,
                obSealKey,
                supportsSca,
                bankProfile.BankProfileEnum,
                idTokenSubClaimType,
                customBehaviour?.AccountAccessConsentRefreshTokenGrantPost,
                customBehaviour?.JwksGet,
                readParams.ModifiedBy);

        // Retrieve endpoint URL
        string urlStringWihoutQuery = readParams.ExternalApiAccountId switch
        {
            null => $"{accountAndTransactionApi.BaseUrl}/accounts",
            { } extAccountId => $"{accountAndTransactionApi.BaseUrl}/accounts/{extAccountId}"
        };
        Uri externalApiUrl =
            new UriBuilder(urlStringWihoutQuery) { Query = readParams.QueryString ?? string.Empty }.Uri;

        // Get external object from bank API
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription = readParams.ExternalApiAccountId is null
                ? "GET {AispBaseUrl}/accounts"
                : "GET {AispBaseUrl}/accounts/{AccountId}",
            BankProfile = bankProfile.BankProfileEnum
        };
        JsonSerializerSettings? jsonSerializerSettings = null;
        AccountAndTransactionModelsPublic.OBReadAccount6 externalApiResponse;
        string? xFapiInteractionId;
        IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
        switch (accountAndTransactionApi.ApiVersion)
        {
            case AccountAndTransactionApiVersion.Version3p1p11:
                IApiGetRequests<AccountAndTransactionModelsV3p1p11.OBReadAccount6> apiRequestsV3 =
                    new ApiGetRequests<AccountAndTransactionModelsV3p1p11.OBReadAccount6,
                        AccountAndTransactionModelsV3p1p11.OBReadAccount6>(
                        new ApiGetRequestProcessor(bankFinancialId, accessToken));
                (AccountAndTransactionModelsV3p1p11.OBReadAccount6 externalApiResponseV3, xFapiInteractionId,
                        newNonErrorMessages) =
                    await apiRequestsV3.GetAsync(
                        externalApiUrl,
                        readParams.ExtraHeaders,
                        tppReportingRequestInfo,
                        jsonSerializerSettings,
                        apiClient,
                        _mapper);
                externalApiResponse =
                    AccountAndTransactionModelsPublic.Mappings.MapToOBReadAccount6(externalApiResponseV3);
                break;
            case AccountAndTransactionApiVersion.VersionPublic:
                IApiGetRequests<AccountAndTransactionModelsPublic.OBReadAccount6> apiRequests =
                    new ApiGetRequests<AccountAndTransactionModelsPublic.OBReadAccount6,
                        AccountAndTransactionModelsPublic.OBReadAccount6>(
                        new ApiGetRequestProcessor(bankFinancialId, accessToken));
                (externalApiResponse, xFapiInteractionId, newNonErrorMessages) =
                    await apiRequests.GetAsync(
                        externalApiUrl,
                        readParams.ExtraHeaders,
                        tppReportingRequestInfo,
                        jsonSerializerSettings,
                        apiClient,
                        _mapper);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.ApiVersion} not supported.");
        }
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Transform links 
        if (externalApiResponse.Links is not null)
        {
            string? transformedLinkUrlWithoutQuery = readParams.PublicRequestUrlWithoutQuery;
            var expectedLinkUrlWithoutQuery = new Uri(urlStringWihoutQuery);
            var linksUrlOperations = LinksUrlOperations.CreateLinksUrlOperations(
                LinksUrlOperations.GetMethodExpectedLinkUrls(expectedLinkUrlWithoutQuery, readWriteGetCustomBehaviour),
                transformedLinkUrlWithoutQuery,
                readWriteGetCustomBehaviour?.ResponseLinksMayHaveIncorrectUrlBeforeQuery ?? false,
                true);
            externalApiResponse.Links.Self = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Self);
            if (externalApiResponse.Links.First is not null)
            {
                externalApiResponse.Links.First =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.First);
            }
            if (externalApiResponse.Links.Prev is not null)
            {
                externalApiResponse.Links.Prev =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Prev);
            }
            if (externalApiResponse.Links.Next is not null)
            {
                externalApiResponse.Links.Next =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Next);
            }
            if (externalApiResponse.Links.Last is not null)
            {
                externalApiResponse.Links.Last =
                    linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Last);
            }
        }

        // Create response
        var response = new AccountsResponse
        {
            ExternalApiResponse = externalApiResponse,
            ExternalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId }
        };

        return (response, nonErrorMessages);
    }
}
