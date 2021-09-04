// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration
{
    public class ObCertificateProfilesSettings : Dictionary<string, ObCertificateProfile>,
        ISettings<ObCertificateProfilesSettings>
    {
        public string SettingsSectionName => "ObCertificateProfiles";

        /// <summary>
        ///     Placeholder. Validation is performed only on individual <see cref="ObCertificateProfile" /> entries
        ///     that are to be used by Open Banking Connector. This validation is performed in the constructor
        ///     of <see cref="SoftwareStatementProfileCache" />
        /// </summary>
        /// <returns></returns>
        public ObCertificateProfilesSettings Validate()
        {
            return this;
        }
    }
}
