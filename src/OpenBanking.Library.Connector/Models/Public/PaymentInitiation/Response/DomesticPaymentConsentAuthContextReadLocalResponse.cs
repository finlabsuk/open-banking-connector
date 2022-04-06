// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentConsentAuthContextPublicQuery : IBaseQuery
    {
        public Guid DomesticPaymentConsentId { get; }
    }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class DomesticPaymentConsentAuthContextReadLocalResponse : BaseResponse,
        IDomesticPaymentConsentAuthContextPublicQuery
    {
        internal DomesticPaymentConsentAuthContextReadLocalResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid domesticPaymentConsentId) : base(id, name, created, createdBy)
        {
            DomesticPaymentConsentId = domesticPaymentConsentId;
        }

        public Guid DomesticPaymentConsentId { get; }
    }

    /// <summary>
    ///     Response to Get, Post
    /// </summary>
    public class
        DomesticPaymentConsentAuthContextCreateLocalResponse : DomesticPaymentConsentAuthContextReadLocalResponse
    {
        internal DomesticPaymentConsentAuthContextCreateLocalResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid domesticPaymentConsentId,
            string authUrl) : base(id, name, created, createdBy, domesticPaymentConsentId)
        {
            AuthUrl = authUrl;
        }

        public string AuthUrl { get; }
    }
}
