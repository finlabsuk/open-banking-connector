// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access;
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
using Newtonsoft.Json;
using OBWriteDomestic =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBWriteDomestic2;
using OBWriteDomestic2 =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomestic2;
using RequestDomesticPayment =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class CreateDomesticPayment
    {
        private readonly IDbReadOnlyEntityRepository<BankApiInformation> _bankProfileRepo;
        private readonly IDbReadOnlyEntityRepository<BankRegistration> _bankRegistrationRepo;
        private readonly IDbReadOnlyEntityRepository<Bank> _bankRepo;
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly IDbReadOnlyEntityRepository<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly IDbEntityRepository<DomesticPayment> _domesticPaymentRepo;
        private readonly IEntityMapper _mapper;
        private readonly IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public CreateDomesticPayment(
            IDbReadOnlyEntityRepository<Bank> bankRepo,
            IDbReadOnlyEntityRepository<BankApiInformation> bankProfileRepo,
            IDbReadOnlyEntityRepository<DomesticPaymentConsent> domesticConsentRepo,
            IEntityMapper mapper,
            IDbReadOnlyEntityRepository<BankRegistration> bankRegistrationRepo,
            IDbEntityRepository<DomesticPayment> domesticPaymentRepo,
            ITimeProvider timeProvider,
            IDbMultiEntityMethods dbMultiEntityMethods,
            IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> softwareStatementProfileRepo)
        {
            _bankRepo = bankRepo;
            _bankProfileRepo = bankProfileRepo;
            _domesticConsentRepo = domesticConsentRepo;
            _mapper = mapper;
            _bankRegistrationRepo = bankRegistrationRepo;
            _domesticPaymentRepo = domesticPaymentRepo;
            _timeProvider = timeProvider;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
        }

        public async Task<DomesticPaymentResponse> CreateAsync(
            RequestDomesticPayment request,
            string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Load relevant data objects
            Guid domesticPaymentConsentId = request.ConsentId;
            DomesticPaymentConsent paymentConsent =
                await _domesticConsentRepo.GetAsync(
                    id: domesticPaymentConsentId,
                    detachFirst: true) // ensure detached to clear cache as may have been updated in another DB context
                ?? throw new KeyNotFoundException("The Consent does not exist.");
            BankApiInformation bankApiInformation = await _bankProfileRepo.GetAsync(paymentConsent.BankApiInformationId)
                                                    ?? throw new KeyNotFoundException(
                                                        "The API Profile does not exist.");

            // Checks
            PaymentInitiationApi paymentInitiationApi = bankApiInformation.PaymentInitiationApi ??
                                                        throw new NullReferenceException(
                                                            "Bank profile does not support PaymentInitiation API");

            // Load relevant data objects
            BankRegistration bankRegistration = await _bankRegistrationRepo.GetAsync(paymentConsent.BankRegistrationId)
                                                ?? throw new KeyNotFoundException(
                                                    "The Bank Client Profile does not exist.");
            Bank bank = await _bankRepo.GetAsync(bankApiInformation.BankId)
                        ?? throw new KeyNotFoundException("No record found for BankId in BankProfile.");
            SoftwareStatementProfileCached softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankRegistration.SoftwareStatementProfileId);

            TokenEndpointResponse tokenEndpointResponse =
                _mapper.Map<TokenEndpointResponse>(paymentConsent.TokenEndpointResponse.Data);

            // Create new Open Banking payment by posting JWT
            OBWriteDomesticConsent4 obConsent = paymentConsent.OBWriteDomesticConsent;
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
                case ApiVersion.V3p1p1:
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
                            orgId: bank.FinancialId);
                    paymentResponse = _mapper.Map<OBWriteDomesticResponse4>(rawPaymentResponse);
                    break;
                case ApiVersion.V3p1p2:
                    throw new ArgumentOutOfRangeException();
                case ApiVersion.V3p1p4:
                    paymentResponse = await PostDomesticPayment<OBWriteDomestic, OBWriteDomesticResponse4>(
                        payment: referencePayment,
                        paymentInitiationApi: paymentInitiationApi,
                        softwareStatementProfile: softwareStatementProfile,
                        bankClientProfile: bankRegistration,
                        tokenEndpointResponse: tokenEndpointResponse,
                        orgId: bank.FinancialId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Create and store persistent object
            DomesticPayment persistedDomesticPayment = new DomesticPayment(
                timeProvider: _timeProvider,
                domesticPaymentConsentId: domesticPaymentConsentId,
                obWriteDomesticResponse: paymentResponse,
                createdBy: createdBy);
            await _domesticPaymentRepo.AddAsync(persistedDomesticPayment);
            await _dbMultiEntityMethods.SaveChangesAsync();

            DomesticPaymentResponse response = persistedDomesticPayment.PublicResponse;

            return response;
        }

        private async Task<ApiResponseType> PostDomesticPayment<ApiRequestType, ApiResponseType>(
            ApiRequestType payment,
            PaymentInitiationApi paymentInitiationApi,
            SoftwareStatementProfileCached softwareStatementProfile,
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
                .RequestJsonAsync<ApiResponseType>(
                    client: softwareStatementProfile.ApiClient,
                    requestContentIsJson: true);
        }

        private static List<HttpHeader> CreateRequestHeaders<ApiRequestType>(
            SoftwareStatementProfileCached softwareStatement,
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
