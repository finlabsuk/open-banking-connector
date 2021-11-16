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
        ISupportsFluentDeleteLocal<DomesticPaymentConsentAuthContext>,
        IDomesticPaymentConsentAuthContextPublicQuery
    {
        // Parent consent (optional to avoid warning due to non-support of global query filter)
        [ForeignKey("DomesticPaymentConsentId")]
        public DomesticPaymentConsent DomesticPaymentConsentNavigation { get; set; } = null!;

        public Guid DomesticPaymentConsentId { get; set; }
    }

    internal partial class DomesticPaymentConsentAuthContext :
        ISupportsFluentLocalEntityPost<DomesticPaymentConsentAuthContextRequest,
            DomesticPaymentConsentAuthContextPostResponse>
    {
        public void Initialise(
            DomesticPaymentConsentAuthContextRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            base.Initialise(Guid.NewGuid(), request.Name, createdBy, timeProvider);
            DomesticPaymentConsentId = request.DomesticPaymentConsentId;
            TokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                null,
                timeProvider,
                createdBy);
        }

        public DomesticPaymentConsentAuthContextPostResponse PublicPostResponse =>
            throw new NotImplementedException("Do not use; use customised version instead.");

        public DomesticPaymentConsentAuthContextPostResponse PublicPostResponseCustomised(string authUrl) =>
            new DomesticPaymentConsentAuthContextPostResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                DomesticPaymentConsentId,
                authUrl);
    }

    internal partial class DomesticPaymentConsentAuthContext :
        ISupportsFluentLocalEntityGet<DomesticPaymentConsentAuthContextResponse>
    {
        public DomesticPaymentConsentAuthContextResponse PublicGetResponse =>
            new DomesticPaymentConsentAuthContextResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                DomesticPaymentConsentId);
    }
}
