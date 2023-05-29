// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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
        DateTimeOffset modified,
        string? modifiedBy,
        string? keyId,
        AccessToken accessToken,
        string associatedData,
        byte[] encryptionKey,
        Guid accountAccessConsentId) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy,
        modified,
        modifiedBy,
        keyId,
        accessToken.PlainText,
        associatedData,
        encryptionKey)
    {
        AccountAccessConsentId = accountAccessConsentId;
    }

    // Parent consent
    [ForeignKey(nameof(AccountAccessConsentId))]
    public AccountAccessConsent AccountAccessConsentNavigation { get; private set; } = null!;

    public Guid AccountAccessConsentId { get; }

    public void UpdateAccessToken(
        AccessToken accessToken,
        string associatedData,
        byte[] encryptionKey,
        DateTimeOffset modified,
        string? modifiedBy,
        string? keyId) =>
        UpdatePlainText(accessToken.PlainText, associatedData, encryptionKey, modified, modifiedBy, keyId);

    public AccessToken GetAccessToken(byte[] encryptionKey)
    {
        string plainText = GetPlainText(AccountAccessConsentId.ToString(), encryptionKey);
        return JsonConvert.DeserializeObject<AccessToken>(plainText)!;
    }
}
