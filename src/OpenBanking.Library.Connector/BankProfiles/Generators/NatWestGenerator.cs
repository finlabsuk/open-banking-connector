// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class NatWestGenerator : BankProfileGeneratorBase<NatWestBank>
{
    public NatWestGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.NatWest) { }

    public override BankProfile GetBankProfile(
        NatWestBank bank,
        IInstrumentationClient instrumentationClient)
    {
        return new BankProfile(
            _bankGroupData.GetBankProfile(bank),
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
                NatWestBank.Mettle =>
                    "https://auth.openbanking.prd-mettle.co.uk", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/mettle/3.1.10
                NatWestBank.Coutts =>
                    "https://secure1.coutts.com", // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/coutts/3.1.8
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            bank switch
            {
                NatWestBank.NatWestSandbox
                    or NatWestBank.NatWest
                    or NatWestBank.NatWestBankline
                    or NatWestBank.NatWestClearSpend
                    or NatWestBank.Mettle =>
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
                NatWestBank.Coutts => "0015800000ti1PbAAI", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/coutts/3.1.8#tls-requirements
                _ => throw new ArgumentOutOfRangeException()
            },
            GetAccountAndTransactionApi(bank),
            GetPaymentInitiationApi(bank),
            GetVariableRecurringPaymentsApi(bank),
            bank is not (NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox),
            instrumentationClient)
        {
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                TokenEndpointAuthMethod = bank switch
                {
                    NatWestBank.Mettle => TokenEndpointAuthMethodSupportedValues.TlsClientAuth,
                    _ => TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt
                },
                IdTokenSubClaimType =
                    bank is NatWestBank.Coutts ? IdTokenSubClaimType.EndUserId : IdTokenSubClaimType.ConsentId
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentTemplateExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove =
                        new List<AccountAndTransactionModelsPublic.Permissions>
                        {
                            AccountAndTransactionModelsPublic.Permissions.ReadParty,
                            AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU,
                            AccountAndTransactionModelsPublic.Permissions.ReadPAN
                        };
                    if (bank is NatWestBank.Coutts)
                    {
                        elementsToRemove.AddRange(
                        [
                            AccountAndTransactionModelsPublic.Permissions.ReadOffers, AccountAndTransactionModelsPublic
                                .Permissions
                                .ReadStatementsBasic,
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadStatementsDetail
                        ]);
                    }

                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                }
            },
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    TransportCertificateSubjectDnOrgIdEncoding = bank switch
                    {
                        NatWestBank.Mettle or NatWestBank.Coutts => SubjectDnOrgIdEncoding.StringAttributeType,
                        _ => SubjectDnOrgIdEncoding.DottedDecimalAttributeType
                    },
                    ScopeClaimResponseJsonConverter =
                        bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox
                            ? DelimitedStringConverterOptions.JsonIsStringArrayNotString
                            : null
                },
                AccountAccessConsentAuthGet = new ConsentAuthGetCustomBehaviour
                {
                    AudClaim = GetAudClaim(bank),
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                AccountAccessConsentAuthCodeGrantPost = new AuthCodeGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                AccountAccessConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { IdTokenMayBeAbsent = true },
                AccountAccessConsentPost = bank is NatWestBank.Coutts
                    ? new ReadWritePostCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                AccountAccessConsentGet = bank is NatWestBank.Coutts
                    ? new ReadWriteGetCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                AccountGet = bank is NatWestBank.Coutts
                    ? new ReadWriteGetCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                BalanceGet = bank is NatWestBank.Coutts
                    ? new ReadWriteGetCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                DirectDebitGet = bank is NatWestBank.Coutts
                    ? new DirectDebitGetCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                MonzoPotGet = bank is NatWestBank.Coutts
                    ? new ReadWriteGetCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                Party2Get = bank is NatWestBank.Coutts
                    ? new ReadWriteGetCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                PartyGet = bank is NatWestBank.Coutts
                    ? new ReadWriteGetCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                StandingOrderGet = bank is NatWestBank.Coutts
                    ? new ReadWriteGetCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                TransactionGet = bank is NatWestBank.Coutts
                    ? new ReadWriteGetCustomBehaviour { ResponseLinksMayAddSlash = true }
                    : null,
                DomesticPaymentConsentAuthGet = new ConsentAuthGetCustomBehaviour
                {
                    AudClaim = GetAudClaim(bank),
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                DomesticPaymentConsentAuthCodeGrantPost = new AuthCodeGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                DomesticPaymentConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { IdTokenMayBeAbsent = true },
                DomesticVrpConsentAuthGet = new ConsentAuthGetCustomBehaviour
                {
                    AudClaim = GetAudClaim(bank),
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                DomesticVrpConsentAuthCodeGrantPost = new AuthCodeGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                DomesticVrpConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { IdTokenMayBeAbsent = true },
                DomesticPaymentConsent =
                    new DomesticPaymentConsentCustomBehaviour
                    {
                        PreferMisspeltContractPresentIndicator = true,
                        ResponseDataFundsAvailableResultFundsAvailableMayBeWrong =
                            bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox,
                        ResponseLinksMayAddSlash = bank is NatWestBank.Coutts,
                        ResponseRiskContractPresentIndicatorMayBeMissingOrWrong = bank is NatWestBank.Coutts
                    },
                DomesticPayment =
                    new DomesticPaymentCustomBehaviour
                    {
                        PreferMisspeltContractPresentIndicator = true,
                        ResponseDataStatusMayBeWrong =
                            bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox,
                        ResponseDataDebtorSchemeNameMayBeMissingOrWrong =
                            bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox,
                        ResponseDataDebtorIdentificationMayBeMissingOrWrong =
                            bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox,
                        ResponseLinksMayAddSlash = bank is NatWestBank.Coutts
                    },
                DomesticVrpConsent =
                    new DomesticVrpConsentCustomBehaviour
                    {
                        PreferMisspeltContractPresentIndicator = true,
                        ResponseRiskContractPresentIndicatorMayBeMissingOrWrong =
                            bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox,
                        ResponseDataFundsAvailableResultFundsAvailableMayBeWrong =
                            bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox
                    },
                DomesticVrp = new DomesticVrpCustomBehaviour
                {
                    PreferMisspeltContractPresentIndicator = true,
                    ResponseDataStatusMayBeWrong =
                        bank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox
                }
            },
            AspspBrandId = bank switch
            {
                NatWestBank.NatWestSandbox => 100001, // sandbox
                NatWestBank.NatWest
                    or NatWestBank.NatWestBankline
                    or NatWestBank.NatWestClearSpend
                    or NatWestBank.Mettle => 13,
                NatWestBank.RoyalBankOfScotlandSandbox => 100002, // sandbox
                NatWestBank.RoyalBankOfScotland
                    or NatWestBank.RoyalBankOfScotlandBankline
                    or NatWestBank.RoyalBankOfScotlandClearSpend
                    or NatWestBank.TheOne
                    or NatWestBank.NatWestOne
                    or NatWestBank.VirginOne => 14,
                NatWestBank.UlsterBankNi
                    or NatWestBank.UlsterBankNiBankline
                    or NatWestBank.UlsterBankNiClearSpend =>
                    13,
                NatWestBank.Coutts => 13,
                _ => throw new ArgumentOutOfRangeException()
            }
        };
    }

    private string? GetAudClaim(NatWestBank bank) => bank switch
    {
        NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox => null,
        NatWestBank.NatWest
            or NatWestBank.NatWestBankline
            or NatWestBank.NatWestClearSpend => "https://secure1.natwest.com",
        NatWestBank.RoyalBankOfScotland
            or NatWestBank.RoyalBankOfScotlandBankline
            or NatWestBank.RoyalBankOfScotlandClearSpend
            or NatWestBank.TheOne
            or NatWestBank.NatWestOne
            or NatWestBank.VirginOne => "https://secure1.rbs.co.uk",
        NatWestBank.UlsterBankNi
            or NatWestBank.UlsterBankNiBankline
            or NatWestBank.UlsterBankNiClearSpend =>
            "https://secure1.ulsterbank.co.uk",
        NatWestBank.Mettle => null, // use default
        NatWestBank.Coutts => null, // use default
        _ => throw new ArgumentOutOfRangeException(
            nameof(bank),
            bank,
            null)
    };

    private VariableRecurringPaymentsApi? GetVariableRecurringPaymentsApi(NatWestBank bank)
    {
        if (bank is NatWestBank.Coutts)
        {
            return null;
        }
        return new VariableRecurringPaymentsApi { BaseUrl = GetPaymentsBaseUrl(bank) };
    }

    private string GetPaymentsBaseUrl(NatWestBank bank)
    {
        return bank switch
        {
            NatWestBank.NatWestSandbox => GetPaymentInitiationApiBaseUrl(bank),
            NatWestBank.RoyalBankOfScotlandSandbox => GetPaymentInitiationApiBaseUrl(bank),
            NatWestBank.NatWest
                or NatWestBank.NatWestBankline
                or NatWestBank.NatWestClearSpend =>
                "https://api.natwest.com/open-banking/v3.1/pisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/nwb/3.1.11
            NatWestBank.RoyalBankOfScotland
                or NatWestBank.RoyalBankOfScotlandBankline
                or NatWestBank.RoyalBankOfScotlandClearSpend
                or NatWestBank.TheOne
                or NatWestBank.NatWestOne
                or NatWestBank.VirginOne =>
                "https://api.rbs.co.uk/open-banking/v3.1/pisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/rbs/3.1.11
            NatWestBank.UlsterBankNi
                or NatWestBank.UlsterBankNiBankline
                or NatWestBank.UlsterBankNiClearSpend =>
                "https://api.ulsterbank.co.uk/open-banking/v3.1/pisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/ubn/3.1.11
            NatWestBank.Mettle =>
                "https://api.openbanking.prd-mettle.co.uk/apis/open-banking/v3.1/pisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/mettle/3.1.10
            NatWestBank.Coutts =>
                "https://api.coutts.com/open-banking/v3.1/pisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/coutts/3.1.8#api-specification
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private AccountAndTransactionApi GetAccountAndTransactionApi(NatWestBank bank)
    {
        return new AccountAndTransactionApi
        {
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
                        "https://api.rbs.co.uk/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/rbs/3.1.10
                    NatWestBank.UlsterBankNi
                        or NatWestBank.UlsterBankNiBankline
                        or NatWestBank.UlsterBankNiClearSpend =>
                        "https://api.ulsterbank.co.uk/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/ubn/3.1.10
                    NatWestBank.Mettle =>
                        "https://api.openbanking.prd-mettle.co.uk/apis/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/mettle/3.1.10
                    NatWestBank.Coutts =>
                        "https://api.coutts.com/open-banking/v3.1/aisp", // from https://www.bankofapis.com/products/natwest-group-open-banking/accounts/documentation/coutts/3.1.8#api-specification
                    _ => throw new ArgumentOutOfRangeException()
                }
        };
    }

    private PaymentInitiationApi GetPaymentInitiationApi(NatWestBank bank) =>
        new() { BaseUrl = GetPaymentsBaseUrl(bank) };
}
