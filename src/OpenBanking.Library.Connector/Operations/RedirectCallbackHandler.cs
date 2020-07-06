// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using AuthorisationCallbackDataPublic =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.AuthorisationCallbackData;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public class RedirectCallbackHandler
    {
        private readonly IApiClient _apiClient;
        private readonly IDbMultiEntityMethods _dbContextService;
        private readonly IDbEntityRepository<DomesticConsent> _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly IDbEntityRepository<BankClientProfile> _openBankingClientRepo;
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;

        public RedirectCallbackHandler(
            IApiClient apiClient,
            IEntityMapper mapper,
            IDbEntityRepository<BankClientProfile> openBankingClientRepo,
            IDbEntityRepository<DomesticConsent> domesticConsentRepo,
            ISoftwareStatementProfileService softwareStatementProfileService,
            IDbMultiEntityMethods dbMultiEntityMethods)
        {
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _mapper = mapper.ArgNotNull(nameof(mapper));
            _openBankingClientRepo = openBankingClientRepo.ArgNotNull(nameof(openBankingClientRepo));
            _domesticConsentRepo = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
            _softwareStatementProfileService = softwareStatementProfileService;
            _dbContextService = dbMultiEntityMethods;
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
                throw new KeyNotFoundException($"Client profile with ID '{consent.ApiProfileId}' not found.");
            }

            SoftwareStatementProfile softwareStatementProfile =
                _softwareStatementProfileService.GetSoftwareStatementProfile(clientProfile.SoftwareStatementProfileId);

            // Obtain token for consent
            string redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            TokenEndpointResponse tokenEndpointResponse =
                await PostAuthCodeGrant(
                    authCode: redirectData.Response.Code,
                    redirectUrl: redirectUrl,
                    client: clientProfile);

            // Update consent with token
            consent.TokenEndpointResponse = tokenEndpointResponse;
            await _dbContextService.SaveChangesAsync();
        }

        private async Task<TokenEndpointResponse> PostAuthCodeGrant(
            string authCode,
            string redirectUrl,
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
                client.BankClientRegistrationData.ClientSecret.ArgNotNull("No client secret available.");
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
                new HttpHeader(name: "x-fapi-financial-id", value: client.XFapiFinancialId)
            };
            if (authHeader != null)
            {
                headers.Add(new HttpHeader(name: "Authorization", value: authHeader));
            }

            TokenEndpointResponse resp = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/x-www-form-urlencoded")
                .SetContent(content)
                .Create()
                .RequestJsonAsync<TokenEndpointResponse>(client: _apiClient, requestContentIsJson: false);

            return resp;
        }
    }
}
