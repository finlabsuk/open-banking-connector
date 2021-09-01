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
    ///     Banks used for testing.
    /// </summary>
    public enum DomesticPaymentFunctionalSubtestEnum
    {
        PersonToPersonSubtest,
        PersonToMerchantSubtest
    }

    public static class DomesticPaymentFunctionalSubtestHelper
    {
        static DomesticPaymentFunctionalSubtestHelper()
        {
            AllDomesticPaymentFunctionalTests = Enum.GetValues(typeof(DomesticPaymentFunctionalSubtestEnum))
                .Cast<DomesticPaymentFunctionalSubtestEnum>().ToHashSet();
        }

        public static ISet<DomesticPaymentFunctionalSubtestEnum> AllDomesticPaymentFunctionalTests { get; }

        public static DomesticPaymentFunctionalSubtest Test(DomesticPaymentFunctionalSubtestEnum subtestEnum) =>
            subtestEnum switch
            {
                DomesticPaymentFunctionalSubtestEnum.PersonToPersonSubtest => TestDefinitions.PersonToPersonSubtest,
                DomesticPaymentFunctionalSubtestEnum.PersonToMerchantSubtest => TestDefinitions.PersonToMerchantSubtest,
                _ => throw new ArgumentException(
                    $"{nameof(subtestEnum)} is not valid DomesticPaymentFunctionalTestEnum or needs to be added to this switch statement.")
            };

        public static DomesticPaymentTypeEnum DomesticPaymentType(DomesticPaymentFunctionalSubtestEnum subtestEnum) =>
            subtestEnum switch
            {
                DomesticPaymentFunctionalSubtestEnum.PersonToPersonSubtest => DomesticPaymentTypeEnum
                    .PersonToPerson,
                DomesticPaymentFunctionalSubtestEnum.PersonToMerchantSubtest => DomesticPaymentTypeEnum
                    .PersonToMerchant,
                _ => throw new ArgumentException(
                    $"{nameof(subtestEnum)} is not valid DomesticPaymentFunctionalTestEnum or needs to be added to this switch statement.")
            };
    }
}
