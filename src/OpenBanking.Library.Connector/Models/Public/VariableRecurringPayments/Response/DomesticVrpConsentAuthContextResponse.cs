// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response
{
    public interface IDomesticVrpConsentAuthContextPublicQuery : IBaseQuery { }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class DomesticVrpConsentAuthContextResponse : BaseResponse,
        IDomesticPaymentConsentAuthContextPublicQuery
    {
        internal DomesticVrpConsentAuthContextResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy) : base(id, name, created, createdBy) { }
    }

    /// <summary>
    ///     Response to Get, Post
    /// </summary>
    public class DomesticVrpConsentAuthContextPostResponse : DomesticVrpConsentAuthContextResponse
    {
        internal DomesticVrpConsentAuthContextPostResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            string authUrl) : base(id, name, created, createdBy)
        {
            AuthUrl = authUrl;
        }

        public string AuthUrl { get; }
    }
}
