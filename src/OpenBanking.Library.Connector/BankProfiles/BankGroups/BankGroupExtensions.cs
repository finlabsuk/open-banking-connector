// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public static class BankGroupExtensions
{
    private static readonly ConcurrentDictionary<BankGroup, IBankGroupData> BankGroupsDictionary = new()
    {
        [BankGroup.Barclays] = new Barclays(),
        [BankGroup.Cooperative] = new Cooperative(),
        [BankGroup.Danske] = new Danske(),
        [BankGroup.Hsbc] = new Hsbc(),
        [BankGroup.Lloyds] = new Lloyds(),
        [BankGroup.Monzo] = new Monzo(),
        [BankGroup.Nationwide] = new Nationwide(),
        [BankGroup.NatWest] = new NatWest(),
        [BankGroup.Obie] = new Obie(),
        [BankGroup.Revolut] = new Revolut(),
        [BankGroup.Santander] = new Santander(),
        [BankGroup.Starling] = new Starling(),
        [BankGroup.Tsb] = new Tsb()
    };

    public static IBankGroupData<TBank> GetBankGroupData<TBank>(this BankGroup bankGroup)
        where TBank : struct, Enum
    {
        IBankGroupData<TBank> bankGroupData =
            BankGroupsDictionary[bankGroup] as IBankGroupData<TBank> ??
            throw new ArgumentException(
                $"Bank group {nameof(bankGroup)} does not represent a bank of type {nameof(TBank)}.");
        return bankGroupData;
    }

    public static IBankGroupData<TBank, TRegistrationGroup> GetBankGroupData<TBank, TRegistrationGroup>(
        this BankGroup bankGroup)
        where TBank : struct, Enum
        where TRegistrationGroup : struct, Enum
    {
        IBankGroupData<TBank, TRegistrationGroup> bankGroupData =
            BankGroupsDictionary[bankGroup] as IBankGroupData<TBank, TRegistrationGroup> ??
            throw new ArgumentException(
                $"Bank group {nameof(bankGroup)} not found with bank type {nameof(TBank)} and registration group type {nameof(TRegistrationGroup)}.");
        return bankGroupData;
    }
}
