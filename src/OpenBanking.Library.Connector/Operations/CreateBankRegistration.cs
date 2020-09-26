// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.ObApi.Base.Json;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using RequestBankRegistration = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class CreateBankRegistration
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<BankRegistration> _bankRegistrationRepo;
        private readonly IDbEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;
        private readonly ITimeProvider _timeProvider;

        public CreateBankRegistration(
            IApiClient apiClient,
            IDbEntityRepository<BankRegistration> bankRegistrationRepo,
            IDbEntityRepository<Bank> bankRepo,
            IDbMultiEntityMethods dbMultiEntityMethods,
            ISoftwareStatementProfileService softwareStatementProfileService,
            ITimeProvider timeProvider)
        {
            _apiClient = apiClient;
            _bankRegistrationRepo = bankRegistrationRepo;
            _bankRepo = bankRepo;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _softwareStatementProfileService = softwareStatementProfileService;
            _timeProvider = timeProvider;
        }

        public async Task<BankRegistrationResponse> CreateAsync(
            RequestBankRegistration requestBankRegistration,
            string? createdBy)
        {
            requestBankRegistration.ArgNotNull(nameof(requestBankRegistration));

            // Load relevant objects
            SoftwareStatementProfile softwareStatementProfile =
                _softwareStatementProfileService.GetSoftwareStatementProfile(
                    requestBankRegistration.SoftwareStatementProfileId);
            Bank bank;
            try
            {
                bank = (await _bankRepo.GetAsync(b => b.Name == requestBankRegistration.BankName)).Single();
            }
            catch (Exception)
            {
                throw new ArgumentException("No record found for BankName.");
            }

            // STEP 1
            // Compute claims associated with Open Banking client

            // Get OpenID Connect configuration info
            OpenIdConfiguration openIdConfiguration =
                await GetOpenIdConfigurationAsync(bank.IssuerUrl);
            new OpenBankingOpenIdConfigurationResponseValidator().Validate(openIdConfiguration)
                .RaiseErrorOnValidationError();

            // Create claims for client reg
            string aud = requestBankRegistration.BankClientRegistrationClaimsOverrides?.RequestAudience ??
                         bank.XFapiFinancialId;
            OBClientRegistration1 registrationClaims = RegistrationClaimsFactory.CreateRegistrationClaims(
                sProfile: softwareStatementProfile,
                audValue: aud);

            // STEP 2
            // Check for existing registration with bank.
            IQueryable<BankRegistration> clientList = await _bankRegistrationRepo
                .GetAsync(c => c.BankId == bank.Id);
            BankRegistration? existingClient = clientList
                .FirstOrDefault();
            if (existingClient is object && !requestBankRegistration.AllowMultipleRegistrations)
            {
                throw new InvalidOperationException(
                    "There is already at least one registration for this bank. Set AllowMultipleRegistrations to force creation of an additional registration.");
            }

            // STEP 3
            // Create new Open Banking client by posting JWT
            JwtFactory jwtFactory = new JwtFactory();
            string jwt = jwtFactory.CreateJwt(
                profile: softwareStatementProfile,
                claims: registrationClaims,
                useOpenBankingJwtHeaders: false,
                paymentInitiationApiVersion: ApiVersion.V3P1P1); // version doesn't matter here.
            JsonSerializerSettings? jsonSerializerSettings = null;
            if (!(requestBankRegistration.RegistrationResponseJsonOptions is null))
            {
                IEnumerable<string> optionList1 =
                    Enum.GetValues(typeof(DateTimeOffsetUnixConverterOptions))
                        .Cast<Enum>()
                        .Where(
                            x => !Equals(objA: (int) (object) x, objB: 0)
                                 && requestBankRegistration.RegistrationResponseJsonOptions
                                     .DateTimeOffsetUnixConverterOptions
                                     .HasFlag(x))
                        .Select(x => $"{nameof(DateTimeOffsetUnixConverterOptions)}:{x.ToString()}");
                IEnumerable<string> optionList2 =
                    Enum.GetValues(typeof(DelimitedStringConverterOptions))
                        .Cast<Enum>()
                        .Where(
                            x => !Equals(objA: (int) (object) x, objB: 0)
                                 && requestBankRegistration.RegistrationResponseJsonOptions
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

            OBClientRegistration1 registrationResponse = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(openIdConfiguration.RegistrationEndpoint)
                .SetContent(jwt)
                .SetContentType("application/jwt")
                .Create()
                .RequestJsonAsync<OBClientRegistration1>(
                    client: _apiClient,
                    requestContentIsJson: false,
                    jsonSerializerSettings: jsonSerializerSettings);

            // Validate response
            if (registrationResponse.ClientId is null)
            {
                throw new NullReferenceException("No client ID provided by bank.");
            }

            if (registrationResponse.ClientIdIssuedAt is null)
            {
                throw new NullReferenceException("No ClientIdIssuedAt provided by bank.");
            }

            // Create and store Open Banking client
            BankRegistration bankRegistration = new BankRegistration(
                timeProvider: _timeProvider,
                softwareStatementProfileId: requestBankRegistration.SoftwareStatementProfileId,
                openIdConfiguration: openIdConfiguration,
                bankClientRegistration: registrationResponse,
                bankId: bank.Id,
                bankClientRegistrationRequest: registrationClaims,
                createdBy: createdBy);
            await _bankRegistrationRepo.AddAsync(bankRegistration);

            // Update registration references
            if (requestBankRegistration.ReplaceDefaultBankRegistration ||
                requestBankRegistration.ReplaceStagingBankRegistration)
            {
                ReadWriteProperty<string?> regId = new ReadWriteProperty<string?>(
                    data: bankRegistration.Id,
                    timeProvider: _timeProvider,
                    modifiedBy: null);
                if (requestBankRegistration.ReplaceDefaultBankRegistration)
                {
                    bank.DefaultBankRegistrationId = regId;
                }

                if (requestBankRegistration.ReplaceStagingBankRegistration)
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
