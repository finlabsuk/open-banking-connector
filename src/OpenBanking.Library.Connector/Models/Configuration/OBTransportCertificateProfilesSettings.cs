﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    /// <summary>
    ///     UK Open Banking transport certificate type
    /// </summary>
    public enum TransportCertificateType
    {
        /// <summary>
        ///     Legacy certificate used by UK Open Banking Directory
        /// </summary>
        OBLegacy,

        /// <summary>
        ///     New OBWAC certificate used by UK Open Banking Directory.
        /// </summary>
        OBWac
    }

    /// <summary>
    ///     Open Banking Transport Certificate Profile provided to Open Banking Connector as part of
    ///     <see cref="OBTransportCertificateProfilesSettings" />.
    ///     This class captures a transport certificate and associated data.
    /// </summary>
    public class OBTransportCertificateProfile
    {
        public OBTransportCertificateProfile(
            string certificateType,
            string transportKey,
            string transportCertificate,
            bool disableTlsCertificateVerification,
            string certificateDnOrgName,
            string certificateDnOrgId,
            string certificateDnOrgIdHexEncoded)
        {
            CertificateType = certificateType;
            TransportKey = transportKey;
            TransportCertificate = transportCertificate;
            DisableTlsCertificateVerification = disableTlsCertificateVerification;
            CertificateDnOrgName = certificateDnOrgName;
            CertificateDnOrgId = certificateDnOrgId;
            CertificateDnOrgIdHexEncoded = certificateDnOrgIdHexEncoded;
        }

        public OBTransportCertificateProfile() { }

        /// <summary>
        ///     Type of certificate used - see <see cref="TransportCertificateType" />
        /// </summary>
        public string CertificateType { get; set; } = null!;

        /// <summary>
        ///     Disable verification of external bank TLS certificates. Not for production use but
        ///     helpful when testing against sandboxes using self-signed certificates.
        /// </summary>
        public bool DisableTlsCertificateVerification { get; set; }

        /// <summary>
        ///     Org name to use when constructing DN for DCR (ignored when using <see cref="TransportCertificateType.OBLegacy" />
        ///     cert)
        /// </summary>
        public string CertificateDnOrgName { get; set; } = null!;

        /// <summary>
        ///     Org ID to use when constructing DN for DCR (ignored when using <see cref="TransportCertificateType.OBLegacy" />
        ///     cert). Whether <see cref="CertificateDnOrgId" /> or <see cref="CertificateDnOrgIdHexEncoded" /> is used is
        ///     determined by <see cref="Models.Public.Request.BankRegistration.UseHexEncodedTransportCertificateDnOrgId" />
        /// </summary>
        public string CertificateDnOrgId { get; set; } = null!;

        /// <summary>
        ///     Hex-encoded Org ID to use when constructing DN for DCR (ignored when using
        ///     <see cref="TransportCertificateType.OBLegacy" /> cert). Whether <see cref="CertificateDnOrgId" /> or
        ///     <see cref="CertificateDnOrgIdHexEncoded" /> is used is
        ///     determined by <see cref="Models.Public.Request.BankRegistration.UseHexEncodedTransportCertificateDnOrgId" />
        /// </summary>
        public string CertificateDnOrgIdHexEncoded { get; set; } = null!;

        /// <summary>
        ///     Transport key (PKCS #8) as "stringified" PEM file with "PRIVATE KEY" label.
        ///     Example: "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        /// </summary>
        public string TransportKey { get; set; } = null!;

        /// <summary>
        ///     Transport certificate (X.509) as "stringified" PEM file with "CERTIFICATE" label.
        ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        /// </summary>
        public string TransportCertificate { get; set; } = null!;
    }

    public class OBTransportCertificateProfilesSettings : Dictionary<string, OBTransportCertificateProfile>,
        ISettings<OBTransportCertificateProfilesSettings>
    {
        public string SettingsSectionName => "ObTransportCertificateProfiles";

        /// <summary>
        ///     Placeholder. Validation is performed only on individual <see cref="OBTransportCertificateProfile" /> entries
        ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
        ///     of <see cref="ProcessedSoftwareStatementProfileStore" />
        /// </summary>
        /// <returns></returns>
        public OBTransportCertificateProfilesSettings Validate()
        {
            return this;
        }
    }
}
