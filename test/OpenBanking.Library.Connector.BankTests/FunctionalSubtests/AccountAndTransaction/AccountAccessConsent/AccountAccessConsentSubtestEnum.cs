// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.AccountAndTransaction;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.AccountAndTransaction.
    AccountAccessConsent
{
    /// <summary>
    ///     Account access consent functional subtest
    /// </summary>
    public enum AccountAccessConsentSubtestEnum
    {
        MaximumPermissionsSubtest
    }

    public static class AccountAccessConsentSubtestHelper
    {
        static AccountAccessConsentSubtestHelper()
        {
            AllAccountAccessConsentSubtests = Enum.GetValues(typeof(AccountAccessConsentSubtestEnum))
                .Cast<AccountAccessConsentSubtestEnum>().ToHashSet();
        }

        public static ISet<AccountAccessConsentSubtestEnum> AllAccountAccessConsentSubtests { get; }

        public static AccountAccessConsentType GetAccountAccessConsentType(AccountAccessConsentSubtestEnum subtestEnum) =>
            subtestEnum switch
            {
                AccountAccessConsentSubtestEnum.MaximumPermissionsSubtest => AccountAccessConsentType.MaximumPermissions,
                _ => throw new ArgumentException(
                    $"{nameof(subtestEnum)} is not valid {nameof(AccountAccessConsentSubtestEnum)} or needs to be added to this switch statement.")
            };
    }
}
