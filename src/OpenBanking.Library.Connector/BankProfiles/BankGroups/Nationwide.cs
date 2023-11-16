// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum NationwideBank
{
    Nationwide
}

public class Nationwide : BankGroupBase<NationwideBank>
{
    public Nationwide(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, NationwideBank> BankProfileToBank { get; } =
        new() { [BankProfileEnum.Nationwide_Nationwide] = NationwideBank.Nationwide };
}
