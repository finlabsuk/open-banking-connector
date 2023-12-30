// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum RevolutBank
{
    Revolut
}

public class Revolut : BankGroupBase<RevolutBank, RevolutRegistrationGroup>
{
    public Revolut(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, RevolutBank> BankProfileToBank { get; } =
        new() { [BankProfileEnum.Revolut_Revolut] = RevolutBank.Revolut };

    public override RevolutRegistrationGroup? GetRegistrationGroup(
        RevolutBank bank,
        RegistrationScopeEnum registrationScopeEnum) => bank;
}
