// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        IObjectRead<AccountAccessConsentReadResponse, ConsentReadParams>
{
    private readonly IDbReadOnlyEntityMethods<AccountAndTransactionApiEntity> _bankApiSetMethods;

    private readonly IBankProfileService _bankProfileService;

    private readonly ConsentCommon<AccountAccessConsentPersisted,
        AccountAccessConsentRequest,
        AccountAccessConsentCreateResponse,
        AccountAndTransactionModelsPublic.OBReadConsent1,
        AccountAndTransactionModelsPublic.OBReadConsentResponse1> _consentCommon;

    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _entityMethods;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
    private readonly ITimeProvider _timeProvider;

    public AccountAccessConsentOperations(
        IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IGrantPost grantPost,
        IDbReadOnlyEntityMethods<AccountAndTransactionApiEntity> bankApiSetMethods,
        IBankProfileService bankProfileService,
        IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods)
    {
        _bankApiSetMethods = bankApiSetMethods;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
        _entityMethods = entityMethods;
        _mapper = mapper;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
        _instrumentationClient = instrumentationClient;
        _consentCommon =
            new ConsentCommon<AccountAccessConsentPersisted, AccountAccessConsentRequest, AccountAccessConsentCreateResponse,
                AccountAndTransactionModelsPublic.OBReadConsent1, AccountAndTransactionModelsPublic.OBReadConsentResponse1>(
                bankRegistrationMethods,
                instrumentationClient,
                softwareStatementProfileRepo);
    }

    protected string ClientCredentialsGrantScope => "accounts";

    protected string RelativePathBeforeId => "/account-access-consents";

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
        AccountAndTransactionModelsPublic.OBReadConsentResponse1? externalApiResponse;
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
            IApiPostRequests<AccountAndTransactionModelsPublic.OBReadConsent1, AccountAndTransactionModelsPublic.OBReadConsentResponse1> apiRequests =
                ApiRequests(
                    accountAndTransactionApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            var externalApiUrl = new Uri(accountAndTransactionApi.BaseUrl + RelativePathBeforeId);
            AccountAndTransactionModelsPublic.OBReadConsent1 externalApiRequest =
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
            externalApiResponse.Links.Self = linksUrlOperations.TransformLinksUrl(externalApiResponse.Links.Self);
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
                persistedConsent.AccountAndTransactionApiId,
                persistedConsent.ExternalApiId,
                externalApiResponse);

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    public async
        Task<(AccountAccessConsentReadResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(ConsentReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load AccountAccessConsent and related
        (AccountAccessConsentPersisted persistedConsent, string externalApiConsentId,
                AccountAndTransactionApiEntity accountAndTransactionApi, BankRegistration bankRegistration,
                string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await GetAccountAccessConsent(readParams.Id);

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
        IApiGetRequests<AccountAndTransactionModelsPublic.OBReadConsentResponse1> apiRequests =
            ApiRequests(
                accountAndTransactionApi.ApiVersion,
                bankFinancialId,
                ccGrantAccessToken,
                processedSoftwareStatementProfile);
        var externalApiUrl = new Uri(
            accountAndTransactionApi.BaseUrl + RelativePathBeforeId + $"/{externalApiConsentId}");
        (AccountAndTransactionModelsPublic.OBReadConsentResponse1 externalApiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
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

        // Create response
        var response =
            new AccountAccessConsentReadResponse(
            persistedConsent.Id,
            persistedConsent.Created,
            persistedConsent.CreatedBy,
            persistedConsent.Reference,
            null,
            persistedConsent.BankRegistrationId,
            persistedConsent.AccountAndTransactionApiId,
            persistedConsent.ExternalApiId,
            externalApiResponse);

        return (response, nonErrorMessages);
    }

    private async Task<AccountAndTransactionApiEntity> GetAccountAndTransactionApi(
        Guid accountAndTransactionApiId,
        Guid bankId)
    {
        AccountAndTransactionApiEntity accountAndTransactionApi =
            await _bankApiSetMethods
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

    private async Task<(AccountAccessConsentPersisted persistedConsent, string externalApiConsentId,
        AccountAndTransactionApiEntity accountAndTransactionApi, BankRegistration bankRegistration, string
        bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile)> GetAccountAccessConsent(
        Guid consentId)
    {
        AccountAccessConsentPersisted persistedConsent =
            await _entityMethods
                .DbSetNoTracking
                .Include(o => o.AccountAccessConsentAuthContextsNavigation)
                .Include(o => o.AccountAndTransactionApiNavigation)
                .Include(o => o.BankRegistrationNavigation.BankNavigation)
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Account Access Consent with ID {consentId}.");
        string externalApiConsentId = persistedConsent.ExternalApiId;
        AccountAndTransactionApiEntity accountAndTransactionApi =
            persistedConsent.AccountAndTransactionApiNavigation;
        BankRegistration bankRegistration = persistedConsent.BankRegistrationNavigation;
        string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

        // Get software statement profile
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                bankRegistration.SoftwareStatementProfileId,
                bankRegistration.SoftwareStatementProfileOverride);
        return (persistedConsent, externalApiConsentId, accountAndTransactionApi, bankRegistration, bankFinancialId,
            processedSoftwareStatementProfile);
    }

    private IApiRequests<AccountAndTransactionModelsPublic.OBReadConsent1, AccountAndTransactionModelsPublic.OBReadConsentResponse1> ApiRequests(
        AccountAndTransactionApiVersion accountAndTransactionApiVersion,
        string bankFinancialId,
        string accessToken,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =>
        accountAndTransactionApiVersion switch
        {
            AccountAndTransactionApiVersion.Version3p1p7 => new ApiRequests<
                AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1,
                BankApiModels.UkObRw.V3p1p7.Aisp.Models.OBReadConsent1,
                BankApiModels.UkObRw.V3p1p7.Aisp.Models.OBReadConsentResponse1>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken),
                new AccountAndTransactionPostRequestProcessor<
                    BankApiModels.UkObRw.V3p1p7.Aisp.Models.OBReadConsent1>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient)),
            AccountAndTransactionApiVersion.Version3p1p10 => new ApiRequests<
                AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1,
                AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1>(
                new ApiGetRequestProcessor(bankFinancialId, accessToken),
                new AccountAndTransactionPostRequestProcessor<
                    AccountAndTransactionModelsPublic.OBReadConsent1>(
                    bankFinancialId,
                    accessToken,
                    _instrumentationClient)),
            _ => throw new ArgumentOutOfRangeException(
                $"AISP API version {accountAndTransactionApiVersion} not supported.")
        };
}
