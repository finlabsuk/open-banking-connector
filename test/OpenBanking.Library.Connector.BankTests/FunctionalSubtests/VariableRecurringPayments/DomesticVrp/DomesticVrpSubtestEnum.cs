// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrp;

/// <summary>
///     Domestic VRP functional subtest
/// </summary>
public enum DomesticVrpSubtestEnum
{
    VrpWithDebtorAccountSpecifiedByPisp,
    VrpWithDebtorAccountSpecifiedDuringConsentAuthorisation,
    VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation
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
            DomesticVrpSubtestEnum.VrpWithDebtorAccountSpecifiedByPisp =>
                DomesticVrpTemplateType.VrpWithDebtorAccountSpecifiedByPisp,
            DomesticVrpSubtestEnum.VrpWithDebtorAccountSpecifiedDuringConsentAuthorisation =>
                DomesticVrpTemplateType
                    .VrpWithDebtorAccountSpecifiedDuringConsentAuthorisation,
            DomesticVrpSubtestEnum
                    .VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation
                =>
                DomesticVrpTemplateType
                    .VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation,
            _ => throw new ArgumentException(
                $"{nameof(subtestEnum)} is not valid {nameof(DomesticVrpSubtestEnum)} or needs to be added to this switch statement.")
        };
}
