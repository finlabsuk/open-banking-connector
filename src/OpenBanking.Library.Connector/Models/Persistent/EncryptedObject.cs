// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
[Index(nameof(Nonce), IsUnique = true)]
internal class EncryptedObject : BaseEntity
{
    protected EncryptedObject(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        string text,
        string tag,
        string? nonce) : base(id, reference, isDeleted, isDeletedModified, isDeletedModifiedBy, created, createdBy)
    {
        Text = text;
        Tag = tag;
        Nonce = nonce;
    }

    /// <summary>
    ///     Text
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Tag
    /// </summary>
    public string Tag { get; }

    /// <summary>
    ///     Encryption "nonce".
    /// </summary>
    public string? Nonce { get; }
}
