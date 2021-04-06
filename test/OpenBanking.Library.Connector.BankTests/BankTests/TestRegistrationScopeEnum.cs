// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    /// <summary>
    ///     Bank tests.
    /// </summary>
    public enum TestRegistrationScopeEnum
    {
        PaymentInitiation,
        MultipleElementScope
    }

    public delegate bool ApiTypeSetsIncludedFilter(RegistrationScope registrationScope);

    public static class TestRegistrationScopeEnumHelper
    {
        static TestRegistrationScopeEnumHelper()
        {
            AllBankTests = Enum.GetValues(typeof(TestRegistrationScopeEnum))
                .Cast<TestRegistrationScopeEnum>();
        }

        public static IEnumerable<TestRegistrationScopeEnum> AllBankTests { get; }

        /// <summary>
        ///     Convert bank test string (BankTestEnum member name) to BankTestEnum
        /// </summary>
        /// <param name="bankTestString"></param>
        /// <returns></returns>
        public static TestRegistrationScopeEnum GetBankTestEnum(string bankTestString) =>
            Enum.Parse<TestRegistrationScopeEnum>(bankTestString);

        public static ApiTypeSetsIncludedFilter ApiTypeSetsIncludedFilter(
            TestRegistrationScopeEnum testRegistrationScope)
        {
            return testRegistrationScope switch
            {
                TestRegistrationScopeEnum.PaymentInitiation => x => x == RegistrationScope.PaymentInitiation,
                TestRegistrationScopeEnum.MultipleElementScope => x =>
                {
                    foreach (RegistrationScopeElement apiType in RegistrationScopeApiHelper.AllApiTypes)
                    {
                        if (x == RegistrationScopeApiHelper.ApiTypeSetWithSingleApiType(apiType))
                        {
                            return false;
                        }
                    }

                    return true;
                },
                _ => throw new ArgumentException(
                    $"{nameof(testRegistrationScope)} is not valid BankTestEnum or needs to be added to this switch statement.")
            };
        }
    }
}
