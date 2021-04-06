// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    /// <summary>
    ///     UK Open Banking certificate type
    /// </summary>
    public enum CertificateType
    {
        /// <summary>
        ///     Used for mock software statement profile only
        /// </summary>
        None,

        /// <summary>
        ///     Legacy certificates used by UK Open Banking Directory
        /// </summary>
        LegacyOB,

        /// <summary>
        ///     New OBWAC and OBSeal certificates used by UK Open Banking Directory.
        ///     For future use.
        /// </summary>
        OBWacAndOBSeal
    }

    /// <summary>
    ///     Software statement profile provided to Open Banking Connector as part of
    ///     <see cref="SoftwareStatementProfilesSettings" />.
    ///     This class captures a software statement and associated data such as keys and certificates.
    /// </summary>
    public class SoftwareStatementProfile
    {
        public SoftwareStatementProfile(
            string softwareStatement,
            string certificateType,
            string signingKeyId,
            string signingKey,
            string signingCertificate,
            string transportKey,
            string transportCertificate,
            string defaultFragmentRedirectUrl)
        {
            SoftwareStatement = softwareStatement ?? throw new ArgumentNullException(nameof(softwareStatement));
            SigningKeyId = signingKeyId ?? throw new ArgumentNullException(nameof(signingKeyId));
            SigningKey = signingKey ?? throw new ArgumentNullException(nameof(signingKey));
            SigningCertificate = signingCertificate ?? throw new ArgumentNullException(nameof(signingCertificate));
            TransportKey = transportKey ?? throw new ArgumentNullException(nameof(transportKey));
            TransportCertificate =
                transportCertificate ?? throw new ArgumentNullException(nameof(transportCertificate));
            DefaultFragmentRedirectUrl = defaultFragmentRedirectUrl ??
                                         throw new ArgumentNullException(nameof(defaultFragmentRedirectUrl));
            CertificateType = certificateType;
        }

        public SoftwareStatementProfile() { }

        /// Software statement as string, e.g. "A.B.C"
        public string SoftwareStatement { get; set; } = null!;

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

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        public string DefaultFragmentRedirectUrl { get; set; } = null!;
    }
}
