// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using OBWriteDomestic2 =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBWriteDomestic2;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateDomesticPayment
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<ApiProfile> _apiProfileRepo;
        private readonly IDbEntityRepository<DomesticConsent> _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly IDbEntityRepository<BankClientProfile> _openBankingClientRepo;
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;

        public CreateDomesticPayment(
            IApiClient apiClient,
            IEntityMapper mapper,
            IDbEntityRepository<BankClientProfile> openBankingClientRepo,
            IDbEntityRepository<DomesticConsent> domesticConsentRepo,
            IDbEntityRepository<ApiProfile> apiProfileRepo,
            ISoftwareStatementProfileService softwareStatementProfileService)
        {
            _apiClient = apiClient;
            _mapper = mapper;
            _openBankingClientRepo = openBankingClientRepo;
            _domesticConsentRepo = domesticConsentRepo;
            _apiProfileRepo = apiProfileRepo;
            _softwareStatementProfileService = softwareStatementProfileService;
        }

        public async Task<OBWriteDomesticResponse4> CreateAsync(string consentId)
        {
            // Load relevant data objects
            DomesticConsent consent = await _domesticConsentRepo.GetAsync(consentId)
                                      ?? throw new KeyNotFoundException("The Consent does not exist.");
            ApiProfile apiProfile = await _apiProfileRepo.GetAsync(consent.ApiProfileId)
                                    ?? throw new KeyNotFoundException("The API Profile does not exist.");
            BankClientProfile bankClientProfile = await _openBankingClientRepo.GetAsync(apiProfile.BankClientProfileId)
                                                  ?? throw new KeyNotFoundException(
                                                      "The Bank Client Profile does not exist.");
            SoftwareStatementProfile softwareStatementProfile =
                _softwareStatementProfileService.GetSoftwareStatementProfile(
                    bankClientProfile.SoftwareStatementProfileId);

            TokenEndpointResponse tokenEndpointResponse =
                _mapper.Map<TokenEndpointResponse>(consent.TokenEndpointResponse);

            // Create new Open Banking payment by posting JWT
            OBWriteDomesticConsent4 obConsent = consent.ObWriteDomesticConsent;
            OBWriteDomestic2 referencePayment = new OBWriteDomestic2
            {
                Data = new OBWriteDomestic2Data
                {
                    ConsentId = consent.BankId,
                    Initiation = obConsent.Data.Initiation
                },
                Risk = obConsent.Risk
            };

            // Create new Open Banking payment by posting JWT
            OBWriteDomesticResponse4 paymentResponse;
            switch (apiProfile.ApiVersion)
            {
                case ApiVersion.V3P1P1:
                    ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomestic2 newPayment =
                        _mapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomestic2>(referencePayment);
                    OBWriteDomesticResponse2 rawPaymentResponse = await
                        PostDomesticPayment<ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomestic2,
                            OBWriteDomesticResponse2>(
                            payment: newPayment,
                            apiProfile: apiProfile,
                            softwareStatementProfile: softwareStatementProfile,
                            bankClientProfile: bankClientProfile,
                            tokenEndpointResponse: tokenEndpointResponse);
                    paymentResponse = _mapper.Map<OBWriteDomesticResponse4>(rawPaymentResponse);
                    break;
                case ApiVersion.V3P1P2:
                    throw new ArgumentOutOfRangeException();
                case ApiVersion.V3P1P4:
                    paymentResponse = await PostDomesticPayment<OBWriteDomestic2, OBWriteDomesticResponse4>(
                        payment: referencePayment,
                        apiProfile: apiProfile,
                        softwareStatementProfile: softwareStatementProfile,
                        bankClientProfile: bankClientProfile,
                        tokenEndpointResponse: tokenEndpointResponse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return paymentResponse;
        }

        private async Task<ApiResponseType> PostDomesticPayment<ApiRequestType, ApiResponseType>(
            ApiRequestType payment,
            ApiProfile apiProfile,
            SoftwareStatementProfile softwareStatementProfile,
            BankClientProfile bankClientProfile,
            TokenEndpointResponse tokenEndpointResponse)
            where ApiRequestType : class
            where ApiResponseType : class
        {
            string payloadJson = JsonConvert.SerializeObject(payment);
            UriBuilder ub = new UriBuilder(new Uri(apiProfile.BaseUrl + "/domestic-payments"));

            List<HttpHeader> headers = CreateRequestHeaders(
                softwareStatement: softwareStatementProfile,
                payment: payment,
                client: bankClientProfile,
                tokenEndpointResponse: tokenEndpointResponse);

            return await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/json")
                .SetContent(payloadJson)
                .Create()
                .RequestJsonAsync<ApiResponseType>(client: _apiClient, requestContentIsJson: true);
        }

        private static List<HttpHeader> CreateRequestHeaders<ApiRequestType>(
            SoftwareStatementProfile softwareStatement,
            ApiRequestType payment,
            BankClientProfile client,
            TokenEndpointResponse tokenEndpointResponse)
            where ApiRequestType : class
        {
            JwtFactory jwtFactory = new JwtFactory();
            string jwt = jwtFactory.CreateJwt(
                profile: softwareStatement,
                claims: payment,
                useOpenBankingJwtHeaders: true);
            string[] jwsComponents = jwt.Split('.');
            string jwsSig = $"{jwsComponents[0]}..{jwsComponents[2]}";
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader(name: "x-fapi-financial-id", value: client.XFapiFinancialId),
                new HttpHeader(name: "Authorization", value: "Bearer " + tokenEndpointResponse.AccessToken),
                new HttpHeader(name: "x-idempotency-key", value: Guid.NewGuid().ToString()),
                new HttpHeader(name: "x-jws-signature", value: jwsSig)
            };
            return headers;
        }
    }
}
