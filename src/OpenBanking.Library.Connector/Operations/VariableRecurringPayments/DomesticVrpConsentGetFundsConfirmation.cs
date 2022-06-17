// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        DomesticVrpConsentGetFundsConfirmation : ConsentRead<DomesticVrpConsentPersisted,
            DomesticVrpConsentReadFundsConfirmationResponse,
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse, ConsentBaseReadParams>
    {
        private readonly AuthContextAccessTokenGet _authContextAccessTokenGet;

        public DomesticVrpConsentGetFundsConfirmation(
            IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IGrantPost grantPost,
            AuthContextAccessTokenGet authContextAccessTokenGet) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper,
            grantPost)
        {
            _authContextAccessTokenGet = authContextAccessTokenGet;
        }

        protected string RelativePathBeforeId => "/domestic-vrp-consents";
        protected string RelativePathAfterId => "/funds-confirmation";

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
                VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiGetRequests<
                    VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse,
                    VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>(
                    new PaymentInitiationGetRequestProcessor(
                        bankFinancialId,
                        accessToken)),
                null => throw new NullReferenceException("No VRP API specified for this bank."),

                _ => throw new ArgumentOutOfRangeException(
                    $"VRP API version {bankApiSet.VariableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion} not supported.")
            };

        protected override async Task<(Uri endpointUrl, DomesticVrpConsentPersisted persistedObject, BankApiSet2
            bankApiInformation, BankRegistration bankRegistration, string bankFinancialId, string? accessToken,
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
                    .Include(o => o.VariableRecurringPaymentsApiNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {id}.");
            string bankApiId = persistedObject.ExternalApiId;
            VariableRecurringPaymentsApiEntity variableRecurringPaymentsApi =
                persistedObject.VariableRecurringPaymentsApiNavigation;
            var bankApiSet2 = new BankApiSet2
            {
                VariableRecurringPaymentsApi = new VariableRecurringPaymentsApi
                {
                    VariableRecurringPaymentsApiVersion = variableRecurringPaymentsApi.ApiVersion,
                    BaseUrl = variableRecurringPaymentsApi.BaseUrl
                }
            };
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

            // Get access token
            string? requestObjectAudClaim =
                persistedObject.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                    ?.DomesticVrpConsentAuthGet
                    ?.AudClaim;
            string bankIssuerUrl =
                requestObjectAudClaim ??
                bankRegistration.BankNavigation.IssuerUrl ??
                throw new Exception("Cannot determine issuer URL for bank");
            string accessToken =
                await _authContextAccessTokenGet.GetAccessTokenAndUpdateConsent(
                    persistedObject,
                    bankIssuerUrl,
                    "openid payments",
                    bankRegistration,
                    modifiedBy);

            string baseUrl = variableRecurringPaymentsApi.BaseUrl;
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            return (endpointUrl, persistedObject, bankApiSet2, bankRegistration, bankFinancialId,
                accessToken, nonErrorMessages);
        }

        protected override DomesticVrpConsentReadFundsConfirmationResponse GetPublicResponse(
            DomesticVrpConsentPersisted persistedObject,
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse apiResponse,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery)
        {
            // TODO: Update links

            return new DomesticVrpConsentReadFundsConfirmationResponse(
                persistedObject.Id,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.Reference,
                persistedObject.BankRegistrationId,
                persistedObject.VariableRecurringPaymentsApiId,
                persistedObject.ExternalApiId,
                apiResponse);
        }
    }
}
