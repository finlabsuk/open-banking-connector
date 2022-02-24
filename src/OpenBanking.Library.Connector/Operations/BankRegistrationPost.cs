// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BankRegistrationRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;
using OAuth2RequestObjectClaimsOverridesRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.OAuth2RequestObjectClaimsOverrides;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class
        BankRegistrationPost : EntityPost<
            BankRegistration,
            BankRegistrationRequest, BankRegistrationResponse, ClientRegistrationModelsPublic.OBClientRegistration1,
            ClientRegistrationModelsPublic.OBClientRegistration1Response>
    {
        private readonly IApiClient _apiClient;
        private readonly IDbReadOnlyEntityMethods<Bank> _bankMethods;

        public BankRegistrationPost(
            IDbReadWriteEntityMethods<BankRegistration> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsent> domesticPaymentConsentMethods,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IApiClient apiClient,
            IDbReadOnlyEntityMethods<Bank> bankMethods) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper)
        {
            _apiClient = apiClient;
            _bankMethods = bankMethods;
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
            Task<(BankRegistration persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPost(PostRequestInfo requestInfo)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data and checks
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

            // Load relevant objects
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    requestInfo.Request.SoftwareStatementProfileId,
                    requestInfo.Request.SoftwareStatementAndCertificateProfileOverrideCase);

            // Determine registration scope
            RegistrationScope registrationScope =
                requestInfo.Request.RegistrationScope ??
                processedSoftwareStatementProfile.SoftwareStatementPayload.RegistrationScope;

            // STEP 1
            // Compute claims associated with Open Banking client

            // Get OpenID Connect configuration (normally from (well-known endpoint)
            OpenIdConfiguration openIdConfiguration;
            if (requestInfo.Request.OpenIdConfigurationReplacement is null)
            {
                openIdConfiguration = await GetOpenIdConfigurationAsync(bank.IssuerUrl);
            }
            else
            {
                openIdConfiguration =
                    JsonConvert.DeserializeObject<OpenIdConfiguration>(
                        requestInfo.Request.OpenIdConfigurationReplacement,
                        new JsonSerializerSettings()) ??
                    throw new Exception("Can't de-serialise supplied bank API response");
            }

            // Update OpenID Connect configuration based on overrides
            string? registrationEndpointOverride =
                requestInfo.Request.OpenIdConfigurationOverrides?.RegistrationEndpoint;
            if (!(registrationEndpointOverride is null))
            {
                openIdConfiguration.RegistrationEndpoint = registrationEndpointOverride;
            }

            IList<string>? responseModesSupportedOverride =
                requestInfo.Request.OpenIdConfigurationOverrides?.ResponseModesSupported;
            if (!(responseModesSupportedOverride is null))
            {
                openIdConfiguration.ResponseModesSupported = responseModesSupportedOverride;
            }

            IList<OpenIdConfigurationTokenEndpointAuthMethodEnum>? tokenEndpointAuthMethodsSupportedOverride =
                requestInfo.Request.OpenIdConfigurationOverrides?.TokenEndpointAuthMethodsSupported;
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

            // Select tokenEndpointAuthMethod based on most preferred
            TokenEndpointAuthMethodEnum tokenEndpointAuthMethod;
            if (openIdConfiguration.TokenEndpointAuthMethodsSupported.Contains(
                    OpenIdConfigurationTokenEndpointAuthMethodEnum.TlsClientAuth))
            {
                tokenEndpointAuthMethod = TokenEndpointAuthMethodEnum.TlsClientAuth;
            }
            else if (openIdConfiguration.TokenEndpointAuthMethodsSupported.Contains(
                         OpenIdConfigurationTokenEndpointAuthMethodEnum.PrivateKeyJwt))
            {
                tokenEndpointAuthMethod = TokenEndpointAuthMethodEnum.PrivateKeyJwt;
            }
            else if (openIdConfiguration.TokenEndpointAuthMethodsSupported.Contains(
                         OpenIdConfigurationTokenEndpointAuthMethodEnum.ClientSecretBasic))
            {
                tokenEndpointAuthMethod = TokenEndpointAuthMethodEnum.ClientSecretBasic;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    $"No supported value in {openIdConfiguration.TokenEndpointAuthMethodsSupported}");
            }

            // Create claims for client reg
            ClientRegistrationModelsPublic.OBClientRegistration1 apiRequest =
                RegistrationClaimsFactory.CreateRegistrationClaims(
                    tokenEndpointAuthMethod,
                    processedSoftwareStatementProfile,
                    registrationScope,
                    requestInfo.Request.BankRegistrationClaimsOverrides,
                    bank.FinancialId,
                    requestInfo.Request.UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues);

            // STEP 3
            var uri = new Uri(openIdConfiguration.RegistrationEndpoint);

            // Create request serialiser settings.
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            if (!(requestInfo.Request.BankRegistrationClaimsJsonOptions is null))
            {
                var optionsDict = new Dictionary<JsonConverterLabel, int>
                {
                    {
                        JsonConverterLabel.DcrRegScope,
                        (int) requestInfo.Request.BankRegistrationClaimsJsonOptions.ScopeConverterOptions
                    }
                };
                requestJsonSerializerSettings = new JsonSerializerSettings
                {
                    Context = new StreamingContext(
                        StreamingContextStates.All,
                        optionsDict)
                };
            }

            // Create response serialiser settings.
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            if (!(requestInfo.Request.BankRegistrationResponseJsonOptions is null))
            {
                var optionsDict = new Dictionary<JsonConverterLabel, int>
                {
                    {
                        JsonConverterLabel.DcrRegClientIdIssuedAt,
                        (int) requestInfo.Request.BankRegistrationResponseJsonOptions.ClientIdIssuedAtConverterOptions
                    },
                    {
                        JsonConverterLabel.DcrRegScope,
                        (int) requestInfo.Request.BankRegistrationResponseJsonOptions.ScopeConverterOptions
                    }
                };
                responseJsonSerializerSettings = new JsonSerializerSettings
                {
                    Context = new StreamingContext(
                        StreamingContextStates.All,
                        optionsDict)
                };
            }

            // Create new Open Banking registration by processing override or posting JWT
            IApiPostRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
                new BankRegistration()
                    .ApiPostRequests(
                        requestInfo.Request.ClientRegistrationApi,
                        processedSoftwareStatementProfile,
                        _instrumentationClient,
                        requestInfo.Request.UseApplicationJoseNotApplicationJwtContentTypeHeader);

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

            // Create persisted entity
            var persistedObject = new BankRegistration();
            persistedObject.Initialise(
                requestInfo.Request,
                requestInfo.ModifiedBy,
                _timeProvider);

            persistedObject.Create(
                requestInfo.Request,
                requestInfo.ModifiedBy,
                _timeProvider,
                registrationScope,
                openIdConfiguration, // TODO: update to store raw response
                openIdConfiguration.TokenEndpoint,
                openIdConfiguration.AuthorizationEndpoint,
                openIdConfiguration.RegistrationEndpoint,
                tokenEndpointAuthMethod,
                apiRequest);
            
            persistedObject.UpdateOpenIdGet(
                registrationScope,
                openIdConfiguration, // TODO: update to store raw response
                openIdConfiguration.TokenEndpoint,
                openIdConfiguration.AuthorizationEndpoint,
                openIdConfiguration.RegistrationEndpoint,
                tokenEndpointAuthMethod);
            persistedObject.UpdateBeforeApiPost(apiRequest);
            
            // Update with results of POST
            persistedObject.UpdateAfterApiPost(apiResponse, "", _timeProvider);

            return (persistedObject, nonErrorMessages2);
        }
    }
}
