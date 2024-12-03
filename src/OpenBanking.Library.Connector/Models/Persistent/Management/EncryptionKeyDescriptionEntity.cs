// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
[Index(nameof(LegacyName), IsUnique = true)]
public partial class EncryptionKeyDescriptionEntity :
    BaseEntity,
    IEncryptionKeyDescriptionPublicQuery
{
    public EncryptionKeyDescriptionEntity(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        SecretDescription key,
        string? legacyName) : base(id, reference, isDeleted, isDeletedModified, isDeletedModifiedBy, created, createdBy)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        LegacyName = legacyName;
    }

    public string? LegacyName { get; }

    /// <summary>
    ///     Encryption key (PKCS #8) provided as PEM file text (with "PRIVATE KEY" label).
    ///     Newlines in PEM file text should be replaced by "\n".
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    public SecretDescription Key { get; }
}

public partial class EncryptionKeyDescriptionEntity :
    ISupportsFluentLocalEntityGet<EncryptionKeyDescriptionResponse>
{
    public EncryptionKeyDescriptionResponse PublicGetLocalResponse => new()
    {
        Id = Id,
        Created = Created,
        CreatedBy = CreatedBy,
        Reference = Reference,
        Key = Key
    };
}
