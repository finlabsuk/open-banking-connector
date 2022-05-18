// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentConsentPublicQuery : IBaseQuery
    {
        /// <summary>
        ///     Associated BankRegistration object
        /// </summary>
        Guid BankRegistrationId { get; }

        /// <summary>
        ///     Associated PaymentInitiationApi object
        /// </summary>
        Guid PaymentInitiationApiId { get; }

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        string ExternalApiId { get; }
    }

    /// <summary>
    ///     Response to ReadLocal requests
    /// </summary>
    public class DomesticPaymentConsentReadLocalResponse : BaseResponse, IDomesticPaymentConsentPublicQuery
    {
        internal DomesticPaymentConsentReadLocalResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid bankRegistrationId,
            Guid paymentInitiationApiId,
            string externalApiId) : base(id, created, createdBy, reference)
        {
            BankRegistrationId = bankRegistrationId;
            PaymentInitiationApiId = paymentInitiationApiId;
            ExternalApiId = externalApiId;
        }

        /// <summary>
        ///     Associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     Associated PaymentInitiationApi object
        /// </summary>
        public Guid PaymentInitiationApiId { get; }

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; }
    }

    /// <summary>
    ///     Response to Read and Create requests
    /// </summary>
    public class DomesticPaymentConsentReadResponse : DomesticPaymentConsentReadLocalResponse
    {
        internal DomesticPaymentConsentReadResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid bankRegistrationId,
            Guid paymentInitiationApiId,
            string externalApiId,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            bankRegistrationId,
            paymentInitiationApiId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 ExternalApiResponse { get; }
    }

    /// <summary>
    ///     Response to ReadFundsConfirmation requests
    /// </summary>
    public class DomesticPaymentConsentReadFundsConfirmationResponse : DomesticPaymentConsentReadLocalResponse
    {
        internal DomesticPaymentConsentReadFundsConfirmationResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid bankRegistrationId,
            Guid paymentInitiationApiId,
            string externalApiId,
            PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            bankRegistrationId,
            paymentInitiationApiId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 ExternalApiResponse { get; }
    }
}
