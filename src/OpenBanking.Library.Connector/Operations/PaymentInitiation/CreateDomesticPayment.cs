// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModel.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;
using TokenEndpointResponse = FinnovationLabs.OpenBanking.Library.Connector.Model.Public.TokenEndpointResponse;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateDomesticPayment
    {
        private readonly IApiClient _apiClient;
        private readonly IDomesticConsentRepository _domesticConsentRepo;
        private readonly IEntityMapper _mapper;
        private readonly IOpenBankingClientProfileRepository _openBankingClientProfileRepo;
        private readonly IOpenBankingClientRepository _openBankingClientRepo;
        private readonly ISoftwareStatementProfileRepository _softwareStatementRepo;

        public CreateDomesticPayment(IApiClient apiClient, IEntityMapper mapper,
            ISoftwareStatementProfileRepository softwareStatementRepo,
            IOpenBankingClientProfileRepository openBankingClientProfileRepo,
            IOpenBankingClientRepository openBankingClientRepo, IDomesticConsentRepository domesticConsentRepo)
        {
            _domesticConsentRepo = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
            _openBankingClientProfileRepo =
                openBankingClientProfileRepo.ArgNotNull(nameof(openBankingClientProfileRepo));
            _openBankingClientRepo = openBankingClientRepo.ArgNotNull(nameof(openBankingClientRepo));
            _mapper = mapper.ArgNotNull(nameof(mapper));
            _apiClient = apiClient.ArgNotNull(nameof(apiClient));
            _softwareStatementRepo = softwareStatementRepo.ArgNotNull(nameof(softwareStatementRepo));
        }

        public async Task<OBWriteDomesticResponse> CreateAsync(string consentId)
        {
            consentId.ArgNotNull(nameof(consentId));

            // Load relevant objects
            var consent = await _domesticConsentRepo.GetAsync(consentId) ??
                          throw new KeyNotFoundException("The Consent does not exist.");
            var obClientProfile = await _openBankingClientProfileRepo.GetAsync(consent.OpenBankingClientProfileId) ??
                                  throw new KeyNotFoundException("The OB Client Profile does not exist.");
            var client = await _openBankingClientRepo.GetAsync(obClientProfile.OpenBankingClientId) ??
                         throw new KeyNotFoundException("The OB Client Profile does not exist.");
            var softwareStatement = await _softwareStatementRepo.GetAsync(client.SoftwareStatementProfileId) ??
                                    throw new KeyNotFoundException("The Software statement does not exist.");

            // TODO: update token endpiont response generation
            var tokenEndpointResponse = new TokenEndpointResponse();

            // Create new Open Banking payment by posting JWT
            var payment = _mapper.Map<OBWriteDomestic2>(consent);
            payment.Data.ConsentId = consentId;
            var payloadJson = JsonConvert.SerializeObject(payment);
            var ub = new UriBuilder(new Uri(obClientProfile.PaymentInitiationApiBaseUrl + "/domestic-payments"));

            var headers = CreateRequestHeaders(softwareStatement, payment, client, tokenEndpointResponse);

            var paymentResponse = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Post)
                .SetUri(ub.Uri)
                .SetHeaders(headers)
                .SetContentType("application/json")
                .SetContent(payloadJson)
                .Create()
                .RequestJsonAsync<OBWriteDomesticResponse2>(_apiClient);

            var resp = _mapper.Map<OBWriteDomesticResponse>(paymentResponse);

            return resp;
        }

        private static List<HttpHeader> CreateRequestHeaders(SoftwareStatementProfile softwareStatement,
            OBWriteDomestic2 payment,
            OpenBankingClient client, TokenEndpointResponse tokenEndpointResponse)
        {
            var jwtFactory = new JwtFactory();
            var jwt = jwtFactory.CreateJwt(softwareStatement, payment, true);
            var jwsComponents = jwt.Split('.');
            var jwsSig = $"{jwsComponents[0]}..{jwsComponents[2]}";
            var headers = new List<HttpHeader>
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
