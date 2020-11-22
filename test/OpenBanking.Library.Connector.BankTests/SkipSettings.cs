// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public delegate bool UnskippedFilter(BankProfile bankProfile, RegistrationScopeApiSet registrationScopeApiSet);

    public delegate bool DomesticPaymentSubtestUnskippedFilter(
        DomesticPaymentFunctionalSubtestEnum domesticPaymentFunctionalSubtestEnum,
        BankProfile bankProfile,
        RegistrationScopeApiSet registrationScopeApiSet);

    public static class SkipSettings
    {
        public static DomesticPaymentSubtestUnskippedFilter DomesticPaymentSubtestUnskippedFilter { get; } =
            (domesticPaymentFunctionalSubtestEnum, bankProfile, registrationScopeApiSet) =>
            {
                return domesticPaymentFunctionalSubtestEnum switch
                {
                    DomesticPaymentFunctionalSubtestEnum.PersonToPersonSubtest =>
                        false,
                    DomesticPaymentFunctionalSubtestEnum.PersonToMerchantSubtest => true,
                    _ => throw new ArgumentException(
                        $"{nameof(domesticPaymentFunctionalSubtestEnum)} is not valid {nameof(DomesticPaymentFunctionalSubtestEnum)} or needs to be added to this switch statement.")
                };
            };

        public static UnskippedFilter UnskippedFilter(BankTestEnum bankTest)
        {
            bool skipAll = false;
            if (skipAll)
            {
                return (bank, set) => false;
            }

            ISet<BankProfileEnum> bankBlacklist = new HashSet<BankProfileEnum>
            {
            };
            ISet<BankProfileEnum> bankWhitelist = new HashSet<BankProfileEnum>(); // default empty whitelist
            //ISet<BankEnum> bankWhitelist = new HashSet<BankEnum> { BankEnum.NatWest };
            Func<BankProfile, bool> bankUnskipped = b =>
            {
                bool result = !bankBlacklist.Contains(b.BankProfileEnum); // default to using blacklist
                if (bankWhitelist.Any()) // use whitelist if defined
                {
                    result = bankWhitelist.Contains(b.BankProfileEnum);
                }

                return result;
            };

            UnskippedFilter filter = bankTest switch
            {
                BankTestEnum.PaymentInitiationTest => (b, a) => bankUnskipped(b),
                BankTestEnum.MultipleApiTypesTest => (b, a) => bankUnskipped(b),
                _ => throw new ArgumentException(
                    $"{nameof(bankTest)} is not valid BankTestEnum or needs to be added to this switch statement.")
            };


            return filter;
        }
    }
}
