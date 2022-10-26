// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public abstract class BankGroupBase<TBank> : IBankGroup
    where TBank : Enum
{
    protected abstract ConcurrentDictionary<BankProfileEnum, TBank> BankProfileToBank { get; }

    public ReadOnlyDictionary<BankProfileEnum, string> BankProfileToBankName =>
        new(BankProfileToBank.ToDictionary(keyValuePair => keyValuePair.Key, x => x.Value.ToString()));

    public string GetBankString(BankProfileEnum bankProfile) => GetBank(bankProfile).ToString();

    public abstract BankProfile GetBankProfile(
        BankProfileEnum bankProfileEnum,
        HiddenPropertiesDictionary hiddenPropertiesDictionary);

    // This is public due to use of extension methods in BankTests assembly
    public TBank GetBank(BankProfileEnum bankProfile) =>
        BankProfileToBank.TryGetValue(bankProfile, out TBank? bank)
            ? bank
            : throw new ArgumentOutOfRangeException(nameof(bankProfile), bankProfile, null);
}
