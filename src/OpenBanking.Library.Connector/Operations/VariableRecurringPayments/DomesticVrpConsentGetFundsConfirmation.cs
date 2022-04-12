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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
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
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    internal class
        DomesticVrpConsentGetFundsConfirmation : ReadWriteGet<DomesticVrpConsentPersisted,
            DomesticVrpConsentReadFundsConfirmationResponse,
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>
    {
        private readonly AuthContextAccessTokenGet _authContextAccessTokenGet;

        public DomesticVrpConsentGetFundsConfirmation(
            IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
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
            mapper)
        {
            _authContextAccessTokenGet = new AuthContextAccessTokenGet(
                softwareStatementProfileRepo,
                dbSaveChangesMethod,
                timeProvider);
        }

        protected override string RelativePathBeforeId => "/domestic-vrp-consents";
        protected override string RelativePathAfterId => "/funds-confirmation";

        protected override string ClientCredentialsGrantScope => "payments";


        protected override IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>
            ApiRequests(
                BankApiSet2 bankApiSet,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            bankApiSet.VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersionEnum.Version3p1p8 => new ApiGetRequests<
                    VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse,
                    VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>(
                    new PaymentInitiationGetRequestProcessor(
                        bankFinancialId,
                        accessToken)),
                null => throw new NullReferenceException("No VRP API specified for this bank."),

                _ => throw new ArgumentOutOfRangeException(
                    $"VRP API version {bankApiSet.VariableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion} not supported.")
            };

        protected override async Task<(
            string bankApiId,
            Uri endpointUrl,
            DomesticVrpConsentPersisted persistedObject,
            BankApiSet2 bankApiInformation,
            BankRegistration bankRegistration,
            string bankFinancialId,
            string? accessToken,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetRequestData(Guid id, string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            DomesticVrpConsentPersisted persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.DomesticVrpConsentAuthContextsNavigation)
                    .Include(o => o.BankApiSetNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {id}.");
            string bankApiId = persistedObject.ExternalApiId;
            BankApiSet bankApiSet = persistedObject.BankApiSetNavigation;
            var bankApiSet2 = new BankApiSet2
            {
                VariableRecurringPaymentsApi = new VariableRecurringPaymentsApi
                {
                    VariableRecurringPaymentsApiVersion =
                        bankApiSet.VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion ??
                        throw new InvalidOperationException(),
                    BaseUrl = bankApiSet.VariableRecurringPaymentsApi.BaseUrl
                }
            };
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

            // Get access token
            string accessToken =
                await _authContextAccessTokenGet.GetAccessToken(
                    persistedObject.DomesticVrpConsentAuthContextsNavigation,
                    bankRegistration,
                    modifiedBy);

            string baseUrl =
                bankApiSet.VariableRecurringPaymentsApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Variable Recurring Payments API.");
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            return (bankApiId, endpointUrl, persistedObject, bankApiSet2, bankRegistration, bankFinancialId,
                accessToken, nonErrorMessages);
        }

        protected override DomesticVrpConsentReadFundsConfirmationResponse GetReadResponse(
            DomesticVrpConsentPersisted persistedObject,
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse apiResponse)
        {
            return new DomesticVrpConsentReadFundsConfirmationResponse(
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
