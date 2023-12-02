// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum MonzoBank
{
    Sandbox,
    Monzo
}

public enum MonzoRegistrationGroup
{
    Sandbox_Aisp,
    Sandbox_Pisp,
    Production_Aisp,
    Production_Pisp
}

public class Monzo : BankGroupBase<MonzoBank, MonzoRegistrationGroup>
{
    public Monzo(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, MonzoBank> BankProfileToBank { get; } =
        new()
        {
            [BankProfileEnum.Monzo_Sandbox] = MonzoBank.Sandbox,
            [BankProfileEnum.Monzo_Monzo] = MonzoBank.Monzo
        };

    public override MonzoRegistrationGroup? GetRegistrationGroup(
        MonzoBank bank,
        RegistrationScopeEnum registrationScopeEnum) =>
        (bank, registrationScopeEnum) switch
        {
            (MonzoBank.Sandbox, RegistrationScopeEnum.AccountAndTransaction) => MonzoRegistrationGroup.Sandbox_Aisp,
            (MonzoBank.Sandbox, RegistrationScopeEnum.PaymentInitiation) => MonzoRegistrationGroup.Sandbox_Pisp,
            (MonzoBank.Monzo, RegistrationScopeEnum.AccountAndTransaction) => MonzoRegistrationGroup.Production_Aisp,
            (MonzoBank.Monzo, RegistrationScopeEnum.PaymentInitiation) => MonzoRegistrationGroup.Production_Pisp,
            _ => throw new ArgumentOutOfRangeException()
        };
}
