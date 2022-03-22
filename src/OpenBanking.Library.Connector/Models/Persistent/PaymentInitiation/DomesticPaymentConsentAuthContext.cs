// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticPaymentConsentAuthContext :
        AuthContext,
        IDomesticPaymentConsentAuthContextPublicQuery
    {
        public DomesticPaymentConsentAuthContext() { }

        public DomesticPaymentConsentAuthContext(
            Guid id,
            string? name,
            ReadWriteProperty<TokenEndpointResponse?> tokenEndpointResponse,
            Guid domesticPaymentConsentId,
            string? createdBy,
            ITimeProvider timeProvider) : base(id, name, tokenEndpointResponse, createdBy, timeProvider)
        {
            DomesticPaymentConsentId = domesticPaymentConsentId;
        }

        // Parent consent (optional to avoid warning due to non-support of global query filter)
        [ForeignKey("DomesticPaymentConsentId")]
        public DomesticPaymentConsent DomesticPaymentConsentNavigation { get; set; } = null!;

        public Guid DomesticPaymentConsentId { get; set; }
    }

    internal partial class DomesticPaymentConsentAuthContext :
        ISupportsFluentLocalEntityGet<DomesticPaymentConsentAuthContextReadLocalResponse>
    {
        public DomesticPaymentConsentAuthContextReadLocalResponse PublicGetLocalResponse =>
            new DomesticPaymentConsentAuthContextReadLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                DomesticPaymentConsentId);
    }
}
