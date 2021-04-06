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
    internal class GetDomesticPaymentConsent
    {
        private readonly IDbReadOnlyEntityMethods<BankApiInformation> _bankApiInformationRepo;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationRepo;
        private readonly IDbReadOnlyEntityMethods<Bank> _bankRepo;
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly IDbReadOnlyEntityMethods<DomesticPaymentConsent> _domesticConsentRepo;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IApiVariantMapper _mapper;
        private readonly IReadOnlyRepository<SoftwareStatementProfileCached> _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public GetDomesticPaymentConsent(
            IDbReadOnlyEntityMethods<BankApiInformation> bankApiInformationRepo,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationRepo,
            IDbReadOnlyEntityMethods<Bank> bankRepo,
            IDbSaveChangesMethod dbSaveChangesMethod,
            IDbReadOnlyEntityMethods<DomesticPaymentConsent> domesticConsentRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfileRepo,
            ITimeProvider timeProvider)
        {
            _bankApiInformationRepo = bankApiInformationRepo;
            _bankRegistrationRepo = bankRegistrationRepo;
            _bankRepo = bankRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _domesticConsentRepo = domesticConsentRepo;
            _instrumentationClient = instrumentationClient;
            _mapper = mapper;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _timeProvider = timeProvider;
        }

        public async Task<(DomesticPaymentConsentGetResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            GetAsync(Guid id)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data objects
            DomesticPaymentConsent domesticPaymentConsent =
                await _domesticConsentRepo.GetNoTrackingAsync(id)
                ?? throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {id}.");
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

            // Get client credentials grant (we will not cache token for now but simply use to POST consent)
            JsonSerializerSettings? jsonSerializerSettings1 = null;
            TokenEndpointResponse tokenEndpointResponse =
                await PostTokenRequest.PostClientCredentialsGrantAsync(
                    "payments",
                    bankRegistration,
                    jsonSerializerSettings1,
                    softwareStatementProfile.ApiClient);

            // GET from external API
            var getApiObject = new GetRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>();
            string domesticConsentExternalId = domesticPaymentConsent.OBId;
            Uri uri = new Uri(
                paymentInitiationApi.BaseUrl + domesticPaymentConsent.ExternalApiPath +
                $"/{domesticConsentExternalId}");
            JsonSerializerSettings? jsonSerializerSettings = null;
            var (domesticResponse, newNonErrorMessages) =
                paymentInitiationApi.PaymentInitiationApiVersion switch
                {
                    PaymentInitiationApiVersion.Version3p1p4 => await getApiObject
                        .GetAsync<PaymentInitiationModelsV3p1p4.OBWriteDomesticConsentResponse4>(
                            uri,
                            new PaymentInitiationGetRequestProcessor(
                                bank.FinancialId,
                                tokenEndpointResponse,
                                softwareStatementProfile,
                                paymentInitiationApi,
                                _instrumentationClient),
                            jsonSerializerSettings,
                            softwareStatementProfile.ApiClient,
                            _mapper),
                    PaymentInitiationApiVersion.Version3p1p6 => await getApiObject
                        .GetAsync<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                            uri,
                            new PaymentInitiationGetRequestProcessor(
                                bank.FinancialId,
                                tokenEndpointResponse,
                                softwareStatementProfile,
                                paymentInitiationApi,
                                _instrumentationClient),
                            jsonSerializerSettings,
                            softwareStatementProfile.ApiClient,
                            _mapper),
                    _ => throw new ArgumentOutOfRangeException(
                        $"PaymentInitiationApi version {paymentInitiationApi.PaymentInitiationApiVersion}")
                };
            nonErrorMessages.AddRange(newNonErrorMessages);

            DomesticPaymentConsentGetResponse basePostResponse = domesticPaymentConsent.PublicGetResponse;
            basePostResponse.OBWriteDomesticConsentResponse = domesticResponse;

            // Return success response (thrown exceptions produce error response)
            return (basePostResponse, nonErrorMessages);
        }
    }
}
