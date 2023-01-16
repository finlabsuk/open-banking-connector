// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class
    AccountAccessConsentOperations :
        IObjectCreate<AccountAccessConsentRequest, AccountAccessConsentCreateResponse, ConsentCreateParams>,
        IObjectRead<AccountAccessConsentCreateResponse, ConsentReadParams>
{
    private readonly AccountAccessConsentCommon _accountAccessConsentCommon;
    private readonly IDbReadOnlyEntityMethods<AccountAndTransactionApiEntity> _apiEntityMethods;

    private readonly IBankProfileService _bankProfileService;

    private readonly ConsentCommon<AccountAccessConsentPersisted,
        AccountAccessConsentRequest,
        AccountAccessConsentCreateResponse,
        OBReadConsent1,
        OBReadConsentResponse1> _consentCommon;

    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _entityMethods;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ITimeProvider _timeProvider;

    public AccountAccessConsentOperations(
        IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IGrantPost grantPost,
        IDbReadOnlyEntityMethods<AccountAndTransactionApiEntity> apiEntityMethods,
        IBankProfileService bankProfileService,
        IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods)
    {
        _entityMethods = entityMethods;
        _apiEntityMethods = apiEntityMethods;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _accountAccessConsentCommon = new AccountAccessConsentCommon(entityMethods, softwareStatementProfileRepo);
        _mapper = mapper;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _consentCommon =
            new ConsentCommon<AccountAccessConsentPersisted, AccountAccessConsentRequest,
                AccountAccessConsentCreateResponse,
                OBReadConsent1, OBReadConsentResponse1>(
                bankRegistrationMethods,
                instrumentationClient,
                softwareStatementProfileRepo);
    }

    private string ClientCredentialsGrantScope => "accounts";

    private string RelativePathBeforeId => "/account-access-consents";

    public async Task<(AccountAccessConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)>
        CreateAsync(
            AccountAccessConsentRequest request,
            ConsentCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Resolve bank profile
        BankProfile? bankProfile = null;
        if (request.BankProfile is not null)
        {
            bankProfile = _bankProfileService.GetBankProfile(request.BankProfile.Value);
        }

        // Determine entity ID
        var entityId = Guid.NewGuid();

        // Create new or use existing external API object
        OBReadConsentResponse1? externalApiResponse;
        string externalApiId;
        if (request.ExternalApiObject is null)
        {
            // Load BankRegistration and related
            (BankRegistration bankRegistration, string bankFinancialId, string tokenEndpoint,
                    ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
                await _consentCommon.GetBankRegistration(request.BankRegistrationId);

            // Load AccountAndTransactionApi
            AccountAndTransactionApiEntity accountAndTransactionApi =
                await GetAccountAndTransactionApi(request.AccountAndTransactionApiId, bankRegistration.BankId);

            // Get client credentials grant access token
            string ccGrantAccessToken =
                (await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    tokenEndpoint,
                    null,
                    processedSoftwareStatementProfile.ApiClient,
                    _instrumentationClient))
                .AccessToken;

            // Create new object at external API
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiPostRequests<OBReadConsent1, OBReadConsentResponse1> apiRequests =
                ApiRequests(
                    accountAndTransactionApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            var externalApiUrl = new Uri(accountAndTransactionApi.BaseUrl + RelativePathBeforeId);
            OBReadConsent1 externalApiRequest =
                AccountAccessConsentPublicMethods.ResolveExternalApiRequest(
                    request.ExternalApiRequest,
                    request.TemplateRequest,
                    bankProfile);
            (externalApiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.PostAsync(
                    externalApiUrl,
                    externalApiRequest,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Transform links
            externalApiId = externalApiResponse.Data.ConsentId;
            var apiGetRequestUrl = new Uri(externalApiUrl + $"/{externalApiId}");
            string? publicGetRequestUrlWithoutQuery = createParams.PublicRequestUrlWithoutQuery switch
            {
                { } x => x + $"/{entityId}",
                null => null
            };
            var validQueryParameters = new List<string>();
            var linksUrlOperations = new LinksUrlOperations(
                apiGetRequestUrl,
                publicGetRequestUrlWithoutQuery,
                true,
                validQueryParameters);
            //externalApiResponse.Links.Self = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Self);
            externalApiResponse.Links.First = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.First);
            externalApiResponse.Links.Prev = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Prev);
            externalApiResponse.Links.Next = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Next);
            externalApiResponse.Links.Last = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Last);
        }
        else
        {
            externalApiResponse = null;
            externalApiId = request.ExternalApiObject.ExternalApiId;
        }

        // Create persisted entity and return response
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var persistedConsent = new AccountAccessConsentPersisted(
            entityId,
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            null,
            0,
            utcNow,
            request.CreatedBy,
            null,
            request.BankRegistrationId,
            externalApiId,
            null,
            null,
            utcNow,
            request.CreatedBy,
            request.ExternalApiUserId,
            utcNow,
            request.CreatedBy,
            request.AccountAndTransactionApiId);

        AccessToken? accessToken = request.ExternalApiObject?.AccessToken;
        if (accessToken is not null)
        {
            persistedConsent.UpdateAccessToken(
                accessToken.Token,
                accessToken.ExpiresIn,
                accessToken.RefreshToken,
                utcNow,
                request.CreatedBy);
        }

        // Save entity
        await _entityMethods.AddAsync(persistedConsent);

        // Create response (may involve additional processing based on entity)
        var response =
            new AccountAccessConsentCreateResponse(
                persistedConsent.Id,
                persistedConsent.Created,
                persistedConsent.CreatedBy,
                persistedConsent.Reference,
                null,
                persistedConsent.BankRegistrationId,
                persistedConsent.ExternalApiId,
                persistedConsent.ExternalApiUserId,
                persistedConsent.AuthContextModified,
                persistedConsent.AuthContextModifiedBy,
                persistedConsent.AccountAndTransactionApiId,
                externalApiResponse);

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    public async
        Task<(AccountAccessConsentCreateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ConsentReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load AccountAccessConsent and related
        (AccountAccessConsentPersisted persistedConsent, string externalApiConsentId,
                AccountAndTransactionApiEntity accountAndTransactionApi, BankRegistration bankRegistration,
                string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile, string
                    bankTokenIssuerClaim) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(readParams.Id);

        bool includeExternalApiOperation =
            readParams.IncludeExternalApiOperation;
        OBReadConsentResponse1? externalApiResponse;
        if (includeExternalApiOperation)
        {
            // Get client credentials grant access token
            string ccGrantAccessToken =
                (await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    bankRegistration.BankNavigation.TokenEndpoint,
                    null,
                    processedSoftwareStatementProfile.ApiClient,
                    _instrumentationClient))
                .AccessToken;

            // Read object from external API
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiGetRequests<OBReadConsentResponse1> apiRequests =
                ApiRequests(
                    accountAndTransactionApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            var externalApiUrl = new Uri(
                accountAndTransactionApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
            (externalApiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.GetAsync(
                    externalApiUrl,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Transform links 
            var validQueryParameters = new List<string>();
            var linksUrlOperations = new LinksUrlOperations(
                externalApiUrl,
                readParams.PublicRequestUrlWithoutQuery,
                true,
                validQueryParameters);
            externalApiResponse.Links.Self = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Self);
            externalApiResponse.Links.First = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.First);
            externalApiResponse.Links.Prev = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Prev);
            externalApiResponse.Links.Next = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Next);
            externalApiResponse.Links.Last = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Last);
        }
        else
        {
            externalApiResponse = null;
        }

        // Create response
        var response =
            new AccountAccessConsentCreateResponse(
                persistedConsent.Id,
                persistedConsent.Created,
                persistedConsent.CreatedBy,
                persistedConsent.Reference,
                null,
                persistedConsent.BankRegistrationId,
                persistedConsent.ExternalApiId,
                persistedConsent.ExternalApiUserId,
                persistedConsent.AuthContextModified,
                persistedConsent.AuthContextModifiedBy,
                persistedConsent.AccountAndTransactionApiId,
                externalApiResponse);

        return (response, nonErrorMessages);
    }

    private async Task<AccountAndTransactionApiEntity> GetAccountAndTransactionApi(
        Guid accountAndTransactionApiId,
        Guid bankId)
    {
        AccountAndTransactionApiEntity accountAndTransactionApi =
            await _apiEntityMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == accountAndTransactionApiId) ??
            throw new KeyNotFoundException(
                $"No record found for AccountAndTransactionApi {accountAndTransactionApiId} specified by request.");

        if (accountAndTransactionApi.BankId != bankId)
        {
            throw new ArgumentException(
                "Specified AccountAndTransactionApi and BankRegistration objects do not share same BankId.");
        }

        return accountAndTransactionApi;
    }

    private IApiRequests<OBReadConsent1, OBReadConsentResponse1> ApiRequests(
        AccountAndTransactionApiVersion accountAndTransactionApiVersion,
        string bankFinancialId,
        string accessToken,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =>
        accountAndTransactionApiVersion switch
        {
            AccountAndTransactionApiVersion.Version3p1p7 => new ApiRequests<
                OBReadConsent1,
                OBReadConsentResponse1,
                BankApiModels.UkObRw.V3p1p7.Aisp.Models.OBReadConsent1,
                BankApiModels.UkObRw.V3p1p7.Aisp.Models.OBReadConsentResponse1>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken),
                new AccountAndTransactionPostRequestProcessor<
                    BankApiModels.UkObRw.V3p1p7.Aisp.Models.OBReadConsent1>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient)),
            AccountAndTransactionApiVersion.Version3p1p10 => new ApiRequests<
                OBReadConsent1,
                OBReadConsentResponse1,
                OBReadConsent1,
                OBReadConsentResponse1>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken),
                new AccountAndTransactionPostRequestProcessor<
                    OBReadConsent1>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient)),
            _ => throw new ArgumentOutOfRangeException(
                $"AISP API version {accountAndTransactionApiVersion} not supported.")
        };
}
