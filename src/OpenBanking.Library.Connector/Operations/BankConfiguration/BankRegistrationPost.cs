// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Bank = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.Bank;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;
using BankRegistrationRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request.BankRegistration;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using ClientRegistrationModelsV3p2 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p2.Models;
using ClientRegistrationModelsV3p1 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class
        BankRegistrationPost : EntityPost<
            BankRegistration,
            BankRegistrationRequest, BankRegistrationResponse, ClientRegistrationModelsPublic.OBClientRegistration1,
            ClientRegistrationModelsPublic.OBClientRegistration1Response, BankRegistrationCreateParams>
    {
        private readonly IDbReadOnlyEntityMethods<Bank> _bankMethods;
        private readonly IBankProfileDefinitions _bankProfileDefinitions;
        private readonly IOpenIdConfigurationRead _configurationRead;

        public BankRegistrationPost(
            IDbReadWriteEntityMethods<BankRegistration> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IOpenIdConfigurationRead configurationRead,
            IDbReadOnlyEntityMethods<Bank> bankMethods,
            IBankProfileDefinitions bankProfileDefinitions) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper)
        {
            _bankMethods = bankMethods;
            _bankProfileDefinitions = bankProfileDefinitions;
            _configurationRead = configurationRead;
        }

        protected override async
            Task<(BankRegistrationResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPost(
                BankRegistrationRequest request,
                BankRegistrationCreateParams createParams)
        {
            // Create non-error list
            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            BankProfile? bankProfile = null;
            if (request.BankProfile is not null)
            {
                bankProfile = _bankProfileDefinitions.GetBankProfile(request.BankProfile.Value);
            }

            OAuth2ResponseMode defaultResponseMode =
                request.DefaultResponseMode ??
                bankProfile?.DefaultResponseMode ??
                throw new InvalidOperationException(
                    "DefaultResponseMode specified as null and cannot be obtained using specified BankProfile.");

            // Load bank from DB and check for existing bank registrations
            Guid bankId = request.BankId;
            Bank bank =
                await _bankMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == bankId) ??
                throw new KeyNotFoundException($"No record found for BankId {bankId} specified by request.");
            CustomBehaviourClass? customBehaviour = bank.CustomBehaviour;
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
                bank.DcrApiVersion;
            string registrationEndpoint = bank.RegistrationEndpoint;

            // Check for existing bank registration
            BankRegistration? existingRegistration =
                await _entityMethods
                    .DbSet
                    .Where(x => x.BankId == bankId)
                    .FirstOrDefaultAsync();
            if (!(existingRegistration is null) &&
                !request.AllowMultipleRegistrations)
            {
                throw new InvalidOperationException(
                    "There is already at least one registration for this bank. Set AllowMultipleRegistrations to force creation of an additional registration.");
            }

            // Load processed software statement profile
            string softwareStatementProfileId = request.SoftwareStatementProfileId;
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    softwareStatementProfileId,
                    request.SoftwareStatementAndCertificateProfileOverrideCase);

            List<string> softwareStatementRedirectUris =
                processedSoftwareStatementProfile.SoftwareStatementPayload.SoftwareRedirectUris;

            if (request.DefaultRedirectUri is not null)
            {
                if (!softwareStatementRedirectUris.Contains(request.DefaultRedirectUri))
                {
                    throw new InvalidOperationException(
                        $"Specified default redirect URI {request.DefaultRedirectUri} not included in software statement.");
                }
            }

            string defaultRedirectUri =
                request.DefaultRedirectUri ??
                processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;

            List<string> otherRedirectUris;
            if (request.OtherRedirectUris is not null)
            {
                foreach (string redirectUri in request.OtherRedirectUris)
                {
                    if (!softwareStatementRedirectUris.Contains(redirectUri))
                    {
                        throw new InvalidOperationException(
                            $"Specified other redirect URI {redirectUri} not included in software statement.");
                    }
                }

                otherRedirectUris = request.OtherRedirectUris;
            }
            else
            {
                otherRedirectUris = softwareStatementRedirectUris;
            }

            otherRedirectUris.Remove(defaultRedirectUri);
            List<string> defaultPlusOtherRedirectUris = new(otherRedirectUris) { defaultRedirectUri };

            // Determine registration scope
            RegistrationScopeEnum registrationScope =
                request.RegistrationScope ??
                processedSoftwareStatementProfile.SoftwareStatementPayload.RegistrationScope;
            RegistrationScopeIsValid? registrationScopeIsValidFcn =
                bankProfile?.BankConfigurationApiSettings.RegistrationScopeIsValid;
            if (registrationScopeIsValidFcn is not null &&
                !registrationScopeIsValidFcn(registrationScope))
            {
                throw new InvalidOperationException(
                    "RegistrationScope fails RegistrationScopeIsValid check from BankProfile");
            }

            // Get OpenID Provider Configuration if issuer URL available and determine endpoints appropriately
            Uri? openIdConfigurationUrl = null;
            if (customBehaviour?.OpenIdConfigurationGet?.EndpointUnavailable is not true)
            {
                openIdConfigurationUrl = new Uri(
                    customBehaviour?.OpenIdConfigurationGet?.Url
                    ?? string.Join("/", bank.IssuerUrl.TrimEnd('/'), ".well-known/openid-configuration"));
            }

            OpenIdConfiguration? openIdConfiguration;
            TokenEndpointAuthMethod tokenEndpointAuthMethod;
            if (openIdConfigurationUrl is null)
            {
                tokenEndpointAuthMethod =
                    request.TokenEndpointAuthMethod ??
                    throw new ArgumentNullException(
                        "TokenEndpointAuthMethod specified as null and cannot be obtained using specified IssuerUrl (also null).");
            }
            else
            {
                (openIdConfiguration, IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                    await _configurationRead.GetOpenIdConfigurationAsync(
                        openIdConfigurationUrl,
                        customBehaviour?.OpenIdConfigurationGet);
                nonErrorMessages.AddRange(newNonErrorMessages);


                // Select tokenEndpointAuthMethod based on most preferred
                if (request.TokenEndpointAuthMethod is null)
                {
                    if (openIdConfiguration.TokenEndpointAuthMethodsSupported.Contains(
                            OpenIdConfigurationTokenEndpointAuthMethodEnum.TlsClientAuth))
                    {
                        tokenEndpointAuthMethod = TokenEndpointAuthMethod.TlsClientAuth;
                    }
                    else if (openIdConfiguration.TokenEndpointAuthMethodsSupported.Contains(
                                 OpenIdConfigurationTokenEndpointAuthMethodEnum.PrivateKeyJwt))
                    {
                        tokenEndpointAuthMethod = TokenEndpointAuthMethod.PrivateKeyJwt;
                    }
                    else if (openIdConfiguration.TokenEndpointAuthMethodsSupported.Contains(
                                 OpenIdConfigurationTokenEndpointAuthMethodEnum.ClientSecretBasic))
                    {
                        tokenEndpointAuthMethod = TokenEndpointAuthMethod.ClientSecretBasic;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(
                            $"No supported value in {openIdConfiguration.TokenEndpointAuthMethodsSupported}");
                    }
                }
                else
                {
                    tokenEndpointAuthMethod = request.TokenEndpointAuthMethod.Value;
                }
            }

            // Set external (bank) API parameters via DCR or directly
            string externalApiId;
            string? externalApiSecret;
            string? registrationAccessToken;
            ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse = null;
            if (request.ExternalApiObject is null)
            {
                // Get DCR claims
                ClientRegistrationModelsPublic.OBClientRegistration1 apiRequest =
                    RegistrationClaimsFactory.CreateRegistrationClaims(
                        tokenEndpointAuthMethod,
                        defaultPlusOtherRedirectUris,
                        processedSoftwareStatementProfile,
                        registrationScope,
                        customBehaviour?.BankRegistrationPost,
                        bank);

                // Get DCR URL
                var uri = new Uri(registrationEndpoint);

                // Create request serialiser settings.
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

                // Create response serialiser settings.
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

                // Create new Open Banking registration by processing override or posting JWT
                bool useApplicationJoseNotApplicationJwtContentTypeHeader =
                    customBehaviour?.BankRegistrationPost
                        ?.UseApplicationJoseNotApplicationJwtContentTypeHeader ?? false;
                IApiPostRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                    ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
                    dynamicClientRegistrationApiVersion switch
                    {
                        DynamicClientRegistrationApiVersion.Version3p1 =>
                            new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                                ClientRegistrationModelsPublic.OBClientRegistration1Response,
                                ClientRegistrationModelsV3p1.OBClientRegistration1,
                                ClientRegistrationModelsV3p1.OBClientRegistration1>(
                                new JwtRequestProcessor<ClientRegistrationModelsV3p1.OBClientRegistration1>(
                                    processedSoftwareStatementProfile,
                                    _instrumentationClient,
                                    useApplicationJoseNotApplicationJwtContentTypeHeader),
                                new JwtRequestProcessor<ClientRegistrationModelsV3p1.OBClientRegistration1>(
                                    processedSoftwareStatementProfile,
                                    _instrumentationClient,
                                    useApplicationJoseNotApplicationJwtContentTypeHeader)),
                        DynamicClientRegistrationApiVersion.Version3p2 =>
                            new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                                ClientRegistrationModelsPublic.OBClientRegistration1Response,
                                ClientRegistrationModelsV3p2.OBClientRegistration1,
                                ClientRegistrationModelsV3p2.OBClientRegistration1>(
                                new JwtRequestProcessor<ClientRegistrationModelsV3p2.OBClientRegistration1>(
                                    processedSoftwareStatementProfile,
                                    _instrumentationClient,
                                    useApplicationJoseNotApplicationJwtContentTypeHeader),
                                new JwtRequestProcessor<ClientRegistrationModelsV3p2.OBClientRegistration1>(
                                    processedSoftwareStatementProfile,
                                    _instrumentationClient,
                                    useApplicationJoseNotApplicationJwtContentTypeHeader)),
                        DynamicClientRegistrationApiVersion.Version3p3 =>
                            new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                                ClientRegistrationModelsPublic.OBClientRegistration1Response,
                                ClientRegistrationModelsPublic.OBClientRegistration1,
                                ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                                new JwtRequestProcessor<ClientRegistrationModelsPublic.OBClientRegistration1>(
                                    processedSoftwareStatementProfile,
                                    _instrumentationClient,
                                    useApplicationJoseNotApplicationJwtContentTypeHeader),
                                new JwtRequestProcessor<ClientRegistrationModelsPublic.OBClientRegistration1>(
                                    processedSoftwareStatementProfile,
                                    _instrumentationClient,
                                    useApplicationJoseNotApplicationJwtContentTypeHeader)),
                        _ => throw new ArgumentOutOfRangeException(
                            nameof(dynamicClientRegistrationApiVersion),
                            dynamicClientRegistrationApiVersion,
                            null)
                    };

                (ClientRegistrationModelsPublic.OBClientRegistration1Response apiResponse,
                        IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages2) =
                    await EntityPostCommon(
                        apiRequest,
                        apiRequests,
                        processedSoftwareStatementProfile.ApiClient,
                        uri,
                        requestJsonSerializerSettings,
                        responseJsonSerializerSettings,
                        nonErrorMessages);
                nonErrorMessages = nonErrorMessages2;

                externalApiResponse = apiResponse;
                externalApiId = externalApiResponse.ClientId;
                externalApiSecret = externalApiResponse.ClientSecret;
                registrationAccessToken = externalApiResponse.RegistrationAccessToken;
            }
            else
            {
                externalApiId = request.ExternalApiObject.ExternalApiId;
                externalApiSecret = request.ExternalApiObject.ExternalApiSecret;
                registrationAccessToken = request.ExternalApiObject.RegistrationAccessToken;
            }

            // Create persisted entity
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new BankRegistration(
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

            // Create response (may involve additional processing based on entity)
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
                entity.DefaultResponseMode);

            return (response, nonErrorMessages);
        }
    }
}
