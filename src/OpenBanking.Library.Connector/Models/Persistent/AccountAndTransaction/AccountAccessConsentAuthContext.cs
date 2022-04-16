﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class AccountAccessConsentAuthContext :
        AuthContext
    {
        public AccountAccessConsentAuthContext(
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
            Guid accountAccessConsentId) : base(
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
            AccountAccessConsentId = accountAccessConsentId;
        }


        // Parent consent
        [ForeignKey("AccountAccessConsentId")]
        public AccountAccessConsent AccountAccessConsentNavigation { get; set; } = null!;

        public Guid AccountAccessConsentId { get; }
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