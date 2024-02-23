// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum DanskeBank
{
    Sandbox
}

public class Danske : BankGroupBase<DanskeBank, DanskeRegistrationGroup>
{
    public Danske(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, DanskeBank> BankProfileToBank { get; } =
        new() { [BankProfileEnum.Danske_Sandbox] = DanskeBank.Sandbox };

    public override DanskeRegistrationGroup? GetRegistrationGroup(
        DanskeBank bank,
        RegistrationScopeEnum registrationScopeEnum) => bank;
}
