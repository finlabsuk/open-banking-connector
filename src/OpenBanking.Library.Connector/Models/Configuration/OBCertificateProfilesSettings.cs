// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    /// <summary>
    ///     UK Open Banking certificate type
    /// </summary>
    public enum CertificateType
    {
        /// <summary>
        ///     Legacy certificates used by UK Open Banking Directory
        /// </summary>
        LegacyOB,

        /// <summary>
        ///     New OBWAC and OBSeal certificates used by UK Open Banking Directory.
        /// </summary>
        OBWacAndOBSeal
    }

    /// <summary>
    ///     Software statement profile provided to Open Banking Connector as part of
    ///     <see cref="SoftwareStatementProfilesSettings" />.
    ///     This class captures a software statement and associated data such as keys and certificates.
    /// </summary>
    public class OBCertificateProfile
    {
        public OBCertificateProfile(
            string certificateType,
            string signingKeyId,
            string signingKey,
            string signingCertificate,
            string transportKey,
            string transportCertificate,
            bool disableTlsCertificateVerification)
        {
            CertificateType = certificateType;
            SigningKeyId = signingKeyId;
            SigningKey = signingKey;
            SigningCertificate = signingCertificate;
            TransportKey = transportKey;
            TransportCertificate = transportCertificate;
            DisableTlsCertificateVerification = disableTlsCertificateVerification;
        }

        public OBCertificateProfile() { }

        /// <summary>
        ///     Type of certificate used for transport and signing certificates
        /// </summary>
        public string CertificateType { get; set; } = null!;

        /// <summary>
        ///     Disable verification of external bank TLS certificates. Not for production use but
        ///     helpful when testing against sandboxes using self-signed certificates.
        /// </summary>
        public bool DisableTlsCertificateVerification { get; set; }

        /// Open Banking Signing Key ID as string, e.g. "ABC"
        public string SigningKeyId { get; set; } = null!;

        /// Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        public string SigningKey { get; set; } = null!;

        /// Open Banking Signing Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string SigningCertificate { get; set; } = null!;

        /// Open Banking Transport Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        public string TransportKey { get; set; } = null!;

        /// Open Banking Transport Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string TransportCertificate { get; set; } = null!;
    }

    public class OBCertificateProfilesSettings : Dictionary<string, OBCertificateProfile>,
        ISettings<OBCertificateProfilesSettings>
    {
        public string SettingsSectionName => "ObCertificateProfiles";

        /// <summary>
        ///     Placeholder. Validation is performed only on individual <see cref="OBCertificateProfile" /> entries
        ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
        ///     of <see cref="ProcessedSoftwareStatementProfileStore" />
        /// </summary>
        /// <returns></returns>
        public OBCertificateProfilesSettings Validate()
        {
            return this;
        }
    }
}
