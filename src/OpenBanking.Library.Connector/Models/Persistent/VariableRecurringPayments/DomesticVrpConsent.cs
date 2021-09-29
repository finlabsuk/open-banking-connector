// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using DomesticPaymentConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;
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
    internal partial class DomesticVrpConsent :
        EntityBase,
        ISupportsFluentDeleteLocal<DomesticVrpConsent>,
        IDomesticPaymentConsentPublicQuery
    {
        public PaymentInitiationModelsPublic.OBWriteDomesticConsent4 BankApiRequest { get; set; } = null!;

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; set; } = null!;

        [ForeignKey("BankRegistrationId")]
        public BankRegistration BankRegistrationNavigation { get; set; } = null!;

        [ForeignKey("BankApiSetId")]
        public BankApiSet BankApiSetNavigation { get; set; } = null!;

        public List<DomesticPayment> DomesticPaymentsNavigation { get; set; } = null!;

        public List<DomesticVrpConsentAuthContext> DomesticPaymentConsentAuthContextsNavigation { get; set; } =
            null!;

        public ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1?>
            BankApiFundsConfirmationResponse { get; set; } = null!;

        public Guid BankRegistrationId { get; set; }

        public Guid BankApiSetId { get; set; }

        public ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> BankApiResponse
        {
            get;
            set;
        } = null!;
    }

    internal partial class DomesticVrpConsent :
        ISupportsFluentReadWritePost<DomesticPaymentConsentRequest,
            DomesticPaymentConsentResponse, PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>
    {
        public DomesticPaymentConsentResponse PublicGetResponse =>
            new DomesticPaymentConsentResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                BankApiResponse,
                BankRegistrationId,
                BankApiSetId);

        public void Initialise(
            DomesticPaymentConsentRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            base.Initialise(Guid.NewGuid(), request.Name, createdBy, timeProvider);
            BankRegistrationId = request.BankRegistrationId;
            BankApiSetId = request.BankApiSetId;
            BankApiFundsConfirmationResponse =
                new ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1?>(
                    null,
                    timeProvider,
                    createdBy);
        }

        public void UpdateBeforeApiPost(PaymentInitiationModelsPublic.OBWriteDomesticConsent4 apiRequest)
        {
            BankApiRequest = apiRequest;
        }

        public void UpdateAfterApiPost(
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiResponse =
                new ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
            ExternalApiId = BankApiResponse.Data.Data.ConsentId;
        }

        public DomesticPaymentConsentResponse PublicPostResponse => PublicGetResponse;

        public IApiPostRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> ApiPostRequests(
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

        public IApiRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> ApiRequests(
            PaymentInitiationApi paymentInitiationApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient)
            => paymentInitiationApi.PaymentInitiationApiVersion switch
            {
                PaymentInitiationApiVersion.Version3p1p4 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4,
                    PaymentInitiationModelsV3p1p4.OBWriteDomesticConsentResponse4>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, tokenEndpointResponse),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4>(
                        bankFinancialId,
                        tokenEndpointResponse,
                        instrumentationClient,
                        paymentInitiationApi,
                        processedSoftwareStatementProfile)),
                PaymentInitiationApiVersion.Version3p1p6 => new ApiRequests<
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsent4,
                    PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, tokenEndpointResponse),
                    new PaymentInitiationPostRequestProcessor<
                        PaymentInitiationModelsPublic.OBWriteDomesticConsent4>(
                        bankFinancialId,
                        tokenEndpointResponse,
                        instrumentationClient,
                        paymentInitiationApi,
                        processedSoftwareStatementProfile)),
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    internal partial class DomesticVrpConsent :
        ISupportsFluentReadWriteGet<DomesticPaymentConsentResponse,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>
    {
        public IApiGetRequests<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> ApiGetRequests(
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

    internal partial class DomesticVrpConsent :
        ISupportsFluentReadWriteGet<DomesticPaymentConsentResponse,
            PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1>
    {
        public void UpdateAfterApiGet(
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiResponse =
                new ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
        }

        public void UpdateAfterApiGet(
            PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiFundsConfirmationResponse =
                new ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1?>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
        }

        IApiGetRequests<PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1>
            ISupportsFluentReadWriteGet<DomesticPaymentConsentResponse,
                PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1>.ApiGetRequests(
                PaymentInitiationApi paymentInitiationApi,
                string bankFinancialId,
                TokenEndpointResponse tokenEndpointResponse,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient)
            => paymentInitiationApi.PaymentInitiationApiVersion switch
            {
                PaymentInitiationApiVersion.Version3p1p6 => new ApiGetRequests<
                    PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1,
                    PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1>(
                    new PaymentInitiationGetRequestProcessor(
                        bankFinancialId,
                        tokenEndpointResponse)),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
