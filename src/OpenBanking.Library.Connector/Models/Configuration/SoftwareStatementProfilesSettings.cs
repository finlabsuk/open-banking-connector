// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    /// <summary>
    ///     Software statement profile provided to Open Banking Connector as part of
    ///     <see cref="SoftwareStatementProfilesSettings" />.
    ///     This class captures a software statement and associated data such as keys and certificates.
    /// </summary>
    public class SoftwareStatementProfile
    {
        public SoftwareStatementProfile(
            string softwareStatement,
            string obCertificateProfileId,
            string defaultFragmentRedirectUrl)
        {
            SoftwareStatement = softwareStatement;
            OBCertificateProfileId = obCertificateProfileId;
            DefaultFragmentRedirectUrl = defaultFragmentRedirectUrl;
        }

        public SoftwareStatementProfile() { }

        /// Software statement as string, e.g. "A.B.C"
        public string SoftwareStatement { get; set; } = null!;

        /// <summary>
        ///     ID of <see cref="OBCertificateProfile" /> to use with this software statement profile
        /// </summary>
        public string OBCertificateProfileId { get; set; } = null!;

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        public string DefaultFragmentRedirectUrl { get; set; } = null!;
    }

    /// <summary>
    ///     Software statement profiles settings. This is a dictionary of <see cref="SoftwareStatementProfile" /> objects
    ///     keyed by ID to Open Banking Connector. It is expected this is provided by a collection of key secrets.
    /// </summary>
    public class SoftwareStatementProfilesSettings : Dictionary<string, SoftwareStatementProfile>,
        ISettings<SoftwareStatementProfilesSettings>
    {
        public string SettingsSectionName => "SoftwareStatementProfiles";

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
