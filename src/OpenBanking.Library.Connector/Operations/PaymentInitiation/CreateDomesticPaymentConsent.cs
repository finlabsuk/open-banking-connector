// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;
using OBWriteDomesticConsentPublic =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.OBWriteDomesticConsent;
using TokenEndpointResponse = FinnovationLabs.OpenBanking.Library.Connector.Models.Ob.TokenEndpointResponse;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateDomesticPaymentConsent
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<ApiProfile> _apiProfileRepo;
        private readonly IDbEntityRepository<BankClientProfile> _bankClientProfileRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly IDbEntityRepository<DomesticConsent> _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly IDbEntityRepository<SoftwareStatementProfile> _softwareStatementProfileRepo;

        public CreateDomesticPaymentConsent(
            IApiClient apiClient,
            IEntityMapper mapper,
            IDbMultiEntityMethods dbMultiEntityMethods,
            IDbEntityRepository<SoftwareStatementProfile> softwareStatementProfileRepo,
            IDbEntityRepository<BankClientProfile> bankClientProfileRepo,
            IDbEntityRepository<ApiProfile> apiProfileRepo,
            IDbEntityRepository<DomesticConsent> domesticConsentRepo)
        {
            _apiClient = apiClient;
            _mapper = mapper;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _bankClientProfileRepo = bankClientProfileRepo;
            _apiProfileRepo = apiProfileRepo;
            _domesticConsentRepo = domesticConsentRepo;
        }

        public async Task<PaymentConsentResponse> CreateAsync(OBWriteDomesticConsentPublic consent)
        {
            consent.ArgNotNull(nameof(consent));

            // Load relevant objects
            ApiProfile apiProfile = await _apiProfileRepo.GetAsync(consent.ApiProfileId)
                                    ?? throw new KeyNotFoundException("The API Profile does not exist.");
            BankClientProfile bankClientProfile = await _bankClientProfileRepo.GetAsync(apiProfile.BankClientProfileId)
                                                  ?? throw new KeyNotFoundException(
                                                      "The Bank Client Profile does not exist.");
            SoftwareStatementProfile softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankClientProfile.SoftwareStatementProfileId)
                ?? throw new KeyNotFoundException("The Software Statement Profile does not exist.");

            // Get client credentials grant (we will not cache token for now but simply use to POST consent)
            TokenEndpointResponse tokenEndpointResponse =
                await PostClientCredentialsGrant("payments", bankClientProfile);
            // TODO: validate the response???
            OBWriteDomesticConsent2 consent2 = _mapper.Map<OBWriteDomesticConsent2>(consent);

            // Create new Open Banking consent by posting JWT
            JwtFactory jwtFactory = new JwtFactory();
            string jwt = jwtFactory.CreateJwt(softwareStatementProfile, consent2, true);
            string[] jwsComponents = jwt.Split('.');
            string jwsSignature = $"{jwsComponents[0]}..{jwsComponents[2]}";
            UriBuilder ub = new UriBuilder(new Uri(apiProfile.BaseUrl + "/domestic-payment-consents"));
            string payloadJson = JsonConvert.SerializeObject(consent2);
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader("x-fapi-financial-id", bankClientProfile.XFapiFinancialId),
                new HttpHeader("Authorization", "Bearer " + tokenEndpointResponse.AccessToken),
                new HttpHeader("x-idempotency-key", Guid.NewGuid().ToString()),
                new HttpHeader("x-jws-signature", jwsSignature)
            };

            OBWriteDomesticConsentResponse2 consentResponse = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/json")
                .SetContent(payloadJson)
                .Create()
                .RequestJsonAsync<OBWriteDomesticConsentResponse2>(_apiClient, true);
            // TODO: validate response

            // Generate URL for user auth
            string consentId = consentResponse.Data.ConsentId;
            string redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            OAuth2RequestObjectClaims oAuth2RequestObjectClaims = Factories.CreateOAuth2RequestObjectClaims(
                bankClientProfile,
                redirectUrl, new[] { "openid", "payments" },
                consentId);
            string requestObjectJwt = jwtFactory.CreateJwt(softwareStatementProfile, oAuth2RequestObjectClaims, false);
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "response_type", oAuth2RequestObjectClaims.ResponseType },
                { "client_id", oAuth2RequestObjectClaims.ClientId },
                { "redirect_uri", oAuth2RequestObjectClaims.RedirectUri },
                { "scope", oAuth2RequestObjectClaims.Scope },
                { "request", requestObjectJwt },
                { "nonce", oAuth2RequestObjectClaims.Nonce },
                { "state", oAuth2RequestObjectClaims.State }
            };
            string queryString = keyValuePairs.ToUrlEncoded();
            string authUrl = bankClientProfile.OpenIdConfiguration.AuthorizationEndpoint + "?" + queryString;

            // Create and store persistent object
            DomesticConsent value = _mapper.Map<DomesticConsent>(consent);
            await _domesticConsentRepo.UpsertAsync(value);
            await _dbMultiEntityMethods.SaveChangesAsync();

            return new PaymentConsentResponse
            {
                AuthUrl = authUrl,
                ConsentId = consentId
            };
        }

        private async Task<TokenEndpointResponse> PostClientCredentialsGrant(string scope, BankClientProfile client)
        {
            UriBuilder ub = new UriBuilder(new Uri(client.OpenIdConfiguration.TokenEndpoint));

            // Assemble URL-encoded form data
            string authHeader = null;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", scope }
            };
            if (client.BankClientRegistrationClaims.TokenEndpointAuthMethod == "tls_client_auth")
            {
                keyValuePairs["client_id"] = client.BankClientRegistrationData.ClientId;
            }
            else if (client.BankClientRegistrationClaims.TokenEndpointAuthMethod ==
                     "client_secret_basic")
            {
                client.BankClientRegistrationData.ClientSecret.InvalidOpOnNull("No client secret available.");
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

            return await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/x-www-form-urlencoded")
                .SetContent(content)
                .Create()
                .RequestJsonAsync<TokenEndpointResponse>(_apiClient, false);
        }
    }
}
