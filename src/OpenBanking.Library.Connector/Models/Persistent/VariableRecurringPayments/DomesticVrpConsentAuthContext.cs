// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using DomesticPaymentConsentAuthContextResponse =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response.
    DomesticPaymentConsentAuthContextResponse;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticVrpConsentAuthContext :
        EntityBase,
        ISupportsFluentDeleteLocal<DomesticVrpConsentAuthContext>
    {
        public Guid DomesticPaymentConsentId { get; set; }

        [ForeignKey("DomesticPaymentConsentId")]
        public DomesticVrpConsent DomesticPaymentConsentNavigation { get; set; } = null!;

        /// <summary>
        ///     Token endpoint response. If null, indicates auth not successfully completed.
        /// </summary>
        public ReadWriteProperty<TokenEndpointResponse?> TokenEndpointResponse { get; set; } = null!;
    }

    internal partial class DomesticVrpConsentAuthContext :
        ISupportsFluentLocalEntityPost<DomesticVrpConsentAuthContextRequest,
            DomesticVrpConsentAuthContextPostResponse>
    {
        public void Initialise(
            DomesticVrpConsentAuthContextRequest request,
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

        public DomesticVrpConsentAuthContextPostResponse PublicPostResponse =>
            throw new NotImplementedException("Do not use; use customised version instead.");

        public DomesticPaymentConsentAuthContextPostResponse PublicPostResponseCustomised(string authUrl) =>
            new DomesticPaymentConsentAuthContextPostResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                authUrl);
    }

    internal partial class DomesticVrpConsentAuthContext :
        ISupportsFluentLocalEntityGet<DomesticPaymentConsentAuthContextResponse>
    {
        public DomesticPaymentConsentAuthContextResponse PublicGetResponse =>
            new DomesticPaymentConsentAuthContextResponse(
                Id,
                Name,
                Created,
                CreatedBy);
    }
}
