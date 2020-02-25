// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using OpenBankingClient = FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.OpenBankingClient;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public class RedirectCallbackHandler
    {
        private readonly IApiClient _apiClient;
        private readonly IDomesticConsentRepository _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly IOpenBankingClientProfileRepository _openBankingClientProfileRepo;
        private readonly IOpenBankingClientRepository _openBankingClientRepo;

        public RedirectCallbackHandler(IApiClient apiClient, IEntityMapper mapper,
            IOpenBankingClientRepository openBankingClientRepo,
            IOpenBankingClientProfileRepository openBankingClientProfileRepo,
            IDomesticConsentRepository domesticConsentRepo)
        {
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _mapper = mapper.ArgNotNull(nameof(mapper));
            _openBankingClientRepo = openBankingClientRepo.ArgNotNull(nameof(openBankingClientRepo));
            _openBankingClientProfileRepo =
                openBankingClientProfileRepo.ArgNotNull(nameof(openBankingClientProfileRepo));
            _domesticConsentRepo = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
        }

        public async Task CreateAsync(AuthorisationCallbackData redirectData)
        {
            redirectData.ArgNotNull(nameof(redirectData));

            // Load relevant data objects
            var consent = (await _domesticConsentRepo.GetAsync(dc => dc.State == redirectData.Body.State))
                .FirstOrDefault();
            if (consent == null)
            {
                throw new KeyNotFoundException($"Consent with redirect state '{redirectData.Body.State}' not found.");
            }

            var clientProfile = await _openBankingClientProfileRepo.GetAsync(consent.OpenBankingClientProfileId);
            if (clientProfile == null)
            {
                throw new KeyNotFoundException(
                    $"Client profile with ID '{consent.OpenBankingClientProfileId}' not found.");
            }

            var client = await _openBankingClientRepo.GetAsync(clientProfile.OpenBankingClientId);

            // Obtain token for consent
            var redirectUrl = clientProfile.RedirectUrl;
            var tokenEndpointResponse =
                await PostAuthCodeGrant(redirectData.Body.AuthorisationCode, redirectUrl, client);

            // Update consent with token
            var value = _mapper.Map<Model.Persistent.TokenEndpointResponse>(tokenEndpointResponse);
            consent.TokenEndpointResponse = value;
            await _domesticConsentRepo.SetAsync(consent);
        }

        private async Task<TokenEndpointResponse> PostAuthCodeGrant(string authCode, string redirectUrl,
            OpenBankingClient client)
        {
            var ub = new UriBuilder(new Uri(client.OpenIdConfiguration.TokenEndpoint));

            // Assemble URL-encoded form data
            string authHeader = null;
            var keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
                { "code", authCode }
            };
            if (client.OpenBankingClientRegistrationClaims.TokenEndpointAuthMethod == "tls_client_auth")
            {
                keyValuePairs["client_id"] = client.OpenBankingRegistrationData.ClientId;
            }
            else if (client.OpenBankingClientRegistrationClaims.TokenEndpointAuthMethod ==
                     "client_secret_basic")
            {
                client.OpenBankingRegistrationData.ClientSecret.ArgNotNull(
                    "No client secret available.");
                var authString = client.OpenBankingRegistrationData.ClientId + ":" +
                                 client.OpenBankingRegistrationData.ClientSecret;
                var plainTextBytes = Encoding.UTF8.GetBytes(authString);
                authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                if (client.OpenBankingClientRegistrationClaims.TokenEndpointAuthMethod == "tls_client_auth")
                {
                    throw new InvalidOperationException("Found unsupported TokenEndpointAuthMethod");
                }
            }

            var content = keyValuePairs.ToUrlEncoded();

            // Assemble headers
            var headers = new List<HttpHeader>
            {
                new HttpHeader("x-fapi-financial-id", client.XFapiFinancialId)
            };
            if (authHeader != null)
            {
                headers.Add(new HttpHeader("Authorization", authHeader));
            }

            var resp = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/x-www-form-urlencoded")
                .SetContent(content)
                .Create()
                .RequestJsonAsync<Model.Ob.TokenEndpointResponse>(_apiClient);

            var result = _mapper.Map<TokenEndpointResponse>(resp);

            return result;
        }
    }
}
