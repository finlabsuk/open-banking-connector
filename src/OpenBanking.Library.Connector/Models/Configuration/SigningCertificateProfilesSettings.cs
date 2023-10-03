// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

/// <summary>
///     Open Banking Signing Certificate Profile provided to Open Banking Connector as part of
///     <see cref="SigningCertificateProfilesSettings" />.
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
}

public class SigningCertificateProfilesSettings : Dictionary<string, SigningCertificateProfile>,
    ISettings<SigningCertificateProfilesSettings>
{
    public string SettingsGroupName => "OpenBankingConnector:SigningCertificateProfiles";

    /// <summary>
    ///     Placeholder. Validation is performed only on individual <see cref="TransportCertificateProfile" /> entries
    ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
    ///     of <see cref="ProcessedSoftwareStatementProfileStore" />
    /// </summary>
    /// <returns></returns>
    public SigningCertificateProfilesSettings Validate()
    {
        foreach ((string key, SigningCertificateProfile value) in this)
        {
            if (string.IsNullOrEmpty(value.AssociatedKeyId))
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    $"No non-empty AssociatedKeyId provided for SigningCertificateProfile {key}.");
            }

            if (string.IsNullOrEmpty(value.AssociatedKey))
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    $"No non-empty AssociatedKey provided for SigningCertificateProfile {key}.");
            }
        }

        return this;
    }
}
