// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Obie;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Jose;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using BankRegistrationRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.BankRegistration;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using ClientRegistrationModelsV3p2 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p2.Models;
using ClientRegistrationModelsV3p1 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal class
    BankRegistrationOperations :
    IObjectCreate<BankRegistrationRequest, BankRegistrationResponse, BankRegistrationCreateParams>,
    IObjectRead<BankRegistrationResponse, BankRegistrationReadParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly ClientAccessTokenGet _clientAccessTokenGet;
    private readonly IOpenIdConfigurationRead _configurationRead;
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IEncryptionKeyInfo _encryptionKeyInfo;
    private readonly IDbReadWriteEntityMethods<BankRegistrationEntity> _entityMethods;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IApiVariantMapper _mapper;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;
    private readonly ISecretProvider _secretProvider;
    private readonly IDbReadWriteEntityMethods<SoftwareStatementEntity> _softwareStatementEntityMethods;
    private readonly ITimeProvider _timeProvider;

    public BankRegistrationOperations(
        IDbReadWriteEntityMethods<BankRegistrationEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IApiVariantMapper mapper,
        IOpenIdConfigurationRead configurationRead,
        IBankProfileService bankProfileService,
        IDbReadWriteEntityMethods<SoftwareStatementEntity> softwareStatementEntityMethods,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods,
        ClientAccessTokenGet clientAccessTokenGet,
        IGrantPost grantPost,
        IEncryptionKeyInfo encryptionKeyInfo,
        ISecretProvider secretProvider)
    {
        _bankProfileService = bankProfileService;
        _softwareStatementEntityMethods = softwareStatementEntityMethods;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _clientAccessTokenGet = clientAccessTokenGet;
        _grantPost = grantPost;
        _encryptionKeyInfo = encryptionKeyInfo;
        _secretProvider = secretProvider;
        _configurationRead = configurationRead;
        _entityMethods = entityMethods;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
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

        // Load software statement
        Guid softwareStatementId = request.SoftwareStatementId;
        SoftwareStatementEntity softwareStatement =
            await _softwareStatementEntityMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == softwareStatementId) ??
            throw new KeyNotFoundException($"No record found for SoftwareStatement with ID {softwareStatementId}.");

        // Get IApiClient
        ObWacCertificate obWacCertificate =
            await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId);
        // IApiClient apiClient = request.UseSimulatedBank
        //     ? bankProfile.ReplayApiClient
        //     : obWacCertificate.ApiClient;
        IApiClient apiClient = obWacCertificate.ApiClient;

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Get external API secret
        string? requestExternalApiSecret = null;
        if (request.ExternalApiSecret is not null &&
            request.ExternalApiSecretFromSecrets is not null)
        {
            throw new ArgumentException(
                "Only one of ExternalApiSecret or ExternalApiSecretFromSecrets should be specified.");
        }
        if (request.ExternalApiSecretFromSecrets is not null)
        {
            SecretResult clientSecretResult =
                await _secretProvider.GetSecretAsync(request.ExternalApiSecretFromSecrets);
            if (!clientSecretResult.SecretObtained)
            {
                string fullMessage =
                    $"Request specifies ExternalApiSecret with Source {request.ExternalApiSecretFromSecrets.Source} " +
                    $"and Name {request.ExternalApiSecretFromSecrets.Name} which could not be obtained. {clientSecretResult.ErrorMessage}";
                throw new KeyNotFoundException(fullMessage);
            }
            requestExternalApiSecret = clientSecretResult.Secret!;
        }
        if (request.ExternalApiSecret is not null)
        {
            requestExternalApiSecret = request.ExternalApiSecret;
        }

        // Get registration access token
        string? requestRegistrationAccessToken = null;
        if (request.RegistrationAccessToken is not null &&
            request.RegistrationAccessTokenFromSecrets is not null)
        {
            throw new ArgumentException(
                "Only one of RegistrationAccessToken or RegistrationAccessTokenFromSecrets should be specified.");
        }
        if (request.RegistrationAccessTokenFromSecrets is not null)
        {
            SecretResult registrationAccessTokenResult =
                await _secretProvider.GetSecretAsync(request.RegistrationAccessTokenFromSecrets);
            if (!registrationAccessTokenResult.SecretObtained)
            {
                string fullMessage =
                    $"Request specifies RegistrationAccessToken with Source {request.RegistrationAccessTokenFromSecrets.Source} " +
                    $"and Name {request.RegistrationAccessTokenFromSecrets.Name} which could not be obtained. {registrationAccessTokenResult.ErrorMessage}";
                throw new KeyNotFoundException(fullMessage);
            }
            requestRegistrationAccessToken = registrationAccessTokenResult.Secret!;
        }
        if (request.RegistrationAccessToken is not null)
        {
            requestRegistrationAccessToken = request.RegistrationAccessToken;
        }

        // Get and process software statement assertion
        string softwareStatementAssertion =
            await GetSoftwareStatementAssertion(softwareStatement, obSealKey, apiClient);
        SsaPayload ssaPayload = ProcessSsa(
            softwareStatementAssertion,
            softwareStatement,
            softwareStatementId);

        // Determine redirect URIs
        (IList<string> redirectUris, string defaultFragmentRedirectUri, string defaultQueryRedirectUri) =
            GetRedirectUris(
                softwareStatement,
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
                bankProfile.BankProfileEnum,
                customBehaviour?.OpenIdConfigurationGet);
        nonErrorMessages.AddRange(newNonErrorMessages1);

        // Determine registration endpoint
        string? registrationEndpoint = openIdConfiguration?.RegistrationEndpoint;
        bool useRegistrationEndpoint = bankProfile.BankConfigurationApiSettings.UseRegistrationEndpoint;
        if (useRegistrationEndpoint && registrationEndpoint is null)
        {
            throw new InvalidOperationException(
                "Registration endpoint required and not obtainable from OpenID Configuration for bank's IssuerUrl.");
        }

        // Determine token endpoint
        string tokenEndpoint =
            openIdConfiguration?.TokenEndpoint ??
            throw new InvalidOperationException(
                "Token endpoint not obtainable from OpenID Configuration for specified IssuerUrl.");

        // Determine auth endpoint
        string authorizationEndpoint =
            openIdConfiguration.AuthorizationEndpoint ??
            throw new InvalidOperationException(
                "Authorisation endpoint not obtainable from OpenID Configuration for specified IssuerUrl.");

        // Determine JWKS URI
        string jwksUri =
            openIdConfiguration.JwksUri ??
            throw new InvalidOperationException(
                "JWKS URI not obtainable from OpenID Configuration for specified IssuerUrl.");

        // Determine TokenEndpointAuthMethod
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        CheckTokenEndpointAuthMethod(
            tokenEndpointAuthMethod,
            supportsSca,
            openIdConfiguration.TokenEndpointAuthMethodsSupported);

        // Get re-usable existing bank registration if possible
        Func<string?, string?, string?, BankProfile, BankGroupEnum, Guid, TokenEndpointAuthMethodSupportedValues,
            RegistrationScopeEnum, IBankProfileService, (BankRegistrationEntity? existingRegistration,
            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> getExistingRegistration = bankGroup switch
        {
            BankGroupEnum.Barclays => GetExistingRegistration<BarclaysBank, BarclaysRegistrationGroup>,
            BankGroupEnum.Cooperative => GetExistingRegistration<CooperativeBank, CooperativeRegistrationGroup>,
            BankGroupEnum.Danske => GetExistingRegistration<DanskeBank, DanskeRegistrationGroup>,
            BankGroupEnum.Hsbc => GetExistingRegistration<HsbcBank, HsbcRegistrationGroup>,
            BankGroupEnum.Lloyds => GetExistingRegistration<LloydsBank, LloydsRegistrationGroup>,
            BankGroupEnum.Monzo => GetExistingRegistration<MonzoBank, MonzoRegistrationGroup>,
            BankGroupEnum.Nationwide =>
                GetExistingRegistration<NationwideBank, NationwideRegistrationGroup>,
            BankGroupEnum.NatWest => GetExistingRegistration<NatWestBank, NatWestRegistrationGroup>,
            BankGroupEnum.Obie => GetExistingRegistration<ObieBank, ObieRegistrationGroup>,
            BankGroupEnum.Revolut => GetExistingRegistration<RevolutBank, RevolutRegistrationGroup>,
            BankGroupEnum.Santander => GetExistingRegistration<SantanderBank, SantanderRegistrationGroup>,
            BankGroupEnum.Starling => GetExistingRegistration<StarlingBank, StarlingRegistrationGroup>,
            _ => throw new ArgumentOutOfRangeException()
        };
        (BankRegistrationEntity? existingGroupRegistration,
            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages2) = getExistingRegistration(
            request.ExternalApiId,
            requestExternalApiSecret,
            requestRegistrationAccessToken,
            bankProfile,
            bankGroup,
            softwareStatementId,
            tokenEndpointAuthMethod,
            registrationScope,
            _bankProfileService);
        nonErrorMessages.AddRange(nonErrorMessages2);

        // Obtain external (bank) API registration
        string externalApiId;
        string? externalApiSecret;
        string? registrationAccessToken;
        ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse;
        if (existingGroupRegistration is not null)
        {
            // Re-use existing external (bank) API registration
            externalApiId = existingGroupRegistration.ExternalApiId;
            externalApiSecret = requestExternalApiSecret; // has been checked to match existing registration
            registrationAccessToken = requestRegistrationAccessToken; // has been checked to match existing registration
            externalApiResponse = null;
        }
        else if (request.ExternalApiId is not null)
        {
            // Use supplied external (bank) API registration
            externalApiId = request.ExternalApiId;
            externalApiSecret = requestExternalApiSecret;
            registrationAccessToken = requestRegistrationAccessToken;
            externalApiResponse = null;
        }
        else
        {
            // Perform Dynamic Client Registration

            // Check endpoint
            if (!useRegistrationEndpoint)
            {
                throw new InvalidOperationException(
                    "DCR (dynamic client registration) via a registration endpoint is not used for this bank. " +
                    "Please create a registration (client) manually (most likely using the bank's developer portal) and then try again supplying " +
                    $"{nameof(request.ExternalApiId)} to specify the registration's client_id. If relevant, " +
                    $"{nameof(request.ExternalApiSecret)} and {nameof(request.RegistrationAccessToken)} for the registration should also be supplied.");
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
            SubjectDnOrgIdEncoding transportCertificateSubjectDnOrgIdEncoding =
                bankRegistrationPostCustomBehaviour?.TransportCertificateSubjectDnOrgIdEncoding ??
                SubjectDnOrgIdEncoding.StringAttributeType;
            string tlsClientAuthSubjectDn = obWacCertificate.SubjectDn[
                transportCertificateSubjectDnOrgIdEncoding];
            externalApiResponse = await PerformDynamicClientRegistration(
                bankRegistrationPostCustomBehaviour,
                dynamicClientRegistrationApiVersion,
                softwareStatement,
                apiClient,
                obSealKey,
                tlsClientAuthSubjectDn,
                bankProfile.BankProfileEnum,
                softwareStatementAssertion,
                registrationEndpoint!, // useRegistrationEndpoint is true
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
        var entity = new BankRegistrationEntity(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            null,
            null,
            null,
            tokenEndpointAuthMethod,
            bankGroup,
            softwareStatementId,
            "",
            null,
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
            registrationScope);

        // Add client secret
        if (externalApiSecret is not null)
        {
            ExternalApiSecretEntity clientSecretEntity = entity.AddNewClientSecret(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy);
            string? currentKeyId = _encryptionKeyInfo.GetCurrentKeyId();
            clientSecretEntity.UpdateClientSecret(
                externalApiSecret,
                entity.GetAssociatedData(),
                _encryptionKeyInfo.GetEncryptionKey(currentKeyId),
                utcNow,
                request.CreatedBy,
                currentKeyId);
        }

        // Add registration access token
        if (registrationAccessToken is not null)
        {
            RegistrationAccessTokenEntity registrationAccessTokenEntity = entity.AddNewRegistrationAccessToken(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy);
            string? currentKeyId = _encryptionKeyInfo.GetCurrentKeyId();
            registrationAccessTokenEntity.UpdateRegistrationAccessToken(
                registrationAccessToken,
                entity.GetAssociatedData(),
                _encryptionKeyInfo.GetEncryptionKey(currentKeyId),
                utcNow,
                request.CreatedBy,
                currentKeyId);
        }

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

        BankRegistrationResponse response = new()
        {
            Id = entity.Id,
            Created = entity.Created,
            CreatedBy = entity.CreatedBy,
            Reference = entity.Reference,
            ExternalApiResponse = externalApiResponse,
            SoftwareStatementId = entity.SoftwareStatementId!.Value,
            BankProfile = entity.BankProfile,
            JwksUri = entity.JwksUri,
            RegistrationEndpoint = entity.RegistrationEndpoint,
            TokenEndpoint = entity.TokenEndpoint,
            AuthorizationEndpoint = entity.AuthorizationEndpoint,
            RegistrationScope = entity.RegistrationScope,
            DefaultFragmentRedirectUri = entity.DefaultFragmentRedirectUri,
            DefaultQueryRedirectUri = entity.DefaultQueryRedirectUri,
            RedirectUris = entity.RedirectUris,
            ExternalApiId = entity.ExternalApiId,
            UseSimulatedBank = entity.UseSimulatedBank,
            DefaultResponseModeOverride = entity.DefaultResponseModeOverride,
            TokenEndpointAuthMethod = entity.TokenEndpointAuthMethod
        };

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
        BankRegistrationEntity entity =
            await _entityMethods
                .DbSetNoTracking
                .Include(o => o.SoftwareStatementNavigation)
                .Include(o => o.ExternalApiSecretsNavigation)
                .Include(o => o.RegistrationAccessTokensNavigation)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
            throw new KeyNotFoundException($"No record found for BankRegistration with ID {readParams.Id}.");
        string externalApiId = entity.ExternalApiId;
        SoftwareStatementEntity softwareStatement = entity.SoftwareStatementNavigation!;
        ExternalApiSecretEntity? externalApiSecret =
            entity.ExternalApiSecretsNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        RegistrationAccessTokenEntity? registrationAccessTokenEntity = entity.RegistrationAccessTokensNavigation
            .SingleOrDefault(x => !x.IsDeleted);

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(entity.BankProfile);
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
            bankProfile.DynamicClientRegistrationApiVersion;

        ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse;
        bool excludeExternalApiOperation =
            readParams.ExcludeExternalApiOperation ||
            !bankProfile.BankConfigurationApiSettings.UseRegistrationGetEndpoint;
        if (!excludeExternalApiOperation)
        {
            string registrationEndpoint =
                entity.RegistrationEndpoint ??
                throw new InvalidOperationException(
                    "BankRegistration does not have a registration endpoint configured.");

            // Get IApiClient
            IApiClient apiClient = entity.UseSimulatedBank
                ? bankProfile.ReplayApiClient
                : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

            // Get OBSeal key
            OBSealKey obSealKey =
                (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

            // Get client credentials grant access token if necessary
            string accessToken;
            bool useRegistrationAccessToken =
                bankProfile.BankConfigurationApiSettings.UseRegistrationAccessToken;
            if (useRegistrationAccessToken)
            {
                if (registrationAccessTokenEntity is null)
                {
                    throw new InvalidOperationException("No registration access token is available.");
                }

                // Extract registration access token
                accessToken = registrationAccessTokenEntity
                    .GetRegistrationAccessToken(
                        entity.GetAssociatedData(),
                        _encryptionKeyInfo.GetEncryptionKey(registrationAccessTokenEntity.KeyId));
            }
            else
            {
                string? scope = customBehaviour?.BankRegistrationPut?.CustomTokenScope;
                accessToken =
                    await _clientAccessTokenGet.GetAccessToken(
                        scope,
                        obSealKey,
                        entity,
                        externalApiSecret,
                        customBehaviour?.ClientCredentialsGrantPost,
                        apiClient,
                        bankProfile.BankProfileEnum);
            }

            // Read object from external API
            JsonSerializerSettings? responseJsonSerializerSettings =
                GetResponseJsonSerializerSettings(customBehaviour?.BankRegistrationPost);
            IApiGetRequests<ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
                ApiRequests(
                    dynamicClientRegistrationApiVersion,
                    obSealKey,
                    false, // not used for GET
                    accessToken);
            var externalApiUrl = new Uri(registrationEndpoint.TrimEnd('/') + $"/{externalApiId}");
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription = "GET {RegistrationEndpoint}/{ClientId}",
                BankProfile = bankProfile.BankProfileEnum
            };

            (externalApiResponse, string? xFapiInteractionId,
                    IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.GetAsync(
                    externalApiUrl,
                    null,
                    tppReportingRequestInfo,
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

        BankRegistrationResponse response = new()
        {
            Id = entity.Id,
            Created = entity.Created,
            CreatedBy = entity.CreatedBy,
            Reference = entity.Reference,
            ExternalApiResponse = externalApiResponse,
            SoftwareStatementId = entity.SoftwareStatementId!.Value,
            BankProfile = entity.BankProfile,
            JwksUri = entity.JwksUri,
            RegistrationEndpoint = entity.RegistrationEndpoint,
            TokenEndpoint = entity.TokenEndpoint,
            AuthorizationEndpoint = entity.AuthorizationEndpoint,
            RegistrationScope = entity.RegistrationScope,
            DefaultFragmentRedirectUri = entity.DefaultFragmentRedirectUri,
            DefaultQueryRedirectUri = entity.DefaultQueryRedirectUri,
            RedirectUris = entity.RedirectUris,
            ExternalApiId = entity.ExternalApiId,
            UseSimulatedBank = entity.UseSimulatedBank,
            DefaultResponseModeOverride = entity.DefaultResponseModeOverride,
            TokenEndpointAuthMethod = entity.TokenEndpointAuthMethod
        };

        return (response, nonErrorMessages);
    }

    private (BankRegistrationEntity? existingRegistration, IList<IFluentResponseInfoOrWarningMessage>
        nonErrorMessages) GetExistingRegistration<TBank, TRegistrationGroup>(
            string? requestExternalApiId,
            string? requestExternalApiSecret,
            string? requestRegistrationAccessToken,
            BankProfile bankProfile,
            BankGroupEnum bankGroupEnum,
            Guid softwareStatementId,
            TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
            RegistrationScopeEnum registrationScope,
            IBankProfileService bankProfileService)
        where TBank : struct, Enum
        where TRegistrationGroup : struct, Enum
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        IBankGroup<TBank, TRegistrationGroup> bankGroup =
            bankProfileService.GetBankGroup<TBank, TRegistrationGroup>(bankGroupEnum);
        TBank bank = bankGroup.GetBank(bankProfile.BankProfileEnum);
        TRegistrationGroup registrationGroup = bankGroup.GetRegistrationGroup(bank, registrationScope);
        var registrationGroupString = registrationGroup.ToString()!;

        // Get existing registrations with same software statement, bank group, and bank registration group
        IEnumerable<BankRegistrationEntity> existingRegistrationsFirstPass =
            _entityMethods
                .DbSetNoTracking
                .Include(x => x.ExternalApiSecretsNavigation)
                .Include(x => x.RegistrationAccessTokensNavigation)
                .AsSplitQuery()
                .Where(
                    x => x.SoftwareStatementId == softwareStatementId &&
                         x.BankGroup == bankGroupEnum)
                .OrderByDescending(x => x.Created) // most recent first
                .AsEnumerable(); // force next where clause to run client-side as cannot be translated
        List<BankRegistrationEntity> existingRegistrations = existingRegistrationsFirstPass
            .Where(
                x => EqualityComparer<TRegistrationGroup>.Default.Equals(
                    bankGroup.GetRegistrationGroup(bankGroup.GetBank(x.BankProfile), x.RegistrationScope),
                    registrationGroup))
            .ToList();

        // Check first registration in same registration group for compatibility (error if not compatible)
        BankRegistrationEntity? existingRegistration = null;
        if (existingRegistrations.Any())
        {
            existingRegistration = existingRegistrations.First();
            ExternalApiSecretEntity? externalApiSecretEntity =
                existingRegistration.ExternalApiSecretsNavigation
                    .SingleOrDefault(x => !x.IsDeleted);
            RegistrationAccessTokenEntity? registrationAccessTokenEntity = existingRegistration
                .RegistrationAccessTokensNavigation
                .SingleOrDefault(x => !x.IsDeleted);

            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages2 =
                CheckExistingRegistrationCompatible(
                    existingRegistration,
                    externalApiSecretEntity,
                    registrationAccessTokenEntity,
                    requestExternalApiId,
                    requestExternalApiSecret,
                    requestRegistrationAccessToken,
                    registrationGroupString,
                    tokenEndpointAuthMethod,
                    registrationScope);
            nonErrorMessages.AddRange(nonErrorMessages2);

            int numberOfExistingRegistrations = existingRegistrations.Count();
            if (numberOfExistingRegistrations > 1)
            {
                string warningMessage =
                    $"More than one (in fact {numberOfExistingRegistrations}) previous registrations were found for " +
                    $"BankRegistrationGroup {registrationGroupString}. The most recently created of these will be re-used. " +
                    $"Normally we expect to find only one previous registration for a given registration group.";
                nonErrorMessages.Add(new FluentResponseWarningMessage(warningMessage));
                _instrumentationClient.Warning(warningMessage);
            }
        }

        return (existingRegistration, nonErrorMessages);
    }

    private IList<IFluentResponseInfoOrWarningMessage> CheckExistingRegistrationCompatible(
        BankRegistrationEntity existingRegistration,
        ExternalApiSecretEntity? externalApiSecretEntity,
        RegistrationAccessTokenEntity? registrationAccessTokenEntity,
        string? requestExternalApiId,
        string? requestExternalApiSecret,
        string? requestRegistrationAccessToken,
        string registrationGroupString,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        RegistrationScopeEnum registrationScope)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // If request contains external API ID, check details match existing registration
        if (requestExternalApiId is not null)
        {
            // Check ExternalApiId
            if (requestExternalApiId != existingRegistration.ExternalApiId)
            {
                throw new
                    InvalidOperationException(
                        $"Previous registration for BankRegistrationGroup {registrationGroupString} " +
                        $"used ExternalApiId {existingRegistration.ExternalApiId} " +
                        $"which is different from specified {requestExternalApiId}.");
            }

            // Check ExternalApiSecret
            string? existingRegExternalApiSecret = null;
            if (externalApiSecretEntity is not null)
            {
                existingRegExternalApiSecret = externalApiSecretEntity
                    .GetClientSecret(
                        existingRegistration.GetAssociatedData(),
                        _encryptionKeyInfo.GetEncryptionKey(externalApiSecretEntity.KeyId));
            }

            if (requestExternalApiSecret != existingRegExternalApiSecret)
            {
                throw new
                    InvalidOperationException(
                        $"Previous registration for BankRegistrationGroup {registrationGroupString} " +
                        $"used ExternalApiSecret " +
                        $"which is different from specified ExternalApiSecret.");
            }

            // Check RegistrationAccessToken
            string? existingRegRegistrationAccessToken = null;
            if (registrationAccessTokenEntity is not null)
            {
                existingRegRegistrationAccessToken = registrationAccessTokenEntity
                    .GetRegistrationAccessToken(
                        existingRegistration.GetAssociatedData(),
                        _encryptionKeyInfo.GetEncryptionKey(registrationAccessTokenEntity.KeyId));
            }

            if (requestRegistrationAccessToken != existingRegRegistrationAccessToken)
            {
                throw new
                    InvalidOperationException(
                        $"Previous registration for BankRegistrationGroup {registrationGroupString} " +
                        $"used RegistrationAccessToken " +
                        $"which is different from specified RegistrationAccessToken.");
            }
        }

        // TODO: compare redirect URLs?

        if (tokenEndpointAuthMethod !=
            existingRegistration.TokenEndpointAuthMethod)
        {
            throw new
                InvalidOperationException(
                    $"Previous registration for BankRegistrationGroup {registrationGroupString} " +
                    $"used TokenEndpointAuthMethod {existingRegistration.TokenEndpointAuthMethod} " +
                    $"which is different from expected {tokenEndpointAuthMethod}.");
        }

        if (registrationScope != existingRegistration.RegistrationScope)
        {
            throw new
                InvalidOperationException(
                    $"Previous registration for BankRegistrationGroup {registrationGroupString} " +
                    $"used RegistrationScope {existingRegistration.RegistrationScope} " +
                    $"which is different from expected {registrationScope}.");
        }

        return nonErrorMessages;
    }

    private async Task<string> GetSoftwareStatementAssertion(
        SoftwareStatementEntity softwareStatement,
        OBSealKey obSealKey,
        IApiClient apiClient)
    {
        // Get access token
        var scope = "ASPSPReadAccess TPPReadAccess AuthoritiesReadAccess";
        string tokenEndpoint = softwareStatement.SandboxEnvironment
            ? "https://matls-sso.openbankingtest.org.uk/as/token.oauth2"
            : "https://matls-sso.openbanking.org.uk/as/token.oauth2";
        string accessToken = (await _grantPost.PostClientCredentialsGrantAsync(
            scope,
            obSealKey,
            TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt,
            tokenEndpoint,
            softwareStatement.SoftwareId,
            null,
            null,
            null,
            apiClient,
            null,
            new Dictionary<string, JsonNode?> { ["scope"] = scope },
            true,
            JwsAlgorithm.RS256)).AccessToken;

        // Get software statement assertion
        var headers = new List<HttpHeader>
        {
            new("Authorization", "Bearer " + accessToken),
            new("Accept", "application/jws+json")
        };
        string url = softwareStatement.SandboxEnvironment
            ? "https://matls-ssaapi.openbankingtest.org.uk/api/v2/tpp/"
            : "https://matls-ssaapi.openbanking.org.uk/api/v2/tpp/";
        url += $"{softwareStatement.OrganisationId}/ssa/{softwareStatement.SoftwareId}";
        string response = await new HttpRequestBuilder()
            .SetUri(url)
            .SetHeaders(headers)
            .Create()
            .SendExpectingStringResponseAsync(null, apiClient);

        return response;
    }

    private SsaPayload ProcessSsa(
        string ssa,
        SoftwareStatementEntity softwareStatement,
        Guid id)
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
        if (softwareStatement.DefaultQueryRedirectUrl is not null &&
            !ssaPayload.SoftwareRedirectUris.Contains(softwareStatement.DefaultQueryRedirectUrl))
        {
            throw new ArgumentException(
                $"Software statement with ID {id} contains DefaultQueryRedirectUrl {softwareStatement.DefaultQueryRedirectUrl} " +
                "which is not included in software statement assertion software_redirect_uris field.");
        }

        // Check DefaultFragmentRedirectUrl
        if (softwareStatement.DefaultFragmentRedirectUrl is not null &&
            !ssaPayload.SoftwareRedirectUris.Contains(softwareStatement.DefaultFragmentRedirectUrl))
        {
            throw new ArgumentException(
                $"Software statement with ID {id} contains DefaultFragmentRedirectUrl {softwareStatement.DefaultFragmentRedirectUrl} " +
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
        SoftwareStatementEntity softwareStatement,
        IApiClient apiClient,
        OBSealKey obSealKey,
        string tlsClientAuthSubjectDn,
        BankProfileEnum bankProfile,
        string softwareStatementAssertion,
        string registrationEndpoint,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
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
                obSealKey,
                useApplicationJoseNotApplicationJwtContentTypeHeader,
                string.Empty // not used for POST
            );
        var externalApiUrl = new Uri(registrationEndpoint);
        ClientRegistrationModelsPublic.OBClientRegistration1 externalApiRequest =
            RegistrationClaimsFactory.CreateRegistrationClaims(
                tokenEndpointAuthMethod,
                redirectUris,
                softwareStatement.SoftwareId,
                tlsClientAuthSubjectDn,
                softwareStatementAssertion,
                registrationScope,
                bankRegistrationPostCustomBehaviour,
                bankFinancialId);
        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription = "POST {RegistrationEndpoint}",
            BankProfile = bankProfile
        };

        (externalApiResponse, string? xFapiInteractionId,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.PostAsync(
                externalApiUrl,
                null,
                externalApiRequest,
                tppReportingRequestInfo,
                requestJsonSerializerSettings,
                responseJsonSerializerSettings,
                apiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);
        return externalApiResponse;
    }

    private static void CheckTokenEndpointAuthMethod(
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        bool supportsSca,
        IList<TokenEndpointAuthMethodOpenIdConfiguration>? tokenEndpointAuthMethodsSupported)
    {
        if (tokenEndpointAuthMethodsSupported is { } methodsSupported)
        {
            var methodsSupportedFilter = new List<TokenEndpointAuthMethodSupportedValues>();
            if (methodsSupported.Contains(TokenEndpointAuthMethodOpenIdConfiguration.TlsClientAuth))
            {
                methodsSupportedFilter.Add(TokenEndpointAuthMethodSupportedValues.TlsClientAuth);
            }

            if (methodsSupported.Contains(TokenEndpointAuthMethodOpenIdConfiguration.PrivateKeyJwt))
            {
                methodsSupportedFilter.Add(TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt);
            }

            if (methodsSupported.Contains(TokenEndpointAuthMethodOpenIdConfiguration.ClientSecretBasic))
            {
                methodsSupportedFilter.Add(TokenEndpointAuthMethodSupportedValues.ClientSecretBasic);
            }

            if (methodsSupported.Contains(TokenEndpointAuthMethodOpenIdConfiguration.ClientSecretPost))
            {
                methodsSupportedFilter.Add(TokenEndpointAuthMethodSupportedValues.ClientSecretPost);
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
            SoftwareStatementEntity softwareStatement,
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
            if (!redirectUris.Contains(softwareStatement.DefaultFragmentRedirectUrl))
            {
                throw new InvalidOperationException(
                    $"Default fragment redirect URI {softwareStatement.DefaultFragmentRedirectUrl} " +
                    $"from software statement profile not included in specified RedirectUris. " +
                    $"Please specify a different one or include this one in specified RedirectUris.");
            }
            defaultFragmentRedirectUri =
                softwareStatement.DefaultFragmentRedirectUrl;
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
            if (!redirectUris.Contains(softwareStatement.DefaultQueryRedirectUrl))
            {
                throw new InvalidOperationException(
                    $"Default query redirect URI {softwareStatement.DefaultQueryRedirectUrl} " +
                    $"from software statement profile not included in specified RedirectUris. " +
                    $"Please specify a different one or include this one in specified RedirectUris.");
            }
            defaultQueryRedirectUri =
                softwareStatement.DefaultQueryRedirectUrl;
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
        OBSealKey obSealKey,
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
                            obSealKey,
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
                            obSealKey,
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
                            obSealKey,
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
