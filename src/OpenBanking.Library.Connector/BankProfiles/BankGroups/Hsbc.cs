// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum HsbcBank
{
    FirstDirect,
    Sandbox,
    UkBusiness,
    UkKinetic,
    UkPersonal,
    HsbcNetUk
}

public class Hsbc : BankGroupBase<HsbcBank>
{
    public Hsbc(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, HsbcBank> BankProfileToBank { get; } =
        new()
        {
            [BankProfileEnum.Hsbc_FirstDirect] = HsbcBank.FirstDirect,
            [BankProfileEnum.Hsbc_Sandbox] = HsbcBank.Sandbox,
            [BankProfileEnum.Hsbc_UkBusiness] = HsbcBank.UkBusiness,
            [BankProfileEnum.Hsbc_UkKinetic] = HsbcBank.UkKinetic,
            [BankProfileEnum.Hsbc_UkPersonal] = HsbcBank.UkPersonal,
            [BankProfileEnum.Hsbc_HsbcNetUk] = HsbcBank.HsbcNetUk
        };
}
