// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum SantanderBank
{
    Santander
}

public class Santander : BankGroupBase<SantanderBank, SantanderRegistrationGroup>
{
    public Santander(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, SantanderBank> BankProfileToBank { get; } =
        new() { [BankProfileEnum.Santander_Santander] = SantanderBank.Santander };

    public override SantanderRegistrationGroup? GetRegistrationGroup(
        SantanderBank bank,
        RegistrationScopeEnum registrationScopeEnum) => bank;
}
