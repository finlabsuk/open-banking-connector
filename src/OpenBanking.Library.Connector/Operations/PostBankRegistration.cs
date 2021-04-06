// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using RequestBankRegistration = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Models;
using ClientRegistrationModelsV3p2 =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p2.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class PostBankRegistration
    {
        private readonly IApiClient _apiClient;
        private readonly IDbReadWriteEntityMethods<BankRegistration> _bankRegistrationRepo;
        private readonly IDbReadWriteEntityMethods<Bank> _bankRepo;
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IApiVariantMapper _mapper;
        private readonly IReadOnlyRepository<SoftwareStatementProfileCached> _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public PostBankRegistration(
            IApiClient apiClient,
            IDbReadWriteEntityMethods<BankRegistration> bankRegistrationRepo,
            IDbReadWriteEntityMethods<Bank> bankRepo,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper)
        {
            _apiClient = apiClient;
            _bankRegistrationRepo = bankRegistrationRepo;
            _bankRepo = bankRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _instrumentationClient = instrumentationClient;
            _mapper = mapper;
        }

        public async Task<(BankRegistrationPostResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostAsync(
                RequestBankRegistration request,
                string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant objects
            SoftwareStatementProfileCached softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(request.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfileId {request.SoftwareStatementProfileId}");
            Bank bank = await _bankRepo.GetAsync(request.BankId) ??
                        throw new KeyNotFoundException(
                            $"No record found for BankId {request.BankId} specified by request.");

            // STEP 1
            // Compute claims associated with Open Banking client

            // Get OpenID Connect configuration info
            OpenIdConfiguration openIdConfiguration =
                await GetOpenIdConfigurationAsync(bank.IssuerUrl);
            string? registrationEndpointOverride = request.OpenIdConfigurationOverrides?.RegistrationEndpoint;
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

            {
                IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages =
                    new OpenBankingOpenIdConfigurationResponseValidator().Validate(openIdConfiguration)
                        .ProcessValidationResultsAndRaiseErrors(messagePrefix: "prefix");
                nonErrorMessages.AddRange(newNonErrorMessages);
            }

            // Determine registration scope
            RegistrationScope registrationScope = request.RegistrationScope ??
                                                  softwareStatementProfile.SoftwareStatementPayload.RegistrationScope;

            // Create claims for client reg
            ClientRegistrationModelsPublic.OBClientRegistration1 registrationClaims =
                RegistrationClaimsFactory.CreateRegistrationClaims(
                    softwareStatementProfile,
                    registrationScope,
                    request.BankRegistrationClaimsOverrides,
                    bank.FinancialId);

            // STEP 2
            // Check for existing registration with bank.
            IQueryable<BankRegistration> clientList = await _bankRegistrationRepo
                .GetAsync(c => c.BankId == bank.Id);
            BankRegistration? existingClient = clientList
                .FirstOrDefault();
            if (!(existingClient is null) &&
                !request.AllowMultipleRegistrations)
            {
                throw new InvalidOperationException(
                    "There is already at least one registration for this bank. Set AllowMultipleRegistrations to force creation of an additional registration.");
            }

            // STEP 3
            // Create serialiser settings.
            JsonSerializerSettings? jsonSerializerSettings = null;
            if (!(request.BankRegistrationResponseJsonOptions is null))
            {
                IEnumerable<string> optionList1 =
                    Enum.GetValues(typeof(DateTimeOffsetUnixConverterOptions))
                        .Cast<Enum>()
                        .Where(
                            x => !Equals((int) (object) x, 0)
                                 && request.BankRegistrationResponseJsonOptions
                                     .DateTimeOffsetUnixConverterOptions
                                     .HasFlag(x))
                        .Select(x => $"{nameof(DateTimeOffsetUnixConverterOptions)}:{x.ToString()}");
                IEnumerable<string> optionList2 =
                    Enum.GetValues(typeof(DelimitedStringConverterOptions))
                        .Cast<Enum>()
                        .Where(
                            x => !Equals((int) (object) x, 0)
                                 && request.BankRegistrationResponseJsonOptions
                                     .DelimitedStringConverterOptions
                                     .HasFlag(x))
                        .Select(x => $"{nameof(DelimitedStringConverterOptions)}:{x.ToString()}");
                List<string> optionList = optionList1
                    .Concat(optionList2)
                    .ToList();
                jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Context = new StreamingContext(
                    StreamingContextStates.All,
                    optionList);
            }

            // Create new Open Banking registration by processing override or posting JWT
            ClientRegistrationModelsPublic.OBClientRegistration1 registrationResponse;
            string? responseOverride = request.BankRegistrationResponseFileName;
            if (!(responseOverride is null))
            {
                string? fileText = await File.ReadAllTextAsync(responseOverride);
                registrationResponse =
                    JsonConvert.DeserializeObject<ClientRegistrationModelsPublic.OBClientRegistration1>(
                        fileText,
                        jsonSerializerSettings) ??
                    throw new Exception("Can't de-serialise supplied bank registration response");
            }
            else
            {
                var jwtFactory = new JwtFactory();
                var postApiObject =
                    new PostRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response>();
                var uri = new Uri(openIdConfiguration.RegistrationEndpoint);
                var (registrationResponseTyped, newNonErrorMessages) = request.ClientRegistrationApi
                    switch
                    {
                        ClientRegistrationApiVersion.Version3p2 => await postApiObject
                            .PostAsync<ClientRegistrationModelsV3p2.OBClientRegistration1,
                                ClientRegistrationModelsV3p2.OBClientRegistration1>(
                                uri,
                                registrationClaims,
                                new JwtRequestProcessor<ClientRegistrationModelsV3p2.OBClientRegistration1>(
                                    softwareStatementProfile,
                                    jwtFactory,
                                    _instrumentationClient),
                                jsonSerializerSettings,
                                softwareStatementProfile.ApiClient,
                                _mapper),
                        ClientRegistrationApiVersion.Version3p3 => await postApiObject
                            .PostAsync<ClientRegistrationModelsPublic.OBClientRegistration1,
                                ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                                uri,
                                registrationClaims,
                                new JwtRequestProcessor<ClientRegistrationModelsPublic.OBClientRegistration1>(
                                    softwareStatementProfile,
                                    jwtFactory,
                                    _instrumentationClient),
                                jsonSerializerSettings,
                                softwareStatementProfile.ApiClient,
                                _mapper),
                        _ => throw new ArgumentOutOfRangeException(
                            $"ClientRegistrationApi version {request.ClientRegistrationApi}")
                    };
                registrationResponse = registrationResponseTyped;
                nonErrorMessages.AddRange(newNonErrorMessages);
            }

            // Validate response
            if (registrationResponse.ClientId is null)
            {
                throw new NullReferenceException("No client ID provided by bank.");
            }

            // Create and store Open Banking client
            var bankRegistration = new BankRegistration(
                request.SoftwareStatementProfileId,
                openIdConfiguration,
                registrationResponse,
                bank.Id,
                registrationClaims,
                request.OAuth2RequestObjectClaimsOverrides,
                request.ClientRegistrationApi,
                registrationScope,
                Guid.NewGuid(),
                request.Name,
                createdBy,
                _timeProvider);
            await _bankRegistrationRepo.AddAsync(bankRegistration);

            // Persist updates
            await _dbSaveChangesMethod.SaveChangesAsync();

            return (bankRegistration.PublicPostResponse, nonErrorMessages);
        }

        private async Task<OpenIdConfiguration> GetOpenIdConfigurationAsync(string issuerUrl)
        {
            var uri = new Uri(string.Join("/", issuerUrl.TrimEnd('/'), ".well-known/openid-configuration"));

            return await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(uri)
                .Create()
                .RequestJsonAsync<OpenIdConfiguration>(_apiClient, false);
        }
    }
}
