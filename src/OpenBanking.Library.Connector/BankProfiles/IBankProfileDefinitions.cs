// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

public interface IBankProfileDefinitions
{
    BankProfile GetBankProfile(BankProfileEnum bankProfileEnum);
    IBankGroup GetBankGroup(BankGroupEnum bankGroupEnum);
    BankGroupEnum GetBankGroupEnum(BankProfileEnum bankProfileEnum);
}
