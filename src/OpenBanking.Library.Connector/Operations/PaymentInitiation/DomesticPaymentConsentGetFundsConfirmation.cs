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
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    internal class
        DomesticPaymentConsentGetFundsConfirmation : ReadWriteGet<DomesticPaymentConsentPersisted,
            DomesticPaymentConsentReadFundsConfirmationResponse,
            PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1>
    {
        public DomesticPaymentConsentGetFundsConfirmation(
            IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper) { }

        protected override string RelativePathBeforeId => "/domestic-payment-consents";
        protected override string RelativePathAfterId => "/funds-confirmation";

        protected override string ClientCredentialsGrantScope => "payments";


        protected override IApiGetRequests<PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1> ApiRequests(
            BankApiSet bankApiSet,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient)
            => bankApiSet.PaymentInitiationApi?.PaymentInitiationApiVersion switch
            {
                PaymentInitiationApiVersion.Version3p1p4 => new ApiGetRequests<
                    PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1,
                    PaymentInitiationModelsV3p1p4.OBWriteFundsConfirmationResponse1>(
                    new PaymentInitiationGetRequestProcessor(
                        bankFinancialId,
                        tokenEndpointResponse)),
                PaymentInitiationApiVersion.Version3p1p6 => new ApiGetRequests<
                    PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1,
                    PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1>(
                    new PaymentInitiationGetRequestProcessor(
                        bankFinancialId,
                        tokenEndpointResponse)),
                null => throw new NullReferenceException("No PISP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"PISP API version {bankApiSet.PaymentInitiationApi.PaymentInitiationApiVersion} not supported.")
            };

        protected override async Task<(
            string bankApiId,
            Uri endpointUrl,
            DomesticPaymentConsentPersisted persistedObject,
            BankApiSet bankApiInformation,
            BankRegistration bankRegistration,
            string bankFinancialId, TokenEndpointResponse? userTokenEndpointResponse,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetRequestData(Guid id)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            DomesticPaymentConsentPersisted persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.DomesticPaymentConsentAuthContextsNavigation)
                    .Include(o => o.BankApiSetNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {id}.");
            string bankApiId = persistedObject.ExternalApiId;
            BankApiSet bankApiSet = persistedObject.BankApiSetNavigation;
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

            // Get token
            List<DomesticPaymentConsentAuthContextPersisted> authContextsWithToken =
                persistedObject.DomesticPaymentConsentAuthContextsNavigation
                    .Where(x => x.TokenEndpointResponse.Value != null)
                    .ToList();

            TokenEndpointResponse userTokenEndpointResponse =
                authContextsWithToken.Any()
                    ? authContextsWithToken
                        .OrderByDescending(x => x.TokenEndpointResponse.Modified)
                        .Select(x => x.TokenEndpointResponse.Value)
                        .First()! // We already filtered out null entries above
                    : throw new InvalidOperationException("No token is available for Domestic Payment Consent.");

            // Determine endpoint URL
            string baseUrl =
                bankApiSet.PaymentInitiationApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Payment Initiation API.");
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            return (bankApiId, endpointUrl, persistedObject, bankApiSet, bankRegistration, bankFinancialId,
                userTokenEndpointResponse, nonErrorMessages);
        }

        protected override DomesticPaymentConsentReadFundsConfirmationResponse GetReadResponse(
            DomesticPaymentConsentPersisted persistedObject,
            PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 apiResponse)
        {
            return new DomesticPaymentConsentReadFundsConfirmationResponse(
                persistedObject.Id,
                persistedObject.Name,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.BankRegistrationId,
                persistedObject.BankApiSetId,
                persistedObject.ExternalApiId,
                apiResponse);
        }
    }
}
