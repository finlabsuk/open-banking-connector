// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum CooperativeBank
{
    Cooperative,
    CooperativeSandbox,
    Smile
}

public enum CooperativeRegistrationGroup
{
    Sandbox,
    Production
}

public class Cooperative : BankGroupBase<CooperativeBank, CooperativeRegistrationGroup>
{
    public Cooperative(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, CooperativeBank> BankProfileToBank { get; } =
        new()
        {
            [BankProfileEnum.Cooperative_Cooperative] = CooperativeBank.Cooperative,
            [BankProfileEnum.Cooperative_CooperativeSandbox] = CooperativeBank.CooperativeSandbox,
            [BankProfileEnum.Cooperative_Smile] = CooperativeBank.Smile
        };

    public override CooperativeRegistrationGroup GetRegistrationGroup(
        CooperativeBank bank,
        RegistrationScopeEnum registrationScopeEnum) =>
        bank switch
        {
            CooperativeBank.Cooperative => CooperativeRegistrationGroup.Production,
            CooperativeBank.CooperativeSandbox => CooperativeRegistrationGroup.Sandbox,
            CooperativeBank.Smile => CooperativeRegistrationGroup.Production,
            _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
        };
}
