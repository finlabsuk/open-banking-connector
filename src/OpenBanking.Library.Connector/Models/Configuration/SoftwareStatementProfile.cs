// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            ObCertificateProfileId = obCertificateProfileId;
            DefaultFragmentRedirectUrl = defaultFragmentRedirectUrl;
        }

        public SoftwareStatementProfile() { }

        /// Software statement as string, e.g. "A.B.C"
        public string SoftwareStatement { get; set; } = null!;

        /// <summary>
        ///     ID of <see cref="ObCertificateProfile" /> to use with this software statement profile
        /// </summary>
        public string ObCertificateProfileId { get; set; } = null!;

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        public string DefaultFragmentRedirectUrl { get; set; } = null!;
    }
}
