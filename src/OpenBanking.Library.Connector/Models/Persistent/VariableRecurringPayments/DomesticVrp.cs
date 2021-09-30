// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;

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
        public Guid DomesticPaymentConsentId { get; set; }

        [ForeignKey("DomesticPaymentConsentId")]
        public DomesticVrp DomesticPaymentConsentNavigation { get; set; } = null!;

        public PaymentInitiationModelsPublic.OBWriteDomestic2 BankApiRequest { get; set; } = null!;

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; set; } = null!;

        public ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticResponse5> BankApiResponse { get; set; } =
            null!;
    }

    internal partial class DomesticVrp :
        ISupportsFluentReadWritePost<DomesticVrpRequest, DomesticVrpResponse,
            PaymentInitiationModelsPublic.OBWriteDomestic2, PaymentInitiationModelsPublic.OBWriteDomesticResponse5>
    {
        public DomesticVrpResponse PublicGetResponse => new DomesticVrpResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            BankApiResponse);

        public void Initialise(
            DomesticVrpRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            base.Initialise(Guid.NewGuid(), request.Name, createdBy, timeProvider);
            DomesticPaymentConsentId = request.DomesticVrpConsentId;
        }

        public DomesticVrpResponse PublicPostResponse => PublicGetResponse;

        public void UpdateBeforeApiPost(PaymentInitiationModelsPublic.OBWriteDomestic2 apiRequest)
        {
            BankApiRequest = apiRequest;
        }

        public void UpdateAfterApiPost(
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5 apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiResponse =
                new ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
            ExternalApiId = BankApiResponse.Data.Data.DomesticPaymentId;
        }

        public IApiPostRequests<PaymentInitiationModelsPublic.OBWriteDomestic2,
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5> ApiPostRequests(
            PaymentInitiationApi paymentInitiationApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient) =>
            ApiRequests(
                paymentInitiationApi,
                bankFinancialId,
                tokenEndpointResponse,
                processedSoftwareStatementProfile,
                instrumentationClient);


        public IApiRequests<PaymentInitiationModelsPublic.OBWriteDomestic2,
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5> ApiRequests(
            PaymentInitiationApi paymentInitiationApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient) =>
            paymentInitiationApi.PaymentInitiationApiVersion switch
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
                        paymentInitiationApi,
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
                        paymentInitiationApi,
                        processedSoftwareStatementProfile)),
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    internal partial class DomesticVrp :
        ISupportsFluentReadWriteGet<DomesticVrpResponse,
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5>
    {
        public void UpdateAfterApiGet(
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5 apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiResponse =
                new ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
        }

        public IApiGetRequests<PaymentInitiationModelsPublic.OBWriteDomesticResponse5> ApiGetRequests(
            PaymentInitiationApi paymentInitiationApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient) =>
            ApiRequests(
                paymentInitiationApi,
                bankFinancialId,
                tokenEndpointResponse,
                processedSoftwareStatementProfile,
                instrumentationClient);
    }
}
