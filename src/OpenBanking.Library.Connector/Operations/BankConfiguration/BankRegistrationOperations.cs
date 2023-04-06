// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
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
using Microsoft.EntityFrameworkCore;
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
        OAuth2ResponseMode defaultResponseMode = bankProfile.DefaultResponseMode;
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        string bankFinancialId = bankProfile.FinancialId;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
            bankProfile.DynamicClientRegistrationApiVersion;

        // Load processed software statement profile
        string softwareStatementProfileId = request.SoftwareStatementProfileId;
        string? softwareStatementProfileOverrideCase = request.SoftwareStatementProfileOverrideCase;
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                softwareStatementProfileId,
                softwareStatementProfileOverrideCase);

        // Determine redirect URIs
        (string defaultRedirectUri, List<string> otherRedirectUris) = GetRedirectUris(
            processedSoftwareStatementProfile,
            request.DefaultRedirectUri,
            request.OtherRedirectUris);

        // Determine registration scope
        RegistrationScopeEnum registrationScope =
            request.RegistrationScope ??
            processedSoftwareStatementProfile.SoftwareStatementPayload.RegistrationScope;
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
        TokenEndpointAuthMethod tokenEndpointAuthMethod = GetTokenEndpointAuthMethod(
            bankProfile,
            supportsSca,
            openIdConfiguration?.TokenEndpointAuthMethodsSupported);

        // Determine bank registration group
        BankRegistrationGroup? bankRegistrationGroup = null;
        if (!request.ForceNullBankRegistrationGroup)
        {
            bankRegistrationGroup =
                bankProfile.BankConfigurationApiSettings.BankRegistrationGroup;
        }

        // Check for existing bank registration(s) in bank registration group
        BankRegistrationPersisted? existingGroupRegistration = null;
        if (bankRegistrationGroup is not null)
        {
            existingGroupRegistration =
                await _entityMethods
                    .DbSetNoTracking
                    .Where(
                        x => x.BankRegistrationGroup == bankRegistrationGroup &&
                             x.SoftwareStatementProfileId == softwareStatementProfileId)
                    .OrderBy(x => x.Created)
                    .FirstOrDefaultAsync();
        }

        // Re-use external API (bank) registration, create new one (DCR), or use supplied one.
        string externalApiId;
        string? externalApiSecret;
        string? registrationAccessToken;
        ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse;
        if (existingGroupRegistration is not null)
        {
            BankProfile existingRegistrationBankProfile =
                _bankProfileService.GetBankProfile(existingGroupRegistration.BankProfile);

            // TODO: compare bankRegistrationPostCustomBehaviour, redirect URLs

            if (request.ExternalApiObject is not null)
            {
                if (request.ExternalApiObject.ExternalApiId !=
                    existingGroupRegistration.ExternalApiObject.ExternalApiId)
                {
                    throw new
                        InvalidOperationException(
                            $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                            $"used ExternalApiObject with ExternalApiId {existingGroupRegistration.ExternalApiObject.ExternalApiId} " +
                            $"which is different from expected {request.ExternalApiObject.ExternalApiId}.");
                }

                string warningMessage1 =
                    $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                    "exists whose ExternalApiId matches ExternalApiId from " +
                    $"ExternalApiObject provided in request ({request.ExternalApiObject.ExternalApiId}). " +
                    "Therefore this registration will be re-used and any ExternalApiSecret from ExternalApiObject provided in request will be ignored and value from " +
                    "previous registration re-used.";
                nonErrorMessages.Add(new FluentResponseWarningMessage(warningMessage1));
                _instrumentationClient.Warning(warningMessage1);

                string warningMessage2 =
                    $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                    "exists whose ExternalApiId matches ExternalApiId from " +
                    $"ExternalApiObject provided in request ({request.ExternalApiObject.ExternalApiId}). " +
                    "Therefore this registration will be re-used and any RegistrationAccessToken from ExternalApiObject provided in request will be ignored and value from " +
                    "previous registration re-used.";
                nonErrorMessages.Add(new FluentResponseWarningMessage(warningMessage2));
                _instrumentationClient.Warning(warningMessage2);
            }

            if (softwareStatementProfileOverrideCase != existingGroupRegistration.SoftwareStatementProfileOverride)
            {
                throw new
                    InvalidOperationException(
                        $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                        $"used software statement profile with override {existingGroupRegistration.SoftwareStatementProfileOverride} " +
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

            if (registrationScope != existingGroupRegistration.RegistrationScope)
            {
                throw new
                    InvalidOperationException(
                        $"Previous registration for BankRegistrationGroup {bankRegistrationGroup} " +
                        $"used RegistrationScope {existingGroupRegistration.RegistrationScope} " +
                        $"which is different from expected {registrationScope}.");
            }

            externalApiId = existingGroupRegistration.ExternalApiObject.ExternalApiId;
            externalApiSecret = existingGroupRegistration.ExternalApiObject.ExternalApiSecret;
            registrationAccessToken = existingGroupRegistration.ExternalApiObject.RegistrationAccessToken;
            externalApiResponse = null;
        }
        else
        {
            if (request.ExternalApiObject is null)
            {
                // Perform DCR
                if (registrationEndpoint is null)
                {
                    throw new InvalidOperationException(
                        $"{nameof(request.RegistrationEndpoint)} specified as null and not obtainable from OpenID Configuration for bank's IssuerUrl. " +
                        "Thus DCR cannot be used. " +
                        "Please create a registration e.g. using the bank's portal and then try again using " +
                        "ExternalApiObject to specify the registration.");
                }

                BankRegistrationPostCustomBehaviour? bankRegistrationPostCustomBehaviour =
                    customBehaviour?.BankRegistrationPost;
                externalApiResponse = await PerformDynamicClientRegistration(
                    bankRegistrationPostCustomBehaviour,
                    dynamicClientRegistrationApiVersion,
                    processedSoftwareStatementProfile,
                    registrationEndpoint,
                    tokenEndpointAuthMethod,
                    otherRedirectUris,
                    defaultRedirectUri,
                    registrationScope,
                    bankFinancialId,
                    nonErrorMessages);

                externalApiId = externalApiResponse.ClientId;
                externalApiSecret = externalApiResponse.ClientSecret;
                registrationAccessToken = externalApiResponse.RegistrationAccessToken;
            }
            else
            {
                externalApiId = request.ExternalApiObject.ExternalApiId;
                externalApiSecret = request.ExternalApiObject.ExternalApiSecret;
                registrationAccessToken = request.ExternalApiObject.RegistrationAccessToken;
                externalApiResponse = null;
            }
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
            externalApiId,
            externalApiSecret,
            registrationAccessToken,
            tokenEndpointAuthMethod,
            defaultResponseMode,
            request.BankProfile,
            jwksUri,
            registrationEndpoint,
            tokenEndpoint,
            authorizationEndpoint,
            bankRegistrationGroup,
            defaultRedirectUri,
            otherRedirectUris,
            softwareStatementProfileId,
            softwareStatementProfileOverrideCase,
            registrationScope,
            null);

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
            new ExternalApiObjectResponse(entity.ExternalApiObject.ExternalApiId),
            externalApiResponse,
            null,
            entity.BankProfile,
            entity.JwksUri,
            entity.RegistrationEndpoint,
            entity.TokenEndpoint,
            entity.AuthorizationEndpoint,
            entity.SoftwareStatementProfileId,
            entity.SoftwareStatementProfileOverride,
            entity.RegistrationScope,
            entity.DefaultRedirectUri,
            entity.OtherRedirectUris,
            entity.BankRegistrationGroup);

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
        string externalApiId = entity.ExternalApiObject.ExternalApiId;

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
                    $"BankRegistration does not have a registration endpoint configured.");

            bool useRegistrationAccessTokenValue =
                readParams.UseRegistrationAccessToken ??
                bankProfile?.BankConfigurationApiSettings.UseRegistrationAccessToken ??
                throw new ArgumentNullException(
                    null,
                    "useRegistrationAccessToken specified as null and cannot be obtained using specified BankProfile (also null).");

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
                accessToken = entity.ExternalApiObject.RegistrationAccessToken ??
                              throw new InvalidOperationException("No registration access token available");
            }
            else
            {
                string? scope = customBehaviour?.BankRegistrationPut?.CustomTokenScope;
                accessToken = (await _grantPost.PostClientCredentialsGrantAsync(
                        scope,
                        processedSoftwareStatementProfile,
                        entity,
                        tokenEndpointAuthMethod,
                        entity.TokenEndpoint,
                        supportsSca,
                        null,
                        customBehaviour?.ClientCredentialsGrantPost,
                        apiClient,
                        _instrumentationClient))
                    .AccessToken;
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
            new ExternalApiObjectResponse(entity.ExternalApiObject.ExternalApiId),
            externalApiResponse,
            null,
            entity.BankProfile,
            entity.JwksUri,
            entity.RegistrationEndpoint,
            entity.TokenEndpoint,
            entity.AuthorizationEndpoint,
            entity.SoftwareStatementProfileId,
            entity.SoftwareStatementProfileOverride,
            entity.RegistrationScope,
            entity.DefaultRedirectUri,
            entity.OtherRedirectUris,
            entity.BankRegistrationGroup);

        return (response, nonErrorMessages);
    }

    private async Task<ClientRegistrationModelsPublic.OBClientRegistration1Response> PerformDynamicClientRegistration(
        BankRegistrationPostCustomBehaviour? bankRegistrationPostCustomBehaviour,
        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
        string registrationEndpoint,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        List<string> otherRedirectUris,
        string defaultRedirectUri,
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
        ClientRegistrationModelsPublic.OBClientRegistration1 externalApiRequest =
            RegistrationClaimsFactory.CreateRegistrationClaims(
                tokenEndpointAuthMethod,
                new List<string>(otherRedirectUris) { defaultRedirectUri },
                processedSoftwareStatementProfile,
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

    private static TokenEndpointAuthMethod GetTokenEndpointAuthMethod(
        BankProfile bankProfile,
        bool supportsSca,
        IList<OpenIdConfigurationTokenEndpointAuthMethodEnum>? tokenEndpointAuthMethodsSupported)
    {
        TokenEndpointAuthMethod tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;

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

        return tokenEndpointAuthMethod;
    }

    private static (string defaultRedirectUri, List<string> otherRedirectUris) GetRedirectUris(
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
        string? requestDefaultRedirectUri,
        List<string>? requestOtherRedirectUris)
    {
        List<string> softwareStatementRedirectUris =
            processedSoftwareStatementProfile.SoftwareStatementPayload.SoftwareRedirectUris;

        if (requestDefaultRedirectUri is not null)
        {
            if (!softwareStatementRedirectUris.Contains(requestDefaultRedirectUri))
            {
                throw new InvalidOperationException(
                    $"Specified default redirect URI {requestDefaultRedirectUri} not included in software statement.");
            }
        }

        string defaultRedirectUri =
            requestDefaultRedirectUri ??
            processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;

        List<string> otherRedirectUris;
        if (requestOtherRedirectUris is not null)
        {
            foreach (string redirectUri in requestOtherRedirectUris)
            {
                if (!softwareStatementRedirectUris.Contains(redirectUri))
                {
                    throw new InvalidOperationException(
                        $"Specified other redirect URI {redirectUri} not included in software statement.");
                }
            }

            otherRedirectUris = requestOtherRedirectUris;
        }
        else
        {
            otherRedirectUris = softwareStatementRedirectUris;
        }

        otherRedirectUris.Remove(defaultRedirectUri);
        return (defaultRedirectUri, otherRedirectUris);
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
