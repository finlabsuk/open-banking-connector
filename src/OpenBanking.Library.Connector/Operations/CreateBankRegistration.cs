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
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.ObApi.Base.Json;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;
using RequestBankRegistration = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class CreateBankRegistration
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<BankRegistration> _bankRegistrationRepo;
        private readonly IDbEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public CreateBankRegistration(
            IApiClient apiClient,
            IDbEntityRepository<BankRegistration> bankRegistrationRepo,
            IDbEntityRepository<Bank> bankRepo,
            IDbMultiEntityMethods dbMultiEntityMethods,
            ITimeProvider timeProvider,
            IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> softwareStatementProfileRepo)
        {
            _apiClient = apiClient;
            _bankRegistrationRepo = bankRegistrationRepo;
            _bankRepo = bankRepo;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _timeProvider = timeProvider;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
        }

        public async Task<BankRegistrationResponse> CreateAsync(
            RequestBankRegistration request,
            string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Load relevant objects
            SoftwareStatementProfileCached softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(request.SoftwareStatementProfileId);
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

            string[]? responseModesSupportedOverride = request.OpenIdConfigurationOverrides?.ResponseModesSupported;
            if (!(responseModesSupportedOverride is null))
            {
                openIdConfiguration.ResponseModesSupported = responseModesSupportedOverride;
            }

            new OpenBankingOpenIdConfigurationResponseValidator().Validate(openIdConfiguration)
                .RaiseErrorOnValidationError();

            // Create claims for client reg
            OBClientRegistration1 registrationClaims = RegistrationClaimsFactory.CreateRegistrationClaims(
                sProfile: softwareStatementProfile,
                registrationScopeApiSet: bank.RegistrationScopeApiSet,
                bankClientRegistrationClaimsOverrides: request.BankRegistrationClaimsOverrides,
                bankXFapiFinancialId: bank.FinancialId);

            // STEP 2
            // Check for existing registration with bank.
            IQueryable<BankRegistration> clientList = await _bankRegistrationRepo
                .GetAsync(c => c.BankId == bank.Id);
            BankRegistration? existingClient = clientList
                .FirstOrDefault();
            if (existingClient is object && !request.AllowMultipleRegistrations)
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
                            x => !Equals(objA: (int) (object) x, objB: 0)
                                 && request.BankRegistrationResponseJsonOptions
                                     .DateTimeOffsetUnixConverterOptions
                                     .HasFlag(x))
                        .Select(x => $"{nameof(DateTimeOffsetUnixConverterOptions)}:{x.ToString()}");
                IEnumerable<string> optionList2 =
                    Enum.GetValues(typeof(DelimitedStringConverterOptions))
                        .Cast<Enum>()
                        .Where(
                            x => !Equals(objA: (int) (object) x, objB: 0)
                                 && request.BankRegistrationResponseJsonOptions
                                     .DelimitedStringConverterOptions
                                     .HasFlag(x))
                        .Select(x => $"{nameof(DelimitedStringConverterOptions)}:{x.ToString()}");
                List<string> optionList = optionList1
                    .Concat(optionList2)
                    .ToList();
                jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Context = new StreamingContext(
                    state: StreamingContextStates.All,
                    additional: optionList);
            }

            // Create new Open Banking registration by processing override or posting JWT
            OBClientRegistration1 registrationResponse;
            string? responseOverride = request.BankRegistrationResponseFileName;
            if (!(responseOverride is null))
            {
                string? fileText = await File.ReadAllTextAsync(responseOverride);
                registrationResponse = JsonConvert.DeserializeObject<OBClientRegistration1>(
                                           value: fileText,
                                           settings: jsonSerializerSettings) ??
                                       throw new Exception("Can't de-serialise supplied bank registration response");
            }
            else
            {
                JwtFactory jwtFactory = new JwtFactory();
                string jwt = jwtFactory.CreateJwt(
                    profile: softwareStatementProfile,
                    claims: registrationClaims,
                    useOpenBankingJwtHeaders: false,
                    paymentInitiationApiVersion: ApiVersion.V3p1p1); // version doesn't matter here.
                registrationResponse = await new HttpRequestBuilder()
                    .SetMethod(HttpMethod.Post)
                    .SetUri(openIdConfiguration.RegistrationEndpoint)
                    .SetContent(jwt)
                    .SetContentType("application/jwt")
                    .Create()
                    .RequestJsonAsync<OBClientRegistration1>(
                        client: softwareStatementProfile.ApiClient,
                        requestContentIsJson: false,
                        jsonSerializerSettings: jsonSerializerSettings);
            }

            // Validate response
            if (registrationResponse.ClientId is null)
            {
                throw new NullReferenceException("No client ID provided by bank.");
            }

            // Create and store Open Banking client
            BankRegistration bankRegistration = new BankRegistration(
                timeProvider: _timeProvider,
                softwareStatementProfileId: request.SoftwareStatementProfileId,
                openIdConfiguration: openIdConfiguration,
                obClientRegistration: registrationResponse,
                bankId: bank.Id,
                obClientRegistrationRequest: registrationClaims,
                claimsOverrides: request.OAuth2RequestObjectClaimsOverrides,
                createdBy: createdBy);
            await _bankRegistrationRepo.AddAsync(bankRegistration);

            // Update registration references
            if (request.ReplaceDefaultBankRegistration ||
                request.ReplaceStagingBankRegistration)
            {
                ReadWriteProperty<Guid?> regId = new ReadWriteProperty<Guid?>(
                    data: bankRegistration.Id,
                    timeProvider: _timeProvider,
                    modifiedBy: null);
                if (request.ReplaceDefaultBankRegistration)
                {
                    bank.DefaultBankRegistrationId = regId;
                }

                if (request.ReplaceStagingBankRegistration)
                {
                    bank.StagingBankRegistrationId = regId;
                }
            }

            // Persist updates
            await _dbMultiEntityMethods.SaveChangesAsync();

            // Return
            return bankRegistration.PublicResponse;
        }

        private async Task<OpenIdConfiguration> GetOpenIdConfigurationAsync(string issuerUrl)
        {
            Uri uri = new Uri(string.Join(separator: "/", issuerUrl.TrimEnd('/'), ".well-known/openid-configuration"));

            return await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(uri)
                .Create()
                .RequestJsonAsync<OpenIdConfiguration>(client: _apiClient, requestContentIsJson: false);
        }
    }
}
