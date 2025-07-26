// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class SettingsEntity
{
    public SettingsEntity(
        Guid id,
        Guid? currentEncryptionKeyDescriptionId,
        DateTimeOffset modified,
        DateTimeOffset created,
        bool disableEncryption)
    {
        if (id != SingletonId)
        {
            throw new ArgumentException("Invalid ID for singleton record/document.");
        }
        Id = id;
        CurrentEncryptionKeyDescriptionId = currentEncryptionKeyDescriptionId;
        Modified = modified;
        Created = created;
        DisableEncryption = disableEncryption;
    }

    public static Guid SingletonId => Guid.Parse("232c4049-a77a-4dbe-b740-ce6e9f4f54cf");

    public Guid Id { get; }

    public Guid? CurrentEncryptionKeyDescriptionId { get; private set; }

    public bool DisableEncryption { get; private set; }

    public DateTimeOffset Modified { get; private set; }

    public DateTimeOffset Created { get; }

    public void UpdateCurrentEncryptionKey(Guid encryptionKeyDescriptionId, DateTimeOffset modified)
    {
        CurrentEncryptionKeyDescriptionId = encryptionKeyDescriptionId;
        Modified = modified;
    }

    public void UpdateDisableEncryption(bool disableEncryption, DateTimeOffset modified)
    {
        DisableEncryption = disableEncryption;
        Modified = modified;
    }
}
