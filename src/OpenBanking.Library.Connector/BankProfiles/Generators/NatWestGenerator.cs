// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class NatWestGenerator : BankProfileGeneratorBase<NatWestBank>
{
    public NatWestGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<NatWestBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(NatWestBank bank)
    {
        return new BankProfile(
            _bankGroup.GetBankProfile(bank),
            bank switch
            {
                NatWestBank.NatWestSandbox => GetIssuerUrl(bank),
                NatWestBank.RoyalBankOfScotlandSandbox => GetIssuerUrl(bank),
                NatWestBank.NatWest =>
                    "https://secure1.natwest.com", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/121798762/Implementation+Guide+National+Westminster+Bank+Plc
                NatWestBank.RoyalBankOfScotland =>
                    "https://secure1.rbs.co.uk", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110264930/Implementation+Guide+Royal+Bank+of+Scotland
                NatWestBank.UlsterBankNi =>
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
                NatWestBank.UlsterBankNi =>
                    "0015800000jfwxXAAQ", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                _ => throw new ArgumentOutOfRangeException()
            },
            GetAccountAndTransactionApi(bank),
            bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox
                ? GetPaymentInitiationApi(bank)
                : null,
            null,
            bank is not (NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox))
        {
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                TokenEndpointAuthMethod = TokenEndpointAuthMethod.PrivateKeyJwt
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                UseGetPartyEndpoint = false,
                UseGetPartiesEndpoint = false
            },
            CustomBehaviour = bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox
                ? new CustomBehaviourClass
                {
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        ScopeClaimResponseJsonConverter =
                            DelimitedStringConverterOptions.JsonIsStringArrayNotString
                    }
                }
                : null
        };
    }

    private VariableRecurringPaymentsApi GetVariableRecurringPaymentsApi(NatWestBank bank)
    {
        return new VariableRecurringPaymentsApi
        {
            VariableRecurringPaymentsApiVersion =
                VariableRecurringPaymentsApiVersion
                    .Version3p1p8, // from https://www.bankofapis.com/products/natwest-group-open-banking/vrp/documentation/nwb/3.1.8
            BaseUrl = GetVariableRecurringPaymentsApiBaseUrl(bank)
        };
    }

    private AccountAndTransactionApi GetAccountAndTransactionApi(NatWestBank bank)
    {
        return new AccountAndTransactionApi
        {
            AccountAndTransactionApiVersion =
                AccountAndTransactionApiVersion
                    .Version3p1p10, // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/nwb/3.1.10
            BaseUrl =
                bank switch
                {
                    NatWestBank.NatWestSandbox => GetAccountAndTransactionApiBaseUrl(bank),
                    NatWestBank.RoyalBankOfScotlandSandbox => GetAccountAndTransactionApiBaseUrl(bank),
                    NatWestBank.NatWest =>
                        "https://api.natwest.com/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/nwb/3.1.10
                    NatWestBank.RoyalBankOfScotland =>
                        "htpps://api.rbs.co.uk/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/rbs/3.1.10
                    NatWestBank.UlsterBankNi =>
                        "https://api.ulsterbank.co.uk/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/ubn/3.1.10
                    _ => throw new ArgumentOutOfRangeException()
                }
        };
    }

    private PaymentInitiationApi GetPaymentInitiationApi(NatWestBank bank)
    {
        return new PaymentInitiationApi
        {
            PaymentInitiationApiVersion =
                PaymentInitiationApiVersion
                    .Version3p1p6, // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/nwb/3.1.6
            BaseUrl = GetPaymentInitiationApiBaseUrl(bank)
        };
    }
}
