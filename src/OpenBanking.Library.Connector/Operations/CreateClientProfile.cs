// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using BankClient = FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.BankClient;
using BankClientProfile = FinnovationLabs.OpenBanking.Library.Connector.Model.Public.BankClientProfile;
using OpenBankingClientRegistrationClaims =
    FinnovationLabs.OpenBanking.Library.Connector.Model.Public.OpenBankingClientRegistrationClaims;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public class CreateClientProfile
    {
        private readonly IApiClient _apiClient;
        private readonly IOpenBankingClientProfileRepository _clientProfileRepo;
        private readonly IOpenBankingClientRepository _clientRepo;
        private readonly IEntityMapper _mapper;
        private readonly ISoftwareStatementProfileRepository _softwareStatementProfileRepo;

        public CreateClientProfile(
            IEntityMapper mapper,
            IApiClient apiClient,
            ISoftwareStatementProfileRepository softwareStatementProfileRepo,
            IOpenBankingClientProfileRepository clientProfileRepo,
            IOpenBankingClientRepository clientRepo
        )
        {
            _mapper = mapper.ArgNotNull(nameof(mapper));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _softwareStatementProfileRepo =
                softwareStatementProfileRepo.ArgNotNull(nameof(softwareStatementProfileRepo));
            _clientProfileRepo = clientProfileRepo.ArgNotNull(nameof(clientProfileRepo));
            _clientRepo = clientRepo.ArgNotNull(nameof(clientRepo));
        }

        public async Task<OpenBankingClientProfileResponse> CreateAsync(BankClientProfile publicClientProfile)
        {
            publicClientProfile.ArgNotNull(nameof(publicClientProfile));

            // Load relevant objects
            var softwareStatementProfileForNewClient =
                await _softwareStatementProfileRepo.GetAsync(publicClientProfile.BankClient
                    .SoftwareStatementProfileId) ??
                throw new KeyNotFoundException("The Software statement does not exist.");

            // STEP 1
            // Compute claims associated with Open Banking client

            // Get OpenID Connect configuration info
            var openIdConfiguration =
                await GetOpenIdConfigurationAsync(publicClientProfile.BankClient.IssuerUrl);
            new OpenBankingOpenIdConfigurationResponseValidator().Validate(openIdConfiguration)
                .RaiseErrorOnValidationError();

            // Create claims for client reg
            var registrationClaims = Factories.CreateRegistrationClaims(publicClientProfile.BankClient.IssuerUrl,
                softwareStatementProfileForNewClient, false);
            var registrationClaimsOverrides =
                publicClientProfile.BankClient.ClientRegistrationClaimsOverrides;
            if (registrationClaimsOverrides != null)
            {
                registrationClaims.Aud = registrationClaimsOverrides.RequestAudience;
            }

            var persistentRegistrationClaims =
                _mapper.Map<Model.Persistent.OpenBankingClientRegistrationClaims>(registrationClaims);

            // STEP 2
            // Check for existing Open Banking client for issuer URL
            // If we have an Open Banking client with the same issuer URL we will check if the claims match.
            // If they do, we will re-use this client.
            // Otherwise we will return an error as only support a single client per issuer URL at present.
            var existingClient =
                (await _clientRepo.GetAsync(c => c.IssuerUrl == publicClientProfile.BankClient.IssuerUrl))
                .SingleOrDefault();
            if (existingClient != null)
            {
                if (existingClient.ClientRegistrationClaims != persistentRegistrationClaims)
                {
                    throw new Exception(
                        "There is already a client for this issuer URL but it cannot be re-used because claims are different.");
                }
            }

            // STEP 3
            // Create new Open Banking client by posting JWT
            string jwt = null; // null by default, will be defined if new client created
            BankClient client;
            if (existingClient == null)
            {
                var jwtFactory = new JwtFactory();
                jwt = jwtFactory.CreateJwt(softwareStatementProfileForNewClient, registrationClaims, false);

                var registrationResponse = await new HttpRequestBuilder()
                    .SetMethod(HttpMethod.Post)
                    .SetUri(openIdConfiguration.RegistrationEndpoint)
                    .SetContent(jwt)
                    .SetContentType("application/jwt")
                    .Create()
                    .RequestJsonAsync<OpenBankingClientRegistrationResponse>(_apiClient, false);

                var openBankingClientResponse = new OpenBankingRegistrationData
                {
                    ClientId = registrationResponse.ClientId,
                    ClientIdIssuedAt = registrationResponse.ClientIdIssuedAt,
                    ClientSecret = registrationResponse.ClientSecret,
                    ClientSecretExpiresAt = registrationResponse.ClientSecretExpiresAt
                };

                // Create and store Open Banking client
                client = _mapper.Map<BankClient>(publicClientProfile.BankClient);
                client = await PersistOpenBankingClient(client, openIdConfiguration, registrationClaims,
                    openBankingClientResponse);
            }
            else
            {
                client = existingClient;
            }

            // Create and store Open Banking client profile
            var clientProfile = _mapper.Map<Model.Persistent.BankClientProfile>(publicClientProfile);
            clientProfile = await PersistOpenBankingClientProfile(clientProfile, client.Id);

            return new OpenBankingClientProfileResponse(clientProfile.Id, jwt);
        }

        private async Task<Model.Persistent.BankClient> PersistOpenBankingClient(
            Model.Persistent.BankClient value,
            OpenIdConfiguration openIdConfiguration,
            OpenBankingClientRegistrationClaims registrationClaims,
            OpenBankingRegistrationData openBankingRegistrationData
        )
        {
            value.Id = Guid.NewGuid().ToString();
            value.State = "ok";
            value.OpenIdConfiguration = openIdConfiguration;
            value.ClientRegistrationClaims =
                _mapper.Map<Model.Persistent.OpenBankingClientRegistrationClaims>(registrationClaims);
            value.ClientRegistrationData = openBankingRegistrationData;

            await _clientRepo.SetAsync(value);

            return value;
        }

        private async Task<Model.Persistent.BankClientProfile> PersistOpenBankingClientProfile(
            Model.Persistent.BankClientProfile value,
            string openBankingClientId
        )
        {
            value.Id = Guid.NewGuid().ToString();
            value.State = "ok";
            value.BankClientId = openBankingClientId;

            await _clientProfileRepo.SetAsync(value);

            return value;
        }

        private async Task<OpenIdConfiguration> GetOpenIdConfigurationAsync(string issuerUrl)
        {
            var ub = new UriBuilder(new Uri(issuerUrl)) { Path = ".well-known/openid-configuration" };

            return await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(ub.Uri)
                .Create()
                .RequestJsonAsync<OpenIdConfiguration>(_apiClient, false);
        }
    }
}
