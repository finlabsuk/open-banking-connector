// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
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
            BankRegistrationRequest, BankRegistrationReadResponse, ClientRegistrationModelsPublic.OBClientRegistration1,
            ClientRegistrationModelsPublic.OBClientRegistration1Response>
    {
        private readonly IApiClient _apiClient;
        private readonly IDbReadOnlyEntityMethods<Bank> _bankMethods;
        private readonly IBankProfileDefinitions _bankProfileDefinitions;

        public BankRegistrationPost(
            IDbReadWriteEntityMethods<BankRegistration> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IApiClient apiClient,
            IDbReadOnlyEntityMethods<Bank> bankMethods,
            IBankProfileDefinitions bankProfileDefinitions) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper)
        {
            _apiClient = apiClient;
            _bankMethods = bankMethods;
            _bankProfileDefinitions = bankProfileDefinitions;
        }

        private async Task<OpenIdConfiguration> GetOpenIdConfigurationAsync(string issuerUrl)
        {
            var uri = new Uri(string.Join("/", issuerUrl.TrimEnd('/'), ".well-known/openid-configuration"));

            return await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(uri)
                .Create()
                .RequestJsonAsync<OpenIdConfiguration>(_apiClient);
        }

        protected override async
            Task<(BankRegistrationReadResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPost(PostRequestInfo requestInfo)
        {
            // Create non-error list
            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            if (requestInfo.Request.BankProfile is not null)
            {
                requestInfo.Request = _bankProfileDefinitions.GetBankProfile(requestInfo.Request.BankProfile.Value)
                    .GetBankRegistrationRequest(
                        requestInfo.Request.BankId,
                        requestInfo.Request.SoftwareStatementProfileId,
                        requestInfo.Request.SoftwareStatementAndCertificateProfileOverrideCase,
                        requestInfo.Request.RegistrationScope,
                        requestInfo.Request.CustomBehaviour);
            }

            // Load bank from DB and check for existing bank registrations
            Guid bankId = requestInfo.Request.BankId;
            Bank bank =
                await _bankMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == bankId) ??
                throw new KeyNotFoundException($"No record found for BankId {bankId} specified by request.");
            BankRegistration? existingRegistration =
                await _entityMethods
                    .DbSet
                    .Where(x => x.BankId == bankId)
                    .FirstOrDefaultAsync();
            if (!(existingRegistration is null) &&
                !requestInfo.Request.AllowMultipleRegistrations)
            {
                throw new InvalidOperationException(
                    "There is already at least one registration for this bank. Set AllowMultipleRegistrations to force creation of an additional registration.");
            }

            // Load processed software statement profile
            string softwareStatementProfileId = requestInfo.Request.SoftwareStatementProfileId;
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    softwareStatementProfileId,
                    requestInfo.Request.SoftwareStatementAndCertificateProfileOverrideCase);

            // Determine registration scope
            RegistrationScopeEnum registrationScope =
                requestInfo.Request.RegistrationScope ??
                processedSoftwareStatementProfile.SoftwareStatementPayload.RegistrationScope;

            // Determine DCR version
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
                requestInfo.Request.DynamicClientRegistrationApiVersion;

            // Get OpenID Provider Configuration if issuer URL available and determine endpoints appropriately
            string? issuerUrl = requestInfo.Request.IssuerUrl;
            OpenIdConfiguration? openIdConfiguration;
            string registrationEndpoint;
            string tokenEndpoint;
            string authorizationEndpoint;
            TokenEndpointAuthMethod tokenEndpointAuthMethod;
            if (issuerUrl is null)
            {
                // Determine endpoints
                registrationEndpoint =
                    requestInfo.Request.RegistrationEndpoint ??
                    throw new ArgumentNullException(
                        "RegistrationEndpoint specified as null and cannot be obtained using specified IssuerUrl (also null).");
                tokenEndpoint =
                    requestInfo.Request.TokenEndpoint ??
                    throw new ArgumentNullException(
                        "TokenEndpoint specified as null and cannot be obtained using specified IssuerUrl (also null).");
                authorizationEndpoint =
                    requestInfo.Request.AuthorizationEndpoint ??
                    throw new ArgumentNullException(
                        "AuthorizationEndpoint specified as null and cannot be obtained using specified IssuerUrl (also null).");
                tokenEndpointAuthMethod =
                    requestInfo.Request.TokenEndpointAuthMethod ??
                    throw new ArgumentNullException(
                        "TokenEndpointAuthMethod specified as null and cannot be obtained using specified IssuerUrl (also null).");
            }
            else
            {
                openIdConfiguration = await GetOpenIdConfigurationAsync(issuerUrl);

                // Update OpenID Provider Configuration based on overrides
                IList<OAuth2ResponseMode>? responseModesSupportedOverride =
                    requestInfo.Request.CustomBehaviour?.OpenIdConfigurationGet?.ResponseModesSupportedResponse;
                if (!(responseModesSupportedOverride is null))
                {
                    openIdConfiguration.ResponseModesSupported = responseModesSupportedOverride;
                }

                IList<OpenIdConfigurationTokenEndpointAuthMethodEnum>? tokenEndpointAuthMethodsSupportedOverride =
                    requestInfo.Request.CustomBehaviour?.OpenIdConfigurationGet
                        ?.TokenEndpointAuthMethodsSupportedResponse;
                if (!(tokenEndpointAuthMethodsSupportedOverride is null))
                {
                    openIdConfiguration.TokenEndpointAuthMethodsSupported = tokenEndpointAuthMethodsSupportedOverride;
                }

                // Validate OpenID Connect configuration
                {
                    IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages =
                        new OpenBankingOpenIdConfigurationResponseValidator()
                            .Validate(openIdConfiguration)
                            .ProcessValidationResultsAndRaiseErrors(messagePrefix: "prefix");
                    nonErrorMessages.AddRange(newNonErrorMessages);
                }

                // Determine endpoints
                registrationEndpoint =
                    requestInfo.Request.RegistrationEndpoint ?? openIdConfiguration.RegistrationEndpoint;
                tokenEndpoint =
                    requestInfo.Request.TokenEndpoint ?? openIdConfiguration.TokenEndpoint;
                authorizationEndpoint =
                    requestInfo.Request.AuthorizationEndpoint ?? openIdConfiguration.AuthorizationEndpoint;

                // Select tokenEndpointAuthMethod based on most preferred
                if (requestInfo.Request.TokenEndpointAuthMethod is null)
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
                    tokenEndpointAuthMethod = requestInfo.Request.TokenEndpointAuthMethod.Value;
                }
            }

            // Set external (bank) API parameters via DCR or directly
            string externalApiId;
            string? externalApiSecret;
            string? registrationAccessToken;
            ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse = null;
            if (requestInfo.Request.ExternalApiObject is null)
            {
                // Get DCR claims
                ClientRegistrationModelsPublic.OBClientRegistration1 apiRequest =
                    RegistrationClaimsFactory.CreateRegistrationClaims(
                        tokenEndpointAuthMethod,
                        processedSoftwareStatementProfile,
                        registrationScope,
                        requestInfo.Request.CustomBehaviour?.BankRegistrationPost,
                        bank.FinancialId);

                // Get DCR URL
                var uri = new Uri(registrationEndpoint);

                // Create request serialiser settings.
                JsonSerializerSettings? requestJsonSerializerSettings = null;
                if (!(requestInfo.Request.CustomBehaviour?.BankRegistrationPost is null))
                {
                    var optionsDict = new Dictionary<JsonConverterLabel, int>();
                    DelimitedStringConverterOptions? scopeClaimJsonConverter = requestInfo.Request.CustomBehaviour
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
                if (!(requestInfo.Request.CustomBehaviour?.BankRegistrationPost is null))
                {
                    var optionsDict = new Dictionary<JsonConverterLabel, int>();

                    DateTimeOffsetConverter? clientIdIssuedAtClaimResponseJsonConverter = requestInfo.Request
                        .CustomBehaviour
                        .BankRegistrationPost
                        .ClientIdIssuedAtClaimResponseJsonConverter;
                    if (clientIdIssuedAtClaimResponseJsonConverter is not null)
                    {
                        optionsDict.Add(
                            JsonConverterLabel.DcrRegClientIdIssuedAt,
                            (int) clientIdIssuedAtClaimResponseJsonConverter);
                    }

                    DelimitedStringConverterOptions? scopeClaimJsonConverter = requestInfo.Request.CustomBehaviour
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
                    requestInfo.Request.CustomBehaviour?.BankRegistrationPost
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
                        requestInfo,
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
                externalApiId = requestInfo.Request.ExternalApiObject.ExternalApiId;
                externalApiSecret = requestInfo.Request.ExternalApiObject.ExternalApiSecret;
                registrationAccessToken = requestInfo.Request.ExternalApiObject.RegistrationAccessToken;
            }

            // Create persisted CustomBehaviour preserving only things actually required for future activities involving this
            // registration
            var customBehaviourPersisted = new CustomBehaviourClass
            {
                BankRegistrationPost =
                    requestInfo.Request.CustomBehaviour?.BankRegistrationPost,
                AccountAccessConsentAuthGet =
                    requestInfo.Request.CustomBehaviour?.AccountAccessConsentAuthGet,
                DomesticPaymentConsentAuthGet =
                    requestInfo.Request.CustomBehaviour?.DomesticPaymentConsentAuthGet,
                DomesticVrpConsentAuthGet =
                    requestInfo.Request.CustomBehaviour?.DomesticVrpConsentAuthGet
            };

            // Create persisted entity
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new BankRegistration(
                Guid.NewGuid(),
                requestInfo.Request.Reference,
                false,
                utcNow,
                requestInfo.Request.CreatedBy,
                utcNow,
                requestInfo.Request.CreatedBy,
                requestInfo.Request.BankId,
                requestInfo.Request.SoftwareStatementProfileId,
                requestInfo.Request.SoftwareStatementAndCertificateProfileOverrideCase,
                registrationScope,
                dynamicClientRegistrationApiVersion,
                tokenEndpoint,
                authorizationEndpoint,
                registrationEndpoint,
                tokenEndpointAuthMethod,
                customBehaviourPersisted,
                externalApiId,
                externalApiSecret,
                registrationAccessToken);


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

            BankRegistrationReadResponse response = new(
                entity.Id,
                entity.Created,
                entity.CreatedBy,
                entity.Reference,
                new ExternalApiObjectResponse(entity.ExternalApiObject.ExternalApiId),
                entity.BankId,
                entity.SoftwareStatementProfileId,
                entity.SoftwareStatementAndCertificateProfileOverrideCase,
                entity.DynamicClientRegistrationApiVersion,
                entity.RegistrationScope,
                entity.RegistrationEndpoint,
                entity.TokenEndpoint,
                entity.AuthorizationEndpoint,
                entity.TokenEndpointAuthMethod,
                entity.CustomBehaviour,
                externalApiResponse);

            return (response, nonErrorMessages);
        }
    }
}
