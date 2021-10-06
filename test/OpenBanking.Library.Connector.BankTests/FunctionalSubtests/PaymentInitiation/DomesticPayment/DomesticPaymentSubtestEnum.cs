// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment
{
    /// <summary>
    ///     Domestic payment functional subtest
    /// </summary>
    public enum DomesticPaymentSubtestEnum
    {
        PersonToPersonSubtest,
        PersonToMerchantSubtest
    }

    public static class DomesticPaymentFunctionalSubtestHelper
    {
        static DomesticPaymentFunctionalSubtestHelper()
        {
            AllDomesticPaymentSubtests = Enum.GetValues(typeof(DomesticPaymentSubtestEnum))
                .Cast<DomesticPaymentSubtestEnum>().ToHashSet();
        }

        public static ISet<DomesticPaymentSubtestEnum> AllDomesticPaymentSubtests { get; }

        public static DomesticPaymentTypeEnum DomesticPaymentType(DomesticPaymentSubtestEnum subtestEnum) =>
            subtestEnum switch
            {
                DomesticPaymentSubtestEnum.PersonToPersonSubtest => DomesticPaymentTypeEnum
                    .PersonToPerson,
                DomesticPaymentSubtestEnum.PersonToMerchantSubtest => DomesticPaymentTypeEnum
                    .PersonToMerchant,
                _ => throw new ArgumentException(
                    $"{nameof(subtestEnum)} is not valid {nameof(DomesticPaymentSubtestEnum)} or needs to be added to this switch statement.")
            };
    }
}
