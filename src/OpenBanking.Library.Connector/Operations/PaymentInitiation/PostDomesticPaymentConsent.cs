// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p4.PaymentInitiation.Models;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;
using RequestDomesticPaymentConsent =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class PostDomesticPaymentConsent
    {
        private readonly IDbReadOnlyEntityMethods<BankApiInformation> _bankProfileRepo;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationRepo;
        private readonly IDbReadOnlyEntityMethods<Bank> _bankRepo;
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly IDbReadWriteEntityMethods<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IApiVariantMapper _mapper;
        private readonly IReadOnlyRepository<SoftwareStatementProfileCached> _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public PostDomesticPaymentConsent(
            IDbReadOnlyEntityMethods<BankApiInformation> bankProfileRepo,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationRepo,
            IDbReadOnlyEntityMethods<Bank> bankRepo,
            IDbSaveChangesMethod dbSaveChangesMethod,
            IDbReadWriteEntityMethods<DomesticPaymentConsent> domesticConsentRepo,
            IApiVariantMapper mapper,
            ITimeProvider timeProvider,
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient)
        {
            _bankProfileRepo = bankProfileRepo;
            _bankRegistrationRepo = bankRegistrationRepo;
            _bankRepo = bankRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _domesticConsentRepo = domesticConsentRepo;
            _mapper = mapper;
            _timeProvider = timeProvider;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _instrumentationClient = instrumentationClient;
        }

        // Load bank and bankProfile
        public async Task<(DomesticPaymentConsentPostResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostAsync(
                RequestDomesticPaymentConsent request,
                string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Determine Bank, BankRegistration, BankProfile based on request properties
            Guid bankRegistrationId = request.BankRegistrationId;
            BankRegistration bankRegistration = await _bankRegistrationRepo.GetNoTrackingAsync(bankRegistrationId) ??
                                                throw new KeyNotFoundException(
                                                    $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
            Guid bankProfileId = request.BankApiInformationId;
            BankApiInformation bankApiInformation = await _bankProfileRepo.GetNoTrackingAsync(bankProfileId) ??
                                                    throw new KeyNotFoundException(
                                                        $"No record found for BankProfileId {bankProfileId} specified by request.");
            if (bankApiInformation.BankId != bankRegistration.BankId)
            {
                throw new ArgumentException("BankRegistrationId and BankProfileId objects do not share same BankId.");
            }

            Guid bankId = bankApiInformation.BankId;
            Bank bank = await _bankRepo.GetNoTrackingAsync(bankId)
                        ?? throw new KeyNotFoundException(
                            $"No record found for BankId {bankId} specified by BankRegistrationId and BankProfileId objects.");

            // Checks
            PaymentInitiationApi paymentInitiationApi = bankApiInformation.PaymentInitiationApi ??
                                                        throw new NullReferenceException(
                                                            "Bank profile does not support PaymentInitiation API");

            // Load relevant objects
            SoftwareStatementProfileCached softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankRegistration.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfileId {bankRegistration.SoftwareStatementProfileId}");

            // Get client credentials grant (we will not cache token for now but simply use to POST consent)
            JsonSerializerSettings? jsonSerializerSettings1 = null;
            TokenEndpointResponse tokenEndpointResponse =
                await PostTokenRequest.PostClientCredentialsGrantAsync(
                    "payments",
                    bankRegistration,
                    jsonSerializerSettings1,
                    softwareStatementProfile.ApiClient);

            // Get request
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4 domesticConsentRequest = request.WriteDomesticConsent;

            // Create new Open Banking consent by posting JWT
            JwtFactory jwtFactory = new JwtFactory();
            var postApiObject
                = new PostRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>();
            Uri uri =
                new Uri(paymentInitiationApi.BaseUrl + "/domestic-payment-consents");
            JsonSerializerSettings? jsonSerializerSettings2 = null;
            var (domesticConsentResponse, newNonErrorMessages) =
                paymentInitiationApi.PaymentInitiationApiVersion switch
                {
                    PaymentInitiationApiVersion.Version3p1p4 => await postApiObject
                        .PostAsync<PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4,
                            PaymentInitiationModelsV3p1p4.OBWriteDomesticConsentResponse4>(
                            uri,
                            domesticConsentRequest,
                            new PaymentInitiationRequestProcessor<
                                PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4>(
                                bank.FinancialId,
                                tokenEndpointResponse,
                                softwareStatementProfile,
                                paymentInitiationApi,
                                jwtFactory,
                                _instrumentationClient),
                            jsonSerializerSettings2,
                            softwareStatementProfile.ApiClient,
                            _mapper),
                    PaymentInitiationApiVersion.Version3p1p6 => await postApiObject
                        .PostAsync<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                            uri,
                            domesticConsentRequest,
                            new PaymentInitiationRequestProcessor<
                                PaymentInitiationModelsPublic.OBWriteDomesticConsent4>(
                                bank.FinancialId,
                                tokenEndpointResponse,
                                softwareStatementProfile,
                                paymentInitiationApi,
                                jwtFactory,
                                _instrumentationClient),
                            jsonSerializerSettings2,
                            softwareStatementProfile.ApiClient,
                            _mapper),
                    _ => throw new ArgumentOutOfRangeException(
                        $"PaymentInitiationApi version {paymentInitiationApi.PaymentInitiationApiVersion}")
                };
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create auth URL
            var state = Guid.NewGuid().ToString();
            string authUrl = CreateAuthUrl.Create(
                domesticConsentResponse.Data.ConsentId,
                softwareStatementProfile,
                bankRegistration,
                bank.IssuerUrl,
                state,
                jwtFactory,
                _instrumentationClient);

            // Create and store persistent object
            DomesticPaymentConsent persistedDomesticPaymentConsent = new DomesticPaymentConsent(
                state,
                bankRegistration.Id,
                bankApiInformation.Id,
                domesticConsentRequest,
                domesticConsentResponse,
                Guid.NewGuid(), 
                "name",
                createdBy,
                _timeProvider);
            await _domesticConsentRepo.AddAsync(persistedDomesticPaymentConsent);
            await _dbSaveChangesMethod.SaveChangesAsync();

            DomesticPaymentConsentPostResponse response = persistedDomesticPaymentConsent.PublicPostResponse(authUrl);

            return (response, nonErrorMessages);
        }
    }
}
