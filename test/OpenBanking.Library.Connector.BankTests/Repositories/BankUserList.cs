// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Repositories
{
    public class BankUserDictionary : Dictionary<BankProfileEnum, List<BankUser>> { }

    public partial class BankUserStore
    {
        private readonly BankUserDictionary _bankUserDictionary;

        public BankUserStore(BankUserDictionary bankUserDictionary)
        {
            _bankUserDictionary = bankUserDictionary;
        }

        public List<BankUser> GetRequiredBankUserList(BankProfileEnum bankProfileEnum)
        {
            if (!_bankUserDictionary.TryGetValue(bankProfileEnum, out List<BankUser>? bankUserList))
            {
                throw new ArgumentNullException(
                    $"At least one bank user must be defined for bank profile {bankProfileEnum} "
                    + "and none can be found.");
            }

            return bankUserList;
        }
    }
}
