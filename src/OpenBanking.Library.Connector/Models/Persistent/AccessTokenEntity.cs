// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal abstract class AccessTokenEntity : EncryptedObject
{
    public AccessTokenEntity(
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

    public void UpdateAccessToken(
        AccessToken accessToken,
        string associatedData,
        byte[] encryptionKey,
        DateTimeOffset modified,
        string? modifiedBy,
        string? keyId) =>
        UpdatePlainText(
            AccessToken.GetPlainText(accessToken),
            associatedData,
            encryptionKey,
            modified,
            modifiedBy,
            keyId);

    public AccessToken GetAccessToken(string associatedData, byte[] encryptionKey)
    {
        string plainText = GetPlainText(associatedData, encryptionKey);
        return JsonConvert.DeserializeObject<AccessToken>(plainText)!;
    }
}
