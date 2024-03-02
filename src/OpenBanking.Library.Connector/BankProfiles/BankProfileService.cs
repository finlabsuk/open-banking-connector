// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

public class BankProfilesDictionary : ConcurrentDictionary<BankProfileEnum, Lazy<BankProfile>> { }

public class BankGroupsDictionary : ConcurrentDictionary<BankGroupEnum, IBankGroup> { }

public class BankProfileGeneratorsDictionary : ConcurrentDictionary<BankGroupEnum, IBankProfileGenerator> { }

public class BankProfileService : IBankProfileService
{
    /// <summary>
    ///     Static dictionary of bank groups.
    /// </summary>
    private readonly BankGroupsDictionary _bankGroupsDictionary;

    /// <summary>
    ///     Static dictionary of profile generators for each bank group.
    /// </summary>
    private readonly BankProfileGeneratorsDictionary _bankProfileGeneratorsDictionary;

    /// <summary>
    ///     Dynamic dictionary of bank profiles created lazily as each bank profile needed. This means
    ///     that, if unused bank profiles require hidden properties which are not present, that
    ///     is not a problem.
    /// </summary>
    private readonly BankProfilesDictionary _bankProfilesDictionary = new();

    private readonly IInstrumentationClient _instrumentationClient;

    public BankProfileService(
        ISettingsProvider<BankProfilesSettings>
            bankProfilesSettingsProvider,
        IInstrumentationClient instrumentationClient)
    {
        _instrumentationClient = instrumentationClient;
        // Populate dictionary of bank groups
        _bankGroupsDictionary = new BankGroupsDictionary
        {
            [BankGroupEnum.Barclays] = new Barclays(BankGroupEnum.Barclays),
            [BankGroupEnum.Danske] = new Danske(BankGroupEnum.Danske),
            [BankGroupEnum.Hsbc] = new Hsbc(BankGroupEnum.Hsbc),
            [BankGroupEnum.Lloyds] = new Lloyds(BankGroupEnum.Lloyds),
            [BankGroupEnum.Obie] = new Obie(BankGroupEnum.Obie),
            [BankGroupEnum.Monzo] = new Monzo(BankGroupEnum.Monzo),
            [BankGroupEnum.Nationwide] = new Nationwide(BankGroupEnum.Nationwide),
            [BankGroupEnum.NatWest] = new NatWest(BankGroupEnum.NatWest),
            [BankGroupEnum.Revolut] = new Revolut(BankGroupEnum.Revolut),
            [BankGroupEnum.Santander] = new Santander(BankGroupEnum.Santander),
            [BankGroupEnum.Starling] = new Starling(BankGroupEnum.Starling)
        };

        // Populate dictionary of bank profile generators
        _bankProfileGeneratorsDictionary = new BankProfileGeneratorsDictionary
        {
            [BankGroupEnum.Barclays] = new BarclaysGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<BarclaysBank>(BankGroupEnum.Barclays)),
            [BankGroupEnum.Danske] = new DanskeGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<DanskeBank>(BankGroupEnum.Danske)),
            [BankGroupEnum.Hsbc] = new HsbcGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<HsbcBank>(BankGroupEnum.Hsbc)),
            [BankGroupEnum.Lloyds] = new LloydsGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<LloydsBank>(BankGroupEnum.Lloyds)),
            [BankGroupEnum.Obie] = new ObieGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<ObieBank>(BankGroupEnum.Obie)),
            [BankGroupEnum.Monzo] = new MonzoGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<MonzoBank>(BankGroupEnum.Monzo)),
            [BankGroupEnum.Nationwide] = new NationwideGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<NationwideBank>(BankGroupEnum.Nationwide)),
            [BankGroupEnum.NatWest] = new NatWestGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<NatWestBank>(BankGroupEnum.NatWest)),
            [BankGroupEnum.Revolut] = new RevolutGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<RevolutBank>(BankGroupEnum.Revolut)),
            [BankGroupEnum.Santander] = new SantanderGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<SantanderBank>(BankGroupEnum.Santander)),
            [BankGroupEnum.Starling] = new StarlingGenerator(
                bankProfilesSettingsProvider,
                GetBankGroup<StarlingBank>(BankGroupEnum.Starling))
        };
    }

    public BankProfile GetBankProfile(BankProfileEnum bankProfileEnum) =>
        _bankProfilesDictionary.GetOrAdd(
                bankProfileEnum,
                profileEnum => new Lazy<BankProfile>(
                    () => GetBankGroupEnum(profileEnum) switch
                    {
                        BankGroupEnum.Barclays => GetBankProfile<BarclaysBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.Danske => GetBankProfile<DanskeBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.Hsbc => GetBankProfile<HsbcBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.Lloyds => GetBankProfile<LloydsBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.Obie => GetBankProfile<ObieBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.Monzo => GetBankProfile<MonzoBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.Nationwide => GetBankProfile<NationwideBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.NatWest => GetBankProfile<NatWestBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.Revolut => GetBankProfile<RevolutBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.Santander => GetBankProfile<SantanderBank>(profileEnum, _instrumentationClient),
                        BankGroupEnum.Starling => GetBankProfile<StarlingBank>(profileEnum, _instrumentationClient),
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication))
            .Value;

    public TBank GetBank<TBank>(BankProfileEnum bankProfileEnum)
        where TBank : struct, Enum
    {
        BankGroupEnum bankGroupEnum = GetBankGroupEnum(bankProfileEnum);
        TBank bank =
            GetBankGroup<TBank>(bankGroupEnum)
                .GetBank(bankProfileEnum);
        return bank;
    }

    public IBankGroup<TBank, TRegistrationGroup> GetBankGroup<TBank, TRegistrationGroup>(BankGroupEnum bankGroupEnum)
        where TBank : struct, Enum
        where TRegistrationGroup : struct, Enum
    {
        IBankGroup<TBank, TRegistrationGroup> bankGroup =
            _bankGroupsDictionary[bankGroupEnum] as IBankGroup<TBank, TRegistrationGroup> ??
            throw new ArgumentException(
                $"Bank group {nameof(bankGroupEnum)} not found with bank type {nameof(TBank)} and registration group type {nameof(TRegistrationGroup)}.");
        return bankGroup;
    }

    public static BankGroupEnum GetBankGroupEnum(BankProfileEnum bankProfileEnum) =>
        bankProfileEnum switch
        {
            BankProfileEnum.Barclays_Sandbox => BankGroupEnum.Barclays,
            BankProfileEnum.Barclays_Personal => BankGroupEnum.Barclays,
            BankProfileEnum.Barclays_Wealth => BankGroupEnum.Barclays,
            BankProfileEnum.Barclays_Barclaycard => BankGroupEnum.Barclays,
            BankProfileEnum.Barclays_Business => BankGroupEnum.Barclays,
            BankProfileEnum.Barclays_Corporate => BankGroupEnum.Barclays,
            BankProfileEnum.Barclays_BarclaycardCommercialPayments => BankGroupEnum.Barclays,
            BankProfileEnum.Danske_Sandbox => BankGroupEnum.Danske,
            BankProfileEnum.Hsbc_FirstDirect => BankGroupEnum.Hsbc,
            BankProfileEnum.Hsbc_Sandbox => BankGroupEnum.Hsbc,
            BankProfileEnum.Hsbc_UkBusiness => BankGroupEnum.Hsbc,
            BankProfileEnum.Hsbc_UkKinetic => BankGroupEnum.Hsbc,
            BankProfileEnum.Hsbc_UkPersonal => BankGroupEnum.Hsbc,
            BankProfileEnum.Hsbc_HsbcNetUk => BankGroupEnum.Hsbc,
            BankProfileEnum.Lloyds_Sandbox => BankGroupEnum.Lloyds,
            BankProfileEnum.Lloyds_LloydsPersonal => BankGroupEnum.Lloyds,
            BankProfileEnum.Lloyds_LloydsBusiness => BankGroupEnum.Lloyds,
            BankProfileEnum.Lloyds_LloydsCommerical => BankGroupEnum.Lloyds,
            BankProfileEnum.Lloyds_HalifaxPersonal => BankGroupEnum.Lloyds,
            BankProfileEnum.Lloyds_BankOfScotlandPersonal => BankGroupEnum.Lloyds,
            BankProfileEnum.Lloyds_BankOfScotlandBusiness => BankGroupEnum.Lloyds,
            BankProfileEnum.Lloyds_BankOfScotlandCommerical => BankGroupEnum.Lloyds,
            BankProfileEnum.Lloyds_MbnaPersonal => BankGroupEnum.Lloyds,
            BankProfileEnum.Monzo_Monzo => BankGroupEnum.Monzo,
            BankProfileEnum.Monzo_Sandbox => BankGroupEnum.Monzo,
            BankProfileEnum.Nationwide_Nationwide => BankGroupEnum.Nationwide,
            BankProfileEnum.NatWest_NatWestSandbox => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_NatWest => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_NatWestBankline => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_NatWestClearSpend => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_RoyalBankOfScotlandSandbox => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_RoyalBankOfScotland => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_RoyalBankOfScotlandBankline => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_RoyalBankOfScotlandClearSpend => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_TheOne => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_NatWestOne => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_VirginOne => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_UlsterBankNi => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_UlsterBankNiBankline => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_UlsterBankNiClearSpend => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_Mettle => BankGroupEnum.NatWest,
            BankProfileEnum.NatWest_Coutts => BankGroupEnum.NatWest,
            BankProfileEnum.Obie_Modelo => BankGroupEnum.Obie,
            BankProfileEnum.Obie_Model2023 => BankGroupEnum.Obie,
            BankProfileEnum.Revolut_Revolut => BankGroupEnum.Revolut,
            BankProfileEnum.Santander_Santander => BankGroupEnum.Santander,
            BankProfileEnum.Starling_Starling => BankGroupEnum.Starling,
            _ => throw new ArgumentOutOfRangeException(nameof(bankProfileEnum), bankProfileEnum, null)
        };

    private BankProfile GetBankProfile<TBank>(
        BankProfileEnum bankProfileEnum,
        IInstrumentationClient instrumentationClient)
        where TBank : struct, Enum
    {
        BankGroupEnum bankGroupEnum = GetBankGroupEnum(bankProfileEnum);
        TBank bank =
            GetBankGroup<TBank>(bankGroupEnum)
                .GetBank(bankProfileEnum);
        BankProfile bankProfile =
            GetBankProfileGenerator<TBank>(bankGroupEnum)
                .GetBankProfile(bank, instrumentationClient);
        return bankProfile;
    }

    private IBankGroup<TBank> GetBankGroup<TBank>(BankGroupEnum bankGroupEnum)
        where TBank : struct, Enum
    {
        IBankGroup<TBank> bankGroup =
            _bankGroupsDictionary[bankGroupEnum] as IBankGroup<TBank> ??
            throw new ArgumentException(
                $"Bank group {nameof(bankGroupEnum)} does not represent a bank of type {nameof(TBank)}.");
        return bankGroup;
    }

    private IBankProfileGenerator<TBank> GetBankProfileGenerator<TBank>(BankGroupEnum bankGroupEnum)
        where TBank : struct, Enum
    {
        IBankProfileGenerator<TBank> bankProfileGenerator =
            _bankProfileGeneratorsDictionary[bankGroupEnum] as IBankProfileGenerator<TBank> ??
            throw new ArgumentException(
                $"Bank group {nameof(bankGroupEnum)} does not represent a bank of type {nameof(TBank)}.");
        return bankProfileGenerator;
    }
}
