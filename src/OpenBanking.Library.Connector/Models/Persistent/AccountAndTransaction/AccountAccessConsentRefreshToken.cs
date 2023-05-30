﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class AccountAccessConsentRefreshToken :
    EncryptedObject
{
    /// <summary>
    ///     Constructor. Ideally would set all fields (full state) of class but unfortunately having parameters which don't
    ///     directly map to properties causes an issue for EF Core. Thus this constructor should be followed by a call
    ///     to <see cref="UpdateRefreshToken" />.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="reference"></param>
    /// <param name="isDeleted"></param>
    /// <param name="isDeletedModified"></param>
    /// <param name="isDeletedModifiedBy"></param>
    /// <param name="created"></param>
    /// <param name="createdBy"></param>
    /// <param name="accountAccessConsentId"></param>
    public AccountAccessConsentRefreshToken(
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

    // Parent consent
    [ForeignKey(nameof(AccountAccessConsentId))]
    public AccountAccessConsent AccountAccessConsentNavigation { get; private set; } = null!;

    public Guid AccountAccessConsentId { get; }

    public void UpdateRefreshToken(
        string refreshToken,
        string associatedData,
        byte[] encryptionKey,
        DateTimeOffset modified,
        string? modifiedBy,
        string? keyId) =>
        UpdatePlainText(refreshToken, associatedData, encryptionKey, modified, modifiedBy, keyId);

    public string GetRefreshToken(byte[] encryptionKey) => GetPlainText(
        AccountAccessConsentId.ToString(),
        encryptionKey);
}
