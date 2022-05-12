// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
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
        ///     Whether profile is active or inactive (ignored by Open Banking Connector). This allows profiles to be "switched on
        ///     and off" for testing etc.
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        ///     Type of UK Open Banking Directory certificate used - see <see cref="TransportCertificateType" />
        /// </summary>
        public TransportCertificateType CertificateType { get; set; } = TransportCertificateType.OBWac;

        /// <summary>
        ///     Disable verification of external bank TLS certificates when using mutual TLS with this certificate profile. Not
        ///     intended for
        ///     production use but
        ///     sometimes helpful for diagnosing issues with bank sandboxes (e.g. if they use self-signed certificates).
        /// </summary>
        public bool DisableTlsCertificateVerification { get; set; }

        /// <summary>
        ///     Transport certificate DN to use for bank registration (DCR) with hex values for dotted-decimal attributes (as
        ///     specified by
        ///     https://datatracker.ietf.org/doc/html/rfc4514#section-2.4). Whether
        ///     <see cref="CertificateDnWithHexDottedDecimalAttributeValues" /> or
        ///     <see cref="CertificateDnWithStringDottedDecimalAttributeValues" /> is used in bank registration is
        ///     determined by bank registration request property
        ///     <see
        ///         cref="CustomBehaviour.UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues" />
        ///     . This setting is ignored when using <see cref="TransportCertificateType.OBLegacy" /> certificates.
        /// </summary>
        public string CertificateDnWithHexDottedDecimalAttributeValues { get; set; } = string.Empty;

        /// <summary>
        ///     Alternative transport certificate DN to use for bank registration (DCR) with string (not hex) values for
        ///     dotted-decimal attributes
        ///     (required by some banks). Whether <see cref="CertificateDnWithHexDottedDecimalAttributeValues" /> or
        ///     <see cref="CertificateDnWithStringDottedDecimalAttributeValues" /> is used in bank registration is
        ///     determined by bank registration request property
        ///     <see
        ///         cref="CustomBehaviour.UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues" />
        ///     . This setting is ignored when using <see cref="TransportCertificateType.OBLegacy" /> certificates.
        /// </summary>
        public string CertificateDnWithStringDottedDecimalAttributeValues { get; set; } = string.Empty;

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
        ///     Bank-specific overrides for
        ///     <see cref="TransportCertificateProfile.CertificateDnWithHexDottedDecimalAttributeValues" />
        /// </summary>
        public Dictionary<string, string> CertificateDnWithHexDottedDecimalAttributeValuesOverrides { get; set; } =
            new();

        /// <summary>
        ///     Bank-specific overrides for
        ///     <see cref="TransportCertificateProfile.CertificateDnWithStringDottedDecimalAttributeValues" />
        /// </summary>
        public Dictionary<string, string> CertificateDnWithStringDottedDecimalAttributeValuesOverrides { get; set; } =
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
                    out string? certificateDnWithHexDottedDecimalAttributeValues))
            {
                newObject.CertificateDnWithHexDottedDecimalAttributeValues =
                    certificateDnWithHexDottedDecimalAttributeValues;
            }

            if (CertificateDnWithStringDottedDecimalAttributeValuesOverrides.TryGetValue(
                    overrideCase,
                    out string? certificateDnWithStringDottedDecimalAttributeValues))
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
}
