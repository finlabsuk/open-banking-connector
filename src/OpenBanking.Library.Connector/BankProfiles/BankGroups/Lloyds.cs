﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public enum LloydsBank
{
    Sandbox,
    LloydsPersonal,
    LloydsBusiness,
    LloydsCommerical,
    HalifaxPersonal,
    BankOfScotlandPersonal,
    BankOfScotlandBusiness,
    BankOfScotlandCommerical,
    MbnaPersonal
}

public class Lloyds : BankGroupBase<LloydsBank>
{
    public Lloyds(BankGroupEnum bankGroupEnum) : base(bankGroupEnum) { }

    protected override ConcurrentDictionary<BankProfileEnum, LloydsBank> BankProfileToBank { get; } =
        new()
        {
            [BankProfileEnum.Lloyds_Sandbox] = LloydsBank.Sandbox,
            [BankProfileEnum.Lloyds_LloydsPersonal] = LloydsBank.LloydsPersonal,
            [BankProfileEnum.Lloyds_LloydsBusiness] = LloydsBank.LloydsBusiness,
            [BankProfileEnum.Lloyds_LloydsCommerical] = LloydsBank.LloydsCommerical,
            [BankProfileEnum.Lloyds_HalifaxPersonal] = LloydsBank.HalifaxPersonal,
            [BankProfileEnum.Lloyds_BankOfScotlandPersonal] = LloydsBank.BankOfScotlandPersonal,
            [BankProfileEnum.Lloyds_BankOfScotlandBusiness] = LloydsBank.BankOfScotlandBusiness,
            [BankProfileEnum.Lloyds_BankOfScotlandCommerical] = LloydsBank.BankOfScotlandCommerical,
            [BankProfileEnum.Lloyds_MbnaPersonal] = LloydsBank.MbnaPersonal
        };
}
