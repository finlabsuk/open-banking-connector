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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using BankClientProfile = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankClientProfile;
using TokenEndpointResponse = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.TokenEndpointResponse;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public class RedirectCallbackHandler
    {
        private readonly IDbEntityRepository<Models.Persistent.SoftwareStatementProfile> _softwareStatementProfileRepo;
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<DomesticConsent> _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly IDbEntityRepository<BankClientProfile> _openBankingClientRepo;

        public RedirectCallbackHandler(
            IDbEntityRepository<Models.Persistent.SoftwareStatementProfile> softwareStatementProfileRepo,
            IApiClient apiClient, IEntityMapper mapper,
            IDbEntityRepository<BankClientProfile> openBankingClientRepo,
            IDbEntityRepository<DomesticConsent> domesticConsentRepo)
        {
            _softwareStatementProfileRepo = softwareStatementProfileRepo.ArgNotNull(nameof(softwareStatementProfileRepo));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _mapper = mapper.ArgNotNull(nameof(mapper));
            _openBankingClientRepo = openBankingClientRepo.ArgNotNull(nameof(openBankingClientRepo));
            _domesticConsentRepo = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
        }

        public async Task CreateAsync(AuthorisationCallbackData redirectData)
        {
            redirectData.ArgNotNull(nameof(redirectData));

            // Load relevant data objects
            var consent = (await _domesticConsentRepo.GetAsync(dc => dc.State == redirectData.Response.State))
                .FirstOrDefault();
            if (consent == null)
            {
                throw new KeyNotFoundException($"Consent with redirect state '{redirectData.Response.State}' not found.");
            }

            var clientProfile = await _openBankingClientRepo.GetAsync(consent.ApiProfileId);
            if (clientProfile == null)
            {
                throw new KeyNotFoundException(
                    $"Client profile with ID '{consent.ApiProfileId}' not found.");
            }

            var softwareStatementProfile = await _softwareStatementProfileRepo.GetAsync(clientProfile.SoftwareStatementProfileId) ??
                                    throw new KeyNotFoundException("The Software statement does not exist.");


            // Obtain token for consent
            var redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            var tokenEndpointResponse =
                await PostAuthCodeGrant(redirectData.Response.AuthorisationCode, redirectUrl, clientProfile);

            // Update consent with token
            var value = _mapper.Map<Models.Persistent.TokenEndpointResponse>(tokenEndpointResponse);
            consent.TokenEndpointResponse = value;
            await _domesticConsentRepo.SetAsync(consent);
        }

        private async Task<TokenEndpointResponse> PostAuthCodeGrant(string authCode, string redirectUrl,
            BankClientProfile client)
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
            if (client.BankClientRegistrationClaims.TokenEndpointAuthMethod == "tls_client_auth")
            {
                keyValuePairs["client_id"] = client.BankClientRegistrationData.ClientId;
            }
            else if (client.BankClientRegistrationClaims.TokenEndpointAuthMethod ==
                     "client_secret_basic")
            {
                client.BankClientRegistrationData.ClientSecret.ArgNotNull(
                    "No client secret available.");
                var authString = client.BankClientRegistrationData.ClientId + ":" +
                                 client.BankClientRegistrationData.ClientSecret;
                var plainTextBytes = Encoding.UTF8.GetBytes(authString);
                authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                if (client.BankClientRegistrationClaims.TokenEndpointAuthMethod == "tls_client_auth")
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
                .RequestJsonAsync<Models.Ob.TokenEndpointResponse>(_apiClient, false);

            var result = _mapper.Map<TokenEndpointResponse>(resp);

            return result;
        }
    }
}
