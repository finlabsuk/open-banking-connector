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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
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
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    internal class
        DomesticVrpGet : ReadWriteGet<DomesticVrpPersisted, DomesticVrpResponse,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>
    {
        public DomesticVrpGet(
            IDbReadWriteEntityMethods<DomesticVrpPersisted> entityMethods,
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

        protected override string RelativePathBeforeId => "/domestic-vrps";

        protected override string ClientCredentialsGrantScope => "payments";


        protected override IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> ApiRequests(
            BankApiSet bankApiSet,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient)
            =>
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
            string bankApiId,
            Uri endpointUrl,
            DomesticVrpPersisted persistedObject,
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
            DomesticVrpPersisted persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(x => x.DomesticVrpConsentNavigation)
                    .ThenInclude(x => x.BankApiSetNavigation)
                    .Include(x => x.DomesticVrpConsentNavigation)
                    .ThenInclude(x => x.BankRegistrationNavigation)
                    .ThenInclude(x => x.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Domestic Payment with ID {id}.");
            DomesticVrpConsent domesticPaymentConsent =
                persistedObject.DomesticVrpConsentNavigation;
            BankApiSet bankApiSet = domesticPaymentConsent.BankApiSetNavigation;
            BankRegistration bankRegistration = domesticPaymentConsent.BankRegistrationNavigation;
            string bankFinancialId = domesticPaymentConsent.BankRegistrationNavigation.BankNavigation.FinancialId;

            string bankApiId = persistedObject.ExternalApiId;

            string baseUrl =
                bankApiSet.VariableRecurringPaymentsApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Variable Recurring Payments API.");
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            return (bankApiId, endpointUrl, persistedObject, bankApiSet, bankRegistration, bankFinancialId,
                null, nonErrorMessages);
        }

        protected override DomesticVrpResponse GetReadResponse(
            DomesticVrpPersisted persistedObject,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse apiResponse)
        {
            return new DomesticVrpResponse(persistedObject.Id, apiResponse);
        }
    }
}
