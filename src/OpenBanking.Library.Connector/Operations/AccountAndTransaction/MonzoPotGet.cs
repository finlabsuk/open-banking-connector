// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

public class MonzoPot
{
    public MonzoPot(
        string potId,
        string accountId,
        string name,
        string type,
        AccountAndTransactionModelsPublic.OBCreditDebitCode_2 creditDebitIndicator,
        AccountAndTransactionModelsPublic.Amount3 balance,
        string style,
        AccountAndTransactionModelsPublic.Amount3 goal,
        DateTimeOffset updated,
        DateTimeOffset created,
        string status)
    {
        PotId = potId;
        AccountId = accountId;
        Name = name;
        Type = type;
        CreditDebitIndicator = creditDebitIndicator;
        Balance = balance;
        Style = style;
        Goal = goal;
        Updated = updated;
        Created = created;
        Status = status;
    }

    public string PotId { get; }

    /// <summary>
    ///     A unique and immutable identifier used to identify the account resource. This identifier has no meaning to
    ///     the account owner.
    /// </summary>
    public string AccountId { get; }

    public string Name { get; }

    public string Type { get; }

    /// <summary>
    ///     Indicates whether the balance is a credit or a debit balance.
    ///     Usage: A zero balance is considered to be a credit balance.
    /// </summary>
    public AccountAndTransactionModelsPublic.OBCreditDebitCode_2 CreditDebitIndicator { get; }

    /// <summary> Amount of money of the cash balance. </summary>
    public AccountAndTransactionModelsPublic.Amount3 Balance { get; }

    public string Style { get; }

    public AccountAndTransactionModelsPublic.Amount3 Goal { get; }

    public DateTimeOffset Updated { get; }

    /// <summary>
    ///     Indicates the date (and time) of the balance.All dates in the JSON payloads are represented in ISO 8601 date-time
    ///     format.
    ///     All date-time fields in responses must include the timezone. An example is below:
    ///     2017-04-05T10:43:07+00:00
    /// </summary>
    public DateTimeOffset Created { get; }

    public string Status { get; }
}

public class MonzoPotData
{
    public MonzoPotData(IReadOnlyList<MonzoPot> pot)
    {
        Pot = pot;
    }

    public IReadOnlyList<MonzoPot> Pot { get; }
}

public class ReadMonzoPot : ISupportsValidation
{
    public ReadMonzoPot(
        MonzoPotData data,
        AccountAndTransactionModelsPublic.Links links,
        AccountAndTransactionModelsPublic.Meta meta)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        Data = data;
        Links = links;
        Meta = meta;
    }

    /// <summary> Gets the data. </summary>
    public MonzoPotData Data { get; }

    public AccountAndTransactionModelsPublic.Links Links { get; }

    /// <summary> Meta Data relevant to the payload. </summary>
    public AccountAndTransactionModelsPublic.Meta Meta { get; }

    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}

internal class
    MonzoPotGet : IAccountAccessConsentExternalRead<MonzoPotsResponse, AccountAccessConsentExternalReadParams>
{
    private readonly AccountAccessConsentCommon _accountAccessConsentCommon;
    private readonly IBankProfileService _bankProfileService;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;

    public MonzoPotGet(
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

    public async Task<(MonzoPotsResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(AccountAccessConsentExternalReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Get consent and associated data
        (AccountAccessConsent persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(readParams.ConsentId, true);

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi();
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        ReadWriteGetCustomBehaviour?
            readWriteGetCustomBehaviour = customBehaviour?.MonzoPotGet;
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
                persistedConsent.BankRegistrationNavigation.TokenEndpoint,
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
            null => $"{accountAndTransactionApi.BaseUrl}/pots",
            { } extAccountId => $"{accountAndTransactionApi.BaseUrl}/accounts/{extAccountId}/pots"
        };
        Uri externalApiUrl =
            new UriBuilder(urlStringWihoutQuery) { Query = readParams.QueryString ?? string.Empty }.Uri;

        // Get external object from bank API
        JsonSerializerSettings jsonSerializerSettings = ApiClient.GetDefaultJsonSerializerSettings;
        IApiGetRequests<ReadMonzoPot> apiRequests =
            accountAndTransactionApi.ApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p11 => new ApiGetRequests<
                    ReadMonzoPot,
                    ReadMonzoPot>(new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.VersionPublic => new ApiGetRequests<
                    ReadMonzoPot,
                    ReadMonzoPot>(new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.ApiVersion} not supported.")
            };
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription = readParams.ExternalApiAccountId is null
                ? "GET {AispBaseUrl}/pots"
                : "GET {AispBaseUrl}/accounts/{AccountId}/pots",
            BankProfile = bankProfile.BankProfileEnum
        };
        (ReadMonzoPot externalApiResponse, string? xFapiInteractionId,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.GetAsync(
                externalApiUrl,
                readParams.ExtraHeaders,
                tppReportingRequestInfo,
                jsonSerializerSettings,
                apiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Transform links 
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
            externalApiResponse.Links.Prev = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Prev);
        }
        if (externalApiResponse.Links.Next is not null)
        {
            externalApiResponse.Links.Next = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Next);
        }
        if (externalApiResponse.Links.Last is not null)
        {
            externalApiResponse.Links.Last = linksUrlOperations.ValidateAndTransformUrl(externalApiResponse.Links.Last);
        }

        // Create response
        var response = new MonzoPotsResponse
        {
            ExternalApiResponse = externalApiResponse,
            ExternalApiResponseInfo = new ExternalApiResponseInfo { XFapiInteractionId = xFapiInteractionId }
        };

        return (response, nonErrorMessages);
    }
}
