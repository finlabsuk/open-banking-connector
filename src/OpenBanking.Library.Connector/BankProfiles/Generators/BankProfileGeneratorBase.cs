// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public abstract class BankProfileGeneratorBase<TBank> : IBankProfileGenerator<TBank>
    where TBank : struct, Enum
{
    protected readonly IBankGroup<TBank> _bankGroup;
    private readonly ConcurrentDictionary<TBank, BankProfileHiddenProperties> _bankProfileHiddenPropertiesDictionary;

    public BankProfileGeneratorBase(
        ISettingsProvider<BankProfilesSettings>
            bankProfilesSettingsProvider,
        IBankGroup<TBank> bankGroup)
    {
        _bankGroup = bankGroup;
        _bankProfileHiddenPropertiesDictionary = new ConcurrentDictionary<TBank, BankProfileHiddenProperties>();

        // Extract hidden properties and populate dictionary
        Dictionary<string, BankProfileHiddenProperties> bankGroupDict =
            bankProfilesSettingsProvider
                .GetSettings()
                .TryGetValue(_bankGroup.BankGroupEnum, out Dictionary<string, BankProfileHiddenProperties>? value)
                ? value
                : new Dictionary<string, BankProfileHiddenProperties>();

        // Check values of bank keys
        foreach ((string bank, BankProfileHiddenProperties _) in bankGroupDict)
        {
            if (bank != "Default" &&
                !Enum.TryParse<TBank>(bank, out _))
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    $"Invalid bank {bank} used with bank group {_bankGroup.BankGroupEnum} in bank profiles settings.");
            }
        }

        // Get default hidden properties
        BankProfileHiddenProperties? defaultHiddenProps =
            bankGroupDict.TryGetValue("Default", out BankProfileHiddenProperties? tmp1)
                ? tmp1
                : null;

        // Get bank hidden properties
        foreach (TBank bankName in Enum.GetValues<TBank>())
        {
            BankProfileHiddenProperties? bankHiddenProps =
                bankGroupDict.TryGetValue(bankName.ToString(), out BankProfileHiddenProperties? tmp2)
                    ? tmp2
                    : null;
            if (bankHiddenProps is null &&
                defaultHiddenProps is null)
            {
                continue;
            }

            // Combine bank and default values
            var combinedHiddenProps = new BankProfileHiddenProperties
            {
                IssuerUrl = GetFirstNonemptyOrNullValue(
                    bankHiddenProps?.IssuerUrl,
                    defaultHiddenProps?.IssuerUrl),
                FinancialId = GetFirstNonemptyOrNullValue(
                    bankHiddenProps?.FinancialId,
                    defaultHiddenProps?.FinancialId),
                DefaultClientRegistrationApiVersion =
                    bankHiddenProps?.DefaultClientRegistrationApiVersion ??
                    defaultHiddenProps?.DefaultClientRegistrationApiVersion,
                Extra1 = GetFirstNonemptyOrNullValue(
                    bankHiddenProps?.Extra1,
                    defaultHiddenProps?.Extra1),
                Extra2 = GetFirstNonemptyOrNullValue(
                    bankHiddenProps?.Extra2,
                    defaultHiddenProps?.Extra2)
            };
            AccountAndTransactionApiVersion? accountAndTransactionApiVersion =
                bankHiddenProps?.AccountAndTransactionApi?.ApiVersion ??
                defaultHiddenProps?.AccountAndTransactionApi?.ApiVersion;
            string? accountAndTransactionApiBaseUrl =
                GetFirstNonemptyOrNullValue(
                    bankHiddenProps?.AccountAndTransactionApi?.BaseUrl,
                    defaultHiddenProps?.AccountAndTransactionApi?.BaseUrl);
            if (accountAndTransactionApiVersion is not null ||
                accountAndTransactionApiBaseUrl is not null)
            {
                combinedHiddenProps.AccountAndTransactionApi =
                    new AccountAndTransactionApiHiddenProperties
                    {
                        ApiVersion = accountAndTransactionApiVersion,
                        BaseUrl = accountAndTransactionApiBaseUrl
                    };
            }

            PaymentInitiationApiVersion? paymentInitiationApiVersion =
                bankHiddenProps?.PaymentInitiationApi?.ApiVersion ??
                defaultHiddenProps?.PaymentInitiationApi?.ApiVersion;
            string? paymentInitiationApiBaseUrl =
                GetFirstNonemptyOrNullValue(
                    bankHiddenProps?.PaymentInitiationApi?.BaseUrl,
                    defaultHiddenProps?.PaymentInitiationApi?.BaseUrl);
            if (paymentInitiationApiVersion is not null ||
                paymentInitiationApiBaseUrl is not null)
            {
                combinedHiddenProps.PaymentInitiationApi =
                    new PaymentInitiationApiHiddenProperties
                    {
                        ApiVersion = paymentInitiationApiVersion,
                        BaseUrl = paymentInitiationApiBaseUrl
                    };
            }

            VariableRecurringPaymentsApiVersion? variableRecurringPaymentsApiVersion =
                bankHiddenProps?.VariableRecurringPaymentsApi?.ApiVersion ??
                defaultHiddenProps?.VariableRecurringPaymentsApi?.ApiVersion;
            string? variableRecurringPaymentsApiBaseUrl =
                GetFirstNonemptyOrNullValue(
                    bankHiddenProps?.VariableRecurringPaymentsApi?.BaseUrl,
                    defaultHiddenProps?.VariableRecurringPaymentsApi?.BaseUrl);
            if (variableRecurringPaymentsApiVersion is not null ||
                variableRecurringPaymentsApiBaseUrl is not null)
            {
                combinedHiddenProps.VariableRecurringPaymentsApi =
                    new VariableRecurringPaymentsApiHiddenProperties
                    {
                        ApiVersion = variableRecurringPaymentsApiVersion,
                        BaseUrl = variableRecurringPaymentsApiBaseUrl
                    };
            }

            // Store hidden properties
            _bankProfileHiddenPropertiesDictionary[bankName] = combinedHiddenProps;
        }
    }

    public abstract BankProfile GetBankProfile(
        TBank bank,
        IInstrumentationClient instrumentationClient);

    private string? GetFirstNonemptyOrNullValue(string? value1, string? value2) =>
        (value1, value2) switch
        {
            (not null and not "", _) => value1,
            (_, not null and not "") => value2,
            _ => null
        };

    protected string GetIssuerUrl(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.IssuerUrl ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                nameof(BankProfileHiddenProperties.IssuerUrl)));

    protected string GetFinancialId(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.FinancialId ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                nameof(BankProfileHiddenProperties.FinancialId)));

    protected DynamicClientRegistrationApiVersion GetClientRegistrationApiVersion(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.DefaultClientRegistrationApiVersion ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                nameof(BankProfileHiddenProperties.DefaultClientRegistrationApiVersion)));

    protected AccountAndTransactionApiVersion GetAccountAndTransactionApiVersion(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.AccountAndTransactionApi?.ApiVersion ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                $"{nameof(BankProfileHiddenProperties.AccountAndTransactionApi)}:{nameof(BankProfileHiddenProperties.AccountAndTransactionApi.ApiVersion)}"));

    protected string GetAccountAndTransactionApiBaseUrl(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.AccountAndTransactionApi?.BaseUrl ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                $"{nameof(BankProfileHiddenProperties.AccountAndTransactionApi)}:{nameof(BankProfileHiddenProperties.AccountAndTransactionApi.BaseUrl)}"));

    private string GetHiddenPropertyExceptionMessage(TBank bank, string propertyName) =>
        "Configuration or key secrets error: " +
        $"Bank profile {_bankGroup.GetBankProfile(bank)} (bank: {bank}, bank group: {_bankGroup.BankGroupEnum}) " +
        $"requires hidden property {propertyName} to be specified.";

    protected PaymentInitiationApiVersion GetPaymentInitiationApiVersion(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.PaymentInitiationApi?.ApiVersion ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                $"{nameof(BankProfileHiddenProperties.PaymentInitiationApi)}:{nameof(BankProfileHiddenProperties.PaymentInitiationApi.ApiVersion)}"));

    protected string GetPaymentInitiationApiBaseUrl(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.PaymentInitiationApi?.BaseUrl ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                $"{nameof(BankProfileHiddenProperties.PaymentInitiationApi)}:{nameof(BankProfileHiddenProperties.PaymentInitiationApi.BaseUrl)}"));

    protected VariableRecurringPaymentsApiVersion GetVariableRecurringPaymentsApiVersion(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.VariableRecurringPaymentsApi?.ApiVersion ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                $"{nameof(BankProfileHiddenProperties.VariableRecurringPaymentsApi)}:{nameof(BankProfileHiddenProperties.VariableRecurringPaymentsApi.ApiVersion)}"));

    protected string GetVariableRecurringPaymentsApiBaseUrl(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.VariableRecurringPaymentsApi?.BaseUrl ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                $"{nameof(BankProfileHiddenProperties.VariableRecurringPaymentsApi)}:{nameof(BankProfileHiddenProperties.VariableRecurringPaymentsApi.BaseUrl)}"));

    protected string GetExtra1(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.Extra1 ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                nameof(BankProfileHiddenProperties.Extra1)));

    protected string GetExtra2(TBank bank) =>
        GetBankProfileHiddenProperties(bank)
            ?.Extra2 ??
        throw new KeyNotFoundException(
            GetHiddenPropertyExceptionMessage(
                bank,
                nameof(BankProfileHiddenProperties.Extra2)));

    private BankProfileHiddenProperties? GetBankProfileHiddenProperties(TBank bank) =>
        _bankProfileHiddenPropertiesDictionary.TryGetValue(
            bank,
            out BankProfileHiddenProperties? bankProfileHiddenProperties)
            ? bankProfileHiddenProperties
            : null;
}
