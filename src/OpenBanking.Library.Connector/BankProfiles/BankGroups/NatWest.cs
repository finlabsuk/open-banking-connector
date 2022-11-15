// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum NatWestBank
    {
        NatWestSandbox,
        RoyalBankOfScotlandSandbox,
        NatWest,
        RoyalBankOfScotland,
        UlsterBankNI
    }

    public class NatWest : BankGroupBase<NatWestBank>
    {
        public NatWest(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

        protected override ConcurrentDictionary<BankProfileEnum, NatWestBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.NatWest_NatWestSandbox] = NatWestBank.NatWestSandbox,
                [BankProfileEnum.NatWest_RoyalBankOfScotlandSandbox] = NatWestBank.RoyalBankOfScotlandSandbox,
                [BankProfileEnum.NatWest_NatWest] = NatWestBank.NatWest,
                [BankProfileEnum.NatWest_RoyalBankOfScotland] = NatWestBank.RoyalBankOfScotland,
                [BankProfileEnum.NatWest_UlsterBankNI] = NatWestBank.UlsterBankNI
            };
    }
}
