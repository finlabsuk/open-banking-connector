// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class AccountAccessConsentAccessToken :
    EncryptedObject
{
    public AccountAccessConsentAccessToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        string text,
        string tag,
        string? nonce,
        Guid accountAccessConsentId) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy,
        text,
        tag,
        nonce)
    {
        AccountAccessConsentId = accountAccessConsentId;
    }

    // Parent consent
    [ForeignKey("AccountAccessConsentId")]
    public AccountAccessConsent AccountAccessConsentNavigation { get; set; } = null!;

    public Guid AccountAccessConsentId { get; }
}
