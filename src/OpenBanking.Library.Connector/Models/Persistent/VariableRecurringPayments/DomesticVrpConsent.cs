// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
using DomesticVrpConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrpConsent;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticVrpConsent :
        EntityBase,
        ISupportsFluentDeleteLocal<DomesticVrpConsent>,
        IDomesticVrpConsentPublicQuery
    {
        public VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest BankApiRequest { get; set; } = null!;

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; set; } = null!;

        [ForeignKey("BankRegistrationId")]
        public BankRegistration BankRegistrationNavigation { get; set; } = null!;

        [ForeignKey("BankApiSetId")]
        public BankApiSet BankApiSetNavigation { get; set; } = null!;

        public List<DomesticVrp> DomesticVrpsNavigation { get; set; } = null!;

        public List<DomesticVrpConsentAuthContext> DomesticVrpConsentAuthContextsNavigation { get; set; } =
            null!;

        public ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse?>
            BankApiFundsConfirmationResponse { get; set; } = null!;

        public Guid BankRegistrationId { get; set; }

        public Guid BankApiSetId { get; set; }

        public ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> BankApiResponse
        {
            get;
            set;
        } = null!;
    }

    internal partial class DomesticVrpConsent :
        ISupportsFluentReadWritePost<DomesticVrpConsentRequest,
            DomesticVrpConsentResponse, VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>
    {
        public DomesticVrpConsentResponse PublicGetResponse =>
            new DomesticVrpConsentResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                BankApiResponse,
                BankRegistrationId,
                BankApiSetId);

        public void Initialise(
            DomesticVrpConsentRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            base.Initialise(Guid.NewGuid(), request.Name, createdBy, timeProvider);
            BankRegistrationId = request.BankRegistrationId;
            BankApiSetId = request.BankApiSetId;
            BankApiFundsConfirmationResponse =
                new ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse?>(
                    null,
                    timeProvider,
                    createdBy);
        }

        public void UpdateBeforeApiPost(VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest apiRequest)
        {
            BankApiRequest = apiRequest;
        }

        public void UpdateAfterApiPost(
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiResponse =
                new ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
            ExternalApiId = BankApiResponse.Data.Data.ConsentId;
        }

        public DomesticVrpConsentResponse PublicPostResponse => PublicGetResponse;

        public IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> ApiPostRequests(
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

        public IApiRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> ApiRequests(
            PaymentInitiationApi? paymentInitiationApi,
            VariableRecurringPaymentsApi? variableRecurringPaymentsApi,
            string bankFinancialId,
            TokenEndpointResponse tokenEndpointResponse,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient)
            => variableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiRequests<
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>(
                    new PaymentInitiationGetRequestProcessor(bankFinancialId, tokenEndpointResponse),
                    new PaymentInitiationPostRequestProcessor<
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest>(
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

    internal partial class DomesticVrpConsent :
        ISupportsFluentReadWriteGet<DomesticVrpConsentResponse,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>
    {
        public IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse> ApiGetRequests(
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

    internal partial class DomesticVrpConsent :
        ISupportsFluentReadWriteGet<DomesticVrpConsentResponse,
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>
    {
        public void UpdateAfterApiGet(
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiResponse =
                new ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
        }

        public void UpdateAfterApiGet(
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiFundsConfirmationResponse =
                new ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse?>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
        }

        IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>
            ISupportsFluentReadWriteGet<DomesticVrpConsentResponse,
                VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>.ApiGetRequests(
                PaymentInitiationApi? paymentInitiationApi,
                VariableRecurringPaymentsApi? variableRecurringPaymentsApi,
                string bankFinancialId,
                TokenEndpointResponse tokenEndpointResponse,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient)
            => variableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiGetRequests<
                    VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse,
                    VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse>(
                    new PaymentInitiationGetRequestProcessor(
                        bankFinancialId,
                        tokenEndpointResponse)),
                null => throw new NullReferenceException("No VRP API specified for this bank."),

                _ => throw new ArgumentOutOfRangeException(
                    $"VRP API version {variableRecurringPaymentsApi.VariableRecurringPaymentsApiVersion} not supported.")
            };
    }
}
