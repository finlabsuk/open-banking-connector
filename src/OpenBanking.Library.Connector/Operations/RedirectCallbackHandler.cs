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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using AuthorisationCallbackDataPublic =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.AuthorisationCallbackData;
using TokenEndpointResponsePublic = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.TokenEndpointResponse;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public class RedirectCallbackHandler
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<DomesticConsent> _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly IDbEntityRepository<BankClientProfile> _openBankingClientRepo;
        private readonly IDbEntityRepository<SoftwareStatementProfile> _softwareStatementProfileRepo;

        public RedirectCallbackHandler(
            IDbEntityRepository<SoftwareStatementProfile> softwareStatementProfileRepo,
            IApiClient apiClient, IEntityMapper mapper,
            IDbEntityRepository<BankClientProfile> openBankingClientRepo,
            IDbEntityRepository<DomesticConsent> domesticConsentRepo)
        {
            _softwareStatementProfileRepo =
                softwareStatementProfileRepo.ArgNotNull(nameof(softwareStatementProfileRepo));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _mapper = mapper.ArgNotNull(nameof(mapper));
            _openBankingClientRepo = openBankingClientRepo.ArgNotNull(nameof(openBankingClientRepo));
            _domesticConsentRepo = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
        }

        public async Task CreateAsync(AuthorisationCallbackDataPublic redirectData)
        {
            redirectData.ArgNotNull(nameof(redirectData));

            // Load relevant data objects
            DomesticConsent consent =
                (await _domesticConsentRepo.GetAsync(dc => dc.State == redirectData.Response.State))
                .FirstOrDefault();
            if (consent == null)
            {
                throw new KeyNotFoundException(
                    $"Consent with redirect state '{redirectData.Response.State}' not found.");
            }

            BankClientProfile clientProfile = await _openBankingClientRepo.GetAsync(consent.ApiProfileId);
            if (clientProfile == null)
            {
                throw new KeyNotFoundException(
                    $"Client profile with ID '{consent.ApiProfileId}' not found.");
            }

            SoftwareStatementProfile softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(clientProfile.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException("The Software statement does not exist.");


            // Obtain token for consent
            string redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            TokenEndpointResponsePublic tokenEndpointResponse =
                await PostAuthCodeGrant(redirectData.Response.AuthorisationCode, redirectUrl, clientProfile);

            // Update consent with token
            TokenEndpointResponse value =
                _mapper.Map<TokenEndpointResponse>(tokenEndpointResponse);
            consent.TokenEndpointResponse = value;
            await _domesticConsentRepo.UpsertAsync(consent);
        }

        private async Task<TokenEndpointResponsePublic> PostAuthCodeGrant(string authCode, string redirectUrl,
            BankClientProfile client)
        {
            UriBuilder ub = new UriBuilder(new Uri(client.OpenIdConfiguration.TokenEndpoint));

            // Assemble URL-encoded form data
            string authHeader = null;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
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
                string authString = client.BankClientRegistrationData.ClientId + ":" +
                                    client.BankClientRegistrationData.ClientSecret;
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(authString);
                authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                if (client.BankClientRegistrationClaims.TokenEndpointAuthMethod == "tls_client_auth")
                {
                    throw new InvalidOperationException("Found unsupported TokenEndpointAuthMethod");
                }
            }

            string content = keyValuePairs.ToUrlEncoded();

            // Assemble headers
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader("x-fapi-financial-id", client.XFapiFinancialId)
            };
            if (authHeader != null)
            {
                headers.Add(new HttpHeader("Authorization", authHeader));
            }

            Models.Ob.TokenEndpointResponse resp = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/x-www-form-urlencoded")
                .SetContent(content)
                .Create()
                .RequestJsonAsync<Models.Ob.TokenEndpointResponse>(_apiClient, false);

            TokenEndpointResponsePublic result = _mapper.Map<TokenEndpointResponsePublic>(resp);

            return result;
        }
    }
}
