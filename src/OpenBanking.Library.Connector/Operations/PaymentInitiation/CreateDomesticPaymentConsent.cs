// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModel.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;
using BankClient = FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.BankClient;
using OBWriteDomesticConsent =
    FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation.OBWriteDomesticConsent;
using TokenEndpointResponse = FinnovationLabs.OpenBanking.Library.Connector.Model.Ob.TokenEndpointResponse;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateDomesticPaymentConsent
    {
        private readonly IApiClient _apiClient;
        private readonly IOpenBankingClientProfileRepository _clientProfileRepo;
        private readonly IOpenBankingClientRepository _clientRepo;
        private readonly IDomesticConsentRepository _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly ISoftwareStatementProfileRepository _softwareStatementProfileRepo;

        public CreateDomesticPaymentConsent(
            IEntityMapper mapper,
            IApiClient apiClient,
            ISoftwareStatementProfileRepository softwareStatementProfileRepo,
            IOpenBankingClientProfileRepository clientProfileRepo,
            IOpenBankingClientRepository clientRepo,
            IDomesticConsentRepository domesticConsentRepo
        )
        {
            _mapper = mapper.ArgNotNull(nameof(mapper));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _softwareStatementProfileRepo =
                softwareStatementProfileRepo.ArgNotNull(nameof(softwareStatementProfileRepo));
            _clientProfileRepo = clientProfileRepo.ArgNotNull(nameof(clientProfileRepo));
            _clientRepo = clientRepo.ArgNotNull(nameof(clientRepo));
            _domesticConsentRepo = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
        }

        public async Task<PaymentConsentResponse> CreateAsync(OBWriteDomesticConsent consent)
        {
            consent.ArgNotNull(nameof(consent));

            // Load relevant objects
            var clientProfile = await _clientProfileRepo.GetAsync(consent.OpenBankingClientProfileId) ??
                                throw new KeyNotFoundException("The OB Client Profile does not exist.");
            var client = await _clientRepo.GetAsync(clientProfile.BankClientId) ??
                         throw new KeyNotFoundException("The OB Client Profile does not exist.");
            var softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(client.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException("The Software statement does not exist.");

            // Get client credentials grant (we will not cache token for now but simply use to POST consent)
            var tokenEndpointResponse = await PostClientCredentialsGrant("payments", client);
            // TODO: validate the response???
            var consent2 = _mapper.Map<OBWriteDomesticConsent2>(consent);

            // Create new Open Banking consent by posting JWT
            var jwtFactory = new JwtFactory();
            var jwt = jwtFactory.CreateJwt(softwareStatementProfile, consent2, true);
            var jwsComponents = jwt.Split('.');
            var jwsSignature = $"{jwsComponents[0]}..{jwsComponents[2]}";
            var ub = new UriBuilder(new Uri(clientProfile.PaymentInitiationApiBaseUrl + "/domestic-payment-consents"));
            var payloadJson = JsonConvert.SerializeObject(consent2);
            var headers = new List<HttpHeader>
            {
                new HttpHeader("x-fapi-financial-id", client.XFapiFinancialId),
                new HttpHeader("Authorization", "Bearer " + tokenEndpointResponse.AccessToken),
                new HttpHeader("x-idempotency-key", Guid.NewGuid().ToString()),
                new HttpHeader("x-jws-signature", jwsSignature)
            };

            var consentResponse = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/json")
                .SetContent(payloadJson)
                .Create()
                .RequestJsonAsync<OBWriteDomesticConsentResponse2>(_apiClient, true);
            // TODO: validate response

            // Generate URL for user auth
            var consentId = consentResponse.Data.ConsentId;
            var redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            var oAuth2RequestObjectClaims = Factories.CreateOAuth2RequestObjectClaims(client,
                redirectUrl, new[] { "openid", "payments" },
                consentId);
            var requestObjectJwt = jwtFactory.CreateJwt(softwareStatementProfile, oAuth2RequestObjectClaims, false);
            var keyValuePairs = new Dictionary<string, string>
            {
                { "response_type", oAuth2RequestObjectClaims.ResponseType },
                { "client_id", oAuth2RequestObjectClaims.ClientId },
                { "redirect_uri", oAuth2RequestObjectClaims.RedirectUri },
                { "scope", oAuth2RequestObjectClaims.Scope },
                { "request", requestObjectJwt },
                { "nonce", oAuth2RequestObjectClaims.Nonce },
                { "state", oAuth2RequestObjectClaims.State }
            };
            var queryString = keyValuePairs.ToUrlEncoded();
            var authUrl = client.OpenIdConfiguration.AuthorizationEndpoint + "?" + queryString;

            // Create and store persistent object
            var value = _mapper.Map<DomesticConsent>(consent);
            value = await _domesticConsentRepo.SetAsync(value);

            return new PaymentConsentResponse
            {
                AuthUrl = authUrl,
                ConsentId = consentId
            };
        }

        private async Task<TokenEndpointResponse> PostClientCredentialsGrant(string scope, BankClient client)
        {
            var ub = new UriBuilder(new Uri(client.OpenIdConfiguration.TokenEndpoint));

            // Assemble URL-encoded form data
            string authHeader = null;
            var keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", scope }
            };
            if (client.ClientRegistrationClaims.TokenEndpointAuthMethod == "tls_client_auth")
            {
                keyValuePairs["client_id"] = client.ClientRegistrationData.ClientId;
            }
            else if (client.ClientRegistrationClaims.TokenEndpointAuthMethod ==
                     "client_secret_basic")
            {
                client.ClientRegistrationData.ClientSecret.InvalidOpOnNull("No client secret available.");
                var authString = client.ClientRegistrationData.ClientId + ":" +
                                 client.ClientRegistrationData.ClientSecret;
                var plainTextBytes = Encoding.UTF8.GetBytes(authString);
                authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                if (client.ClientRegistrationClaims.TokenEndpointAuthMethod == "tls_client_auth")
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
