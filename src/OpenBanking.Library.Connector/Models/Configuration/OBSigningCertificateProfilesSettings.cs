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
    ///     <see cref="OBSigningCertificateProfilesSettings" />.
    ///     This class captures a signing certificate and associated data.
    /// </summary>
    public class OBSigningCertificateProfile
    {
        public OBSigningCertificateProfile(
            string certificateType,
            string signingKeyId,
            string signingKey,
            string signingCertificate)
        {
            CertificateType = certificateType;
            SigningKeyId = signingKeyId;
            SigningKey = signingKey;
            SigningCertificate = signingCertificate;
        }

        public OBSigningCertificateProfile() { }

        /// <summary>
        ///     Type of certificate used - see <see cref="SigningCertificateType" />
        /// </summary>
        public string CertificateType { get; set; } = null!;

        /// Signing Key ID as string, e.g. "ABC"
        public string SigningKeyId { get; set; } = null!;

        /// <summary>
        ///     Signing key (PKCS #8) as "stringified" PEM file with "PRIVATE KEY" label.
        ///     Example: "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        /// </summary>
        public string SigningKey { get; set; } = null!;

        /// <summary>
        ///     Signing certificate (X.509) as "stringified" PEM file with "CERTIFICATE" label.
        ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        /// </summary>
        public string SigningCertificate { get; set; } = null!;
    }

    public class OBSigningCertificateProfilesSettings : Dictionary<string, OBSigningCertificateProfile>,
        ISettings<OBSigningCertificateProfilesSettings>
    {
        public string SettingsSectionName => "OBSigningCertificateProfiles";

        /// <summary>
        ///     Placeholder. Validation is performed only on individual <see cref="OBTransportCertificateProfile" /> entries
        ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
        ///     of <see cref="ProcessedSoftwareStatementProfileStore" />
        /// </summary>
        /// <returns></returns>
        public OBSigningCertificateProfilesSettings Validate()
        {
            return this;
        }
    }
}
