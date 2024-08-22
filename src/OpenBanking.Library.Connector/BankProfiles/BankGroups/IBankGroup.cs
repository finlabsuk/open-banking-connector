// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

public interface IBankGroup { }

public interface IBankGroup<TBank> : IBankGroup
    where TBank : struct, Enum
{
    BankGroupEnum BankGroupEnum { get; }
    TBank GetBank(BankProfileEnum bankProfile);
    BankProfileEnum GetBankProfile(TBank bank);
}

public interface IBankGroup<TBank, TRegistrationGroup> : IBankGroup<TBank>
    where TRegistrationGroup : struct, Enum
    where TBank : struct, Enum
{
    TRegistrationGroup GetRegistrationGroup(TBank bank, RegistrationScopeEnum registrationScopeEnum);
}
