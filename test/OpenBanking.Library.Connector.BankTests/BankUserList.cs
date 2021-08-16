// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public partial class BankUsers
    {
        private readonly Dictionary<string, Dictionary<string, List<BankUser>>> _bankUserDictionary;

        public BankUsers(Dictionary<string, Dictionary<string, List<BankUser>>> bankUserDictionary)
        {
            _bankUserDictionary = bankUserDictionary;
        }

        public List<BankUser> GetRequiredBankUserList(BankProfileEnum bankProfileEnum)
        {
            // Get "Sandbox" dictionary
            Dictionary<string, List<BankUser>> innerDict =
                _bankUserDictionary["Sandbox"] ?? throw new Exception("No Sandbox bank users found.");

            // Get bankProfileEnum list
            List<BankUser>? bankUserList = innerDict[bankProfileEnum.ToString()];

            if (bankUserList is null ||
                !bankUserList.Any())
            {
                throw new Exception($"No bank users found for {bankProfileEnum}.");
            }

            return bankUserList;
        }
    }
}
