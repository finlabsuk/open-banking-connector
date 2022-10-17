// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
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
        DomesticVrpConsentPost : ConsentPost<DomesticVrpConsentPersisted,
            DomesticVrpConsent,
            DomesticVrpConsentResponse,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>
    {
        private readonly IDbReadOnlyEntityMethods<VariableRecurringPaymentsApiEntity> _bankApiSetMethods;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationMethods;

        public DomesticVrpConsentPost(
            IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IGrantPost grantPost,
            IDbReadOnlyEntityMethods<VariableRecurringPaymentsApiEntity> bankApiSetMethods,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper,
            grantPost)
        {
            _bankApiSetMethods = bankApiSetMethods;
            _bankRegistrationMethods = bankRegistrationMethods;
        }

        protected override string ClientCredentialsGrantScope => "payments";

        protected override async Task<DomesticVrpConsentResponse> AddEntity(
            DomesticVrpConsent request,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? apiResponse,
            Uri apiRequestUrl,
            string? publicRequestUrlWithoutQuery,
            ITimeProvider timeProvider)
        {
            // TODO: Transform links

            string externalApiId =
                request.ExternalApiObject is null
                    ? apiResponse!.Data.ConsentId
                    : request.ExternalApiObject.ExternalApiId;

            DateTimeOffset utcNow = timeProvider.GetUtcNow();
            var persistedObject = new DomesticVrpConsentPersisted(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy,
                null,
                0,
                utcNow,
                request.CreatedBy,
                null,
                request.BankRegistrationId,
                externalApiId,
                null,
                null,
                utcNow,
                request.CreatedBy,
                request.VariableRecurringPaymentsApiId);

            AccessToken? accessToken = request.ExternalApiObject?.AccessToken;
            if (accessToken is not null)
            {
                persistedObject.UpdateAccessToken(
                    accessToken.Token,
                    accessToken.ExpiresIn,
                    accessToken.RefreshToken,
                    utcNow,
                    request.CreatedBy);
            }

            // Save entity
            await _entityMethods.AddAsync(persistedObject);

            // Create response (may involve additional processing based on entity)
            var response =
                new DomesticVrpConsentResponse(
                    persistedObject.Id,
                    persistedObject.Created,
                    persistedObject.CreatedBy,
                    persistedObject.Reference,
                    persistedObject.BankRegistrationId,
                    persistedObject.VariableRecurringPaymentsApiId,
                    persistedObject.ExternalApiId,
                    apiResponse);

            return response;
        }

        protected override
            IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> ApiRequests(
                BankApiSet2 bankApiSet,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            bankApiSet.VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiRequests<
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken),
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
                BankApiSet2 bankApiInformation,
                BankRegistration bankRegistration,
                string bankFinancialId,
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
            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            Guid variableRecurringPaymentsApiId = request.VariableRecurringPaymentsApiId;
            VariableRecurringPaymentsApiEntity variableRecurringPaymentsApiEntity =
                await _bankApiSetMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == variableRecurringPaymentsApiId) ??
                throw new KeyNotFoundException(
                    $"No record found for VariableRecurringPaymentsApi {variableRecurringPaymentsApiId} specified by request.");
            var bankApiSet2 = new BankApiSet2
            {
                VariableRecurringPaymentsApi = new VariableRecurringPaymentsApi
                {
                    VariableRecurringPaymentsApiVersion = variableRecurringPaymentsApiEntity.ApiVersion,
                    BaseUrl = variableRecurringPaymentsApiEntity.BaseUrl
                }
            };

            if (variableRecurringPaymentsApiEntity.BankId != bankRegistration.BankId)
            {
                throw new ArgumentException(
                    "Specified VariableRecurringPaymentsApi and BankRegistration objects do not share same BankId.");
            }

            // Create request
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest apiRequest =
                request.ExternalApiRequest;

            // Determine endpoint URL
            string baseUrl = variableRecurringPaymentsApiEntity.BaseUrl;
            var endpointUrl = new Uri(baseUrl + "/domestic-vrp-consents");

            return (apiRequest, endpointUrl, bankApiSet2, bankRegistration, bankFinancialId, nonErrorMessages);
        }
    }
}
