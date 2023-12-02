// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public abstract class BankGroupBase<TBank, TRegistrationGroup> : IBankGroup<TBank, TRegistrationGroup>
    where TBank : struct, Enum
    where TRegistrationGroup : struct, Enum
{
    protected BankGroupBase(BankGroupEnum bankGroupEnum)
    {
        BankGroupEnum = bankGroupEnum;
    }

    protected abstract ConcurrentDictionary<BankProfileEnum, TBank> BankProfileToBank { get; }

    public abstract TRegistrationGroup? GetRegistrationGroup(TBank bank, RegistrationScopeEnum registrationScopeEnum);

    public BankGroupEnum BankGroupEnum { get; }

    /// <summary>
    ///     Convert bank profile enum to bank enum.
    /// </summary>
    /// <param name="bankProfile"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public TBank GetBank(BankProfileEnum bankProfile) =>
        BankProfileToBank.TryGetValue(bankProfile, out TBank bank)
            ? bank
            : throw new ArgumentOutOfRangeException(
                nameof(bankProfile),
                bankProfile,
                $"Bank profile {nameof(bankProfile)} does not represent a bank of type {nameof(TBank)}");

    /// <summary>
    ///     Convert bank enum to bank profile enum
    /// </summary>
    /// <param name="bank"></param>
    /// <returns></returns>
    public BankProfileEnum GetBankProfile(TBank bank) =>
        BankProfileToBank.Single(x => x.Value.Equals(bank)).Key; // NB: always exists
}
