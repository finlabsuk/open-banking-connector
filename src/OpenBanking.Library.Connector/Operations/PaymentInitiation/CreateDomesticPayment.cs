// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using OBWriteDomestic =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBWriteDomestic2;
using OBWriteDomestic2 =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomestic2;
using RequestDomesticPayment =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateDomesticPayment
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<BankProfile> _bankProfileRepo;
        private readonly IDbEntityRepository<BankRegistration> _bankRegistrationRepo;
        private readonly IDbEntityRepository<Bank> _bankRepo;
        private readonly IDbEntityRepository<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;

        public CreateDomesticPayment(
            IApiClient apiClient,
            IDbEntityRepository<Bank> bankRepo,
            IDbEntityRepository<BankProfile> bankProfileRepo,
            IDbEntityRepository<DomesticPaymentConsent> domesticConsentRepo,
            IEntityMapper mapper,
            IDbEntityRepository<BankRegistration> bankRegistrationRepo,
            ISoftwareStatementProfileService softwareStatementProfileService)
        {
            _apiClient = apiClient;
            _bankRepo = bankRepo;
            _bankProfileRepo = bankProfileRepo;
            _domesticConsentRepo = domesticConsentRepo;
            _mapper = mapper;
            _bankRegistrationRepo = bankRegistrationRepo;
            _softwareStatementProfileService = softwareStatementProfileService;
        }

        public async Task<DomesticPaymentResponse> CreateAsync(RequestDomesticPayment requestDomesticPayment)
        {
            requestDomesticPayment.ArgNotNull(nameof(requestDomesticPayment));

            // Load relevant data objects
            DomesticPaymentConsent paymentConsent =
                await _domesticConsentRepo.GetAsync(requestDomesticPayment.ConsentId)
                ?? throw new KeyNotFoundException("The Consent does not exist.");
            BankProfile bankProfile = await _bankProfileRepo.GetAsync(paymentConsent.BankProfileId)
                                      ?? throw new KeyNotFoundException("The API Profile does not exist.");

            // Checks
            PaymentInitiationApi paymentInitiationApi = bankProfile.PaymentInitiationApi ??
                                                        throw new NullReferenceException(
                                                            "Bank profile does not support PaymentInitiation API");

            // Load relevant data objects
            BankRegistration bankRegistration = await _bankRegistrationRepo.GetAsync(bankProfile.BankRegistrationId)
                                                ?? throw new KeyNotFoundException(
                                                    "The Bank Client Profile does not exist.");
            Bank bank = await _bankRepo.GetAsync(bankProfile.BankId)
                        ?? throw new KeyNotFoundException("No record found for BankId in BankProfile.");
            SoftwareStatementProfile softwareStatementProfile =
                _softwareStatementProfileService.GetSoftwareStatementProfile(
                    bankRegistration.SoftwareStatementProfileId);


            TokenEndpointResponse tokenEndpointResponse =
                _mapper.Map<TokenEndpointResponse>(paymentConsent.TokenEndpointResponse);

            // Create new Open Banking payment by posting JWT
            OBWriteDomesticConsent4 obConsent = paymentConsent.ObWriteDomesticConsent;
            OBWriteDomestic referencePayment = new OBWriteDomestic
            {
                Data = new OBWriteDomestic2Data
                {
                    ConsentId = paymentConsent.OBId,
                    Initiation = obConsent.Data.Initiation
                },
                Risk = obConsent.Risk
            };

            // Create new Open Banking payment by posting JWT
            OBWriteDomesticResponse4 paymentResponse;
            switch (paymentInitiationApi.ApiVersion)
            {
                case ApiVersion.V3P1P1:
                    OBWriteDomestic2 newPayment =
                        _mapper.Map<OBWriteDomestic2>(referencePayment);
                    OBWriteDomesticResponse2 rawPaymentResponse = await
                        PostDomesticPayment<OBWriteDomestic2,
                            OBWriteDomesticResponse2>(
                            payment: newPayment,
                            paymentInitiationApi: paymentInitiationApi,
                            softwareStatementProfile: softwareStatementProfile,
                            bankClientProfile: bankRegistration,
                            tokenEndpointResponse: tokenEndpointResponse,
                            orgId: bank.XFapiFinancialId);
                    paymentResponse = _mapper.Map<OBWriteDomesticResponse4>(rawPaymentResponse);
                    break;
                case ApiVersion.V3P1P2:
                    throw new ArgumentOutOfRangeException();
                case ApiVersion.V3P1P4:
                    paymentResponse = await PostDomesticPayment<OBWriteDomestic, OBWriteDomesticResponse4>(
                        payment: referencePayment,
                        paymentInitiationApi: paymentInitiationApi,
                        softwareStatementProfile: softwareStatementProfile,
                        bankClientProfile: bankRegistration,
                        tokenEndpointResponse: tokenEndpointResponse,
                        orgId: bank.XFapiFinancialId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DomesticPaymentResponse(paymentResponse);
        }

        private async Task<ApiResponseType> PostDomesticPayment<ApiRequestType, ApiResponseType>(
            ApiRequestType payment,
            PaymentInitiationApi paymentInitiationApi,
            SoftwareStatementProfile softwareStatementProfile,
            BankRegistration bankClientProfile,
            TokenEndpointResponse tokenEndpointResponse,
            string orgId)
            where ApiRequestType : class
            where ApiResponseType : class
        {
            string payloadJson = JsonConvert.SerializeObject(payment);
            UriBuilder ub = new UriBuilder(new Uri(paymentInitiationApi.BaseUrl + "/domestic-payments"));

            List<HttpHeader> headers = CreateRequestHeaders(
                softwareStatement: softwareStatementProfile,
                payment: payment,
                client: bankClientProfile,
                paymentInitiationApi: paymentInitiationApi,
                tokenEndpointResponse: tokenEndpointResponse,
                orgId: orgId);

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
            BankRegistration client,
            PaymentInitiationApi paymentInitiationApi,
            TokenEndpointResponse tokenEndpointResponse,
            string orgId)
            where ApiRequestType : class
        {
            JwtFactory jwtFactory = new JwtFactory();
            string jwt = jwtFactory.CreateJwt(
                profile: softwareStatement,
                claims: payment,
                useOpenBankingJwtHeaders: true,
                paymentInitiationApiVersion: paymentInitiationApi.ApiVersion);
            string[] jwsComponents = jwt.Split('.');
            string jwsSig = $"{jwsComponents[0]}..{jwsComponents[2]}";
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader(name: "x-fapi-financial-id", value: orgId),
                new HttpHeader(name: "Authorization", value: "Bearer " + tokenEndpointResponse.AccessToken),
                new HttpHeader(name: "x-idempotency-key", value: Guid.NewGuid().ToString()),
                new HttpHeader(name: "x-jws-signature", value: jwsSig)
            };
            return headers;
        }
    }
}
