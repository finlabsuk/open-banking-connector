// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

/// <summary>
///     Open Banking Signing Certificate Profile provided to Open Banking Connector.
///     This class captures a signing certificate and associated data.
/// </summary>
public class SigningCertificateProfile
{
    /// <summary>
    ///     Whether profile is active or inactive (ignored by Open Banking Connector). This allows profiles to be "switched on
    ///     and off" for testing etc.
    /// </summary>
    public bool Active { get; set; } = true;

    /// Signing Key ID (from UK Open Banking Directory) as string. This is not the same as the user-definied profile ID for this signing certificate profile.
    public string AssociatedKeyId { get; set; } = string.Empty;

    /// <summary>
    ///     Signing key (PKCS #8) provided as PEM file text (with "PRIVATE KEY" label).
    ///     Newlines in PEM file text should be replaced by "\n".
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    public string AssociatedKey { get; set; } = string.Empty;

    /// <summary>
    ///     Signing certificate (X.509) as "stringified" PEM file with escaped newline characters ("\n") and "CERTIFICATE"
    ///     label.
    ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    /// </summary>
    public string Certificate { get; set; } = string.Empty;
}
