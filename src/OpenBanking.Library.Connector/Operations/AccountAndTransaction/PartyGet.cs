// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class PartyGet : IAccountAccessConsentExternalRead<PartiesResponse, ExternalEntityReadParams>
{
    protected AuthContextAccessTokenGet _authContextAccessTokenGet;
    protected IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _entityMethods;
    protected IGrantPost _grantPost;
    protected IInstrumentationClient _instrumentationClient;
    protected IApiVariantMapper _mapper;
    protected IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
    private ITimeProvider _timeProvider;

    public PartyGet(
        IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods,
        IInstrumentationClient instrumentationClient,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IApiVariantMapper mapper,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IGrantPost grantPost,
        AuthContextAccessTokenGet authContextAccessTokenGet)
    {
        _entityMethods = entityMethods;
        _instrumentationClient = instrumentationClient;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
        _mapper = mapper;
        _timeProvider = timeProvider;
        _grantPost = grantPost;
        _authContextAccessTokenGet = authContextAccessTokenGet;
    }

    public async Task<(PartiesResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ExternalEntityReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Get consent and associated data
        AccountAccessConsentPersisted persistedConsent =
            await _entityMethods
                .DbSet
                .Include(o => o.AccountAccessConsentAuthContextsNavigation)
                .Include(o => o.AccountAndTransactionApiNavigation)
                .Include(o => o.BankRegistrationNavigation)
                .Include(o => o.BankRegistrationNavigation.BankNavigation)
                .SingleOrDefaultAsync(x => x.Id == readParams.ConsentId) ??
            throw new KeyNotFoundException(
                $"No record found for Account Access Consent with ID {readParams.ConsentId}.");
        AccountAndTransactionApiEntity accountAndTransactionApiEntity =
            persistedConsent.AccountAndTransactionApiNavigation;
        var accountAndTransactionApi = new AccountAndTransactionApi
        {
            AccountAndTransactionApiVersion = accountAndTransactionApiEntity.ApiVersion,
            BaseUrl = accountAndTransactionApiEntity.BaseUrl
        };
        BankRegistration bankRegistration = persistedConsent.BankRegistrationNavigation;
        string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

        // Get API client
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                bankRegistration.SoftwareStatementProfileId,
                bankRegistration.SoftwareStatementProfileOverride);
        IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

        // Get access token
        string? requestObjectAudClaim =
            persistedConsent.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                ?.AccountAccessConsentAuthGet
                ?.AudClaim;
        string bankIssuerUrl =
            requestObjectAudClaim ??
            bankRegistration.BankNavigation.IssuerUrl ??
            throw new Exception("Cannot determine issuer URL for bank");
        string accessToken =
            await _authContextAccessTokenGet.GetAccessTokenAndUpdateConsent(
                persistedConsent,
                bankIssuerUrl,
                "openid accounts",
                bankRegistration,
                persistedConsent.BankRegistrationNavigation.BankNavigation.TokenEndpoint,
                readParams.ModifiedBy);

        // Retrieve endpoint URL
        string urlString = readParams.ExternalApiAccountId switch
        {
            null => $"{accountAndTransactionApi.BaseUrl}/party",
            ( { } extAccountId) => $"{accountAndTransactionApi.BaseUrl}/accounts/{extAccountId}/party",
        };
        Uri apiRequestUrl = new UriBuilder(urlString)
        {
            Query = readParams.QueryString ?? string.Empty
        }.Uri;

        // Get external object from bank API
        JsonSerializerSettings? jsonSerializerSettings = null;
        IApiGetRequests<OBReadParty2> apiRequests = accountAndTransactionApi.AccountAndTransactionApiVersion switch
        {
            AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                OBReadParty2,
                BankApiModels.UkObRw.V3p1p7.Aisp.Models.OBReadParty2>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken)),
            AccountAndTransactionApiVersion.Version3p1p10 => new ApiGetRequests<
                OBReadParty2,
                OBReadParty2>(new ApiGetRequestProcessor(bankFinancialId, accessToken)),
            _ => throw new ArgumentOutOfRangeException(
                $"AISP API version {accountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
        };
        OBReadParty2 apiResponse;
        IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
        (apiResponse, newNonErrorMessages) =
            await apiRequests.GetAsync(
                apiRequestUrl,
                jsonSerializerSettings,
                apiClient,
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
        apiResponse.Links.Self = linksUrlOperations.TransformLinksUrl(apiResponse.Links.Self);
        apiResponse.Links.First = linksUrlOperations.TransformLinksUrl(apiResponse.Links.First);
        apiResponse.Links.Prev = linksUrlOperations.TransformLinksUrl(apiResponse.Links.Prev);
        apiResponse.Links.Next = linksUrlOperations.TransformLinksUrl(apiResponse.Links.Next);
        apiResponse.Links.Last = linksUrlOperations.TransformLinksUrl(apiResponse.Links.Last);
        var response = new PartiesResponse(apiResponse);

        return (response, nonErrorMessages);
    }
}
