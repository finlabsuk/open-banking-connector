// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Obie;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Jose;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Bank = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.Bank;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;
using BankRegistrationRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request.BankRegistration;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using ClientRegistrationModelsV3p2 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p2.Models;
using ClientRegistrationModelsV3p1 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;

internal class
    BankRegistrationOperations :
    IObjectCreate<BankRegistrationRequest, BankRegistrationResponse, BankRegistrationCreateParams>,
    IObjectRead<BankRegistrationResponse, BankRegistrationReadParams>
{
    private readonly IDbReadOnlyEntityMethods<Bank> _bankMethods;
    private readonly IBankProfileService _bankProfileService;
    private readonly IOpenIdConfigurationRead _configurationRead;
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IDbReadWriteEntityMethods<BankRegistrationPersisted> _entityMethods;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
    private readonly ITimeProvider _timeProvider;

    public BankRegistrationOperations(
        IDbReadWriteEntityMethods<BankRegistrationPersisted> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IOpenIdConfigurationRead configurationRead,
        IDbReadOnlyEntityMethods<Bank> bankMethods,
        IBankProfileService bankProfileService,
        IGrantPost grantPost)
    {
        _bankMethods = bankMethods;
        _bankProfileService = bankProfileService;
        _grantPost = grantPost;
        _configurationRead = configurationRead;
        _entityMethods = entityMethods;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
        _instrumentationClient = instrumentationClient;
        _mapper = mapper;
    }

    public async Task<(BankRegistrationResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(BankRegistrationRequest request, BankRegistrationCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(request.BankProfile);
        BankGroupEnum bankGroup = BankProfileService.GetBankGroupEnum(bankProfile.BankProfileEnum);
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        string bankFinancialId = bankProfile.FinancialId;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
            bankProfile.DynamicClientRegistrationApiVersion;
        bool useSimulatedBank = request.UseSimulatedBank;

        // Load processed software statement profile
        string softwareStatementProfileId = request.SoftwareStatementProfileId;
        string? softwareStatementProfileOverrideCase = request.SoftwareStatementProfileOverrideCase;
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                softwareStatementProfileId,
                softwareStatementProfileOverrideCase);

        // Get and process software statement assertion
        string softwareStatementAssertion = await GetSoftwareStatementAssertion(processedSoftwareStatementProfile);
        SsaPayload ssaPayload = ProcessSsa(
            softwareStatementAssertion,
            processedSoftwareStatementProfile,
            softwareStatementProfileId);

        // Determine redirect URIs
        (IList<string> redirectUris, string defaultFragmentRedirectUri, string defaultQueryRedirectUri) =
            GetRedirectUris(
                processedSoftwareStatementProfile,
                ssaPayload,
                request.DefaultFragmentRedirectUri,
                request.DefaultQueryRedirectUri,
                request.RedirectUris);

        // Determine registration scope
        RegistrationScopeEnum registrationScope =
            request.RegistrationScope ??
            ssaPayload.RegistrationScope;
        RegistrationScopeIsValid registrationScopeIsValidFcn =
            bankProfile.BankConfigurationApiSettings.RegistrationScopeIsValid;
        if (!registrationScopeIsValidFcn(registrationScope))
        {
            throw new ArgumentException(
                $"Request property {nameof(request.RegistrationScope)} or default value from SoftwareStatementProfile fails RegistrationScopeIsValid check from BankProfile");
        }

        // Get OpenID Provider Configuration if available
        (OpenIdConfiguration? openIdConfiguration,
                IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages1) =
            await _configurationRead.GetOpenIdConfigurationAsync(
                issuerUrl,
                customBehaviour?.OpenIdConfigurationGet);
        nonErrorMessages.AddRange(newNonErrorMessages1);

        // Determine registration endpoint
        string? registrationEndpoint =
            request.RegistrationEndpoint ??
            openIdConfiguration?.RegistrationEndpoint;
        bool allowNullRegistrationEndpoint =
            bankProfile.BankConfigurationApiSettings.AllowNullRegistrationEndpoint;
        if (registrationEndpoint is null &&
            !allowNullRegistrationEndpoint)
        {
            throw new ArgumentNullException(
                nameof(request.RegistrationEndpoint),
                $"{nameof(request.RegistrationEndpoint)} specified as null and not obtainable from OpenID Configuration for bank's IssuerUrl. " +
                $"Additionally AllowNullRegistrationEndpoint from BankProfile is false.");
        }

        // Determine token endpoint
        string tokenEndpoint =
            request.TokenEndpoint ??
            openIdConfiguration?.TokenEndpoint ??
            throw new ArgumentNullException(
                nameof(request.TokenEndpoint),
                $"{nameof(request.TokenEndpoint)} specified as null and not obtainable from OpenID Configuration for specified IssuerUrl. ");

        // Determine auth endpoint
        string authorizationEndpoint =
            request.AuthorizationEndpoint ??
            openIdConfiguration?.AuthorizationEndpoint ??
            throw new ArgumentNullException(
                nameof(request.AuthorizationEndpoint),
                $"{nameof(request.AuthorizationEndpoint)} specified as null and not obtainable from OpenID Configuration for specified IssuerUrl. ");

        // Determine JWKS URI
        string jwksUri =
            request.JwksUri ??
            openIdConfiguration?.JwksUri ??
            throw new ArgumentNullException(
                nameof(request.JwksUri),
                $"{nameof(request.JwksUri)} specified as null and not obtainable from OpenID Configuration for specified IssuerUrl. ");

        // Determine TokenEndpointAuthMethod
        TokenEndpointAuthMethod tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        CheckTokenEndpointAuthMethod(
            tokenEndpointAuthMethod,
            supportsSca,
            openIdConfiguration?.TokenEndpointAuthMethodsSupported);

        // Get re-usable existing bank registration if possible
        BankRegistrationGroup? bankRegistrationGroup = bankProfile.BankConfigurationApiSettings.BankRegistrationGroup;
        BankRegistrationPersisted? existingGroupRegistration = null;
        if (bankRegistrationGroup is not null)
        {
            // Get existing registrations with same bank group and SSA (ideally software ID)
            IOrderedQueryable<BankRegistrationPersisted> existingRegistrations =
                _entityMethods
                    .DbSetNoTracking
                    .Where(
                        x => x.BankGroup == bankGroup &&
                             x.SoftwareStatementProfileId == softwareStatementProfileId)
                    .OrderByDescending(x => x.Created); // most recent first

            // Search for first registration in same registration group and check compatible (error if not)
            foreach (BankRegistrationPersisted existingReg in existingRegistrations)
            {
                BankProfile existingRegBankProfile = _bankProfileService.GetBankProfile(existingReg.BankProfile);
                if (existingRegBankProfile.BankConfigurationApiSettings.BankRegistrationGroup != bankRegistrationGroup)
                {
                    continue;
                }

                existingGroupRegistration = existingReg;
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages =
                    CheckExistingRegistrationCompatible(
                        existingGroupRegistration,
                        existingRegBankProfile,
                        request.ExternalApiId,
                        bankRegistrationGroup.Value,
                        softwareStatementProfileOverrideCase,
                        tokenEndpointAuthMethod,
                        registrationScope);
                nonErrorMessages.AddRange(newNonErrorMessages);
                break;
            }
        }

        // Obtain external (bank) API registration
        string externalApiId;
        string? externalApiSecret;
        string? registrationAccessToken;
        ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse;
        if (existingGroupRegistration is not null)
        {
            // Re-use existing external (bank) API registration
            externalApiId = existingGroupRegistration.ExternalApiId;
            externalApiSecret = existingGroupRegistration.ExternalApiSecret;
            registrationAccessToken = existingGroupRegistration.RegistrationAccessToken;
            externalApiResponse = null;
        }
        else if (request.ExternalApiId is not null)
        {
            // Use supplied external (bank) API registration
            externalApiId = request.ExternalApiId;
            externalApiSecret = request.ExternalApiSecret;
            registrationAccessToken = request.RegistrationAccessToken;
            externalApiResponse = null;
        }
        else
        {
            // Perform Dynamic Client Registration

            // Check endpoint
            if (registrationEndpoint is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(request.RegistrationEndpoint)} specified as null and not obtainable from OpenID Configuration for bank's IssuerUrl. " +
                    "Thus DCR cannot be used. " +
                    "Please create a registration e.g. using the bank's portal and then try again using " +
                    "ExternalApiObject to specify the registration.");
            }

            // Check registration scope
            bool registrationScopeIsSubset =
                (registrationScope & ssaPayload.RegistrationScope) == registrationScope;
            if (!registrationScopeIsSubset)
            {
                throw new InvalidOperationException(
                    "Software statement assertion does not support specified RegistrationScope.");
            }

            // Check redirect URIs
            CheckRedirectUris(redirectUris, ssaPayload);

            BankRegistrationPostCustomBehaviour? bankRegistrationPostCustomBehaviour =
                customBehaviour?.BankRegistrationPost;
            externalApiResponse = await PerformDynamicClientRegistration(
                bankRegistrationPostCustomBehaviour,
                dynamicClientRegistrationApiVersion,
                processedSoftwareStatementProfile,
                softwareStatementAssertion,
                registrationEndpoint,
                tokenEndpointAuthMethod,
                redirectUris,
                registrationScope,
                bankFinancialId,
                nonErrorMessages);

            externalApiId = externalApiResponse.ClientId;
            externalApiSecret = externalApiResponse.ClientSecret;
            registrationAccessToken = externalApiResponse.RegistrationAccessToken;
        }

        // Create persisted entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var entity = new BankRegistrationPersisted(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            externalApiSecret,
            registrationAccessToken,
            tokenEndpointAuthMethod,
            bankGroup,
            useSimulatedBank,
            externalApiId,
            bankProfile.BankProfileEnum,
            jwksUri,
            registrationEndpoint,
            tokenEndpoint,
            authorizationEndpoint,
            defaultFragmentRedirectUri,
            defaultQueryRedirectUri,
            redirectUris,
            softwareStatementProfileId,
            softwareStatementProfileOverrideCase,
            registrationScope);

        // Save entity
        await _entityMethods.AddAsync(entity);

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        // Create filtered response
        if (externalApiResponse is not null)
        {
            externalApiResponse.ClientSecret = null;
            externalApiResponse.RegistrationAccessToken = null;
        }

        BankRegistrationResponse response = new(
            entity.Id,
            entity.Created,
            entity.CreatedBy,
            entity.Reference,
            externalApiResponse,
            null,
            entity.UseSimulatedBank,
            entity.BankProfile,
            entity.JwksUri,
            entity.RegistrationEndpoint,
            entity.TokenEndpoint,
            entity.AuthorizationEndpoint,
            entity.SoftwareStatementProfileId,
            entity.SoftwareStatementProfileOverride,
            entity.RegistrationScope,
            entity.DefaultFragmentRedirectUri,
            entity.DefaultQueryRedirectUri,
            entity.RedirectUris,
            entity.ExternalApiId);

        return (response, nonErrorMessages);
    }

    public async Task<(BankRegistrationResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)>
        ReadAsync(BankRegistrationReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load BankRegistration
        BankRegistrationPersisted entity =
            await _entityMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
            throw new KeyNotFoundException($"No record found for BankRegistration with ID {readParams.ModifiedBy}.");
        string externalApiId = entity.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(entity.BankProfile);
        TokenEndpointAuthMethod tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        bool supportsSca = bankProfile.SupportsSca;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
            bankProfile.DynamicClientRegistrationApiVersion;

        ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse;
        bool includeExternalApiOperation =
            readParams.IncludeExternalApiOperation ??
            bankProfile.BankConfigurationApiSettings.UseRegistrationGetEndpoint;
        if (includeExternalApiOperation)
        {
            string registrationEndpoint =
                entity.RegistrationEndpoint ??
                throw new InvalidOperationException(
                    "BankRegistration does not have a registration endpoint configured.");

            bool useRegistrationAccessTokenValue =
                bankProfile.BankConfigurationApiSettings.UseRegistrationAccessToken;

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    entity.SoftwareStatementProfileId,
                    entity.SoftwareStatementProfileOverride);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get client credentials grant access token if necessary
            string accessToken;
            if (useRegistrationAccessTokenValue)
            {
                accessToken =
                    entity.RegistrationAccessToken ??
                    throw new InvalidOperationException("No registration access token available");
            }
            else
            {
                string? scope = customBehaviour?.BankRegistrationPut?.CustomTokenScope;
                accessToken = await _grantPost.PostClientCredentialsGrantAsync(
                    scope,
                    processedSoftwareStatementProfile.OBSealKey,
                    tokenEndpointAuthMethod,
                    entity.TokenEndpoint,
                    entity.ExternalApiId,
                    entity.ExternalApiSecret,
                    entity.Id.ToString(),
                    null,
                    customBehaviour?.ClientCredentialsGrantPost,
                    apiClient);
            }

            // Read object from external API
            JsonSerializerSettings? responseJsonSerializerSettings =
                GetResponseJsonSerializerSettings(customBehaviour?.BankRegistrationPost);
            IApiGetRequests<ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
                ApiRequests(
                    dynamicClientRegistrationApiVersion,
                    processedSoftwareStatementProfile,
                    false, // not used for GET
                    accessToken);
            var externalApiUrl = new Uri(registrationEndpoint.TrimEnd('/') + $"/{externalApiId}");
            (externalApiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.GetAsync(
                    externalApiUrl,
                    responseJsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);
        }
        else
        {
            externalApiResponse = null;
        }

        // Create filtered response
        if (externalApiResponse is not null)
        {
            externalApiResponse.ClientSecret = null;
            externalApiResponse.RegistrationAccessToken = null;
        }

        BankRegistrationResponse response = new(
            entity.Id,
            entity.Created,
            entity.CreatedBy,
            entity.Reference,
            externalApiResponse,
            null,
            entity.UseSimulatedBank,
            entity.BankProfile,
            entity.JwksUri,
            entity.RegistrationEndpoint,
            entity.TokenEndpoint,
            entity.AuthorizationEndpoint,
            entity.SoftwareStatementProfileId,
            entity.SoftwareStatementProfileOverride,
            entity.RegistrationScope,
            entity.DefaultFragmentRedirectUri,
            entity.DefaultQueryRedirectUri,
            entity.RedirectUris,
            entity.ExternalApiId);

        return (response, nonErrorMessages);
    }

    private IList<IFluentResponseInfoOrWarningMessage> CheckExistingRegistrationCompatible(
        BankRegistrationPersisted existingRegistration,
        BankProfile existingRegistrationBankProfile,
        string? externalApiId,
        BankRegistrationGroup bankRegistrationGroup,
        string? softwareStatementProfileOverrideCase,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        RegistrationScopeEnum registrationScope)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        if (externalApiId is not null)
        {
            if (externalApiId !=
                existingRegistration.ExternalApiId)
            {
                throw new
                    InvalidOperationException(
                        $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                        $"used ExternalApiObject with ExternalApiId {existingRegistration.ExternalApiId} " +
                        $"which is different from expected {externalApiId}.");
            }

            string warningMessage1 =
                $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                "exists whose ExternalApiId matches ExternalApiId from " +
                $"ExternalApiObject provided in request ({externalApiId}). " +
                "Therefore this registration will be re-used and any ExternalApiSecret from ExternalApiObject provided in request will be ignored and value from " +
                "previous registration re-used.";
            nonErrorMessages.Add(new FluentResponseWarningMessage(warningMessage1));
            _instrumentationClient.Warning(warningMessage1);

            string warningMessage2 =
                $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                "exists whose ExternalApiId matches ExternalApiId from " +
                $"ExternalApiObject provided in request ({externalApiId}). " +
                "Therefore this registration will be re-used and any RegistrationAccessToken from ExternalApiObject provided in request will be ignored and value from " +
                "previous registration re-used.";
            nonErrorMessages.Add(new FluentResponseWarningMessage(warningMessage2));
            _instrumentationClient.Warning(warningMessage2);
        }

        // TODO: compare redirect URLs?

        if (softwareStatementProfileOverrideCase != existingRegistration.SoftwareStatementProfileOverride)
        {
            throw new
                InvalidOperationException(
                    $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                    $"used software statement profile with override {existingRegistration.SoftwareStatementProfileOverride} " +
                    $"which is different from expected {softwareStatementProfileOverrideCase}.");
        }

        if (tokenEndpointAuthMethod !=
            existingRegistrationBankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod)
        {
            throw new
                InvalidOperationException(
                    $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                    $"used TokenEndpointAuthMethod {existingRegistrationBankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod} " +
                    $"which is different from expected {tokenEndpointAuthMethod}.");
        }

        if (registrationScope != existingRegistration.RegistrationScope)
        {
            throw new
                InvalidOperationException(
                    $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                    $"used RegistrationScope {existingRegistration.RegistrationScope} " +
                    $"which is different from expected {registrationScope}.");
        }

        return nonErrorMessages;
    }

    private async Task<string> GetSoftwareStatementAssertion(
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile)
    {
        // Get access token
        var scope = "ASPSPReadAccess TPPReadAccess AuthoritiesReadAccess";
        string tokenEndpoint = processedSoftwareStatementProfile.SandboxEnvironment
            ? "https://matls-sso.openbankingtest.org.uk/as/token.oauth2"
            : "https://matls-sso.openbanking.org.uk/as/token.oauth2";
        string accessToken = await _grantPost.PostClientCredentialsGrantAsync(
            scope,
            processedSoftwareStatementProfile.OBSealKey,
            TokenEndpointAuthMethod.PrivateKeyJwt,
            tokenEndpoint,
            processedSoftwareStatementProfile.SoftwareId,
            null,
            processedSoftwareStatementProfile.SoftwareId,
            null,
            null,
            processedSoftwareStatementProfile.ApiClient,
            new Dictionary<string, JsonNode?> { ["scope"] = scope },
            true,
            JwsAlgorithm.RS256);

        // Get software statement assertion
        var headers = new List<HttpHeader> { new("Authorization", "Bearer " + accessToken) };
        string url = processedSoftwareStatementProfile.SandboxEnvironment
            ? "https://matls-ssaapi.openbankingtest.org.uk/api/v2/tpp/"
            : "https://matls-ssaapi.openbanking.org.uk/api/v2/tpp/";
        url += $"{processedSoftwareStatementProfile.OrganisationId}/ssa/{processedSoftwareStatementProfile.SoftwareId}";
        string response = await new HttpRequestBuilder()
            .SetUri(url)
            .SetHeaders(headers)
            .SetAccept("application/jws+json")
            .Create()
            .SendExpectingStringResponseAsync(processedSoftwareStatementProfile.ApiClient);

        return response;
    }

    private SsaPayload ProcessSsa(
        string ssa,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
        string id)
    {
        // Get parts of SSA and payload
        string[] ssaComponentsBase64 = ssa.Split(new[] { '.' });
        if (ssaComponentsBase64.Length != 3)
        {
            throw new ArgumentException("SSA should have three parts.");
        }
        string ssaHeaderBase64 = ssaComponentsBase64[0];
        string ssaPayloadBase64 = ssaComponentsBase64[1];
        string ssaSignatureBase64 = ssaComponentsBase64[2];
        if (new[] { ssaHeaderBase64, ssaPayloadBase64, ssaSignatureBase64 }
                .JoinString(".") != ssa)
        {
            throw new InvalidOperationException("Can't correctly process software statement");
        }
        SsaPayload ssaPayload =
            SoftwareStatementPayloadFromBase64(ssaComponentsBase64[1]);

        // Check DefaultQueryRedirectUrl
        if (processedSoftwareStatementProfile.DefaultQueryRedirectUrl is not null &&
            !ssaPayload.SoftwareRedirectUris.Contains(processedSoftwareStatementProfile.DefaultQueryRedirectUrl))
        {
            throw new ArgumentException(
                $"Software statement profile with ID {id} contains DefaultQueryRedirectUrl {processedSoftwareStatementProfile.DefaultQueryRedirectUrl} " +
                "which is not included in software statement assertion software_redirect_uris field.");
        }

        // Check DefaultFragmentRedirectUrl
        if (processedSoftwareStatementProfile.DefaultFragmentRedirectUrl is not null &&
            !ssaPayload.SoftwareRedirectUris.Contains(processedSoftwareStatementProfile.DefaultFragmentRedirectUrl))
        {
            throw new ArgumentException(
                $"Software statement profile with ID {id} contains DefaultFragmentRedirectUrl {processedSoftwareStatementProfile.DefaultFragmentRedirectUrl} " +
                "which is not included in software statement assertion software_redirect_uris field.");
        }

        return ssaPayload;
    }

    private string SoftwareStatementPayloadToBase64(SsaPayload payload)
    {
        string jsonData = JsonConvert.SerializeObject(payload);
        return Base64UrlEncoder.Encode(jsonData);
    }

    private SsaPayload SoftwareStatementPayloadFromBase64(string payloadBase64)
    {
        // Perform conversion
        string payloadString = Base64UrlEncoder.Decode(payloadBase64);
        SsaPayload newObject =
            JsonConvert.DeserializeObject<SsaPayload>(payloadString) ??
            throw new ArgumentException("Cannot de-serialise software statement");

        // Check reverse conversion works or throw
        // (If reverse conversion fails, we can never re-generate base64 correctly)
        // if (payloadBase64 != SoftwareStatementPayloadToBase64(newObject))
        // {
        //     throw new ArgumentException("Please update SoftwareStatementPayload type to support your software statement");
        // }

        return newObject;
    }

    private async Task<ClientRegistrationModelsPublic.OBClientRegistration1Response> PerformDynamicClientRegistration(
        BankRegistrationPostCustomBehaviour? bankRegistrationPostCustomBehaviour,
        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
        string softwareStatementAssertion,
        string registrationEndpoint,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        IList<string> redirectUris,
        RegistrationScopeEnum registrationScope,
        string bankFinancialId,
        List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)
    {
        ClientRegistrationModelsPublic.OBClientRegistration1Response externalApiResponse;
        // Create new object at external API
        JsonSerializerSettings? requestJsonSerializerSettings =
            GetRequestJsonSerializerSettings(bankRegistrationPostCustomBehaviour);
        JsonSerializerSettings? responseJsonSerializerSettings =
            GetResponseJsonSerializerSettings(bankRegistrationPostCustomBehaviour);
        bool useApplicationJoseNotApplicationJwtContentTypeHeader =
            bankRegistrationPostCustomBehaviour
                ?.UseApplicationJoseNotApplicationJwtContentTypeHeader ?? false;
        IApiPostRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
            ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
            ApiRequests(
                dynamicClientRegistrationApiVersion,
                processedSoftwareStatementProfile,
                useApplicationJoseNotApplicationJwtContentTypeHeader,
                string.Empty // not used for POST
            );
        var externalApiUrl = new Uri(registrationEndpoint);
        SubjectDnOrgIdEncoding transportCertificateSubjectDnOrgIdEncoding =
            bankRegistrationPostCustomBehaviour?.TransportCertificateSubjectDnOrgIdEncoding ??
            SubjectDnOrgIdEncoding.StringAttributeType;
        ClientRegistrationModelsPublic.OBClientRegistration1 externalApiRequest =
            RegistrationClaimsFactory.CreateRegistrationClaims(
                tokenEndpointAuthMethod,
                redirectUris,
                processedSoftwareStatementProfile.SoftwareId,
                processedSoftwareStatementProfile.TransportCertificateSubjectDn[
                    transportCertificateSubjectDnOrgIdEncoding],
                softwareStatementAssertion,
                registrationScope,
                bankRegistrationPostCustomBehaviour,
                bankFinancialId);
        (externalApiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.PostAsync(
                externalApiUrl,
                externalApiRequest,
                requestJsonSerializerSettings,
                responseJsonSerializerSettings,
                processedSoftwareStatementProfile.ApiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);
        return externalApiResponse;
    }

    private static void CheckTokenEndpointAuthMethod(
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        bool supportsSca,
        IList<OpenIdConfigurationTokenEndpointAuthMethodEnum>? tokenEndpointAuthMethodsSupported)
    {
        if (tokenEndpointAuthMethod is TokenEndpointAuthMethod.ClientSecretBasic &&
            !supportsSca)
        {
            throw new InvalidOperationException(
                $"TokenEndpointAuthMethod resolves to " +
                $"{TokenEndpointAuthMethod.ClientSecretBasic} for bank supporting SCA which is not supported.");
        }

        if (tokenEndpointAuthMethodsSupported is { } methodsSupported)
        {
            var methodsSupportedFilter = new List<TokenEndpointAuthMethod>();
            if (methodsSupported.Contains(OpenIdConfigurationTokenEndpointAuthMethodEnum.TlsClientAuth))
            {
                methodsSupportedFilter.Add(TokenEndpointAuthMethod.TlsClientAuth);
            }

            if (methodsSupported.Contains(OpenIdConfigurationTokenEndpointAuthMethodEnum.PrivateKeyJwt))
            {
                methodsSupportedFilter.Add(TokenEndpointAuthMethod.PrivateKeyJwt);
            }

            if (methodsSupported.Contains(OpenIdConfigurationTokenEndpointAuthMethodEnum.ClientSecretBasic) &&
                !supportsSca)
            {
                methodsSupportedFilter.Add(TokenEndpointAuthMethod.ClientSecretBasic);
            }

            if (!methodsSupportedFilter.Contains(tokenEndpointAuthMethod))
            {
                throw new InvalidOperationException(
                    $"TokenEndpointAuthMethod resolves to " +
                    $"{tokenEndpointAuthMethod} which is not specified as supported by OpenID Configuration for Bank's IssuerUrl.");
            }
        }
    }

    private static (IList<string> redirectUris, string defaultFragmentRedirectUri, string defaultQueryRedirectUri)
        GetRedirectUris(
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            SsaPayload ssaPayload,
            string? requestDefaultFragmentRedirectUri,
            string? requestDefaultQueryRedirectUri,
            IList<string>? requestRedirectUris)
    {
        List<string> ssaRedirectUris = ssaPayload.SoftwareRedirectUris;

        // Determine redirect URIs ensuring list not empty
        IList<string> redirectUris;
        if (requestRedirectUris is not null)
        {
            if (!requestRedirectUris.Any())
            {
                throw new InvalidOperationException("Specified RedirectUris is empty list.");
            }
            redirectUris = requestRedirectUris;
        }
        else
        {
            if (!ssaRedirectUris.Any())
            {
                throw new InvalidOperationException("RedirectUris from software statement is empty list.");
            }
            redirectUris = ssaRedirectUris;
        }

        // Determine default fragment redirect URI ensuring also contained in redirect URIs list
        string defaultFragmentRedirectUri;
        if (requestDefaultFragmentRedirectUri is not null)
        {
            if (!redirectUris.Contains(requestDefaultFragmentRedirectUri))
            {
                throw new InvalidOperationException(
                    $"Specified default fragment redirect URI {requestDefaultFragmentRedirectUri} not included in specified RedirectUris.");
            }
            defaultFragmentRedirectUri = requestDefaultFragmentRedirectUri;
        }
        else
        {
            if (!redirectUris.Contains(processedSoftwareStatementProfile.DefaultFragmentRedirectUrl))
            {
                throw new InvalidOperationException(
                    $"Default fragment redirect URI {processedSoftwareStatementProfile.DefaultFragmentRedirectUrl} " +
                    $"from software statement profile not included in specified RedirectUris. " +
                    $"Please specify a different one or include this one in specified RedirectUris.");
            }
            defaultFragmentRedirectUri =
                processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;
        }

        // Determine default query redirect URI ensuring also contained in redirect URIs list
        string defaultQueryRedirectUri;
        if (requestDefaultQueryRedirectUri is not null)
        {
            if (!redirectUris.Contains(requestDefaultQueryRedirectUri))
            {
                throw new InvalidOperationException(
                    $"Specified default query redirect URI {requestDefaultQueryRedirectUri} not included in specified RedirectUris.");
            }
            defaultQueryRedirectUri = requestDefaultQueryRedirectUri;
        }
        else
        {
            if (!redirectUris.Contains(processedSoftwareStatementProfile.DefaultQueryRedirectUrl))
            {
                throw new InvalidOperationException(
                    $"Default query redirect URI {processedSoftwareStatementProfile.DefaultQueryRedirectUrl} " +
                    $"from software statement profile not included in specified RedirectUris. " +
                    $"Please specify a different one or include this one in specified RedirectUris.");
            }
            defaultQueryRedirectUri =
                processedSoftwareStatementProfile.DefaultQueryRedirectUrl;
        }


        return (redirectUris, defaultFragmentRedirectUri, defaultQueryRedirectUri);
    }

    private static void CheckRedirectUris(
        IList<string> redirectUris,
        SsaPayload ssaPayload)
    {
        List<string> ssaRedirectUris = ssaPayload.SoftwareRedirectUris;

        // Check redirect URIs in SSA
        foreach (string redirectUri in redirectUris)
        {
            if (!ssaRedirectUris.Contains(redirectUri))
            {
                throw new InvalidOperationException(
                    $"Specified URI {redirectUri} in RedirectUris is not included in software statement assertion.");
            }
        }
    }

    private static JsonSerializerSettings? GetRequestJsonSerializerSettings(
        BankRegistrationPostCustomBehaviour? bankRegistrationPostCustomBehaviour)
    {
        JsonSerializerSettings? requestJsonSerializerSettings = null;
        if (!(bankRegistrationPostCustomBehaviour is null))
        {
            var optionsDict = new Dictionary<JsonConverterLabel, int>();
            DelimitedStringConverterOptions? scopeClaimJsonConverter = bankRegistrationPostCustomBehaviour
                .ScopeClaimJsonConverter;
            if (scopeClaimJsonConverter is not null)
            {
                optionsDict.Add(JsonConverterLabel.DcrRegScope, (int) scopeClaimJsonConverter);
            }

            requestJsonSerializerSettings = new JsonSerializerSettings
            {
                Context = new StreamingContext(
                    StreamingContextStates.All,
                    optionsDict)
            };
        }

        return requestJsonSerializerSettings;
    }

    private static JsonSerializerSettings? GetResponseJsonSerializerSettings(
        BankRegistrationPostCustomBehaviour? bankRegistrationPostCustomBehaviour)
    {
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        if (!(bankRegistrationPostCustomBehaviour is null))
        {
            var optionsDict = new Dictionary<JsonConverterLabel, int>();

            DateTimeOffsetUnixConverterEnum? clientIdIssuedAtClaimResponseJsonConverter =
                bankRegistrationPostCustomBehaviour
                    .ClientIdIssuedAtClaimResponseJsonConverter;
            if (clientIdIssuedAtClaimResponseJsonConverter is not null)
            {
                optionsDict.Add(
                    JsonConverterLabel.DcrRegClientIdIssuedAt,
                    (int) clientIdIssuedAtClaimResponseJsonConverter);
            }

            DelimitedStringConverterOptions? scopeClaimJsonConverter = bankRegistrationPostCustomBehaviour
                .ScopeClaimResponseJsonConverter;
            if (scopeClaimJsonConverter is not null)
            {
                optionsDict.Add(JsonConverterLabel.DcrRegScope, (int) scopeClaimJsonConverter);
            }

            responseJsonSerializerSettings = new JsonSerializerSettings
            {
                Context = new StreamingContext(
                    StreamingContextStates.All,
                    optionsDict)
            };
        }

        return responseJsonSerializerSettings;
    }

    private IApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
        ClientRegistrationModelsPublic.OBClientRegistration1Response> ApiRequests(
        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
        bool useApplicationJoseNotApplicationJwtContentTypeHeader,
        string accessToken)
    {
        IApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
            ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
            dynamicClientRegistrationApiVersion switch
            {
                DynamicClientRegistrationApiVersion.Version3p1 =>
                    new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response,
                        ClientRegistrationModelsV3p1.OBClientRegistration1,
                        ClientRegistrationModelsV3p1.OBClientRegistration1>(
                        new BankRegistrationGetRequestProcessor(accessToken),
                        new BankRegistrationPostRequestProcessor<
                            ClientRegistrationModelsV3p1.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            _instrumentationClient,
                            useApplicationJoseNotApplicationJwtContentTypeHeader)),
                DynamicClientRegistrationApiVersion.Version3p2 =>
                    new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response,
                        ClientRegistrationModelsV3p2.OBClientRegistration1,
                        ClientRegistrationModelsV3p2.OBClientRegistration1>(
                        new BankRegistrationGetRequestProcessor(accessToken),
                        new BankRegistrationPostRequestProcessor<
                            ClientRegistrationModelsV3p2.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            _instrumentationClient,
                            useApplicationJoseNotApplicationJwtContentTypeHeader)),
                DynamicClientRegistrationApiVersion.Version3p3 =>
                    new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response,
                        ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                        new BankRegistrationGetRequestProcessor(accessToken),
                        new BankRegistrationPostRequestProcessor<
                            ClientRegistrationModelsPublic.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            _instrumentationClient,
                            useApplicationJoseNotApplicationJwtContentTypeHeader)),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(dynamicClientRegistrationApiVersion),
                    dynamicClientRegistrationApiVersion,
                    null)
            };
        return apiRequests;
    }
}
