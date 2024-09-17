// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

/// <summary>
///     Open Banking Transport Certificate Profile provided to Open Banking Connector.
///     This class captures a transport certificate and associated data.
/// </summary>
public class TransportCertificateProfile
{
    /// <summary>
    ///     Whether profile is active or inactive (ignored by Open Banking Connector). This allows profiles to be "switched on
    ///     and off" for testing etc.
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    ///     Disable verification of external bank TLS certificates when using mutual TLS with this certificate profile. Not
    ///     intended for
    ///     production use but
    ///     sometimes helpful for diagnosing issues with bank sandboxes (e.g. if they use self-signed certificates).
    /// </summary>
    public bool DisableTlsCertificateVerification { get; set; }

    /// <summary>
    ///     Transport key (PKCS #8) as "stringified" PEM file with escaped newline characters ("\n") and "PRIVATE KEY" label.
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    public string AssociatedKey { get; set; } = string.Empty;

    /// <summary>
    ///     Transport certificate (X.509) as "stringified" PEM file with escaped newline characters ("\n") and "CERTIFICATE"
    ///     label.
    ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    /// </summary>
    public string Certificate { get; set; } = string.Empty;
}
