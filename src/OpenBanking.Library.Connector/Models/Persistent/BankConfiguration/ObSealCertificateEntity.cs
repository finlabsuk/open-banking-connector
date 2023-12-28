// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
public partial class ObSealCertificateEntity :
    BaseEntity,
    IObSealCertificatePublicQuery
{
    public ObSealCertificateEntity(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        string associatedKeyId,
        SecretDescription associatedKey,
        string certificate) : base(id, reference, isDeleted, isDeletedModified, isDeletedModifiedBy, created, createdBy)
    {
        AssociatedKeyId = associatedKeyId ?? throw new ArgumentNullException(nameof(associatedKeyId));
        AssociatedKey = associatedKey ?? throw new ArgumentNullException(nameof(associatedKey));
        Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
    }

    /// <summary>
    ///     Key ID of associated key (from UK Open Banking Directory) as string.
    /// </summary>
    public string AssociatedKeyId { get; }

    /// <summary>
    ///     Associated key (PKCS #8) provided as PEM file text (with "PRIVATE KEY" label).
    ///     Newlines in PEM file text should be replaced by "\n".
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    public SecretDescription AssociatedKey { get; }

    /// <summary>
    ///     OB Seal (signing) certificate (X.509) as "stringified" PEM file with escaped newline characters ("\n") and
    ///     "CERTIFICATE"
    ///     label.
    ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    /// </summary>
    public string Certificate { get; }
}

public partial class ObSealCertificateEntity :
    ISupportsFluentLocalEntityGet<ObSealCertificateResponse>
{
    public ObSealCertificateResponse PublicGetLocalResponse => new()
    {
        Id = Id,
        Created = Created,
        CreatedBy = CreatedBy,
        Reference = Reference,
        AssociatedKeyId = AssociatedKeyId,
        AssociatedKey = AssociatedKey,
        Certificate = Certificate
    };
}
