// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum TsbBank
{
    Tsb
}

public class Tsb() : BankGroupBase<TsbBank, TsbRegistrationGroup>(BankGroup.Tsb)
{
    protected override ConcurrentDictionary<BankProfileEnum, TsbBank> BankProfileToBank { get; } =
        new() { [BankProfileEnum.Tsb_Tsb] = TsbBank.Tsb };

    public override TsbRegistrationGroup GetRegistrationGroup(
        TsbBank bank,
        RegistrationScopeEnum registrationScopeEnum) => bank;
}
