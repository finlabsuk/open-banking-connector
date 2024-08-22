// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum NatWestBank
{
    NatWestSandbox,
    NatWest,
    NatWestBankline,
    NatWestClearSpend,
    RoyalBankOfScotlandSandbox,
    RoyalBankOfScotland,
    RoyalBankOfScotlandBankline,
    RoyalBankOfScotlandClearSpend,
    TheOne,
    NatWestOne,
    VirginOne,
    UlsterBankNi,
    UlsterBankNiBankline,
    UlsterBankNiClearSpend,
    Mettle,
    Coutts
}

public enum NatWestRegistrationGroup
{
    NatWestSandbox,
    NatWestProduction,
    RoyalBankOfScotlandSandbox,
    RoyalBankOfScotlandProduction,
    UlsterBankNiProduction,
    MettleProduction,
    CouttsProduction
}

public class NatWest : BankGroupBase<NatWestBank, NatWestRegistrationGroup>
{
    public NatWest(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, NatWestBank> BankProfileToBank { get; } =
        new()
        {
            [BankProfileEnum.NatWest_NatWestSandbox] = NatWestBank.NatWestSandbox,
            [BankProfileEnum.NatWest_NatWest] = NatWestBank.NatWest,
            [BankProfileEnum.NatWest_NatWestBankline] = NatWestBank.NatWestBankline,
            [BankProfileEnum.NatWest_NatWestClearSpend] = NatWestBank.NatWestClearSpend,
            [BankProfileEnum.NatWest_RoyalBankOfScotlandSandbox] = NatWestBank.RoyalBankOfScotlandSandbox,
            [BankProfileEnum.NatWest_RoyalBankOfScotland] = NatWestBank.RoyalBankOfScotland,
            [BankProfileEnum.NatWest_RoyalBankOfScotlandBankline] = NatWestBank.RoyalBankOfScotlandBankline,
            [BankProfileEnum.NatWest_RoyalBankOfScotlandClearSpend] = NatWestBank.RoyalBankOfScotlandClearSpend,
            [BankProfileEnum.NatWest_TheOne] = NatWestBank.TheOne,
            [BankProfileEnum.NatWest_NatWestOne] = NatWestBank.NatWestOne,
            [BankProfileEnum.NatWest_VirginOne] = NatWestBank.VirginOne,
            [BankProfileEnum.NatWest_UlsterBankNi] = NatWestBank.UlsterBankNi,
            [BankProfileEnum.NatWest_UlsterBankNiBankline] = NatWestBank.UlsterBankNiBankline,
            [BankProfileEnum.NatWest_UlsterBankNiClearSpend] = NatWestBank.UlsterBankNiClearSpend,
            [BankProfileEnum.NatWest_Mettle] = NatWestBank.Mettle,
            [BankProfileEnum.NatWest_Coutts] = NatWestBank.Coutts
        };

    public override NatWestRegistrationGroup GetRegistrationGroup(
        NatWestBank bank,
        RegistrationScopeEnum registrationScopeEnum) => bank switch
    {
        NatWestBank.NatWestSandbox => NatWestRegistrationGroup.NatWestSandbox,
        NatWestBank.NatWest
            or NatWestBank.NatWestBankline
            or NatWestBank.NatWestClearSpend => NatWestRegistrationGroup.NatWestProduction,
        NatWestBank.RoyalBankOfScotlandSandbox => NatWestRegistrationGroup.RoyalBankOfScotlandSandbox,
        NatWestBank.RoyalBankOfScotland
            or NatWestBank.RoyalBankOfScotlandBankline
            or NatWestBank.RoyalBankOfScotlandClearSpend
            or NatWestBank.TheOne
            or NatWestBank.NatWestOne
            or NatWestBank.VirginOne => NatWestRegistrationGroup.RoyalBankOfScotlandProduction,
        NatWestBank.UlsterBankNi
            or NatWestBank.UlsterBankNiBankline
            or NatWestBank.UlsterBankNiClearSpend => NatWestRegistrationGroup.UlsterBankNiProduction,
        NatWestBank.Mettle => NatWestRegistrationGroup.MettleProduction,
        NatWestBank.Coutts => NatWestRegistrationGroup.CouttsProduction,
        _ => throw new ArgumentOutOfRangeException()
    };
}
