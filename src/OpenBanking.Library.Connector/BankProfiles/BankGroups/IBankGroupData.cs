// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public interface IBankGroupData { }

public interface IBankGroupData<TBank> : IBankGroupData
    where TBank : struct, Enum
{
    BankGroup BankGroup { get; }
    TBank GetBank(BankProfileEnum bankProfile);
    BankProfileEnum GetBankProfile(TBank bank);
}

public interface IBankGroupData<TBank, out TRegistrationGroup> : IBankGroupData<TBank>
    where TRegistrationGroup : struct, Enum
    where TBank : struct, Enum
{
    TRegistrationGroup GetRegistrationGroup(TBank bank, RegistrationScopeEnum registrationScopeEnum);
}
