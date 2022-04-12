// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class
        DomesticPaymentConsentPost : ReadWritePost<DomesticPaymentConsentPersisted,
            DomesticPaymentConsent,
            DomesticPaymentConsentReadResponse,
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>
    {
        private readonly IDbReadOnlyEntityMethods<BankApiSet> _bankApiSetMethods;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationMethods;

        public DomesticPaymentConsentPost(
            IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IDbReadOnlyEntityMethods<BankApiSet> bankApiSetMethods,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper)
        {
            _bankApiSetMethods = bankApiSetMethods;
            _bankRegistrationMethods = bankRegistrationMethods;
        }

        protected override string RelativePath => "/domestic-payment-consents";

        protected override string ClientCredentialsGrantScope => "payments";

        protected override async Task<DomesticPaymentConsentReadResponse> AddEntity(
            DomesticPaymentConsent request,
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4 apiRequest,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 apiResponse,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var persistedObject = new DomesticPaymentConsentPersisted(
                request.Name,
                request.Reference,
                Guid.NewGuid(),
                false,
                utcNow,
                createdBy,
                utcNow,
                createdBy,
                apiResponse.Data.ConsentId,
                request.BankRegistrationId,
                request.BankApiSetId);

            // Save entity
            await _entityMethods.AddAsync(persistedObject);

            // Create response (may involve additional processing based on entity)
            var response = new DomesticPaymentConsentReadResponse(
                persistedObject.Id,
                persistedObject.Name,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.BankRegistrationId,
                persistedObject.BankApiSetId,
                persistedObject.ExternalApiId,
                apiResponse);

            return response;
        }

        protected override
            IApiPostRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> ApiRequests(
                BankApiSet2 bankApiSet,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            bankApiSet.PaymentInitiationApi?.PaymentInitiationApiVersion switch
            {
                PaymentInitiationApiVersionEnum.Version3p1p4 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4,
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticConsentResponse4>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient,
                        bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion <
                        PaymentInitiationApiVersionEnum.Version3p1p4,
                        processedSoftwareStatementProfile)),
                PaymentInitiationApiVersionEnum.Version3p1p6 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsPublic.OBWriteDomesticConsent4>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient,
                        bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion <
                        PaymentInitiationApiVersionEnum.Version3p1p4,
                        processedSoftwareStatementProfile)),
                null => throw new NullReferenceException("No PISP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"PISP API version {bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion} not supported.")
            };

        protected override async
            Task<(
                PaymentInitiationModelsPublic.OBWriteDomesticConsent4 apiRequest,
                Uri endpointUrl,
                BankApiSet2 bankApiInformation,
                BankRegistration bankRegistration,
                string bankFinancialId,
                string? accessToken,
                List<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)> ApiPostRequestData(DomesticPaymentConsent request)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data and checks
            Guid bankRegistrationId = request.BankRegistrationId;
            BankRegistration bankRegistration =
                await _bankRegistrationMethods
                    .DbSetNoTracking
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == bankRegistrationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
            Guid bankApiInformationId = request.BankApiSetId;
            BankApiSet bankApiSet =
                await _bankApiSetMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == bankApiInformationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankApiInformation {bankApiInformationId} specified by request.");
            var bankApiSet2 = new BankApiSet2
            {
                PaymentInitiationApi = new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        bankApiSet.PaymentInitiationApi?.PaymentInitiationApiVersion ??
                        throw new InvalidOperationException(),
                    BaseUrl = bankApiSet.PaymentInitiationApi.BaseUrl
                }
            };

            if (bankApiSet.BankId != bankRegistration.BankId)
            {
                throw new ArgumentException("BankRegistrationId and BankProfileId objects do not share same BankId.");
            }

            // Determine endpoint URL
            string baseUrl =
                bankApiSet.PaymentInitiationApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Payment Initiation API.");
            var endpointUrl = new Uri(baseUrl + RelativePath);

            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            // Create request
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4 apiRequest = request.ExternalApiRequest;

            return (apiRequest, endpointUrl, bankApiSet2, bankRegistration, bankFinancialId, null,
                nonErrorMessages);
        }
    }
}
