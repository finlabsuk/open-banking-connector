// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum StarlingBank
{
    Starling
}

public class Starling() : BankGroupBase<StarlingBank, StarlingRegistrationGroup>(BankGroup.Starling)
{
    protected override ConcurrentDictionary<BankProfileEnum, StarlingBank> BankProfileToBank { get; } =
        new() { [BankProfileEnum.Starling_Starling] = StarlingBank.Starling };

    public override StarlingRegistrationGroup GetRegistrationGroup(
        StarlingBank bank,
        RegistrationScopeEnum registrationScopeEnum) => bank;
}
