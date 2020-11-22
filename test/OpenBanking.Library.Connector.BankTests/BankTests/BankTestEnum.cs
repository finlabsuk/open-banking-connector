// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    /// <summary>
    ///     Bank tests.
    /// </summary>
    public enum BankTestEnum
    {
        PaymentInitiationTest,
        MultipleApiTypesTest
    }

    public delegate bool ApiTypeSetsIncludedFilter(RegistrationScopeApiSet registrationScopeApiSet);

    public static class BankTestEnumHelper
    {
        static BankTestEnumHelper()
        {
            AllBankTests = Enum.GetValues(typeof(BankTestEnum))
                .Cast<BankTestEnum>();
        }

        public static IEnumerable<BankTestEnum> AllBankTests { get; }

        /// <summary>
        ///     Convert bank test string (BankTestEnum member name) to BankTestEnum
        /// </summary>
        /// <param name="bankTestString"></param>
        /// <returns></returns>
        public static BankTestEnum GetBankTestEnum(string bankTestString) => Enum.Parse<BankTestEnum>(bankTestString);

        public static ApiTypeSetsIncludedFilter ApiTypeSetsIncludedFilter(BankTestEnum bankTest)
        {
            return bankTest switch
            {
                BankTestEnum.PaymentInitiationTest => x => x == RegistrationScopeApiSet.PaymentInitiation,
                BankTestEnum.MultipleApiTypesTest => x =>
                {
                    foreach (RegistrationScopeApi apiType in RegistrationScopeApiHelper.AllApiTypes)
                    {
                        if (x == RegistrationScopeApiHelper.ApiTypeSetWithSingleApiType(apiType))
                        {
                            return false;
                        }
                    }

                    return true;
                },
                _ => throw new ArgumentException(
                    $"{nameof(bankTest)} is not valid BankTestEnum or needs to be added to this switch statement.")
            };
        }
    }
}
