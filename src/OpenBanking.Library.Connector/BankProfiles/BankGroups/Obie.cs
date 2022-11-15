// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum ObieBank
    {
        Modelo
    }

    public class Obie : BankGroupBase<ObieBank>
    {
        public Obie(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

        protected override ConcurrentDictionary<BankProfileEnum, ObieBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.Obie_Modelo] = ObieBank.Modelo
            };
    }
}
