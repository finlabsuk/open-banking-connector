// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response
{
    public interface IAccountAccessConsentAuthContextPublicQuery : IBaseQuery
    {
        public Guid AccountAccessConsentId { get; }
    }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class AccountAccessConsentAuthContextReadLocalResponse : BaseResponse,
        IAccountAccessConsentAuthContextPublicQuery
    {
        internal AccountAccessConsentAuthContextReadLocalResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            Guid accountAccessConsentId) : base(id, created, createdBy)
        {
            AccountAccessConsentId = accountAccessConsentId;
        }

        public Guid AccountAccessConsentId { get; }
    }

    /// <summary>
    ///     Response to Get, Post
    /// </summary>
    public class AccountAccessConsentAuthContextCreateLocalResponse : AccountAccessConsentAuthContextReadLocalResponse
    {
        internal AccountAccessConsentAuthContextCreateLocalResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            Guid AccountAccessConsentId,
            string authUrl) : base(id, created, createdBy, AccountAccessConsentId)
        {
            AuthUrl = authUrl;
        }

        public string AuthUrl { get; }
    }
}
