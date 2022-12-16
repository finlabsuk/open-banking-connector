// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models;
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
                NatWestBank.NatWest =>
                    "https://personal.secure1.natwest.com", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.NatWestBankline =>
                    "https://corporate.secure1.natwest.com", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.NatWestClearSpend =>
                    "https://clearspend.secure1.natwest.com", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.RoyalBankOfScotlandSandbox => GetIssuerUrl(bank),
                NatWestBank.RoyalBankOfScotland =>
                    "https://personal.secure1.rbs.co.uk", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.RoyalBankOfScotlandBankline =>
                    "https://corporate.secure1.rbs.co.uk", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.RoyalBankOfScotlandClearSpend =>
                    "https://clearspend.secure1.rbs.co.uk", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.TheOne =>
                    "https://toa.secure1.rbs.co.uk", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.NatWestOne =>
                    "https://noa.secure1.rbs.co.uk", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.VirginOne =>
                    "https://voa.secure1.rbs.co.uk", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.UlsterBankNi =>
                    "https://personal.secure1.ulsterbank.co.uk", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.UlsterBankNiBankline =>
                    "https://corporate.secure1.ulsterbank.co.uk", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.UlsterBankNiClearSpend =>
                    "https://clearspend.secure1.ulsterbank.co.uk", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            bank switch
            {
                NatWestBank.NatWestSandbox
                    or NatWestBank.NatWest
                    or NatWestBank.NatWestBankline
                    or NatWestBank.NatWestClearSpend =>
                    "0015800000jfwxXAAQ", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.RoyalBankOfScotlandSandbox
                    or NatWestBank.RoyalBankOfScotland
                    or NatWestBank.RoyalBankOfScotlandBankline
                    or NatWestBank.RoyalBankOfScotlandClearSpend
                    or NatWestBank.TheOne
                    or NatWestBank.NatWestOne
                    or NatWestBank.VirginOne =>
                    "0015800000jfwB4AAI", // from https://www.bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                NatWestBank.UlsterBankNi
                    or NatWestBank.UlsterBankNiBankline
                    or NatWestBank.UlsterBankNiClearSpend =>
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
                TokenEndpointAuthMethod = TokenEndpointAuthMethod.PrivateKeyJwt,
                BankRegistrationGroup = bank switch
                {
                    NatWestBank.NatWestSandbox => 
                        BankRegistrationGroup.NatWest_NatWestSandbox,
                    NatWestBank.NatWest
                        or NatWestBank.NatWestBankline
                        or NatWestBank.NatWestClearSpend =>
                        BankRegistrationGroup.NatWest_NatWestProduction,
                    NatWestBank.RoyalBankOfScotlandSandbox =>
                        BankRegistrationGroup.NatWest_RoyalBankOfScotlandSandbox,
                    NatWestBank.RoyalBankOfScotland
                        or NatWestBank.RoyalBankOfScotlandBankline
                        or NatWestBank.RoyalBankOfScotlandClearSpend
                        or NatWestBank.TheOne
                        or NatWestBank.NatWestOne
                        or NatWestBank.VirginOne =>
                        BankRegistrationGroup.NatWest_RoyalBankOfScotlandProduction,
                    NatWestBank.UlsterBankNi
                        or NatWestBank.UlsterBankNiBankline
                        or NatWestBank.UlsterBankNiClearSpend =>
                        BankRegistrationGroup.NatWest_UlsterBankNiProduction,
                    _ => throw new ArgumentOutOfRangeException()
                }
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove = new List<OBReadConsent1DataPermissionsEnum>
                    {
                        OBReadConsent1DataPermissionsEnum.ReadParty,
                        OBReadConsent1DataPermissionsEnum.ReadPartyPSU,
                        OBReadConsent1DataPermissionsEnum.ReadPAN
                    };
                    foreach (OBReadConsent1DataPermissionsEnum element in elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                }
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
                    NatWestBank.NatWest
                        or NatWestBank.NatWestBankline
                        or NatWestBank.NatWestClearSpend =>
                        "https://api.natwest.com/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/nwb/3.1.10
                    NatWestBank.RoyalBankOfScotland
                        or NatWestBank.RoyalBankOfScotlandBankline
                        or NatWestBank.RoyalBankOfScotlandClearSpend
                        or NatWestBank.TheOne
                        or NatWestBank.NatWestOne
                        or NatWestBank.VirginOne =>
                        "htpps://api.rbs.co.uk/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/rbs/3.1.10
                    NatWestBank.UlsterBankNi
                        or NatWestBank.UlsterBankNiBankline
                        or NatWestBank.UlsterBankNiClearSpend =>
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
