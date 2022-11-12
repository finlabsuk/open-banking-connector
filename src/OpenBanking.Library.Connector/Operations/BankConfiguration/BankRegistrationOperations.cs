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
    private readonly IBankProfileDefinitions _bankProfileDefinitions;
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
        IBankProfileDefinitions bankProfileDefinitions,
        IGrantPost grantPost)
    {
        _bankMethods = bankMethods;
        _bankProfileDefinitions = bankProfileDefinitions;
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

        BankProfile? bankProfile = request.BankProfile is { } bankProfileEnum
            ? _bankProfileDefinitions.GetBankProfile(bankProfileEnum)
            : null;

        OAuth2ResponseMode defaultResponseMode =
            request.DefaultResponseMode ??
            bankProfile?.DefaultResponseMode ??
            throw new ArgumentException(
                $"Request property {nameof(request.DefaultResponseMode)} is null and cannot be obtained using request property {request.BankProfile}.");

        // Load bank from DB and check for existing bank registrations
        Guid bankId = request.BankId;
        Bank bank =
            await _bankMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == bankId) ??
            throw new ArgumentException(
                $"No Bank record found for request property {nameof(request.BankId)} = {request.BankId}.");
        CustomBehaviourClass? customBehaviour = bank.CustomBehaviour;
        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
            bank.DcrApiVersion;
        string registrationEndpoint = bank.RegistrationEndpoint;
        string issuerUrl = bank.IssuerUrl;
        bool supportsSca = bank.SupportsSca;

        // Check for existing bank registration(s)
        BankRegistrationPersisted? existingRegistration =
            await _entityMethods
                .DbSet
                .Where(x => x.BankId == bankId)
                .FirstOrDefaultAsync();
        if (existingRegistration is not null &&
            !request.AllowMultipleRegistrations)
        {
            throw new ArgumentException(
                $"One or more BankRegistrations already exist for this bank and request property {nameof(request.AllowMultipleRegistrations)} is false.");
        }

        // Load processed software statement profile
        string softwareStatementProfileId = request.SoftwareStatementProfileId;
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                softwareStatementProfileId,
                request.SoftwareStatementAndCertificateProfileOverrideCase);

        // Determine redirect URIs
        (string defaultRedirectUri, List<string> otherRedirectUris) = GetRedirectUris(
            processedSoftwareStatementProfile,
            request.DefaultRedirectUri,
            request.OtherRedirectUris);

        // Determine registration scope
        RegistrationScopeEnum registrationScope =
            request.RegistrationScope ??
            processedSoftwareStatementProfile.SoftwareStatementPayload.RegistrationScope;
        RegistrationScopeIsValid? registrationScopeIsValidFcn =
            bankProfile?.BankConfigurationApiSettings.RegistrationScopeIsValid;
        if (registrationScopeIsValidFcn is not null &&
            !registrationScopeIsValidFcn(registrationScope))
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

        // Determine TokenEndpointAuthMethod
        TokenEndpointAuthMethod tokenEndpointAuthMethod = GetTokenEndpointAuthMethod(
            request,
            supportsSca,
            openIdConfiguration?.TokenEndpointAuthMethodsSupported);

        // Set external (bank) API parameters via DCR or directly
        string externalApiId;
        string? externalApiSecret;
        string? registrationAccessToken;
        ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse;
        if (request.ExternalApiObject is null)
        {
            // Create new object at external API
            JsonSerializerSettings? requestJsonSerializerSettings =
                GetRequestJsonSerializerSettings(customBehaviour);
            JsonSerializerSettings? responseJsonSerializerSettings =
                GetResponseJsonSerializerSettings(customBehaviour);
            bool useApplicationJoseNotApplicationJwtContentTypeHeader =
                customBehaviour?.BankRegistrationPost
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
                    customBehaviour?.BankRegistrationPost,
                    bank);
            (externalApiResponse, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.PostAsync(
                    externalApiUrl,
                    externalApiRequest,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

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
            defaultRedirectUri,
            otherRedirectUris,
            request.SoftwareStatementProfileId,
            request.SoftwareStatementAndCertificateProfileOverrideCase,
            tokenEndpointAuthMethod,
            registrationScope,
            defaultResponseMode,
            request.BankId);

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
            entity.SoftwareStatementProfileId,
            entity.SoftwareStatementProfileOverride,
            entity.TokenEndpointAuthMethod,
            entity.RegistrationScope,
            entity.BankId,
            entity.DefaultResponseMode,
            entity.DefaultRedirectUri,
            entity.OtherRedirectUris);

        return (response, nonErrorMessages);
    }

    public async Task<(BankRegistrationResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)>
        ReadAsync(BankRegistrationReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        BankProfile? bankProfile = null;
        if (readParams.BankProfileEnum is not null)
        {
            bankProfile = _bankProfileDefinitions.GetBankProfile(readParams.BankProfileEnum.Value);
        }

        // Load BankRegistration
        BankRegistrationPersisted entity =
            await _entityMethods
                .DbSetNoTracking
                .Include(o => o.BankNavigation)
                .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
            throw new KeyNotFoundException($"No record found for BankRegistration with ID {readParams.ModifiedBy}.");
        CustomBehaviourClass? customBehaviour = entity.BankNavigation.CustomBehaviour;
        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
            entity.BankNavigation.DcrApiVersion;
        string externalApiId = entity.ExternalApiObject.ExternalApiId;

        ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse;
        bool includeExternalApiOperation =
            readParams.IncludeExternalApiOperation ??
            bankProfile?.BankConfigurationApiSettings.UseReadEndpoint ??
            throw new ArgumentNullException(
                null,
                "includeExternalApiOperation specified as null and cannot be obtained using specified BankProfile (also null).");
        if (includeExternalApiOperation)
        {
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
            string accessToken = useRegistrationAccessTokenValue
                ? entity.ExternalApiObject.RegistrationAccessToken ??
                  throw new InvalidOperationException("No registration access token available")
                : (await _grantPost.PostClientCredentialsGrantAsync(
                    null,
                    processedSoftwareStatementProfile,
                    entity,
                    entity.BankNavigation.TokenEndpoint,
                    null,
                    apiClient,
                    _instrumentationClient))
                .AccessToken;

            // Read object from external API
            JsonSerializerSettings? responseJsonSerializerSettings =
                GetResponseJsonSerializerSettings(customBehaviour);
            IApiGetRequests<ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
                ApiRequests(
                    dynamicClientRegistrationApiVersion,
                    processedSoftwareStatementProfile,
                    false, // not used for GET
                    accessToken);
            var externalApiUrl = new Uri(entity.BankNavigation.RegistrationEndpoint.TrimEnd('/') + $"/{externalApiId}");
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
            entity.SoftwareStatementProfileId,
            entity.SoftwareStatementProfileOverride,
            entity.TokenEndpointAuthMethod,
            entity.RegistrationScope,
            entity.BankId,
            entity.DefaultResponseMode,
            entity.DefaultRedirectUri,
            entity.OtherRedirectUris);

        return (response, nonErrorMessages);
    }

    private static TokenEndpointAuthMethod GetTokenEndpointAuthMethod(
        BankRegistration request,
        bool supportsSca,
        IList<OpenIdConfigurationTokenEndpointAuthMethodEnum>? tokenEndpointAuthMethodsSupported)
    {
        TokenEndpointAuthMethod tokenEndpointAuthMethod;
        if (request.TokenEndpointAuthMethod is TokenEndpointAuthMethod.ClientSecretBasic &&
            !supportsSca)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.TokenEndpointAuthMethod),
                $"{nameof(request.TokenEndpointAuthMethod)} specified as {TokenEndpointAuthMethod.ClientSecretBasic} for bank supporting SCA which is not supported.");
        }

        if (tokenEndpointAuthMethodsSupported is { } methodsSupported)
        {
            var orderedMethodsSupported = new List<TokenEndpointAuthMethod>();
            if (methodsSupported.Contains(OpenIdConfigurationTokenEndpointAuthMethodEnum.TlsClientAuth))
            {
                orderedMethodsSupported.Add(TokenEndpointAuthMethod.TlsClientAuth);
            }

            if (methodsSupported.Contains(OpenIdConfigurationTokenEndpointAuthMethodEnum.PrivateKeyJwt))
            {
                orderedMethodsSupported.Add(TokenEndpointAuthMethod.PrivateKeyJwt);
            }

            if (methodsSupported.Contains(OpenIdConfigurationTokenEndpointAuthMethodEnum.ClientSecretBasic) &&
                !supportsSca)
            {
                orderedMethodsSupported.Add(TokenEndpointAuthMethod.ClientSecretBasic);
            }

            tokenEndpointAuthMethod = request.TokenEndpointAuthMethod switch
            {
                { } requestAuthMethod => orderedMethodsSupported.Contains(requestAuthMethod)
                    ? requestAuthMethod
                    : throw new ArgumentOutOfRangeException(
                        nameof(request.TokenEndpointAuthMethod),
                        $"{nameof(request.TokenEndpointAuthMethod)} specified as {requestAuthMethod} which is not specified as supported by OpenID Configuration for Bank's IssuerUrl."),
                _ => orderedMethodsSupported.Any()
                    ? orderedMethodsSupported.First()
                    : throw new ArgumentNullException(
                        nameof(request.TokenEndpointAuthMethod),
                        $"{nameof(request.TokenEndpointAuthMethod)} specified as null and not obtainable from OpenID Configuration for Bank's IssuerUrl. ")
            };
        }
        else
        {
            tokenEndpointAuthMethod =
                request.TokenEndpointAuthMethod ??
                throw new ArgumentNullException(
                    nameof(request.TokenEndpointAuthMethod),
                    $"{nameof(request.TokenEndpointAuthMethod)} specified as null and not obtainable from OpenID Configuration for Bank's IssuerUrl. ");
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

    private static JsonSerializerSettings? GetRequestJsonSerializerSettings(CustomBehaviourClass? customBehaviour)
    {
        JsonSerializerSettings? requestJsonSerializerSettings = null;
        if (!(customBehaviour?.BankRegistrationPost is null))
        {
            var optionsDict = new Dictionary<JsonConverterLabel, int>();
            DelimitedStringConverterOptions? scopeClaimJsonConverter = customBehaviour
                .BankRegistrationPost
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

    private static JsonSerializerSettings? GetResponseJsonSerializerSettings(CustomBehaviourClass? customBehaviour)
    {
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        if (!(customBehaviour?.BankRegistrationPost is null))
        {
            var optionsDict = new Dictionary<JsonConverterLabel, int>();

            DateTimeOffsetConverter? clientIdIssuedAtClaimResponseJsonConverter = customBehaviour
                .BankRegistrationPost
                .ClientIdIssuedAtClaimResponseJsonConverter;
            if (clientIdIssuedAtClaimResponseJsonConverter is not null)
            {
                optionsDict.Add(
                    JsonConverterLabel.DcrRegClientIdIssuedAt,
                    (int) clientIdIssuedAtClaimResponseJsonConverter);
            }

            DelimitedStringConverterOptions? scopeClaimJsonConverter = customBehaviour
                .BankRegistrationPost
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
