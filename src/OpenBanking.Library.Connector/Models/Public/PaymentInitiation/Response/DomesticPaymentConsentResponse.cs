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

    public abstract class DomesticPaymentConsentBaseResponse : LocalObjectBaseResponse,
        IDomesticPaymentConsentPublicQuery
    {
        internal DomesticPaymentConsentBaseResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            IList<string>? warnings,
            Guid bankRegistrationId,
            Guid paymentInitiationApiId,
            string externalApiId) : base(id, created, createdBy, reference)
        {
            Warnings = warnings;
            BankRegistrationId = bankRegistrationId;
            PaymentInitiationApiId = paymentInitiationApiId;
            ExternalApiId = externalApiId;
        }

        /// <summary>
        ///     Optional list of warning messages from Open Banking Connector.
        /// </summary>
        public IList<string>? Warnings { get; set; }

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
    ///     Response to DomesticPaymentConsent Create requests
    /// </summary>
    public class DomesticPaymentConsentCreateResponse : DomesticPaymentConsentBaseResponse
    {
        internal DomesticPaymentConsentCreateResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            IList<string>? warnings,
            Guid bankRegistrationId,
            Guid paymentInitiationApiId,
            string externalApiId,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5? externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            warnings,
            bankRegistrationId,
            paymentInitiationApiId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5? ExternalApiResponse { get; }
    }

    /// <summary>
    ///     Response to DomesticPaymentConsent Read requests
    /// </summary>
    public class DomesticPaymentConsentReadResponse : DomesticPaymentConsentBaseResponse
    {
        internal DomesticPaymentConsentReadResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            IList<string>? warnings,
            Guid bankRegistrationId,
            Guid paymentInitiationApiId,
            string externalApiId,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            warnings,
            bankRegistrationId,
            paymentInitiationApiId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 ExternalApiResponse { get; }
    }


    /// <summary>
    ///     Response to DomesticPaymentConsent ReadFundsConfirmation requests
    /// </summary>
    public class DomesticPaymentConsentReadFundsConfirmationResponse : DomesticPaymentConsentBaseResponse
    {
        public DomesticPaymentConsentReadFundsConfirmationResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            IList<string>? warnings,
            Guid bankRegistrationId,
            Guid paymentInitiationApiId,
            string externalApiId,
            PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            warnings,
            bankRegistrationId,
            paymentInitiationApiId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 ExternalApiResponse { get; }
    }
}
