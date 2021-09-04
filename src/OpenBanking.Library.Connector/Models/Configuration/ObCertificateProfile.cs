// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public class ObCertificateProfile
    {
        public ObCertificateProfile(
            string certificateType,
            string signingKeyId,
            string signingKey,
            string signingCertificate,
            string transportKey,
            string transportCertificate)
        {
            CertificateType = certificateType;
            SigningKeyId = signingKeyId;
            SigningKey = signingKey;
            SigningCertificate = signingCertificate;
            TransportKey = transportKey;
            TransportCertificate = transportCertificate;
        }

        public ObCertificateProfile() { }

        /// <summary>
        ///     Type of certificate used for transport and signing certificates
        /// </summary>
        public string CertificateType { get; set; } = null!;

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
}
