// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum ObieBank
{
    Modelo,
    Model2023
}

public class Obie : BankGroupBase<ObieBank, ObieRegistrationGroup>
{
    public Obie(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, ObieBank> BankProfileToBank { get; } =
        new()
        {
            [BankProfileEnum.Obie_Modelo] = ObieBank.Modelo,
            [BankProfileEnum.Obie_Model2023] = ObieBank.Model2023
        };

    public override ObieRegistrationGroup? GetRegistrationGroup(
        ObieBank bank,
        RegistrationScopeEnum registrationScopeEnum) => bank;
}
