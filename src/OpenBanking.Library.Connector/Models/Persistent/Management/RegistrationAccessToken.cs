// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class RegistrationAccessTokenEntity : EncryptedObject
{
    public RegistrationAccessTokenEntity(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        Guid bankRegistrationId) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
    {
        BankRegistrationId = bankRegistrationId;
    }

    // Parent consent
    [ForeignKey(nameof(BankRegistrationId))]
    public BankRegistrationEntity BankRegistrationNavigation { get; private set; } = null!;

    public Guid BankRegistrationId { get; }

    public void UpdateRegistrationAccessToken(
        string registrationAccessToken,
        string associatedData,
        byte[] encryptionKey,
        DateTimeOffset modified,
        string? modifiedBy,
        string? keyId) =>
        UpdatePlainText(registrationAccessToken, associatedData, encryptionKey, modified, modifiedBy, keyId);

    public string GetRegistrationAccessToken(string associatedData, byte[] encryptionKey) => GetPlainText(
        associatedData,
        encryptionKey);
}
