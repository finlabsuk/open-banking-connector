// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticPayment :
        EntityBase
    {
        public DomesticPayment() { }

        public DomesticPayment(
            Guid id,
            string? name,
            DomesticPaymentRequest request,
            PaymentInitiationModelsPublic.OBWriteDomestic2 apiRequest,
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5 apiResponse,
            string? createdBy,
            ITimeProvider timeProvider) : base(
            id,
            name,
            createdBy,
            timeProvider)
        {
            DomesticPaymentConsentId = Guid.NewGuid();
            BankApiRequest = apiRequest;
            BankApiResponse =
                new ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(
                    apiResponse,
                    timeProvider,
                    createdBy);
            ExternalApiId = BankApiResponse.Value.Data.DomesticPaymentId;
        }

        public Guid DomesticPaymentConsentId { get; set; }

        [ForeignKey("DomesticPaymentConsentId")]
        public DomesticPaymentConsent DomesticPaymentConsentNavigation { get; set; } = null!;

        public PaymentInitiationModelsPublic.OBWriteDomestic2 BankApiRequest { get; set; } = null!;

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; set; } = null!;

        public ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticResponse5> BankApiResponse { get; set; } =
            null!;
    }
}
