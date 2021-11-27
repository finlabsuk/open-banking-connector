// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    /// <summary>
    ///     UK Open Banking signing certificate type
    /// </summary>
    public enum SigningCertificateType
    {
        /// <summary>
        ///     Legacy certificate used by UK Open Banking Directory
        /// </summary>
        OBLegacy,

        /// <summary>
        ///     New OBSeal certificate used by UK Open Banking Directory.
        /// </summary>
        OBSeal
    }

    /// <summary>
    ///     Open Banking Signing Certificate Profile provided to Open Banking Connector as part of
    ///     <see cref="SigningCertificateProfilesSettings" />.
    ///     This class captures a signing certificate and associated data.
    /// </summary>
    public class SigningCertificateProfile
    {
        /// <summary>
        ///     Type of certificate used - see <see cref="SigningCertificateType" />
        /// </summary>
        public string CertificateType { get; set; } = string.Empty;

        /// Signing Key ID as string, e.g. "ABC"
        public string AssociatedKeyId { get; set; } = string.Empty;

        /// <summary>
        ///     Signing key (PKCS #8) as "stringified" PEM file with "PRIVATE KEY" label.
        ///     Example: "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        /// </summary>
        public string AssociatedKey { get; set; } = string.Empty;

        /// <summary>
        ///     Signing certificate (X.509) as "stringified" PEM file with "CERTIFICATE" label.
        ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        /// </summary>
        public string Certificate { get; set; } = string.Empty;
    }

    public class SigningCertificateProfilesSettings : Dictionary<string, SigningCertificateProfile>,
        ISettings<SigningCertificateProfilesSettings>
    {
        public string SettingsSectionName => "SigningCertificateProfiles";

        /// <summary>
        ///     Placeholder. Validation is performed only on individual <see cref="TransportCertificateProfile" /> entries
        ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
        ///     of <see cref="ProcessedSoftwareStatementProfileStore" />
        /// </summary>
        /// <returns></returns>
        public SigningCertificateProfilesSettings Validate()
        {
            return this;
        }
    }
}
