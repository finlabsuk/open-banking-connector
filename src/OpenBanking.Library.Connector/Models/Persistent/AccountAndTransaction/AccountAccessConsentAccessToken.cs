﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class AccountAccessConsentAccessToken :
    AccessTokenEntity
{
    /// <summary>
    ///     Constructor. Ideally would set all fields (full state) of class but unfortunately having parameters which don't
    ///     directly map to properties causes an issue for EF Core. Thus this constructor should be followed by a call
    ///     to <see cref="AccessTokenEntity.UpdateAccessToken" />
    /// </summary>
    public AccountAccessConsentAccessToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        Guid accountAccessConsentId) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
    {
        AccountAccessConsentId = accountAccessConsentId;
    }

    public Guid AccountAccessConsentId { get; }
}
