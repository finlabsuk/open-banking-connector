// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrpConsent;

/// <summary>
///     Domestic VRP functional subtest
/// </summary>
public enum DomesticVrpSubtestEnum
{
    SweepingVrp
}

public static class DomesticVrpSubtestHelper
{
    static DomesticVrpSubtestHelper()
    {
        AllDomesticVrpSubtests = Enum.GetValues(typeof(DomesticVrpSubtestEnum))
            .Cast<DomesticVrpSubtestEnum>().ToHashSet();
    }

    public static ISet<DomesticVrpSubtestEnum> AllDomesticVrpSubtests { get; }

    public static DomesticVrpTemplateType GetDomesticVrpConsentTemplateType(DomesticVrpSubtestEnum subtestEnum) =>
        subtestEnum switch
        {
            DomesticVrpSubtestEnum.SweepingVrp =>
                DomesticVrpTemplateType.SweepingVrp,
            _ => throw new ArgumentException(
                $"{nameof(subtestEnum)} is not valid {nameof(DomesticVrpSubtestEnum)} or needs to be added to this switch statement.")
        };
}
