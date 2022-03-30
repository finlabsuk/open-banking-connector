// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;
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
            DomesticPaymentConsentAuthContextCreateLocalResponse, DomesticPaymentConsentAuthContext>
    {
        
        public DomesticPaymentConsentAuthContext () {}

        private DomesticPaymentConsentAuthContext( 
            Guid domesticPaymentConsentId,
            ReadWriteProperty<TokenEndpointResponse?> tokenEndpointResponse,
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider) : base (
            id,
            name,
            createdBy,
            timeProvider)
        {
            DomesticPaymentConsentId = domesticPaymentConsentId;
            TokenEndpointResponse = tokenEndpointResponse;
        }

        public DomesticPaymentConsentAuthContext Create(
            DomesticPaymentConsentAuthContextRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var tokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                null,
                timeProvider,
                createdBy);

            var output = new DomesticPaymentConsentAuthContext(
                request.DomesticPaymentConsentId,
                tokenEndpointResponse,
                Guid.NewGuid(),
                request.Name,
                createdBy,
                timeProvider);

            return output;
        }


        public DomesticPaymentConsentAuthContextCreateLocalResponse PublicPostResponse =>
            throw new NotImplementedException("Do not use; use customised version instead.");

        public DomesticPaymentConsentAuthContextCreateLocalResponse PublicPostResponseCustomised(string authUrl) =>
            new DomesticPaymentConsentAuthContextCreateLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                DomesticPaymentConsentId,
                authUrl);
    }

    internal partial class DomesticPaymentConsentAuthContext :
        ISupportsFluentLocalEntityGet<DomesticPaymentConsentAuthContextReadLocalResponse>
    {
        public DomesticPaymentConsentAuthContextReadLocalResponse PublicGetResponse =>
            new DomesticPaymentConsentAuthContextReadLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                DomesticPaymentConsentId);
    }
}
