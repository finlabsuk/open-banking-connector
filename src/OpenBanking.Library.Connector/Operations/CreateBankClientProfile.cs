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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.ObApi.Base.Json;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using BankClientProfilePublic = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankClientProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public interface ICreateBankClientProfile
    {
        Task<BankClientProfileResponse> CreateAsync(BankClientProfilePublic bankClientProfile);
    }

    public class CreateBankClientProfile : ICreateBankClientProfile
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<BankClientProfile> _bankClientProfileRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly IEntityMapper _mapper;
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;

        public CreateBankClientProfile(
            IApiClient apiClient,
            IEntityMapper mapper,
            IDbMultiEntityMethods dbMultiEntityMethods,
            IDbEntityRepository<BankClientProfile> bankClientProfileRepo,
            ISoftwareStatementProfileService softwareStatementProfileService)
        {
            _apiClient = apiClient;
            _mapper = mapper;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _bankClientProfileRepo = bankClientProfileRepo;
            _softwareStatementProfileService = softwareStatementProfileService;
        }

        public async Task<BankClientProfileResponse> CreateAsync(BankClientProfilePublic bankClientProfile)
        {
            bankClientProfile.ArgNotNull(nameof(bankClientProfile));

            // Load relevant objects
            SoftwareStatementProfile softwareStatementProfile =
                _softwareStatementProfileService.GetSoftwareStatementProfile(
                    bankClientProfile.SoftwareStatementProfileId);

            // STEP 1
            // Compute claims associated with Open Banking client

            // Get OpenID Connect configuration info
            OpenIdConfiguration openIdConfiguration =
                await GetOpenIdConfigurationAsync(bankClientProfile.IssuerUrl);
            new OpenBankingOpenIdConfigurationResponseValidator().Validate(openIdConfiguration)
                .RaiseErrorOnValidationError();

            // Create claims for client reg
            OBClientRegistration1 registrationClaims = Factories.CreateRegistrationClaims(
                sProfile: softwareStatementProfile,
                bProfile: bankClientProfile);

            // STEP 2
            // Check for existing Open Banking client for issuer URL
            // If we have an Open Banking client with the same issuer URL we will check if the claims match.
            // If they do, we will re-use this client.
            // Otherwise we will return an error as only support a single client per issuer URL at present.
            IQueryable<BankClientProfile> clientList = await _bankClientProfileRepo
                .GetAsync(c => c.IssuerUrl == bankClientProfile.IssuerUrl);
            BankClientProfile existingClient = clientList
                .SingleOrDefault();
            if (existingClient is object)
            {
                if (existingClient.BankClientRegistrationRequestData != registrationClaims)
                {
                    throw new Exception(
                        "There is already a client for this issuer URL but it cannot be re-used because claims are different.");
                }
            }

            // STEP 3
            // Create new Open Banking client by posting JWT
            BankClientProfile client;
            if (existingClient is null)
            {
                JwtFactory jwtFactory = new JwtFactory();
                string jwt = jwtFactory.CreateJwt(
                    profile: softwareStatementProfile,
                    claims: registrationClaims,
                    useOpenBankingJwtHeaders: false,
                    paymentInitiationApiVersion: ApiVersion.V3P1P1); // version doesn't matter here.
                JsonSerializerSettings jsonSerializerSettings = null;
                if (!(bankClientProfile.RegistrationResponseJsonOptions is null))
                {
                    IEnumerable<string> optionList1 =
                        Enum.GetValues(typeof(DateTimeOffsetUnixConverterOptions))
                            .Cast<Enum>()
                            .Where(
                                x => !Equals(objA: (int) (object) x, objB: 0)
                                     && bankClientProfile.RegistrationResponseJsonOptions
                                         .DateTimeOffsetUnixConverterOptions
                                         .HasFlag(x))
                            .Select(x => $"{nameof(DateTimeOffsetUnixConverterOptions)}:{x.ToString()}");
                    IEnumerable<string> optionList2 =
                        Enum.GetValues(typeof(DelimitedStringConverterOptions))
                            .Cast<Enum>()
                            .Where(
                                x => !Equals(objA: (int) (object) x, objB: 0)
                                     && bankClientProfile.RegistrationResponseJsonOptions
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
                BankClientProfile newClient = _mapper.Map<BankClientProfile>(bankClientProfile);
                client = await PersistOpenBankingClient(
                    value: newClient,
                    openIdConfiguration: openIdConfiguration,
                    registrationClaims: registrationClaims,
                    openBankingRegistrationData: registrationResponse);
                await _dbMultiEntityMethods.SaveChangesAsync();
            }
            else
            {
                client = existingClient;
            }

            // Return
            return new BankClientProfileResponse(client);
        }

        private async Task<BankClientProfile> PersistOpenBankingClient(
            BankClientProfile value,
            OpenIdConfiguration openIdConfiguration,
            OBClientRegistration1 registrationClaims,
            OBClientRegistration1 openBankingRegistrationData)
        {
            value.State = "ok";
            value.OpenIdConfiguration = openIdConfiguration;
            value.BankClientRegistrationRequestData =
                registrationClaims;
            value.BankClientRegistrationData = openBankingRegistrationData;

            await _bankClientProfileRepo.UpsertAsync(value);

            return value;
        }

        private async Task<BankClientProfile> PersistOpenBankingClientProfile(
            BankClientProfile value,
            string openBankingClientId)
        {
            value.Id = Guid.NewGuid().ToString();
            value.State = "ok";

            await _bankClientProfileRepo.UpsertAsync(value);

            return value;
        }

        private async Task<OpenIdConfiguration> GetOpenIdConfigurationAsync(string issuerUrl)
        {
            UriBuilder ub = new UriBuilder(new Uri(issuerUrl)) { Path = ".well-known/openid-configuration" };

            return await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(ub.Uri)
                .Create()
                .RequestJsonAsync<OpenIdConfiguration>(client: _apiClient, requestContentIsJson: false);
        }
    }
}
