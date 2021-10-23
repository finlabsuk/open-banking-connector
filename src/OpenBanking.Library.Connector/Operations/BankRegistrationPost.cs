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
            IReadOnlyRepository<ProcessedSoftwareStatementProfile> softwareStatementProfileRepo,
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

        protected override async Task<(BankRegistration persistedObject,
            ClientRegistrationModelsPublic.OBClientRegistration1 apiRequest,
            IApiPostRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests, IApiClient apiClient, Uri uri
            , JsonSerializerSettings? jsonSerializerSettings, List<IFluentResponseInfoOrWarningMessage> nonErrorMessages
            )> ApiPostData(
            BankRegistrationRequest request,
            string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Create persisted entity
            BankRegistration persistedObject = new BankRegistration();
            persistedObject.Initialise(
                request,
                modifiedBy,
                _timeProvider);

            // Load relevant data and checks
            Guid bankId = request.BankId;
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
                !request.AllowMultipleRegistrations)
            {
                throw new InvalidOperationException(
                    "There is already at least one registration for this bank. Set AllowMultipleRegistrations to force creation of an additional registration.");
            }

            // Load relevant objects
            string softwareStatementProfileId = request.SoftwareStatementProfileId;
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(softwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfileId {softwareStatementProfileId}");

            // Determine registration scope
            RegistrationScope registrationScope =
                request.RegistrationScope ??
                processedSoftwareStatementProfile.SoftwareStatementPayload.RegistrationScope;

            // STEP 1
            // Compute claims associated with Open Banking client

            // Get OpenID Connect configuration info
            OpenIdConfiguration openIdConfiguration;
            if (request.OpenIdConfigurationReplacement is null)
            {
                openIdConfiguration = await GetOpenIdConfigurationAsync(bank.IssuerUrl);
            }
            else
            {
                openIdConfiguration =
                    JsonConvert.DeserializeObject<OpenIdConfiguration>(
                        request.OpenIdConfigurationReplacement,
                        new JsonSerializerSettings()) ??
                    throw new Exception("Can't de-serialise supplied bank API response");
            }

            // Update OpenID Connect configuration info based on overrides
            string? registrationEndpointOverride =
                request.OpenIdConfigurationOverrides?.RegistrationEndpoint;
            if (!(registrationEndpointOverride is null))
            {
                openIdConfiguration.RegistrationEndpoint = registrationEndpointOverride;
            }

            IList<string>? responseModesSupportedOverride =
                request.OpenIdConfigurationOverrides?.ResponseModesSupported;
            if (!(responseModesSupportedOverride is null))
            {
                openIdConfiguration.ResponseModesSupported = responseModesSupportedOverride;
            }

            // Validate OpenID Configuration
            {
                IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages =
                    new OpenBankingOpenIdConfigurationResponseValidator()
                        .Validate(openIdConfiguration)
                        .ProcessValidationResultsAndRaiseErrors(messagePrefix: "prefix");
                nonErrorMessages.AddRange(newNonErrorMessages);
            }

            // Save registration scope and Open ID Connect config
            persistedObject.UpdateOpenIdGet(
                registrationScope,
                openIdConfiguration);

            // Create claims for client reg
            ClientRegistrationModelsPublic.OBClientRegistration1 apiRequest =
                RegistrationClaimsFactory.CreateRegistrationClaims(
                    openIdConfiguration.TokenEndpointAuthMethodsSupported,
                    processedSoftwareStatementProfile,
                    registrationScope,
                    request.BankRegistrationClaimsOverrides,
                    bank.FinancialId);

            // STEP 3
            // Create serialiser settings.
            var uri = new Uri(openIdConfiguration.RegistrationEndpoint);
            JsonSerializerSettings? jsonSerializerSettings = null;
            if (!(request.BankRegistrationResponseJsonOptions is null))
            {
                var optionsDict = new Dictionary<JsonConverterLabel, int>
                {
                    {
                        JsonConverterLabel.DcrRegClientIdIssuedAt,
                        (int) request.BankRegistrationResponseJsonOptions.ClientIdIssuedAtConverterOptions
                    },
                    {
                        JsonConverterLabel.DcrRegScope,
                        (int) request.BankRegistrationResponseJsonOptions.ScopeConverterOptions
                    }
                };
                jsonSerializerSettings = new JsonSerializerSettings
                {
                    Context = new StreamingContext(
                        StreamingContextStates.All,
                        optionsDict)
                };
            }

            // Create new Open Banking registration by processing override or posting JWT
            IApiPostRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
                persistedObject.ApiPostRequests(
                    request.ClientRegistrationApi,
                    processedSoftwareStatementProfile,
                    _instrumentationClient);

            return (persistedObject, apiRequest, apiRequests, processedSoftwareStatementProfile.ApiClient, uri,
                jsonSerializerSettings, nonErrorMessages);
        }
    }
}
