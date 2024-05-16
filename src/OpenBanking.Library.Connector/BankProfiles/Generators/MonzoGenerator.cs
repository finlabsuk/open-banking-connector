// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class MonzoGenerator : BankProfileGeneratorBase<MonzoBank>
{
    public MonzoGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<MonzoBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(
        MonzoBank bank,
        IInstrumentationClient instrumentationClient)
    {
        var authCodeGrantPostCustomBehaviour =
            new AuthCodeGrantPostCustomBehaviour
            {
                ResponseTokenTypeCaseMayBeIncorrect = true,
                ResponseScopeMayIncludeExtraValues = true
            };
        var refreshTokenGrantPostCustomBehaviour = new RefreshTokenGrantPostCustomBehaviour
        {
            ResponseTokenTypeCaseMayBeIncorrect = true,
            ResponseScopeMayIncludeExtraValues = true,
            IdTokenMayBeAbsent = true
        };
        var pispSandboxAdditionalProperties = new Dictionary<string, object>
        {
            ["DesiredStatus"] = "Authorised",
            ["UserID"] = "user_0000A4C4nqORb7K9YYW3r0",
            ["AccountID"] = "acc_0000A4C4o66FCYJoERQhHN"
        };
        return new BankProfile(
            _bankGroup.GetBankProfile(bank),
            bank switch
            {
                MonzoBank.Monzo =>
                    "https://api.monzo.com/open-banking/", // from https://docs.monzo.com/#account-information-services-api
                MonzoBank.Sandbox =>
                    "https://api.s101.nonprod-ffs.io/open-banking/", // from https://docs.monzo.com/#account-information-services-api
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            GetFinancialId(bank),
            GetAccountAndTransactionApi(bank),
            GetPaymentInitiationApi(bank),
            GetVariableRecurringPaymentsApi(bank),
            bank is not MonzoBank.Sandbox,
            instrumentationClient)
        {
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost =
                    new BankRegistrationPostCustomBehaviour
                    {
                        TransportCertificateSubjectDnOrgIdEncoding =
                            SubjectDnOrgIdEncoding.DottedDecimalAttributeType
                    },
                ClientCredentialsGrantPost =
                    new ClientCredentialsGrantPostCustomBehaviour { ResponseTokenTypeCaseMayBeIncorrect = true },
                AccountAccessConsentAuthCodeGrantPost = authCodeGrantPostCustomBehaviour,
                DomesticPaymentConsentAuthCodeGrantPost = authCodeGrantPostCustomBehaviour,
                DomesticVrpConsentAuthCodeGrantPost = authCodeGrantPostCustomBehaviour,
                AccountAccessConsentRefreshTokenGrantPost = refreshTokenGrantPostCustomBehaviour,
                DomesticPaymentConsentRefreshTokenGrantPost = refreshTokenGrantPostCustomBehaviour,
                DomesticVrpConsentRefreshTokenGrantPost = refreshTokenGrantPostCustomBehaviour,
                DomesticPaymentPost =
                    new ReadWritePostCustomBehaviour { ResponseLinksMayHaveIncorrectUrlBeforeQuery = true },
                DomesticPaymentGet =
                    new DomesticPaymentGetCustomBehaviour { ResponseLinksMayHaveIncorrectUrlBeforeQuery = true },
                DomesticVrpPost = new DomesticVrpPostCustomBehaviour
                {
                    RefundResponseJsonConverter =
                        DomesticVrpRefundConverterOptions.ContainsNestedAccountProperty
                },
                DomesticVrpGet =
                    new DomesticVrpGetCustomBehaviour
                    {
                        RefundResponseJsonConverter =
                            DomesticVrpRefundConverterOptions.ContainsNestedAccountProperty
                    }
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentExternalApiRequestAdjustments = bank is MonzoBank.Sandbox
                    ? externalApiRequest =>
                    {
                        externalApiRequest.Data.SupplementaryData =
                            new AccountAndTransactionModelsPublic.OBSupplementaryData1
                            {
                                AdditionalProperties = new Dictionary<string, object>
                                {
                                    ["DesiredStatus"] = "Authorised",
                                    ["UserID"] = "user_0000A4C4ZChWNMEvew2U77",
                                    ["AccountID"] = "acc_0000A4C4ZSskDOixqNPfpR"
                                }
                            };
                        return externalApiRequest;
                    }
                    : x => x,
                UseReauth = false
            },
            PaymentInitiationApiSettings = new PaymentInitiationApiSettings
            {
                DomesticPaymentConsentExternalApiRequestAdjustments = bank is MonzoBank.Sandbox
                    ? externalApiRequest =>
                    {
                        externalApiRequest.Data.Initiation.SupplementaryData =
                            new PaymentInitiationModelsPublic.OBSupplementaryData1
                            {
                                AdditionalProperties = pispSandboxAdditionalProperties
                            };
                        return externalApiRequest;
                    }
                    : x => x,
                DomesticPaymentExternalApiRequestAdjustments = bank is MonzoBank.Sandbox
                    ? externalApiRequest =>
                    {
                        externalApiRequest.Data.Initiation.SupplementaryData =
                            new PaymentInitiationModelsPublic.OBSupplementaryData1
                            {
                                AdditionalProperties = pispSandboxAdditionalProperties
                            };
                        return externalApiRequest;
                    }
                    : x => x
            },
            AspspBrandId = bank switch
            {
                MonzoBank.Sandbox => 10003, // sandbox
                MonzoBank.Monzo => 1430,
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            }
        };
    }

    private AccountAndTransactionApi? GetAccountAndTransactionApi(MonzoBank bank) =>
        bank switch
        {
            MonzoBank.Sandbox => new AccountAndTransactionApi
            {
                BaseUrl =
                    "https://openbanking.s101.nonprod-ffs.io/open-banking/v3.1/aisp" // from https://docs.monzo.com/#account-information-services-api
            },
            MonzoBank.Monzo => new AccountAndTransactionApi
            {
                BaseUrl =
                    "https://openbanking.monzo.com/open-banking/v3.1/aisp" // from https://docs.monzo.com/#account-information-services-api
            },
            _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
        };

    private PaymentInitiationApi GetPaymentInitiationApi(MonzoBank bank) =>
        new() { BaseUrl = GetPaymentsBaseUrl(bank) };

    private static string GetPaymentsBaseUrl(MonzoBank bank)
    {
        return bank switch
        {
            MonzoBank.Sandbox =>
                "https://openbanking.s101.nonprod-ffs.io/open-banking/v3.1/pisp", // from https://docs.monzo.com/#payment-initiation-services-api
            MonzoBank.Monzo => "https://openbanking.monzo.com/open-banking/v3.1/pisp", // from https://docs.monzo.com/#payment-initiation-services-api
            _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
        };
    }

    private VariableRecurringPaymentsApi GetVariableRecurringPaymentsApi(MonzoBank bank) =>
        new() { BaseUrl = GetPaymentsBaseUrl(bank) };
}
