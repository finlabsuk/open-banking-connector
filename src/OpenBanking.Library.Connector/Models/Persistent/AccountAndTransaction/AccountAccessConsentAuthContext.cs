// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class AccountAccessConsentAuthContext :
        AuthContext
    {
        public AccountAccessConsentAuthContext() { }

        public AccountAccessConsentAuthContext(
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider,
            string? accessToken,
            int accessTokenExpiresIn,
            string? refreshToken,
            Guid accountAccessConsentId) : base(
            id,
            name,
            createdBy,
            timeProvider,
            accessToken,
            accessTokenExpiresIn,
            refreshToken)
        {
            AccountAccessConsentId = accountAccessConsentId;
        }

        // Parent consent
        [ForeignKey("AccountAccessConsentId")]
        public AccountAccessConsent AccountAccessConsentNavigation { get; set; } = null!;

        public Guid AccountAccessConsentId { get; set; }
    }

    internal partial class AccountAccessConsentAuthContext :
        ISupportsFluentLocalEntityGet<AccountAccessConsentAuthContextReadLocalResponse>
    {
        public AccountAccessConsentAuthContextReadLocalResponse PublicGetLocalResponse =>
            new AccountAccessConsentAuthContextReadLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                AccountAccessConsentId);
    }
}
