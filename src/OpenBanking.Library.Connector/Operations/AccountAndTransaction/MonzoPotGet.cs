// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FluentValidation.Results;
using Newtonsoft.Json;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

public class MonzoPot
{
    public MonzoPot(
        string potId,
        string accountId,
        string name,
        string type,
        AccountAndTransactionModelsPublic.OBCreditDebitCode2Enum creditDebitIndicator,
        AccountAndTransactionModelsPublic.OBReadBalance1DataBalanceItemAmount balance,
        string style,
        AccountAndTransactionModelsPublic.OBReadBalance1DataBalanceItemAmount goal,
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
    public AccountAndTransactionModelsPublic.OBCreditDebitCode2Enum CreditDebitIndicator { get; }

    /// <summary> Amount of money of the cash balance. </summary>
    public AccountAndTransactionModelsPublic.OBReadBalance1DataBalanceItemAmount Balance { get; }

    public string Style { get; }

    public AccountAndTransactionModelsPublic.OBReadBalance1DataBalanceItemAmount Goal { get; }

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

internal class MonzoPotGet : IAccountAccessConsentExternalRead<MonzoPotsResponse, ExternalEntityReadParams>
{
    private readonly AccountAccessConsentCommon _accountAccessConsentCommon;
    private readonly IBankProfileService _bankProfileService;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;

    public MonzoPotGet(
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

    public async Task<(MonzoPotsResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ExternalEntityReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Get consent and associated data
        (AccountAccessConsent persistedConsent, BankRegistration bankRegistration,
                AccountAccessConsentAccessToken? storedAccessToken,
                AccountAccessConsentRefreshToken? storedRefreshToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(readParams.ConsentId, true);

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        AccountAndTransactionApi accountAndTransactionApi = bankProfile.GetRequiredAccountAndTransactionApi();
        TokenEndpointAuthMethod tokenEndpointAuthMethod =
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
                bankProfile.DefaultResponseMode,
                idTokenSubClaimType,
                customBehaviour?.RefreshTokenGrantPost,
                customBehaviour?.JwksGet,
                readParams.ModifiedBy);

        // Retrieve endpoint URL
        string urlString = readParams.ExternalApiAccountId switch
        {
            null => $"{accountAndTransactionApi.BaseUrl}/pots",
            { } extAccountId => $"{accountAndTransactionApi.BaseUrl}/accounts/{extAccountId}/pots"
        };
        Uri apiRequestUrl = new UriBuilder(urlString) { Query = readParams.QueryString ?? string.Empty }.Uri;

        // Get external object from bank API
        JsonSerializerSettings jsonSerializerSettings = ApiClient.GetDefaultJsonSerializerSettings;
        IApiGetRequests<ReadMonzoPot> apiRequests =
            accountAndTransactionApi.ApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                    ReadMonzoPot,
                    ReadMonzoPot>(new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.VersionPublic => new ApiGetRequests<
                    ReadMonzoPot,
                    ReadMonzoPot>(new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {accountAndTransactionApi.ApiVersion} not supported.")
            };
        (ReadMonzoPot apiResponse,
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
        var response = new MonzoPotsResponse(apiResponse, null);

        return (response, nonErrorMessages);
    }
}
