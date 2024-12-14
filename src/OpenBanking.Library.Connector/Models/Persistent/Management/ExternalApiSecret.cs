// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class ExternalApiSecretEntity : EncryptedObject
{
    public ExternalApiSecretEntity(
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

    public void UpdateClientSecret(
        string clientSecret,
        string associatedData,
        byte[] encryptionKey,
        DateTimeOffset modified,
        string? modifiedBy,
        Guid? keyId) =>
        UpdatePlainText(clientSecret, associatedData, encryptionKey, modified, modifiedBy, keyId);

    public string GetClientSecret(string associatedData, byte[] encryptionKey) => GetPlainText(
        associatedData,
        encryptionKey);
}
