// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticVrp :
        EntityBase,
        ISupportsFluentDeleteLocal<DomesticVrp>,
        IDomesticVrpPublicQuery
    {
        public Guid DomesticVrpConsentId { get; set; }

        [ForeignKey("DomesticVrpConsentId")]
        public DomesticVrpConsent DomesticVrpConsentNavigation { get; set; } = null!;

        public VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest BankApiRequest { get; set; } = null!;

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; set; } = null!;

        public ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> BankApiResponse
        {
            get;
            set;
        } =
            null!;
    }

    internal partial class DomesticVrp :
        ISupportsFluentReadWritePost<DomesticVrpRequest, DomesticVrpResponse,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse, DomesticVrp>
    {
        public DomesticVrpResponse PublicGetResponse => new DomesticVrpResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            BankApiResponse);

        public DomesticVrp() {}

        private DomesticVrp(
            Guid id,
            string? name,
            DomesticVrpRequest request,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest apiRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse apiResponse,
            string? createdBy,
            ITimeProvider timeProvider) : base(
            id,
            name,
            createdBy,
            timeProvider)
        {
            DomesticVrpConsentId = request.DomesticVrpConsentId;
            BankApiRequest = apiRequest;
            BankApiResponse =
                new ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>(
                    apiResponse,
                    timeProvider,
                    createdBy);
            ExternalApiId = BankApiResponse.Data.Data.DomesticVRPId;
        }
        
        public DomesticVrp Create(
            DomesticVrpRequest request,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest apiRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse apiResponse,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var output = new DomesticVrp(
                Guid.NewGuid(),
                request.Name,
                request,
                apiRequest,
                apiResponse,
                createdBy,
                timeProvider);

            return output;
        }

        public DomesticVrp Create(DomesticVrpRequest request, string? createdBy, ITimeProvider timeProvider)
        {
            throw new NotImplementedException();
        }

        public DomesticVrpResponse PublicPostResponse => PublicGetResponse;

        public IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> ApiPostRequests(
            PaymentInitiationApi? paymentInitiationApi,
            VariableRecurringPaymentsApi? variableRecurringPaymentsApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient) =>
            ApiRequests(
                paymentInitiationApi,
                variableRecurringPaymentsApi,
                bankFinancialId,
                tokenEndpointResponse,
                processedSoftwareStatementProfile,
                instrumentationClient);


        public IApiRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> ApiRequests(
            PaymentInitiationApi? paymentInitiationApi,
            VariableRecurringPaymentsApi? variableRecurringPaymentsApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient) =>
            variableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion switch
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
                    $"VRP API version {variableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion} not supported.")
            };
    }

    internal partial class DomesticVrp :
        ISupportsFluentReadWriteGet<DomesticVrpResponse,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>
    {
        public void UpdateAfterApiGet(
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiResponse =
                new ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
        }

        public ReadWriteApiType GetReadWriteApiType() => ReadWriteApiType.VariableRecurringPayments;

        public IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> ApiGetRequests(
            PaymentInitiationApi? paymentInitiationApi,
            VariableRecurringPaymentsApi? variableRecurringPaymentsApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient) =>
            ApiRequests(
                paymentInitiationApi,
                variableRecurringPaymentsApi,
                bankFinancialId,
                tokenEndpointResponse,
                processedSoftwareStatementProfile,
                instrumentationClient);
    }
}
