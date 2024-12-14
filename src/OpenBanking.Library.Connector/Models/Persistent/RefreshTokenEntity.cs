// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal abstract class RefreshTokenEntity : EncryptedObject
{
    protected RefreshTokenEntity(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy) { }

    public void UpdateRefreshToken(
        string refreshToken,
        string associatedData,
        byte[] encryptionKey,
        DateTimeOffset modified,
        string? modifiedBy,
        Guid? keyId) =>
        UpdatePlainText(refreshToken, associatedData, encryptionKey, modified, modifiedBy, keyId);

    public string GetRefreshToken(string associatedData, byte[] encryptionKey) => GetPlainText(
        associatedData,
        encryptionKey);
}
