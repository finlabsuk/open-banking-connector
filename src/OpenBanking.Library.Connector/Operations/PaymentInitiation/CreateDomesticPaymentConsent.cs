// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;

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
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;

        public CreateDomesticPaymentConsent(
            IApiClient apiClient,
            IEntityMapper mapper,
            IDbMultiEntityMethods dbMultiEntityMethods,
            IDbEntityRepository<BankClientProfile> bankClientProfileRepo,
            IDbEntityRepository<ApiProfile> apiProfileRepo,
            IDbEntityRepository<DomesticConsent> domesticConsentRepo,
            ISoftwareStatementProfileService softwareStatementProfileService)
        {
            _apiClient = apiClient;
            _mapper = mapper;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _bankClientProfileRepo = bankClientProfileRepo;
            _apiProfileRepo = apiProfileRepo;
            _domesticConsentRepo = domesticConsentRepo;
            _softwareStatementProfileService = softwareStatementProfileService;
        }

        public async Task<PaymentConsentResponse> CreateAsync(DomesticPaymentConsent consent)
        {
            consent.ArgNotNull(nameof(consent));

            // Load relevant objects
            ApiProfile apiProfile = await _apiProfileRepo.GetAsync(consent.ApiProfileId)
                                    ?? throw new KeyNotFoundException("The API Profile does not exist.");
            BankClientProfile bankClientProfile = await _bankClientProfileRepo.GetAsync(apiProfile.BankClientProfileId)
                                                  ?? throw new KeyNotFoundException(
                                                      "The Bank Client Profile does not exist.");
            SoftwareStatementProfile softwareStatementProfile =
                _softwareStatementProfileService.GetSoftwareStatementProfile(
                    bankClientProfile.SoftwareStatementProfileId);

            // Get client credentials grant (we will not cache token for now but simply use to POST consent)
            TokenEndpointResponse tokenEndpointResponse =
                await PostClientCredentialsGrant(scope: "payments", client: bankClientProfile);
            // TODO: validate the response???

            // Create new Open Banking consent by posting JWT
            JwtFactory jwtFactory = new JwtFactory();
            OBWriteDomesticConsentResponse4 consentResponse;
            switch (apiProfile.ApiVersion)
            {
                case ApiVersion.V3P1P1:
                    OBWriteDomesticConsent2 newDomesticConsent =
                        _mapper.Map<OBWriteDomesticConsent2>(consent.DomesticConsent);
                    OBWriteDomesticConsentResponse2 rawConsentResponse = await
                        PostDomesticConsent<OBWriteDomesticConsent2, OBWriteDomesticConsentResponse2>(
                            jwtFactory: jwtFactory,
                            softwareStatementProfile: softwareStatementProfile,
                            consent: newDomesticConsent,
                            apiProfile: apiProfile,
                            bankClientProfile: bankClientProfile,
                            tokenEndpointResponse: tokenEndpointResponse);
                    consentResponse = _mapper.Map<OBWriteDomesticConsentResponse4>(rawConsentResponse);
                    break;
                case ApiVersion.V3P1P2:
                    throw new ArgumentOutOfRangeException();
                case ApiVersion.V3P1P4:
                    consentResponse = await
                        PostDomesticConsent<OBWriteDomesticConsent4, OBWriteDomesticConsentResponse4>(
                            jwtFactory: jwtFactory,
                            softwareStatementProfile: softwareStatementProfile,
                            consent: consent.DomesticConsent,
                            apiProfile: apiProfile,
                            bankClientProfile: bankClientProfile,
                            tokenEndpointResponse: tokenEndpointResponse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Generate URL for user auth
            string consentId = consentResponse.Data.ConsentId;
            string redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            if (redirectUrl == "")
            {
                redirectUrl = bankClientProfile.BankClientRegistrationClaims.RedirectUris[0];
            }

            OAuth2RequestObjectClaims oAuth2RequestObjectClaims = Factories.CreateOAuth2RequestObjectClaims(
                openBankingClient: bankClientProfile,
                redirectUrl: redirectUrl,
                scope: new[] { "openid", "payments" },
                intentId: consentId);
            string requestObjectJwt = jwtFactory.CreateJwt(
                profile: softwareStatementProfile,
                claims: oAuth2RequestObjectClaims,
                useOpenBankingJwtHeaders: false);
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
            string domesticConsentId = Guid.NewGuid().ToString();
            DomesticConsent value = new DomesticConsent
            {
                State = oAuth2RequestObjectClaims.State,
                SoftwareStatementProfileId = bankClientProfile.SoftwareStatementProfileId,
                IssuerUrl = bankClientProfile.IssuerUrl,
                ApiProfileId = apiProfile.Id,
                ObWriteDomesticConsent = consent.DomesticConsent,
                TokenEndpointResponse = null,
                Id = domesticConsentId,
                BankId = consentId
            };
            await _domesticConsentRepo.UpsertAsync(value);
            await _dbMultiEntityMethods.SaveChangesAsync();

            return new PaymentConsentResponse
            {
                AuthUrl = authUrl,
                ConsentId = domesticConsentId
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
                new HttpHeader(name: "x-fapi-financial-id", value: client.XFapiFinancialId)
            };
            if (authHeader != null)
            {
                headers.Add(new HttpHeader(name: "Authorization", value: authHeader));
            }

            return await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/x-www-form-urlencoded")
                .SetContent(content)
                .Create()
                .RequestJsonAsync<TokenEndpointResponse>(client: _apiClient, requestContentIsJson: false);
        }

        private async Task<ApiResponseType> PostDomesticConsent<ApiRequestType, ApiResponseType>(
            JwtFactory jwtFactory,
            SoftwareStatementProfile softwareStatementProfile,
            ApiRequestType consent,
            ApiProfile apiProfile,
            BankClientProfile bankClientProfile,
            TokenEndpointResponse tokenEndpointResponse)
            where ApiRequestType : class
            where ApiResponseType : class
        {
            string jwt = jwtFactory.CreateJwt(
                profile: softwareStatementProfile,
                claims: consent,
                useOpenBankingJwtHeaders: true);
            string[] jwsComponents = jwt.Split('.');
            string jwsSignature = $"{jwsComponents[0]}..{jwsComponents[2]}";
            UriBuilder ub = new UriBuilder(new Uri(apiProfile.BaseUrl + "/domestic-payment-consents"));
            string payloadJson = JsonConvert.SerializeObject(consent);
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader(name: "x-fapi-financial-id", value: bankClientProfile.XFapiFinancialId),
                new HttpHeader(name: "Authorization", value: "Bearer " + tokenEndpointResponse.AccessToken),
                new HttpHeader(name: "x-idempotency-key", value: Guid.NewGuid().ToString()),
                new HttpHeader(name: "x-jws-signature", value: jwsSignature)
            };
            return await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/json")
                .SetContent(payloadJson)
                .Create()
                .RequestJsonAsync<ApiResponseType>(client: _apiClient, requestContentIsJson: true);
        }
    }
}
