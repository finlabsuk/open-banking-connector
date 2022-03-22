// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentConsentPublicQuery : IBaseQuery
    {
        Guid BankRegistrationId { get; }

        Guid BankApiSetId { get; }

        public string ExternalApiId { get; }
    }

    /// <summary>
    ///     Response to ReadLocal requests
    /// </summary>
    public class DomesticPaymentConsentReadLocalResponse : BaseResponse, IDomesticPaymentConsentPublicQuery
    {
        public DomesticPaymentConsentReadLocalResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            string externalApiId) : base(id, name, created, createdBy)
        {
            BankRegistrationId = bankRegistrationId;
            BankApiSetId = bankApiSetId;
            ExternalApiId = externalApiId;
        }

        /// <summary>
        ///     ID of associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     ID of associated BankApiSet object
        /// </summary>
        public Guid BankApiSetId { get; }

        /// <summary>
        ///     External (bank) API ID for this object
        /// </summary>
        public string ExternalApiId { get; }
    }

    /// <summary>
    ///     Response to Read and Create requests
    /// </summary>
    public class DomesticPaymentConsentReadResponse : DomesticPaymentConsentReadLocalResponse
    {
        public DomesticPaymentConsentReadResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            string externalApiId,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 externalApiResponse) : base(
            id,
            name,
            created,
            createdBy,
            bankRegistrationId,
            bankApiSetId,
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
        public DomesticPaymentConsentReadFundsConfirmationResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            string externalApiId,
            PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 externalApiResponse) : base(
            id,
            name,
            created,
            createdBy,
            bankRegistrationId,
            bankApiSetId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 ExternalApiResponse { get; }
    }
}
