// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum NatWestBank
    {
        NatWestSandbox,
        RoyalBankOfScotlandSandbox,
        NatWest,
        RoyalBankOfScotland,
        UlsterBankNI
    }

    public class NatWest : BankGroupBase<NatWestBank>
    {
        protected override ConcurrentDictionary<BankProfileEnum, NatWestBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.NatWest_NatWestSandbox] = NatWestBank.NatWestSandbox,
                [BankProfileEnum.NatWest_RoyalBankOfScotlandSandbox] = NatWestBank.RoyalBankOfScotlandSandbox,
                [BankProfileEnum.NatWest_NatWest] = NatWestBank.NatWest,
                [BankProfileEnum.NatWest_RoyalBankOfScotland] = NatWestBank.RoyalBankOfScotland,
                [BankProfileEnum.NatWest_UlsterBankNI] = NatWestBank.UlsterBankNI
            };

        public override BankProfile GetBankProfile(
            BankProfileEnum bankProfileEnum,
            HiddenPropertiesDictionary hiddenPropertiesDictionary)
        {
            NatWestBank bank = GetBank(bankProfileEnum);
            BankProfileHiddenProperties bankProfileHiddenProperties =
                hiddenPropertiesDictionary[bankProfileEnum] ??
                throw new Exception(
                    $"Hidden properties are required for bank profile {bankProfileEnum} and cannot be found.");
            return new BankProfile(
                bankProfileEnum,
                bank switch
                {
                    NatWestBank.NatWestSandbox => bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                    NatWestBank.RoyalBankOfScotlandSandbox => bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                    NatWestBank.NatWest =>
                        "https://secure1.natwest.com", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/121798762/Implementation+Guide+National+Westminster+Bank+Plc
                    NatWestBank.RoyalBankOfScotland =>
                        "https://secure1.rbs.co.uk", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110264930/Implementation+Guide+Royal+Bank+of+Scotland
                    NatWestBank.UlsterBankNI =>
                        "https://secure1.ulsterbank.co.uk", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/121766033/Implementation+Guide+Ulster+Bank+Ltd
                    _ => throw new ArgumentOutOfRangeException()
                },
                bank switch
                {
                    NatWestBank.NatWestSandbox =>
                        "0015800000jfwxXAAQ", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                    NatWestBank.RoyalBankOfScotlandSandbox =>
                        "0015800000jfwB4AAI", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                    NatWestBank
                        .NatWest => "0015800000jfwxXAAQ", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                    NatWestBank.RoyalBankOfScotland =>
                        "0015800000jfwB4AAI", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                    NatWestBank.UlsterBankNI =>
                        "0015800000jfwxXAAQ", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                    _ => throw new ArgumentOutOfRangeException()
                },
                GetAccountAndTransactionApi(bank, bankProfileHiddenProperties),
                bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox
                    ? GetPaymentInitiationApi(bank, bankProfileHiddenProperties)
                    : null,
                null,
                bank is not (NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox))
            {
                CustomBehaviour = bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox
                    ? new CustomBehaviourClass
                    {
                        BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                        {
                            ScopeClaimResponseJsonConverter =
                                DelimitedStringConverterOptions.JsonIsStringArrayNotString
                        }
                    }
                    : null,
            };
        }

        private static VariableRecurringPaymentsApi GetVariableRecurringPaymentsApi(
            NatWestBank bank,
            BankProfileHiddenProperties bankProfileHiddenProperties)
        {
            return new VariableRecurringPaymentsApi
            {
                VariableRecurringPaymentsApiVersion =
                    VariableRecurringPaymentsApiVersion
                        .Version3p1p8, // from https://www.bankofapis.com/products/natwest-group-open-banking/vrp/documentation/nwb/3.1.8
                BaseUrl = bankProfileHiddenProperties.GetRequiredVariableRecurringPaymentsApiBaseUrl()
            };
        }

        private static AccountAndTransactionApi GetAccountAndTransactionApi(
            NatWestBank bank,
            BankProfileHiddenProperties bankProfileHiddenProperties)
        {
            return new AccountAndTransactionApi
            {
                AccountAndTransactionApiVersion =
                    AccountAndTransactionApiVersion
                        .Version3p1p10, // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/nwb/3.1.10
                BaseUrl =
                    bank switch
                    {
                        NatWestBank.NatWestSandbox => bankProfileHiddenProperties
                            .GetRequiredAccountAndTransactionApiBaseUrl(),
                        NatWestBank.RoyalBankOfScotlandSandbox => bankProfileHiddenProperties
                            .GetRequiredAccountAndTransactionApiBaseUrl(),
                        NatWestBank.NatWest =>
                            "https://api.natwest.com/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/nwb/3.1.10
                        NatWestBank.RoyalBankOfScotland =>
                            "htpps://api.rbs.co.uk/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/rbs/3.1.10
                        NatWestBank.UlsterBankNI =>
                            "https://api.ulsterbank.co.uk/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/ubn/3.1.10
                        _ => throw new ArgumentOutOfRangeException()
                    }
            };
        }

        private static PaymentInitiationApi GetPaymentInitiationApi(
            NatWestBank bank,
            BankProfileHiddenProperties bankProfileHiddenProperties)
        {
            return new PaymentInitiationApi
            {
                PaymentInitiationApiVersion =
                    PaymentInitiationApiVersion
                        .Version3p1p6, // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/nwb/3.1.6
                BaseUrl = bankProfileHiddenProperties
                    .GetRequiredPaymentInitiationApiBaseUrl()
            };
        }
    }
}
