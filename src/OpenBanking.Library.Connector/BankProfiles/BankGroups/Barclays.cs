// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum BarclaysBank
    {
        Sandbox,
        Personal,
        Wealth,
        Barclaycard,
        Business,
        Corporate,
        BarclaycardCommercialPayments
    }

    public class Barclays : BankGroupBase<BarclaysBank>
    {
        public Barclays(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

        protected override ConcurrentDictionary<BankProfileEnum, BarclaysBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.Barclays_Sandbox] = BarclaysBank.Sandbox,
                [BankProfileEnum.Barclays_Personal] = BarclaysBank.Personal,
                [BankProfileEnum.Barclays_Wealth] = BarclaysBank.Wealth,
                [BankProfileEnum.Barclays_Barclaycard] = BarclaysBank.Barclaycard,
                [BankProfileEnum.Barclays_Business] = BarclaysBank.Business,
                [BankProfileEnum.Barclays_Corporate] = BarclaysBank.Corporate,
                [BankProfileEnum.Barclays_BarclaycardCommercialPayments] = BarclaysBank.BarclaycardCommercialPayments
            };
    }
}
