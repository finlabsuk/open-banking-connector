// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrpConsent;

/// <summary>
///     Domestic VRP functional subtest
/// </summary>
public enum DomesticVrpSubtestEnum
{
    SweepingVrpPayment
}

public static class DomesticVrpSubtestHelper
{
    static DomesticVrpSubtestHelper()
    {
        AllDomesticVrpSubtests = Enum.GetValues(typeof(DomesticVrpSubtestEnum))
            .Cast<DomesticVrpSubtestEnum>().ToHashSet();
    }

    public static ISet<DomesticVrpSubtestEnum> AllDomesticVrpSubtests { get; }
}
