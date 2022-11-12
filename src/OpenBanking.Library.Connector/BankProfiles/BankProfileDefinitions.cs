// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public class HiddenPropertiesDictionary : ConcurrentDictionary<BankProfileEnum, BankProfileHiddenProperties> { }

    public class BankProfilesDictionary : ConcurrentDictionary<BankProfileEnum, Lazy<BankProfile>> { }

    public class BankGroupsDictionary : ConcurrentDictionary<BankGroupEnum, Lazy<IBankGroup>> { }

    public partial class BankProfileDefinitions : IBankProfileDefinitions
    {
        private readonly BankGroupsDictionary _bankGroupsDictionary = new();
        private readonly BankProfilesDictionary _bankProfilesDictionary = new();
        private readonly HiddenPropertiesDictionary _hiddenPropertiesDictionary = new();


        public BankProfileDefinitions(
            ISettingsProvider<BankProfilesSettings>
                bankProfilesSettingsProvider)
        {
            // Load bank profile settings and check
            BankProfilesSettings topLevelDict = bankProfilesSettingsProvider.GetSettings();
            foreach ((string key, Dictionary<string, BankProfileHiddenProperties> value) in topLevelDict)
            {
                if (!Enum.TryParse(key, out BankGroupEnum bankGroupEnum))
                {
                    throw new ArgumentException(
                        "Configuration or key secrets error: " +
                        $"Invalid bank group {key} used in bank profile setting.");
                }

                IBankGroup bankGroup = GetBankGroup(bankGroupEnum);
                foreach ((string innerKey, BankProfileHiddenProperties _) in value)
                {
                    string _ = string.Equals(innerKey, "Default")
                        ? "Default"
                        : bankGroup.BankProfileToBankName.Values.FirstOrDefault(x => x == innerKey) ??
                          throw new ArgumentException(
                              "Configuration or key secrets error: " +
                              $"Invalid bank {innerKey} used with bank group {bankGroupEnum} in bank profile setting.");
                }
            }

            foreach (BankGroupEnum bankGroupEnum in Enum.GetValues<BankGroupEnum>())
            {
                // Get bank group dictionary if available
                if (!topLevelDict.TryGetValue(
                        bankGroupEnum.ToString(),
                        out Dictionary<string, BankProfileHiddenProperties>? bankGroupDict))
                {
                    continue;
                }

                // Get default hidden properties
                BankProfileHiddenProperties? defaultHiddenProps =
                    bankGroupDict.TryGetValue("Default", out BankProfileHiddenProperties? tmp1)
                        ? tmp1
                        : null;

                // Get bank hidden properties
                IBankGroup bankGroupBase = GetBankGroup(bankGroupEnum);
                foreach ((BankProfileEnum bankProfileEnum, string bankName) in bankGroupBase.BankProfileToBankName)
                {
                    BankProfileHiddenProperties? bankHiddenProps =
                        bankGroupDict.TryGetValue(bankName, out BankProfileHiddenProperties? tmp2) ? tmp2 : null;
                    if (bankHiddenProps is null &&
                        defaultHiddenProps is null)
                    {
                        continue;
                    }

                    // Combine bank and default values
                    var combinedHiddenProps = new BankProfileHiddenProperties();
                    combinedHiddenProps.IssuerUrl = GetFirstNonemptyValueOrNull(
                        bankHiddenProps?.IssuerUrl,
                        defaultHiddenProps?.IssuerUrl);
                    combinedHiddenProps.FinancialId = GetFirstNonemptyValueOrNull(
                        bankHiddenProps?.FinancialId,
                        defaultHiddenProps?.FinancialId);
                    combinedHiddenProps.DefaultClientRegistrationApiVersion =
                        bankHiddenProps?.DefaultClientRegistrationApiVersion ??
                        defaultHiddenProps?.DefaultClientRegistrationApiVersion;
                    AccountAndTransactionApiVersion? accountAndTransactionApiVersion =
                        bankHiddenProps?.AccountAndTransactionApi?.ApiVersion ??
                        defaultHiddenProps?.AccountAndTransactionApi?.ApiVersion;
                    string? accountAndTransactionApiBaseUrl =
                        bankHiddenProps?.AccountAndTransactionApi?.BaseUrl ??
                        defaultHiddenProps?.AccountAndTransactionApi?.BaseUrl;
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
                        bankHiddenProps?.PaymentInitiationApi?.BaseUrl ??
                        defaultHiddenProps?.PaymentInitiationApi?.BaseUrl;
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
                        bankHiddenProps?.VariableRecurringPaymentsApi?.BaseUrl ??
                        defaultHiddenProps?.VariableRecurringPaymentsApi?.BaseUrl;
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

                    combinedHiddenProps.Extra1 = GetFirstNonemptyValueOrNull(
                        bankHiddenProps?.Extra1,
                        defaultHiddenProps?.Extra1);

                    combinedHiddenProps.Extra2 = GetFirstNonemptyValueOrNull(
                        bankHiddenProps?.Extra2,
                        defaultHiddenProps?.Extra2);

                    // Store hidden properties
                    _hiddenPropertiesDictionary[bankProfileEnum] = combinedHiddenProps;
                }
            }
        }

        public BankProfile GetBankProfile(BankProfileEnum bankProfileEnum) =>
            _bankProfilesDictionary.GetOrAdd(
                    bankProfileEnum,
                    profileEnum => new Lazy<BankProfile>(
                        () => GetBankGroup(GetBankGroupEnum(profileEnum)).GetBankProfile(
                            profileEnum,
                            _hiddenPropertiesDictionary),
                        LazyThreadSafetyMode.ExecutionAndPublication))
                .Value;

        public IBankGroup GetBankGroup(BankGroupEnum bankGroupEnum) =>
            bankGroupEnum switch
            {
                BankGroupEnum.Danske => GetBankGroup<Danske>(bankGroupEnum),
                BankGroupEnum.Hsbc => GetBankGroup<Hsbc>(bankGroupEnum),
                BankGroupEnum.Lloyds => GetBankGroup<Lloyds>(bankGroupEnum),
                BankGroupEnum.Obie => GetBankGroup<Obie>(bankGroupEnum),
                BankGroupEnum.Monzo => GetBankGroup<Monzo>(bankGroupEnum),
                BankGroupEnum.NatWest => GetBankGroup<NatWest>(bankGroupEnum),
                _ => throw new ArgumentOutOfRangeException(nameof(bankGroupEnum), bankGroupEnum, null)
            };

        public BankGroupEnum GetBankGroupEnum(BankProfileEnum bankProfileEnum) =>
            bankProfileEnum switch
            {
                BankProfileEnum.Obie_Modelo => BankGroupEnum.Obie,
                BankProfileEnum.NatWest_NatWestSandbox => BankGroupEnum.NatWest,
                BankProfileEnum.NatWest_RoyalBankOfScotlandSandbox => BankGroupEnum.NatWest,
                BankProfileEnum.NatWest_NatWest => BankGroupEnum.NatWest,
                BankProfileEnum.NatWest_RoyalBankOfScotland => BankGroupEnum.NatWest,
                BankProfileEnum.NatWest_UlsterBankNI => BankGroupEnum.NatWest,
                BankProfileEnum.Lloyds => BankGroupEnum.Lloyds,
                BankProfileEnum.Hsbc_FirstDirect => BankGroupEnum.Hsbc,
                BankProfileEnum.Hsbc_Sandbox => BankGroupEnum.Hsbc,
                BankProfileEnum.Hsbc_UkBusiness => BankGroupEnum.Hsbc,
                BankProfileEnum.Hsbc_UkKinetic => BankGroupEnum.Hsbc,
                BankProfileEnum.Hsbc_UkPersonal => BankGroupEnum.Hsbc,
                BankProfileEnum.Danske => BankGroupEnum.Danske,
                BankProfileEnum.Monzo => BankGroupEnum.Monzo,
                _ => throw new ArgumentOutOfRangeException(nameof(bankProfileEnum), bankProfileEnum, null)
            };

        private string? GetFirstNonemptyValueOrNull(string? value1, string? value2) =>
            (value1, value2) switch
            {
                (not null and not "", _) => value1,
                (_, not null and not "") => value2,
                _ => null,
            };

        private IBankGroup GetBankGroup<TBankGroup>(BankGroupEnum bankGroupEnum)
            where TBankGroup : IBankGroup, new() =>
            _bankGroupsDictionary.GetOrAdd(
                    bankGroupEnum,
                    _ => new Lazy<IBankGroup>(
                        () => new TBankGroup(),
                        LazyThreadSafetyMode.ExecutionAndPublication))
                .Value;

        private BankProfileHiddenProperties GetRequiredBankProfileHiddenProperties(BankProfileEnum bankProfileEnum)
        {
            if (!_hiddenPropertiesDictionary.TryGetValue(
                    bankProfileEnum,
                    out BankProfileHiddenProperties? bankProfileHiddenProperties))
            {
                throw new ArgumentNullException(
                    $"Hidden properties are required for bank profile {bankProfileEnum} " +
                    "and cannot be found.");
            }

            return bankProfileHiddenProperties;
        }
    }
}
