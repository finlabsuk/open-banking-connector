// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrp
{
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

        public static DomesticVrpTypeEnum DomesticVrpType(DomesticVrpSubtestEnum subtestEnum) =>
            subtestEnum switch
            {
                DomesticVrpSubtestEnum.VrpWithDebtorAccountSpecifiedByPisp =>
                    DomesticVrpTypeEnum.VrpWithDebtorAccountSpecifiedByPisp,
                DomesticVrpSubtestEnum.VrpWithDebtorAccountSpecifiedDuringConsentAuthorisation =>
                    DomesticVrpTypeEnum.VrpWithDebtorAccountSpecifiedDuringConsentAuthorisation,
                DomesticVrpSubtestEnum
                        .VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation
                    =>
                    DomesticVrpTypeEnum
                        .VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation,
                _ => throw new ArgumentException(
                    $"{nameof(subtestEnum)} is not valid {nameof(DomesticVrpSubtestEnum)} or needs to be added to this switch statement.")
            };

        // TODO: remove
        public static DomesticPaymentTypeEnum DomesticPaymentType(DomesticVrpSubtestEnum subtestEnum) =>
            subtestEnum switch
            {
                DomesticVrpSubtestEnum.VrpWithDebtorAccountSpecifiedByPisp => DomesticPaymentTypeEnum
                    .PersonToPerson,
                _ => throw new ArgumentException(
                    $"{nameof(subtestEnum)} is not valid {nameof(DomesticVrpSubtestEnum)} or needs to be added to this switch statement.")
            };
    }
}
