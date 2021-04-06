// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public class BankUser
    {
        public BankUser(string userNameOrNumber, string password)
        {
            UserNameOrNumber = userNameOrNumber;
            Password = password;
        }

        public string UserNameOrNumber { get; }

        public string Password { get; }
    }
}
