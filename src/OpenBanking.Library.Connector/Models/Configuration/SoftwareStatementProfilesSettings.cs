// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    public class SoftwareStatementProfile
    {
        /// Software statement as string, e.g. "A.B.C"
        public string SoftwareStatement { get; set; } = string.Empty;

        /// <summary>
        ///     ID of <see cref="TransportCertificateProfile" /> to use with this software statement profile
        /// </summary>
        public string TransportCertificateProfileId { get; set; } = string.Empty;

        /// <summary>
        ///     ID of <see cref="SigningCertificateProfile" /> to use with this software statement profile
        /// </summary>
        public string SigningCertificateProfileId { get; set; } = string.Empty;

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        public string DefaultFragmentRedirectUrl { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Software statement profile provided to Open Banking Connector as part of
    ///     <see cref="SoftwareStatementProfilesSettings" />.
    ///     This class captures a software statement and associated data such as keys and certificates.
    /// </summary>
    public class SoftwareStatementProfileWithOverrideProperties : SoftwareStatementProfile
    {
        /// <summary>
        ///     Bank-specific overrides for
        ///     <see cref="SoftwareStatementProfile.TransportCertificateProfileId" />
        /// </summary>
        public Dictionary<string, string> TransportCertificateProfileIdOverrides { get; set; } =
            new Dictionary<string, string>();

        /// <summary>
        ///     Bank-specific overrides for
        ///     <see cref="SoftwareStatementProfile.SigningCertificateProfileId" />
        /// </summary>
        public Dictionary<string, string> SigningCertificateProfileIdOverrides { get; set; } =
            new Dictionary<string, string>();

        /// <summary>
        ///     Returns profile with override substitution based on override case and override properties removed
        /// </summary>
        /// <param name="overrideCase"></param>
        /// <returns></returns>
        public SoftwareStatementProfile ApplyOverrides(string? overrideCase)
        {
            var newObject = new SoftwareStatementProfile
            {
                SoftwareStatement = SoftwareStatement,
                TransportCertificateProfileId = TransportCertificateProfileId,
                SigningCertificateProfileId = SigningCertificateProfileId,
                DefaultFragmentRedirectUrl = DefaultFragmentRedirectUrl
            };

            if (overrideCase is null)
            {
                return newObject;
            }

            if (TransportCertificateProfileIdOverrides.TryGetValue(
                    overrideCase,
                    out string transportCertificateProfileId))
            {
                newObject.TransportCertificateProfileId = transportCertificateProfileId;
            }

            if (SigningCertificateProfileIdOverrides.TryGetValue(
                    overrideCase,
                    out string signingCertificateProfileId))
            {
                newObject.SigningCertificateProfileId = signingCertificateProfileId;
            }

            return newObject;
        }
    }

    /// <summary>
    ///     Software statement profiles settings. This is a dictionary of <see cref="SoftwareStatementProfile" /> objects
    ///     keyed by ID to Open Banking Connector. It is expected this is provided by a collection of key secrets.
    /// </summary>
    public class SoftwareStatementProfilesSettings : Dictionary<string, SoftwareStatementProfileWithOverrideProperties>,
        ISettings<SoftwareStatementProfilesSettings>
    {
        public string SettingsGroupName => "SoftwareStatementProfiles";

        /// <summary>
        ///     Placeholder. Validation is performed only on individual <see cref="SoftwareStatementProfile" /> entries
        ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
        ///     of <see cref="ProcessedSoftwareStatementProfileStore" />
        /// </summary>
        /// <returns></returns>
        public SoftwareStatementProfilesSettings Validate()
        {
            return this;
        }
    }
}
