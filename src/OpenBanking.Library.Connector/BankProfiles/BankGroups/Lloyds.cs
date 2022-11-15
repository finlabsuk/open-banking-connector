// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum LloydsBank
    {
        Lloyds
    }

    public class Lloyds : BankGroupBase<LloydsBank>
    {
        public Lloyds(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

        protected override ConcurrentDictionary<BankProfileEnum, LloydsBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.Lloyds] = LloydsBank.Lloyds
            };
    }
}
