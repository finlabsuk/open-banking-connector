// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

public class BankProfilesSettings : Dictionary<BankGroup, Dictionary<string, BankProfileHiddenProperties>>,
    ISettings<BankProfilesSettings>
{
    public string SettingsGroupName => "OpenBankingConnector:BankProfiles";

    public BankProfilesSettings Validate() => this;
}
