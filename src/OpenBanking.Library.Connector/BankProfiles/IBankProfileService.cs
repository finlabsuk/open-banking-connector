// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

public interface IBankProfileService
{
    BankProfile GetBankProfile(BankProfileEnum bankProfileEnum);
    BankGroupEnum GetBankGroupEnum(BankProfileEnum bankProfileEnum);

    TBank GetBank<TBank>(BankProfileEnum bankProfileEnum)
        where TBank : struct, Enum;
}
