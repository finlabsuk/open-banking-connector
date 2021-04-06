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
using RequestDomesticPayment =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class PostDomesticPayment
    {
        private readonly IDbReadOnlyEntityMethods<BankApiInformation> _bankApiInformationRepo;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationRepo;
        private readonly IDbReadOnlyEntityMethods<Bank> _bankRepo;
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly IDbReadOnlyEntityMethods<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly IDbReadWriteEntityMethods<DomesticPayment> _domesticPaymentRepo;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IApiVariantMapper _mapper;
        private readonly IReadOnlyRepository<SoftwareStatementProfileCached> _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public PostDomesticPayment(
            IDbReadOnlyEntityMethods<Bank> bankRepo,
            IDbReadOnlyEntityMethods<BankApiInformation> bankApiInformationRepo,
            IDbReadOnlyEntityMethods<DomesticPaymentConsent> domesticConsentRepo,
            IApiVariantMapper mapper,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationRepo,
            IDbReadWriteEntityMethods<DomesticPayment> domesticPaymentRepo,
            ITimeProvider timeProvider,
            IDbSaveChangesMethod dbSaveChangesMethod,
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient)
        {
            _bankRepo = bankRepo;
            _bankApiInformationRepo = bankApiInformationRepo;
            _domesticConsentRepo = domesticConsentRepo;
            _mapper = mapper;
            _bankRegistrationRepo = bankRegistrationRepo;
            _domesticPaymentRepo = domesticPaymentRepo;
            _timeProvider = timeProvider;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _instrumentationClient = instrumentationClient;
        }

        public async
            Task<(DomesticPaymentPostResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            PostAsync(
                RequestDomesticPayment request,
                string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data objects
            Guid domesticPaymentConsentId = request.DomesticPaymentConsentId;
            DomesticPaymentConsent domesticPaymentConsent =
                await _domesticConsentRepo.GetNoTrackingAsync(domesticPaymentConsentId)
                ?? throw new KeyNotFoundException(
                    $"No record found for Domestic Payment Consent with ID {domesticPaymentConsentId}.");
            BankApiInformation bankApiInformation =
                await _bankApiInformationRepo.GetNoTrackingAsync(domesticPaymentConsent.BankApiInformationId)
                ?? throw new KeyNotFoundException(
                    $"No record found for Bank API Information with ID {domesticPaymentConsent.BankApiInformationId}.");
            BankRegistration bankRegistration =
                await _bankRegistrationRepo.GetNoTrackingAsync(domesticPaymentConsent.BankRegistrationId)
                ?? throw new KeyNotFoundException(
                    $"No record found for Bank Registration with ID {domesticPaymentConsent.BankRegistrationId}.");

            // Checks
            PaymentInitiationApi paymentInitiationApi = bankApiInformation.PaymentInitiationApi ??
                                                        throw new NullReferenceException(
                                                            "Bank API Information record does not specify Payment Initiation API.");
            if (bankRegistration.BankId != bankApiInformation.BankId)
            {
                throw new ArgumentException(
                    "Bank Registration and Bank API Information records are not associated with the same Bank");
            }

            Guid bankId = bankApiInformation.BankId;

            // Load relevant data objects
            Bank bank = await _bankRepo.GetNoTrackingAsync(bankId)
                        ?? throw new KeyNotFoundException($"No record found for Bank with ID {bankId}.");
            SoftwareStatementProfileCached softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankRegistration.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfile with ID {bankRegistration.SoftwareStatementProfileId}.");
            TokenEndpointResponse tokenEndpointResponse = domesticPaymentConsent.TokenEndpointResponse.Data ??
                                                          throw new InvalidOperationException(
                                                              "No token is available for Domestic Payment Consent.");

            // Create payment request from consent request
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4
                consentRequest = domesticPaymentConsent.OBWriteDomesticConsent;
            _mapper.Map(
                consentRequest.Data.Initiation,
                out PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiation initiation);
            PaymentInitiationModelsPublic.OBWriteDomestic2 domesticRequest =
                new PaymentInitiationModelsPublic.OBWriteDomestic2
                {
                    Data = new PaymentInitiationModelsPublic.OBWriteDomestic2Data
                    {
                        ConsentId = domesticPaymentConsent.OBId,
                        Initiation = initiation
                    },
                    Risk = consentRequest.Risk
                };

            // Create new Open Banking payment by posting JWT
            JwtFactory jwtFactory = new JwtFactory();
            var postApiObject =
                new PostRequests<PaymentInitiationModelsPublic.OBWriteDomestic2,
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5>();
            Uri uri = new Uri(paymentInitiationApi.BaseUrl + new DomesticPayment().GetPath);
            JsonSerializerSettings? jsonSerializerSettings = null;
            var (domesticResponse, newNonErrorMessages) =
                paymentInitiationApi.PaymentInitiationApiVersion switch
                {
                    PaymentInitiationApiVersion.Version3p1p4 => await postApiObject
                        .PostAsync<PaymentInitiationModelsV3p1p4.OBWriteDomestic2,
                            PaymentInitiationModelsV3p1p4.OBWriteDomesticResponse4>(
                            uri,
                            domesticRequest,
                            new PaymentInitiationRequestProcessor<PaymentInitiationModelsV3p1p4.OBWriteDomestic2>(
                                bank.FinancialId,
                                tokenEndpointResponse,
                                softwareStatementProfile,
                                paymentInitiationApi,
                                jwtFactory,
                                _instrumentationClient),
                            jsonSerializerSettings,
                            softwareStatementProfile.ApiClient,
                            _mapper),
                    PaymentInitiationApiVersion.Version3p1p6 => await postApiObject
                        .PostAsync<PaymentInitiationModelsPublic.OBWriteDomestic2,
                            PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(
                            uri,
                            domesticRequest,
                            new PaymentInitiationRequestProcessor<PaymentInitiationModelsPublic.OBWriteDomestic2>(
                                bank.FinancialId,
                                tokenEndpointResponse,
                                softwareStatementProfile,
                                paymentInitiationApi,
                                jwtFactory,
                                _instrumentationClient),
                            jsonSerializerSettings,
                            softwareStatementProfile.ApiClient,
                            _mapper),
                    _ => throw new ArgumentOutOfRangeException(
                        $"PaymentInitiationApi version {paymentInitiationApi.PaymentInitiationApiVersion}")
                };
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create and store persistent object
            DomesticPayment persistedDomesticPayment = new DomesticPayment(
                domesticPaymentConsentId,
                domesticResponse,
                Guid.NewGuid(),
                "name",
                createdBy,
                _timeProvider);
            await _domesticPaymentRepo.AddAsync(persistedDomesticPayment);
            await _dbSaveChangesMethod.SaveChangesAsync();

            DomesticPaymentPostResponse response = persistedDomesticPayment.PublicPostResponse;

            return (response, nonErrorMessages);
        }
    }
}
