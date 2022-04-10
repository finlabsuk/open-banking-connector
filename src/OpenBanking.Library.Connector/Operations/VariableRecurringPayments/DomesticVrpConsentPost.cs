// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    internal class
        DomesticVrpConsentPost : ReadWritePost<DomesticVrpConsentPersisted,
            DomesticVrpConsent,
            DomesticVrpConsentReadResponse,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>
    {
        private readonly IDbReadOnlyEntityMethods<BankApiSet> _bankApiSetMethods;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationMethods;

        public DomesticVrpConsentPost(
            IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
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

        protected override string RelativePath => "/domestic-vrp-consents";

        protected override string ClientCredentialsGrantScope => "payments";

        protected override async Task<DomesticVrpConsentReadResponse> AddEntity(
            DomesticVrpConsent request,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest apiRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse apiResponse,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var persistedObject = new DomesticVrpConsentPersisted(
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
            var response =
                new DomesticVrpConsentReadResponse(
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
            IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> ApiRequests(
                BankApiSet bankApiSet,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            bankApiSet.VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersionEnum.Version3p1p8 => new ApiRequests<
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient,
                        false,
                        processedSoftwareStatementProfile)),
                null => throw new NullReferenceException("No VRP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"VRP API version {bankApiSet.VariableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion} not supported.")
            };

        protected override async
            Task<(
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest apiRequest,
                Uri endpointUrl,
                BankApiSet bankApiInformation,
                BankRegistration bankRegistration,
                string bankFinancialId,
                string? accessToken,
                List<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)> ApiPostRequestData(DomesticVrpConsent request)
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
            if (bankApiSet.BankId != bankRegistration.BankId)
            {
                throw new ArgumentException("BankRegistrationId and BankProfileId objects do not share same BankId.");
            }

            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            // Create request
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest apiRequest =
                request.ExternalApiRequest;

            // Determine endpoint URL
            string baseUrl =
                bankApiSet.VariableRecurringPaymentsApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Variable Recurring Payments API.");
            var endpointUrl = new Uri(baseUrl + RelativePath);

            return (apiRequest, endpointUrl, bankApiSet, bankRegistration, bankFinancialId, null,
                nonErrorMessages);
        }
    }
}
