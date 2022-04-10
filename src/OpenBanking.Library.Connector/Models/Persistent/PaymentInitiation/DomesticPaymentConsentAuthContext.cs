// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
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
        public DomesticPaymentConsentAuthContext(
            string? name,
            string? reference,
            Guid id,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string? accessTokenValue,
            int accessTokenExpiresIn,
            string? accessTokenRefreshToken,
            DateTimeOffset accessTokenModified,
            string? accessTokenModifiedBy,
            Guid domesticPaymentConsentId) : base(
            name,
            reference,
            id,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy,
            accessTokenValue,
            accessTokenExpiresIn,
            accessTokenRefreshToken,
            accessTokenModified,
            accessTokenModifiedBy)
        {
            DomesticPaymentConsentId = domesticPaymentConsentId;
        }


        // Parent consent (optional to avoid warning due to non-support of global query filter)
        [ForeignKey("DomesticPaymentConsentId")]
        public DomesticPaymentConsent DomesticPaymentConsentNavigation { get; set; } = null!;

        public Guid DomesticPaymentConsentId { get; }
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
