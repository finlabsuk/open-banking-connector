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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;
using TokenEndpointResponsePublic = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.TokenEndpointResponse;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateDomesticPayment
    {
        private readonly IApiClient _apiClient;
        private readonly IDbEntityRepository<ApiProfile> _apiProfileRepo;
        private readonly IDbEntityRepository<DomesticConsent> _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly IDbEntityRepository<BankClientProfile> _openBankingClientRepo;
        private readonly IDbEntityRepository<SoftwareStatementProfile> _softwareStatementProfileRepo;

        public CreateDomesticPayment(IApiClient apiClient, IEntityMapper mapper,
            IDbEntityRepository<SoftwareStatementProfile> softwareStatementRepo,
            IDbEntityRepository<BankClientProfile> openBankingClientRepo,
            IDbEntityRepository<DomesticConsent> domesticConsentRepo, IDbEntityRepository<ApiProfile> apiProfileRepo)
        {
            _apiClient = apiClient;
            _mapper = mapper;
            _softwareStatementProfileRepo = softwareStatementRepo;
            _openBankingClientRepo = openBankingClientRepo;
            _domesticConsentRepo = domesticConsentRepo;
            _apiProfileRepo = apiProfileRepo;
        }

        public async Task<OBWriteDomesticResponse> CreateAsync(string consentId)
        {
            // Load relevant objects
            DomesticConsent consent = await _domesticConsentRepo.GetAsync(consentId)
                                      ?? throw new KeyNotFoundException("The Consent does not exist.");
            ApiProfile apiProfile = await _apiProfileRepo.GetAsync(consent.ApiProfileId)
                                    ?? throw new KeyNotFoundException("The API Profile does not exist.");
            BankClientProfile bankClientProfile = await _openBankingClientRepo.GetAsync(apiProfile.BankClientProfileId)
                                                  ?? throw new KeyNotFoundException(
                                                      "The Bank Client Profile does not exist.");
            SoftwareStatementProfile softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankClientProfile.SoftwareStatementProfileId)
                ?? throw new KeyNotFoundException("The Software Statement Profile does not exist.");

            // TODO: update token endpiont response generation
            TokenEndpointResponsePublic tokenEndpointResponse = new TokenEndpointResponsePublic();

            // Create new Open Banking payment by posting JWT
            OBWriteDomestic2 payment = _mapper.Map<OBWriteDomestic2>(consent);
            payment.Data.ConsentId = consentId;
            string payloadJson = JsonConvert.SerializeObject(payment);
            UriBuilder ub = new UriBuilder(new Uri(apiProfile.BaseUrl + "/domestic-payments"));

            List<HttpHeader> headers = CreateRequestHeaders(softwareStatementProfile, payment, bankClientProfile,
                tokenEndpointResponse);

            OBWriteDomesticResponse2 paymentResponse = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/json")
                .SetContent(payloadJson)
                .Create()
                .RequestJsonAsync<OBWriteDomesticResponse2>(_apiClient, true);

            OBWriteDomesticResponse resp = _mapper.Map<OBWriteDomesticResponse>(paymentResponse);

            return resp;
        }

        private static List<HttpHeader> CreateRequestHeaders(SoftwareStatementProfile softwareStatement,
            OBWriteDomestic2 payment,
            BankClientProfile client, TokenEndpointResponsePublic tokenEndpointResponse)
        {
            JwtFactory jwtFactory = new JwtFactory();
            string jwt = jwtFactory.CreateJwt(softwareStatement, payment, true);
            string[] jwsComponents = jwt.Split('.');
            string jwsSig = $"{jwsComponents[0]}..{jwsComponents[2]}";
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader("x-fapi-financial-id", client.XFapiFinancialId),
                new HttpHeader("Authorization", "Bearer " + tokenEndpointResponse.AccessToken),
                new HttpHeader("x-idempotency-key", Guid.NewGuid().ToString()),
                new HttpHeader("x-jws-signature", jwsSig)
            };
            return headers;
        }
    }
}
