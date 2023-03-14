// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

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
    UlsterBankNiClearSpend
}

public class NatWest : BankGroupBase<NatWestBank>
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
            [BankProfileEnum.NatWest_UlsterBankNiClearSpend] = NatWestBank.UlsterBankNiClearSpend
        };
}
