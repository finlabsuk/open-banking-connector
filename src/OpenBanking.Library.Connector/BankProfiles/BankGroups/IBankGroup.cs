// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public interface IBankGroup
{
    ReadOnlyDictionary<BankProfileEnum, string> BankProfileToBankName { get; }
    string GetBankString(BankProfileEnum bankProfile);

    BankProfile GetBankProfile(
        BankProfileEnum bankProfileEnum,
        HiddenPropertiesDictionary hiddenPropertiesDictionary);
}
