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
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class
        DomesticPaymentGet : ReadWriteGet<DomesticPaymentPersisted, DomesticPaymentResponse,
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5>
    {
        public DomesticPaymentGet(
            IDbReadWriteEntityMethods<DomesticPaymentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsent>
                domesticPaymentConsentMethods,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper) { }

        protected override string RelativePathBeforeId => "/domestic-payments";

        protected override string ClientCredentialsGrantScope => "payments";


        protected override IApiGetRequests<PaymentInitiationModelsPublic.OBWriteDomesticResponse5> ApiRequests(
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
            string bankApiId,
            Uri endpointUrl,
            DomesticPayment persistedObject,
            BankApiSet bankApiInformation,
            BankRegistration bankRegistration,
            string bankFinancialId,
            TokenEndpointResponse? userTokenEndpointResponse,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetRequestData(Guid id)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            DomesticPayment persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(x => x.DomesticPaymentConsentNavigation)
                    .ThenInclude(x => x.BankApiSetNavigation)
                    .Include(x => x.DomesticPaymentConsentNavigation)
                    .ThenInclude(x => x.BankRegistrationNavigation)
                    .ThenInclude(x => x.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Domestic Payment with ID {id}.");
            DomesticPaymentConsent domesticPaymentConsent =
                persistedObject.DomesticPaymentConsentNavigation;
            BankApiSet bankApiSet = domesticPaymentConsent.BankApiSetNavigation;
            BankRegistration bankRegistration = domesticPaymentConsent.BankRegistrationNavigation;
            string bankFinancialId = domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.FinancialId;

            string bankApiId = persistedObject.ExternalApiId;

            // Determine endpoint URL
            string baseUrl =
                bankApiSet.PaymentInitiationApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Payment Initiation API.");
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            return (bankApiId, endpointUrl, persistedObject, bankApiSet, bankRegistration, bankFinancialId,
                null, nonErrorMessages);
        }

        protected override DomesticPaymentResponse GetReadResponse(
            DomesticPayment persistedObject,
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5 apiResponse)
        {
            return new DomesticPaymentResponse(apiResponse, persistedObject.Id);
        }
    }
}
