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
public partial class ObWacCertificateEntity :
    BaseEntity,
    IObWacCertificatePublicQuery
{
    public ObWacCertificateEntity(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        SecretDescription associatedKey,
        string certificate) : base(id, reference, isDeleted, isDeletedModified, isDeletedModifiedBy, created, createdBy)
    {
        AssociatedKey = associatedKey ?? throw new ArgumentNullException(nameof(associatedKey));
        Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
    }

    /// <summary>
    ///     Associated key (PKCS #8) as "stringified" PEM file with escaped newline characters ("\n") and "PRIVATE KEY" label.
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    public SecretDescription AssociatedKey { get; }

    /// <summary>
    ///     OB WAC (transport) certificate (X.509) as "stringified" PEM file with escaped newline characters ("\n") and
    ///     "CERTIFICATE"
    ///     label.
    ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    /// </summary>
    public string Certificate { get; }
}

public partial class ObWacCertificateEntity :
    ISupportsFluentLocalEntityGet<ObWacCertificateResponse>
{
    public ObWacCertificateResponse PublicGetLocalResponse => new(
        Id,
        Created,
        CreatedBy,
        Reference,
        AssociatedKey,
        Certificate);
}
