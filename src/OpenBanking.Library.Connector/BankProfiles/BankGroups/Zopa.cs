// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum ZopaBank
{
    Zopa
}

public class Zopa() : BankGroupBase<ZopaBank, ZopaRegistrationGroup>(BankGroup.Zopa)
{
    protected override ConcurrentDictionary<BankProfileEnum, ZopaBank> BankProfileToBank { get; } =
        new() { [BankProfileEnum.Zopa_Zopa] = ZopaBank.Zopa };

    public override ZopaRegistrationGroup GetRegistrationGroup(
        ZopaBank bank,
        RegistrationScopeEnum registrationScopeEnum) => bank;
}
