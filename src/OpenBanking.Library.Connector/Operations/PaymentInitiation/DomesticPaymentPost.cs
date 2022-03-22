// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
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
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;
using DomesticPaymentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPayment;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class
        DomesticPaymentPost : ReadWritePost<DomesticPaymentPersisted, DomesticPaymentRequest,
            DomesticPaymentResponse,
            PaymentInitiationModelsPublic.OBWriteDomestic2, PaymentInitiationModelsPublic.OBWriteDomesticResponse5>
    {
        protected readonly IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted> _domesticPaymentConsentMethods;

        public DomesticPaymentPost(
            IDbReadWriteEntityMethods<DomesticPaymentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted>
                domesticPaymentConsentMethods,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper)
        {
            _domesticPaymentConsentMethods = domesticPaymentConsentMethods;
        }

        protected override string RelativePath => "/domestic-payments";

        protected override string ClientCredentialsGrantScope => "payments";

        protected override async Task<DomesticPaymentResponse> CreateLocalEntity(
            DomesticPaymentRequest request,
            PaymentInitiationModelsPublic.OBWriteDomestic2 apiRequest,
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5 apiResponse,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var persistedObject = new DomesticPaymentPersisted(
                Guid.NewGuid(),
                request.Name,
                request,
                apiRequest,
                apiResponse,
                createdBy,
                timeProvider);

            // Save entity
            await _entityMethods.AddAsync(persistedObject);

            // Create response (may involve additional processing based on entity)
            var response = new DomesticPaymentResponse(apiResponse, persistedObject.Id);

            return response;
        }

        protected override
            IApiPostRequests<PaymentInitiationModelsPublic.OBWriteDomestic2,
                PaymentInitiationModelsPublic.OBWriteDomesticResponse5> ApiRequests(
                BankApiSet bankApiSet,
                string bankFinancialId,
                TokenEndpointResponse tokenEndpointResponse,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            bankApiSet.PaymentInitiationApi?.PaymentInitiationApiVersion switch
            {
                PaymentInitiationApiVersion.Version3p1p4 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomestic2,
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5,
                    PaymentInitiationModelsV3p1p4.OBWriteDomestic2,
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticResponse4>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, tokenEndpointResponse),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsV3p1p4.OBWriteDomestic2>(
                        bankFinancialId,
                        tokenEndpointResponse,
                        instrumentationClient,
                        bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion <
                        PaymentInitiationApiVersion.Version3p1p4,
                        processedSoftwareStatementProfile)),
                PaymentInitiationApiVersion.Version3p1p6 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomestic2,
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5,
                    PaymentInitiationModelsPublic.OBWriteDomestic2,
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, tokenEndpointResponse),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsPublic.OBWriteDomestic2>(
                        bankFinancialId,
                        tokenEndpointResponse,
                        instrumentationClient,
                        bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion <
                        PaymentInitiationApiVersion.Version3p1p4,
                        processedSoftwareStatementProfile)),
                null => throw new NullReferenceException("No PISP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"PISP API version {bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion} not supported.")
            };

        protected override async Task<(
            PaymentInitiationModelsPublic.OBWriteDomestic2 apiRequest,
            Uri endpointUrl,
            BankApiSet bankApiInformation,
            BankRegistration bankRegistration,
            string bankFinancialId,
            TokenEndpointResponse? userTokenEndpointResponse,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages
            )> ApiPostRequestData(DomesticPaymentRequest request)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data
            Guid domesticPaymentConsentId = request.DomesticPaymentConsentId;
            DomesticPaymentConsentPersisted domesticPaymentConsent =
                await _domesticPaymentConsentMethods
                    .DbSetNoTracking
                    .Include(o => o.DomesticPaymentConsentAuthContextsNavigation)
                    .Include(o => o.BankApiSetNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == domesticPaymentConsentId) ??
                throw new KeyNotFoundException(
                    $"No record found for Domestic Payment Consent with ID {domesticPaymentConsentId}.");
            BankApiSet bankApiSet = domesticPaymentConsent.BankApiSetNavigation;
            BankRegistration bankRegistration = domesticPaymentConsent.BankRegistrationNavigation;
            string bankFinancialId = domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.FinancialId;

            // Create request
            PaymentInitiationModelsPublic.OBWriteDomestic2 apiRequest = request.ExternalApiRequest;
            if (request.ExternalApiRequest.Data.ConsentId is null)
            {
                apiRequest.Data.ConsentId = domesticPaymentConsent.ExternalApiId;
            }
            else if (apiRequest.Data.ConsentId != domesticPaymentConsent.ExternalApiId)
            {
                throw new ArgumentException(
                    $"OBWriteDomestic contains consent ID that differs from {domesticPaymentConsent.ExternalApiId} (inferred from DomesticPaymentConsentId)");
            }

            // Get token
            List<DomesticPaymentConsentAuthContextPersisted> authContextsWithToken =
                domesticPaymentConsent.DomesticPaymentConsentAuthContextsNavigation
                    .Where(x => x.TokenEndpointResponse.Data != null)
                    .ToList();

            TokenEndpointResponse userTokenEndpointResponse =
                authContextsWithToken.Any()
                    ? authContextsWithToken
                        .OrderByDescending(x => x.TokenEndpointResponse.Modified)
                        .Select(x => x.TokenEndpointResponse.Data)
                        .First()! // We already filtered out null entries above
                    : throw new InvalidOperationException("No token is available for Domestic Payment Consent.");

            // Determine endpoint URL
            string baseUrl =
                bankApiSet.PaymentInitiationApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Payment Initiation API.");
            var endpointUrl = new Uri(baseUrl + RelativePath);

            return (apiRequest, endpointUrl, bankApiSet, bankRegistration, bankFinancialId,
                userTokenEndpointResponse, nonErrorMessages);
        }
    }
}
