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

public class BankProfileGeneratorsDictionary : ConcurrentDictionary<BankGroup, IBankProfileGenerator> { }

public class BankProfileService : IBankProfileService
{
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

        // Populate dictionary of bank profile generators
        _bankProfileGeneratorsDictionary = new BankProfileGeneratorsDictionary
        {
            [BankGroup.Barclays] = new BarclaysGenerator(bankProfilesSettingsProvider),
            [BankGroup.Chase] = new ChaseGenerator(bankProfilesSettingsProvider),
            [BankGroup.Cooperative] = new CooperativeGenerator(bankProfilesSettingsProvider),
            [BankGroup.Danske] = new DanskeGenerator(bankProfilesSettingsProvider),
            [BankGroup.Hsbc] = new HsbcGenerator(bankProfilesSettingsProvider),
            [BankGroup.Lloyds] = new LloydsGenerator(bankProfilesSettingsProvider),
            [BankGroup.Obie] = new ObieGenerator(bankProfilesSettingsProvider),
            [BankGroup.Monzo] = new MonzoGenerator(bankProfilesSettingsProvider),
            [BankGroup.Nationwide] = new NationwideGenerator(bankProfilesSettingsProvider),
            [BankGroup.NatWest] = new NatWestGenerator(bankProfilesSettingsProvider),
            [BankGroup.Revolut] = new RevolutGenerator(bankProfilesSettingsProvider),
            [BankGroup.Santander] = new SantanderGenerator(bankProfilesSettingsProvider),
            [BankGroup.Starling] = new StarlingGenerator(bankProfilesSettingsProvider),
            [BankGroup.Tsb] = new TsbGenerator(bankProfilesSettingsProvider)
        };
    }

    public BankProfile GetBankProfile(BankProfileEnum bankProfileEnum) =>
        _bankProfilesDictionary.GetOrAdd(
                bankProfileEnum,
                profileEnum => new Lazy<BankProfile>(
                    () => profileEnum.GetBankGroup() switch
                    {
                        BankGroup.Barclays => GetBankProfile<BarclaysBank>(profileEnum, _instrumentationClient),
                        BankGroup.Chase => GetBankProfile<ChaseBank>(profileEnum, _instrumentationClient),
                        BankGroup.Cooperative => GetBankProfile<CooperativeBank>(
                            profileEnum,
                            _instrumentationClient),
                        BankGroup.Danske => GetBankProfile<DanskeBank>(profileEnum, _instrumentationClient),
                        BankGroup.Hsbc => GetBankProfile<HsbcBank>(profileEnum, _instrumentationClient),
                        BankGroup.Lloyds => GetBankProfile<LloydsBank>(profileEnum, _instrumentationClient),
                        BankGroup.Obie => GetBankProfile<ObieBank>(profileEnum, _instrumentationClient),
                        BankGroup.Monzo => GetBankProfile<MonzoBank>(profileEnum, _instrumentationClient),
                        BankGroup.Nationwide => GetBankProfile<NationwideBank>(profileEnum, _instrumentationClient),
                        BankGroup.NatWest => GetBankProfile<NatWestBank>(profileEnum, _instrumentationClient),
                        BankGroup.Revolut => GetBankProfile<RevolutBank>(profileEnum, _instrumentationClient),
                        BankGroup.Santander => GetBankProfile<SantanderBank>(profileEnum, _instrumentationClient),
                        BankGroup.Starling => GetBankProfile<StarlingBank>(profileEnum, _instrumentationClient),
                        BankGroup.Tsb => GetBankProfile<TsbBank>(profileEnum, _instrumentationClient),
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication))
            .Value;


    private BankProfile GetBankProfile<TBank>(
        BankProfileEnum bankProfileEnum,
        IInstrumentationClient instrumentationClient)
        where TBank : struct, Enum
    {
        BankGroup bankGroup = bankProfileEnum.GetBankGroup();
        TBank bank =
            bankGroup.GetBankGroupData<TBank>()
                .GetBank(bankProfileEnum);
        BankProfile bankProfile =
            GetBankProfileGenerator<TBank>(bankGroup)
                .GetBankProfile(bank, instrumentationClient);
        return bankProfile;
    }

    private IBankProfileGenerator<TBank> GetBankProfileGenerator<TBank>(BankGroup bankGroup)
        where TBank : struct, Enum
    {
        IBankProfileGenerator<TBank> bankProfileGenerator =
            _bankProfileGeneratorsDictionary[bankGroup] as IBankProfileGenerator<TBank> ??
            throw new ArgumentException(
                $"Bank group {nameof(bankGroup)} does not represent a bank of type {nameof(TBank)}.");
        return bankProfileGenerator;
    }
}
