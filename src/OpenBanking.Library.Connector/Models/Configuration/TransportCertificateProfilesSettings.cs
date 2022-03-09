// Licensed to Finnovation Labs Limited under one or more agreements.
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
    ///     <see cref="TransportCertificateProfilesSettings" />.
    ///     This class captures a transport certificate and associated data.
    /// </summary>
    public class TransportCertificateProfile
    {
        /// <summary>
        ///     Type of certificate used - see <see cref="TransportCertificateType" />
        /// </summary>
        public TransportCertificateType CertificateType { get; set; } = TransportCertificateType.OBWac;

        /// <summary>
        ///     Disable verification of external bank TLS certificates. Not for production use but
        ///     helpful when testing against sandboxes using self-signed certificates.
        /// </summary>
        public bool DisableTlsCertificateVerification { get; set; }

        /// <summary>
        ///     Transport certificate DN to use for DCR with hex values for dotted-decimal attributes (as specified by
        ///     https://datatracker.ietf.org/doc/html/rfc4514#section-2.4). Whether
        ///     <see cref="CertificateDnWithHexDottedDecimalAttributeValues" /> or
        ///     <see cref="CertificateDnWithStringDottedDecimalAttributeValues" /> is used is
        ///     determined by
        ///     <see
        ///         cref="Models.Public.Request.BankRegistration.UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues" />
        ///     . Neither is used for DCR with <see cref="TransportCertificateType.OBLegacy" /> certificates.
        /// </summary>
        public string CertificateDnWithHexDottedDecimalAttributeValues { get; set; } = string.Empty;

        /// <summary>
        ///     Alternative transport certificate DN to use for DCR with string (not hex) values for dotted-decimal attributes
        ///     (required by some banks). Whether <see cref="CertificateDnWithHexDottedDecimalAttributeValues" /> or
        ///     <see cref="CertificateDnWithStringDottedDecimalAttributeValues" /> is used is
        ///     determined by
        ///     <see
        ///         cref="Models.Public.Request.BankRegistration.UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues" />
        ///     . Neither is used for DCR with <see cref="TransportCertificateType.OBLegacy" /> certificates.
        /// </summary>
        public string CertificateDnWithStringDottedDecimalAttributeValues { get; set; } = string.Empty;

        /// <summary>
        ///     Transport key (PKCS #8) as "stringified" PEM file with "PRIVATE KEY" label.
        ///     Example: "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        /// </summary>
        public string AssociatedKey { get; set; } = string.Empty;

        /// <summary>
        ///     Transport certificate (X.509) as "stringified" PEM file with "CERTIFICATE" label.
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
            new Dictionary<string, bool>();

        /// <summary>
        ///     Bank-specific overrides for
        ///     <see cref="TransportCertificateProfile.CertificateDnWithHexDottedDecimalAttributeValues" />
        /// </summary>
        public Dictionary<string, string> CertificateDnWithHexDottedDecimalAttributeValuesOverrides { get; set; } =
            new Dictionary<string, string>();

        /// <summary>
        ///     Bank-specific overrides for
        ///     <see cref="TransportCertificateProfile.CertificateDnWithStringDottedDecimalAttributeValues" />
        /// </summary>
        public Dictionary<string, string> CertificateDnWithStringDottedDecimalAttributeValuesOverrides { get; set; } =
            new Dictionary<string, string>();

        /// <summary>
        ///     Returns profile with override substitution based on override case and override properties removed
        /// </summary>
        /// <param name="overrideCase"></param>
        /// <returns></returns>
        public TransportCertificateProfile ApplyOverrides(string? overrideCase)
        {
            var newObject = new TransportCertificateProfile
            {
                CertificateType = CertificateType,
                DisableTlsCertificateVerification = DisableTlsCertificateVerification,
                CertificateDnWithHexDottedDecimalAttributeValues = CertificateDnWithHexDottedDecimalAttributeValues,
                CertificateDnWithStringDottedDecimalAttributeValues =
                    CertificateDnWithStringDottedDecimalAttributeValues,
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

            if (CertificateDnWithHexDottedDecimalAttributeValuesOverrides.TryGetValue(
                    overrideCase,
                    out string certificateDnWithHexDottedDecimalAttributeValues))
            {
                newObject.CertificateDnWithHexDottedDecimalAttributeValues =
                    certificateDnWithHexDottedDecimalAttributeValues;
            }

            if (CertificateDnWithStringDottedDecimalAttributeValuesOverrides.TryGetValue(
                    overrideCase,
                    out string certificateDnWithStringDottedDecimalAttributeValues))
            {
                newObject.CertificateDnWithStringDottedDecimalAttributeValues =
                    certificateDnWithStringDottedDecimalAttributeValues;
            }

            return newObject;
        }
    }

    public class TransportCertificateProfilesSettings :
        Dictionary<string, TransportCertificateProfileWithOverrideProperties>,
        ISettings<TransportCertificateProfilesSettings>
    {
        public string SettingsGroupName => "TransportCertificateProfiles";

        /// <summary>
        ///     Placeholder. Validation is performed only on individual <see cref="TransportCertificateProfile" /> entries
        ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
        ///     of <see cref="ProcessedSoftwareStatementProfileStore" />
        /// </summary>
        /// <returns></returns>
        public TransportCertificateProfilesSettings Validate()
        {
            return this;
        }
    }
}
