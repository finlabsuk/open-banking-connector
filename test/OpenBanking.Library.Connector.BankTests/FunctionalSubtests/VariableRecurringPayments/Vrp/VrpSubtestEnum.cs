// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.Vrp
{
    /// <summary>
    ///     VRP functional subtest
    /// </summary>
    public enum VrpSubtestEnum
    {
        VrpWithDebtorAccountSpecifiedByPisp
    }

    public static class VrpSubtestHelper
    {
        static VrpSubtestHelper()
        {
            AllDomesticPaymentFunctionalTests = Enum.GetValues(typeof(VrpSubtestEnum))
                .Cast<VrpSubtestEnum>().ToHashSet();
        }

        public static ISet<VrpSubtestEnum> AllDomesticPaymentFunctionalTests { get; }

        public static DomesticPaymentTypeEnum DomesticPaymentType(VrpSubtestEnum subtestEnum) =>
            subtestEnum switch
            {
                VrpSubtestEnum.VrpWithDebtorAccountSpecifiedByPisp => DomesticPaymentTypeEnum
                    .PersonToPerson,
                _ => throw new ArgumentException(
                    $"{nameof(subtestEnum)} is not valid {nameof(VrpSubtestEnum)} or needs to be added to this switch statement.")
            };
    }
}
