// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;

public interface IObSealCertificatePublicQuery : IEntityBaseQuery
{
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

/// <summary>
///     Response to ObSealCertificate read and create requests.
/// </summary>
public class ObSealCertificateResponse : EntityBaseResponse, IObSealCertificatePublicQuery
{
    /// <summary>
    ///     Key ID of associated key (from UK Open Banking Directory) as string.
    /// </summary>
    public required string AssociatedKeyId { get; init; }

    /// <summary>
    ///     Associated key (PKCS #8) provided as PEM file text (with "PRIVATE KEY" label).
    ///     Newlines in PEM file text should be replaced by "\n".
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    public required SecretDescription AssociatedKey { get; init; }

    /// <summary>
    ///     OB Seal (signing) certificate (X.509) as "stringified" PEM file with escaped newline characters ("\n") and
    ///     "CERTIFICATE"
    ///     label.
    ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    /// </summary>
    public required string Certificate { get; init; }
}
