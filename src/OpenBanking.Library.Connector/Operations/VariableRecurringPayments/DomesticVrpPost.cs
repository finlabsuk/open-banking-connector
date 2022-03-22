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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using DomesticVrpPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrp;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    internal class
        DomesticVrpPost : ReadWritePost<DomesticVrpPersisted, DomesticVrpRequest,
            DomesticVrpResponse,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>
    {
        protected readonly IDbReadOnlyEntityMethods<DomesticVrpConsentPersisted> _domesticVrpConsentMethods;


        public DomesticVrpPost(
            IDbReadWriteEntityMethods<DomesticVrpPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticVrpConsentPersisted>
                domesticVrpConsentMethods,
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
            _domesticVrpConsentMethods = domesticVrpConsentMethods;
        }

        protected override string RelativePath => "/domestic-vrps";

        protected override string ClientCredentialsGrantScope => "payments";

        protected override async Task<DomesticVrpResponse> CreateLocalEntity(
            DomesticVrpRequest request,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest apiRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse apiResponse,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var persistedObject = new DomesticVrpPersisted(
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
            var response =
                new DomesticVrpResponse(
                    persistedObject.Id,
                    apiResponse);

            return response;
        }

        protected override
            IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> ApiRequests(
                BankApiSet bankApiSet,
                string bankFinancialId,
                TokenEndpointResponse tokenEndpointResponse,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            bankApiSet.VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiRequests<
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, tokenEndpointResponse),
                    new PaymentInitiationPostRequestProcessor<
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest>(
                        bankFinancialId,
                        tokenEndpointResponse,
                        instrumentationClient,
                        false,
                        processedSoftwareStatementProfile)),
                null => throw new NullReferenceException("No VRP API specified for this bank."),

                _ => throw new ArgumentOutOfRangeException(
                    $"VRP API version {bankApiSet.VariableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion} not supported.")
            };

        protected override async Task<(
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest apiRequest,
            Uri endpointUrl,
            BankApiSet bankApiInformation,
            BankRegistration bankRegistration,
            string bankFinancialId,
            TokenEndpointResponse? userTokenEndpointResponse,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages
            )> ApiPostRequestData(DomesticVrpRequest request)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data
            Guid domesticPaymentConsentId = request.DomesticVrpConsentId;
            DomesticVrpConsentPersisted domesticPaymentConsent =
                await _domesticVrpConsentMethods
                    .DbSetNoTracking
                    .Include(o => o.DomesticVrpConsentAuthContextsNavigation)
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
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest apiRequest = request.ExternalApiRequest;
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
            List<DomesticVrpConsentAuthContextPersisted> authContextsWithToken =
                domesticPaymentConsent.DomesticVrpConsentAuthContextsNavigation
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
                bankApiSet.VariableRecurringPaymentsApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Variable Recurring Payments API.");
            var endpointUrl = new Uri(baseUrl + RelativePath);

            return (apiRequest, endpointUrl, bankApiSet, bankRegistration, bankFinancialId,
                userTokenEndpointResponse, nonErrorMessages);
        }
    }
}
