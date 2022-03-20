// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    /// <summary>
    ///     Whitelist of override cases for software statement (<see cref="SoftwareStatementProfile" />), transport certificate
    ///     (<see cref="TransportCertificateProfile" />) and signing certificate (<see cref="SigningCertificateProfile" />)
    ///     profiles to be extracted from key secrets. Each override case should be separated by spaces.
    ///     Override cases are keys in dictionary properties of those profiles with end in "Overrides". For example
    ///     <see cref="SoftwareStatementProfileWithOverrideProperties.TransportCertificateProfileIdOverrides" /> allows
    ///     override cases for
    ///     <see cref="SoftwareStatementProfile.TransportCertificateProfileId" />.
    ///     They are typically bank names allowing profiles to customised for a particular bank.
    ///     Only override cases listed here will be extracted from key secrets by Open Banking Connector.
    /// </summary>
    public class SoftwareStatementAndCertificateProfileOverridesSettings : List<string>,
        ISettings<SoftwareStatementAndCertificateProfileOverridesSettings>
    {
        public string SettingsGroupName => "OpenBankingConnector:SoftwareStatementAndCertificateProfileOverrideCases";

        public SoftwareStatementAndCertificateProfileOverridesSettings Validate()
        {
            // Note that all values have defaults
            return this;
        }
    }
}
