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
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access;
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
using Newtonsoft.Json;
using OBWriteDomesticConsent =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBWriteDomesticConsent4;
using OBWriteDomesticConsentResponse =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.
    OBWriteDomesticConsentResponse4;
using RequestDomesticPaymentConsent =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class CreateDomesticPaymentConsent
    {
        private readonly IDbReadOnlyEntityRepository<BankApiInformation> _bankProfileRepo;
        private readonly IDbReadOnlyEntityRepository<BankRegistration> _bankRegistrationRepo;
        private readonly IDbReadOnlyEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly IDbEntityRepository<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IEntityMapper _mapper;
        private readonly IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public CreateDomesticPaymentConsent(
            IDbReadOnlyEntityRepository<BankApiInformation> bankProfileRepo,
            IDbReadOnlyEntityRepository<BankRegistration> bankRegistrationRepo,
            IDbReadOnlyEntityRepository<Bank> bankRepo,
            IDbMultiEntityMethods dbMultiEntityMethods,
            IDbEntityRepository<DomesticPaymentConsent> domesticConsentRepo,
            IEntityMapper mapper,
            ITimeProvider timeProvider,
            IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient)
        {
            _bankProfileRepo = bankProfileRepo;
            _bankRegistrationRepo = bankRegistrationRepo;
            _bankRepo = bankRepo;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _domesticConsentRepo = domesticConsentRepo;
            _mapper = mapper;
            _timeProvider = timeProvider;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _instrumentationClient = instrumentationClient;
        }

        // Load bank and bankProfile
        public async Task<DomesticPaymentConsentResponse> CreateAsync(
            RequestDomesticPaymentConsent request,
            string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Determine Bank, BankRegistration, BankProfile based on request properties
            Bank bank;
            BankRegistration bankRegistration;
            BankApiInformation bankApiInformation;
            if (request.BankRegistrationId is null || request.BankApiInformationId is null)
            {
                if (request.BankId is null)
                {
                    throw new ArgumentException(
                        "If either BankRegistrationId or BankProfileId is null, BankId must be non-null.");
                }

                Guid bankId = request.BankId.Value;
                bank = await _bankRepo.GetAsync(bankId) ??
                       throw new KeyNotFoundException($"No record found for BankId {bankId} specified by request.");
                Guid bankRegistrationId;
                if (request.BankRegistrationId is null)
                {
                    if (request.UseStagingNotDefaultBankRegistration)
                    {
                        bankRegistrationId = bank.StagingBankRegistrationId.Data ?? throw new ArgumentException(
                            $"Specified BankId {bankId} refers to object with null StagingBankRegistrationId.");
                    }
                    else
                    {
                        bankRegistrationId = bank.DefaultBankRegistrationId.Data ??
                                             throw new ArgumentException(
                                                 $"Specified BankId {bankId} refers to object with null DefaultBankRegistrationId.");
                    }
                }
                else
                {
                    bankRegistrationId = request.BankRegistrationId.Value;
                }

                bankRegistration = await _bankRegistrationRepo.GetAsync(bankRegistrationId) ??
                                   throw new KeyNotFoundException(
                                       $"No record found for BankRegistrationId {bankRegistrationId} specified by request properties.");


                Guid bankProfileId;
                if (request.BankApiInformationId is null)
                {
                    if (request.UseStagingNotDefaultBankProfile)
                    {
                        bankProfileId = bank.StagingBankProfileId.Data ?? throw new ArgumentException(
                            $"Specified BankId {bankId} refers to object with null StagingBankProfileId.");
                    }
                    else
                    {
                        bankProfileId = bank.DefaultBankProfileId.Data ??
                                        throw new ArgumentException(
                                            $"Specified BankId {bankId} refers to object with null DefaultBankProfileId.");
                    }
                }
                else
                {
                    bankProfileId = request.BankApiInformationId.Value;
                }

                bankApiInformation = await _bankProfileRepo.GetAsync(bankProfileId) ??
                                     throw new KeyNotFoundException(
                                         $"No record found for BankProfileId {bankProfileId} specified by request properties.");
            }
            else
            {
                Guid bankRegistrationId = request.BankRegistrationId.Value;
                bankRegistration = await _bankRegistrationRepo.GetAsync(bankRegistrationId) ??
                                   throw new KeyNotFoundException(
                                       $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
                Guid bankProfileId = request.BankApiInformationId.Value;
                bankApiInformation = await _bankProfileRepo.GetAsync(bankProfileId) ??
                                     throw new KeyNotFoundException(
                                         $"No record found for BankProfileId {bankProfileId} specified by request.");
                if (bankApiInformation.BankId != bankRegistration.BankId)
                {
                    throw new ArgumentException(
                        "BankRegistrationId and BankProfileId objects do not share same BankId.");
                }

                Guid bankId = bankApiInformation.BankId;
                bank = await _bankRepo.GetAsync(bankId)
                       ?? throw new KeyNotFoundException(
                           $"No record found for BankId {bankId} specified by BankRegistrationId and BankProfileId objects.");
            }

            // Checks
            PaymentInitiationApi paymentInitiationApi = bankApiInformation.PaymentInitiationApi ??
                                                        throw new NullReferenceException(
                                                            "Bank profile does not support PaymentInitiation API");

            // Load relevant objects
            SoftwareStatementProfileCached softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankRegistration.SoftwareStatementProfileId);

            // Get client credentials grant (we will not cache token for now but simply use to POST consent)
            TokenEndpointResponse tokenEndpointResponse =
                await PostClientCredentialsGrant(
                    scope: "payments",
                    client: bankRegistration,
                    apiClient: softwareStatementProfile.ApiClient,
                    orgId: bank.FinancialId);
            // TODO: validate the response???

            // Create new Open Banking consent by posting JWT
            JwtFactory jwtFactory = new JwtFactory();
            OBWriteDomesticConsentResponse consentResponse;
            OBWriteDomesticConsent requestDomesticConsent = DomesticPaymentConsent.GetOBWriteDomesticConsent(request);
            switch (paymentInitiationApi.ApiVersion)
            {
                case ApiVersion.V3p1p1:
                    OBWriteDomesticConsent2 newDomesticConsent =
                        _mapper.Map<OBWriteDomesticConsent2>(requestDomesticConsent);
                    OBWriteDomesticConsentResponse2 rawConsentResponse = await
                        PostDomesticConsent<OBWriteDomesticConsent2, OBWriteDomesticConsentResponse2>(
                            jwtFactory: jwtFactory,
                            softwareStatementProfile: softwareStatementProfile,
                            consent: newDomesticConsent,
                            paymentInitiationApi: paymentInitiationApi,
                            tokenEndpointResponse: tokenEndpointResponse,
                            orgId: bank.FinancialId,
                            instrumentationClient: _instrumentationClient);
                    consentResponse = _mapper.Map<OBWriteDomesticConsentResponse>(rawConsentResponse);
                    break;
                case ApiVersion.V3p1p2:
                    throw new ArgumentOutOfRangeException();
                case ApiVersion.V3p1p4:
                    consentResponse = await
                        PostDomesticConsent<OBWriteDomesticConsent, OBWriteDomesticConsentResponse>(
                            jwtFactory: jwtFactory,
                            softwareStatementProfile: softwareStatementProfile,
                            consent: requestDomesticConsent,
                            paymentInitiationApi: paymentInitiationApi,
                            tokenEndpointResponse: tokenEndpointResponse,
                            orgId: bank.FinancialId,
                            instrumentationClient: _instrumentationClient);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Create request object
            string consentId = consentResponse.Data.ConsentId;
            string redirectUrl = softwareStatementProfile.DefaultFragmentRedirectUrl;
            if (redirectUrl == "")
            {
                redirectUrl = bankRegistration.OBClientRegistrationRequest.RedirectUris[0];
            }

            OAuth2RequestObjectClaims oAuth2RequestObjectClaims =
                OAuth2RequestObjectClaimsFactory.CreateOAuth2RequestObjectClaims(
                    bankRegistration: bankRegistration,
                    redirectUrl: redirectUrl,
                    scope: new[] { "openid", "payments" },
                    intentId: consentId,
                    issuerUrl: bank.IssuerUrl);
            string requestObjectJwt = jwtFactory.CreateJwt(
                profile: softwareStatementProfile,
                claims: oAuth2RequestObjectClaims,
                useOpenBankingJwtHeaders: false,
                paymentInitiationApiVersion: paymentInitiationApi.ApiVersion);
            StringBuilder requestTraceSb = new StringBuilder()
                .AppendLine("#### JWT (Request Object)")
                .Append(requestObjectJwt);
            _instrumentationClient.Info(requestTraceSb.ToString());

            // Create auth URL
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
            StringBuilder authUrlTraceSb = new StringBuilder()
                .AppendLine("#### Auth URL (Domestic Consent)")
                .Append(authUrl);
            _instrumentationClient.Info(authUrlTraceSb.ToString());

            // Create and store persistent object
            DomesticPaymentConsent persistedDomesticPaymentConsent = new DomesticPaymentConsent(
                timeProvider: _timeProvider,
                state: oAuth2RequestObjectClaims.State,
                bankRegistrationId: bankRegistration.Id,
                bankApiInformationId: bankApiInformation.Id,
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
            ApiClient apiClient,
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
            if (client.OBClientRegistrationRequest.TokenEndpointAuthMethod ==
                TokenEndpointAuthMethodEnum.TlsClientAuth)
            {
                keyValuePairs["client_id"] = client.OBClientRegistration.ClientId;
            }
            else if (client.OBClientRegistrationRequest.TokenEndpointAuthMethod ==
                     TokenEndpointAuthMethodEnum.ClientSecretBasic)
            {
                client.OBClientRegistration.ClientSecret.InvalidOpOnNull("No client secret available.");
                string authString = client.OBClientRegistration.ClientId + ":" +
                                    client.OBClientRegistration.ClientSecret;
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(authString);
                authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                if (client.OBClientRegistrationRequest.TokenEndpointAuthMethod ==
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
                .RequestJsonAsync<TokenEndpointResponse>(client: apiClient, requestContentIsJson: false);
        }

        private async Task<ApiResponseType> PostDomesticConsent<ApiRequestType, ApiResponseType>(
            JwtFactory jwtFactory,
            SoftwareStatementProfileCached softwareStatementProfile,
            ApiRequestType consent,
            PaymentInitiationApi paymentInitiationApi,
            TokenEndpointResponse tokenEndpointResponse,
            string orgId,
            IInstrumentationClient instrumentationClient)
            where ApiRequestType : class
            where ApiResponseType : class
        {
            // Create JWT
            string jwt = jwtFactory.CreateJwt(
                profile: softwareStatementProfile,
                claims: consent,
                useOpenBankingJwtHeaders: true,
                paymentInitiationApiVersion: paymentInitiationApi.ApiVersion);
            StringBuilder requestTraceSb = new StringBuilder()
                .AppendLine("#### JWT (Domestic Consent)")
                .Append(jwt);
            instrumentationClient.Info(requestTraceSb.ToString());

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
                .RequestJsonAsync<ApiResponseType>(
                    client: softwareStatementProfile.ApiClient,
                    requestContentIsJson: true);
        }
    }
}
