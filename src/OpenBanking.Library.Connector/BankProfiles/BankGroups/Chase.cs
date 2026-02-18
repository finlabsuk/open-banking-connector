// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum ChaseBank
{
    Chase
}

public class Chase() : BankGroupBase<ChaseBank, ChaseRegistrationGroup>(BankGroup.Chase)
{
    protected override ConcurrentDictionary<BankProfileEnum, ChaseBank> BankProfileToBank { get; } =
        new() { [BankProfileEnum.Chase_Chase] = ChaseBank.Chase };

    public override ChaseRegistrationGroup GetRegistrationGroup(
        ChaseBank bank,
        RegistrationScopeEnum registrationScopeEnum) => bank;
}
