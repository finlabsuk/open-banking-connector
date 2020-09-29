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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using OBWriteDomesticConsent =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBWriteDomesticConsent4;
using OBWriteDomesticConsentResponse =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.
    OBWriteDomesticConsentResponse4;
using RequestDomesticPaymentConsent =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class CreateDomesticPaymentConsent
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<BankRegistration> _bankClientProfileRepo;
        private readonly IDbEntityRepository<BankProfile> _bankProfileRepo;
        private readonly IDbEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly IDbEntityRepository<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;
        private readonly ITimeProvider _timeProvider;

        public CreateDomesticPaymentConsent(
            IApiClient apiClient,
            IDbEntityRepository<BankProfile> bankProfileRepo,
            IDbEntityRepository<BankRegistration> bankClientProfileRepo,
            IDbEntityRepository<Bank> bankRepo,
            IDbMultiEntityMethods dbMultiEntityMethods,
            IDbEntityRepository<DomesticPaymentConsent> domesticConsentRepo,
            IEntityMapper mapper,
            ISoftwareStatementProfileService softwareStatementProfileService,
            ITimeProvider timeProvider)
        {
            _apiClient = apiClient;
            _bankProfileRepo = bankProfileRepo;
            _bankClientProfileRepo = bankClientProfileRepo;
            _bankRepo = bankRepo;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _domesticConsentRepo = domesticConsentRepo;
            _mapper = mapper;
            _softwareStatementProfileService = softwareStatementProfileService;
            _timeProvider = timeProvider;
        }

        // Load bank and bankProfile
        public async Task<DomesticPaymentConsentResponse> CreateAsync(
            RequestDomesticPaymentConsent request,
            string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            Bank bank;
            BankProfile bankProfile;
            switch (request.BankProfileId)
            {
                // No bank profile specified directly or indirectly
                case null when request.BankName is null:
                    throw new ArgumentException("One of BankProfileId and BankName must be non-null.");

                // Look up bank registration
                case null:
                {
                    try
                    {
                        bank = (await _bankRepo.GetAsync(b => b.Name == request.BankName))
                            .Single();
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("No record found for BankName.");
                    }

                    string bankProfileId;
                    if (request.UseStagingBankProfile)
                    {
                        bankProfileId = bank.StagingBankProfileId.Data ?? throw new ArgumentException(
                            "Bank specified by BankName doesn't have StagingBankProfileId.");
                        bankProfile = await _bankProfileRepo.GetAsync(bankProfileId) ??
                                      throw new KeyNotFoundException(
                                          "No record found for BankProfileId specified in bank StagingBankProfileId.");
                    }
                    else
                    {
                        bankProfileId = bank.DefaultBankProfileId.Data ?? throw new ArgumentException(
                            "Bank specified by BankName doesn't have DefaultBankProfileId.");
                        bankProfile = await _bankProfileRepo.GetAsync(bankProfileId) ??
                                      throw new KeyNotFoundException(
                                          "No record found for BankProfileId specified in bank DefaultBankProfileId.");
                    }

                    break;
                }

                default:
                {
                    if (!(request.BankName is null))
                    {
                        throw new ArgumentException("Both BankProfileId and BankName are non-null.");
                    }

                    bankProfile = await _bankProfileRepo.GetAsync(request.BankProfileId) ??
                                  throw new KeyNotFoundException(
                                      "No record found for BankProfileId specified in bank DefaultBankProfileId.");
                    bank = await _bankRepo.GetAsync(bankProfile.BankId) ??
                           throw new KeyNotFoundException("No record found for BankId specified in bank BankProfile.");
                    break;
                }
            }

            // Checks
            PaymentInitiationApi paymentInitiationApi = bankProfile.PaymentInitiationApi ??
                                                        throw new NullReferenceException(
                                                            "Bank profile does not support PaymentInitiation API");

            // Load relevant objects
            BankRegistration bankRegistration = await _bankClientProfileRepo.GetAsync(bankProfile.BankRegistrationId)
                                                ?? throw new KeyNotFoundException(
                                                    "The Bank Client Profile does not exist.");
            SoftwareStatementProfile softwareStatementProfile =
                _softwareStatementProfileService.GetSoftwareStatementProfile(
                    bankRegistration.SoftwareStatementProfileId);

            // Get client credentials grant (we will not cache token for now but simply use to POST consent)
            TokenEndpointResponse tokenEndpointResponse =
                await PostClientCredentialsGrant(
                    scope: "payments",
                    client: bankRegistration,
                    orgId: bank.XFapiFinancialId);
            // TODO: validate the response???

            // Create new Open Banking consent by posting JWT
            JwtFactory jwtFactory = new JwtFactory();
            OBWriteDomesticConsentResponse consentResponse;
            OBWriteDomesticConsent requestDomesticConsent = DomesticPaymentConsent.GetOBWriteDomesticConsent(request);
            switch (paymentInitiationApi.ApiVersion)
            {
                case ApiVersion.V3P1P1:
                    OBWriteDomesticConsent2 newDomesticConsent =
                        _mapper.Map<OBWriteDomesticConsent2>(requestDomesticConsent);
                    OBWriteDomesticConsentResponse2 rawConsentResponse = await
                        PostDomesticConsent<OBWriteDomesticConsent2, OBWriteDomesticConsentResponse2>(
                            jwtFactory: jwtFactory,
                            softwareStatementProfile: softwareStatementProfile,
                            consent: newDomesticConsent,
                            paymentInitiationApi: paymentInitiationApi,
                            bankClientProfile: bankRegistration,
                            tokenEndpointResponse: tokenEndpointResponse,
                            orgId: bank.XFapiFinancialId);
                    consentResponse = _mapper.Map<OBWriteDomesticConsentResponse>(rawConsentResponse);
                    break;
                case ApiVersion.V3P1P2:
                    throw new ArgumentOutOfRangeException();
                case ApiVersion.V3P1P4:
                    consentResponse = await
                        PostDomesticConsent<OBWriteDomesticConsent, OBWriteDomesticConsentResponse>(
                            jwtFactory: jwtFactory,
                            softwareStatementProfile: softwareStatementProfile,
                            consent: requestDomesticConsent,
                            paymentInitiationApi: paymentInitiationApi,
                            bankClientProfile: bankRegistration,
                            tokenEndpointResponse: tokenEndpointResponse,
                            orgId: bank.XFapiFinancialId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Generate URL for user auth
            string consentId = consentResponse.Data.ConsentId;
            string redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            if (redirectUrl == "")
            {
                redirectUrl = bankRegistration.BankClientRegistrationRequest.RedirectUris[0];
            }

            OAuth2RequestObjectClaims oAuth2RequestObjectClaims =
                OAuth2RequestObjectClaimsFactory.CreateOAuth2RequestObjectClaims(
                    openBankingClient: bankRegistration,
                    redirectUrl: redirectUrl,
                    scope: new[] { "openid", "payments" },
                    intentId: consentId,
                    issuerUrl: bank.IssuerUrl);
            string requestObjectJwt = jwtFactory.CreateJwt(
                profile: softwareStatementProfile,
                claims: oAuth2RequestObjectClaims,
                useOpenBankingJwtHeaders: false,
                paymentInitiationApiVersion: paymentInitiationApi.ApiVersion);
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
            string authUrl = bankRegistration.OpenIdConfiguration.AuthorizationEndpoint + "?" + queryString;

            // Create and store persistent object
            DomesticPaymentConsent persistedDomesticPaymentConsent = new DomesticPaymentConsent(
                timeProvider: _timeProvider,
                state: oAuth2RequestObjectClaims.State,
                bankProfileId: bankProfile.Id,
                obWriteDomesticConsent: requestDomesticConsent,
                obWriteDomesticConsentResponse: consentResponse,
                createdBy: createdBy);
            await _domesticConsentRepo.AddAsync(persistedDomesticPaymentConsent);
            await _dbMultiEntityMethods.SaveChangesAsync();

            DomesticPaymentConsentResponse response = persistedDomesticPaymentConsent.PublicResponse;
            response.AuthUrl = authUrl;

            return response;
        }

        private async Task<TokenEndpointResponse> PostClientCredentialsGrant(
            string scope,
            BankRegistration client,
            string orgId)
        {
            UriBuilder ub = new UriBuilder(new Uri(client.OpenIdConfiguration.TokenEndpoint));

            // Assemble URL-encoded form data
            string? authHeader = null;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", scope }
            };
            if (client.BankClientRegistrationRequest.TokenEndpointAuthMethod ==
                TokenEndpointAuthMethodEnum.TlsClientAuth)
            {
                keyValuePairs["client_id"] = client.BankClientRegistration.ClientId;
            }
            else if (client.BankClientRegistrationRequest.TokenEndpointAuthMethod ==
                     TokenEndpointAuthMethodEnum.ClientSecretBasic)
            {
                client.BankClientRegistration.ClientSecret.InvalidOpOnNull("No client secret available.");
                string authString = client.BankClientRegistration.ClientId + ":" +
                                    client.BankClientRegistration.ClientSecret;
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(authString);
                authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                if (client.BankClientRegistrationRequest.TokenEndpointAuthMethod ==
                    TokenEndpointAuthMethodEnum.TlsClientAuth)
                {
                    throw new InvalidOperationException("Found unsupported TokenEndpointAuthMethod");
                }
            }

            string content = keyValuePairs.ToUrlEncoded();

            // Assemble headers
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader(name: "x-fapi-financial-id", value: orgId)
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
            PaymentInitiationApi paymentInitiationApi,
            BankRegistration bankClientProfile,
            TokenEndpointResponse tokenEndpointResponse,
            string orgId)
            where ApiRequestType : class
            where ApiResponseType : class
        {
            string jwt = jwtFactory.CreateJwt(
                profile: softwareStatementProfile,
                claims: consent,
                useOpenBankingJwtHeaders: true,
                paymentInitiationApiVersion: paymentInitiationApi.ApiVersion);
            string[] jwsComponents = jwt.Split('.');
            string jwsSignature = $"{jwsComponents[0]}..{jwsComponents[2]}";
            UriBuilder ub = new UriBuilder(new Uri(paymentInitiationApi.BaseUrl + "/domestic-payment-consents"));
            string payloadJson = JsonConvert.SerializeObject(consent);
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader(name: "x-fapi-financial-id", value: orgId),
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
