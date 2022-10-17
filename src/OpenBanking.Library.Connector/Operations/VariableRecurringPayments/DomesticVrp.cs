// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    internal class DomesticVrp : DomesticVrpConsentExternalObject<DomesticVrpRequest,
        DomesticVrpResponse,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>
    {
        public DomesticVrp(
            IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
            IInstrumentationClient instrumentationClient,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IApiVariantMapper mapper,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IGrantPost grantPost,
            AuthContextAccessTokenGet authContextAccessTokenGet) : base(
            entityMethods,
            instrumentationClient,
            softwareStatementProfileRepo,
            mapper,
            dbSaveChangesMethod,
            timeProvider,
            grantPost,
            authContextAccessTokenGet) { }

        protected override string ClientCredentialsGrantScope => "payments";

        protected override VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest GetApiRequest(
            DomesticVrpRequest request,
            string externalApiConsentId)
        {
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest apiRequest = request.ExternalApiRequest;
            if (request.ExternalApiRequest.Data.ConsentId is null)
            {
                apiRequest.Data.ConsentId = externalApiConsentId;
            }
            else if (apiRequest.Data.ConsentId != externalApiConsentId)
            {
                throw new ArgumentException(
                    $"ExternalApiRequest contains consent ID that differs from {externalApiConsentId} " +
                    "inferred from specified DomesticVrpConsent.");
            }

            return apiRequest;
        }

        protected override Uri RetrieveGetUrl(string baseUrl, string externalId) =>
            new(baseUrl + "/domestic-vrps" + $"/{externalId}");

        protected override Uri RetrievePostUrl(string baseUrl) => new(baseUrl + "/domestic-vrps");


        protected override DomesticVrpResponse PublicGetResponse(
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse apiResponse) => new(apiResponse);

        protected override IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> ApiGetRequests(
            VariableRecurringPaymentsApi variableRecurringPaymentsApi,
            string bankFinancialId,
            string accessToken,
            IInstrumentationClient instrumentationClient) =>
            variableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiGetRequests<
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken)),
                _ => throw new ArgumentOutOfRangeException(
                    $"Variable Recurring Payments API version {variableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion} not supported.")
            };

        protected override
            IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> ApiPostRequests(
                VariableRecurringPaymentsApi variableRecurringPaymentsApi,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            variableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiRequests<
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient,
                        false,
                        processedSoftwareStatementProfile)),
                _ => throw new ArgumentOutOfRangeException(
                    $"Variable Recurring Payments API version {variableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion} not supported.")
            };
    }
}
