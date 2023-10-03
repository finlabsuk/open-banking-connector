// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

/// <summary>
///     Open Banking Transport Certificate Profile provided to Open Banking Connector as part of
///     <see cref="TransportCertificateProfilesSettings" />.
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

public class TransportCertificateProfileWithOverrideProperties : TransportCertificateProfile
{
    /// <summary>
    ///     Bank-specific overrides for
    ///     <see cref="TransportCertificateProfile.DisableTlsCertificateVerification" />
    /// </summary>
    public Dictionary<string, bool> DisableTlsCertificateVerificationOverrides { get; set; } =
        new();

    /// <summary>
    ///     Returns profile with override substitution based on override case and override properties removed
    /// </summary>
    /// <param name="overrideCase"></param>
    /// <returns></returns>
    public TransportCertificateProfile ApplyOverrides(string? overrideCase)
    {
        var newObject = new TransportCertificateProfile
        {
            DisableTlsCertificateVerification = DisableTlsCertificateVerification,
            AssociatedKey = AssociatedKey,
            Certificate = Certificate
        };

        if (overrideCase is null)
        {
            return newObject;
        }

        if (DisableTlsCertificateVerificationOverrides.TryGetValue(
                overrideCase,
                out bool disableTlsCertificateVerification))
        {
            newObject.DisableTlsCertificateVerification = disableTlsCertificateVerification;
        }


        return newObject;
    }
}

public class TransportCertificateProfilesSettings :
    Dictionary<string, TransportCertificateProfileWithOverrideProperties>,
    ISettings<TransportCertificateProfilesSettings>
{
    public string SettingsGroupName => "OpenBankingConnector:TransportCertificateProfiles";

    /// <summary>
    ///     Placeholder. Validation is performed only on individual <see cref="TransportCertificateProfile" /> entries
    ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
    ///     of <see cref="ProcessedSoftwareStatementProfileStore" />
    /// </summary>
    /// <returns></returns>
    public TransportCertificateProfilesSettings Validate()
    {
        foreach ((string key, TransportCertificateProfileWithOverrideProperties value) in this)
        {
            if (string.IsNullOrEmpty(value.AssociatedKey))
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    $"No non-empty AssociatedKey provided for TransportCertificateProfile {key}.");
            }

            if (string.IsNullOrEmpty(value.Certificate))
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    $"No non-empty Certificate provided for TransportCertificateProfile {key}.");
            }
        }

        return this;
    }
}
