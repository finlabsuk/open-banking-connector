// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository
{
    public class Account
    {
        public Account(string schemeName, string identification, string name)
        {
            SchemeName = schemeName;
            Identification = identification;
            Name = name;
        }

        public string SchemeName { get; }

        public string Identification { get; }

        public string Name { get; }

        public string? SecondaryIdentification { get; set; }
    }

    public class DomesticVrpAccountIndexPair
    {
        public DomesticVrpAccountIndexPair(int source, int dest)
        {
            Source = source;
            Dest = dest;
        }

        public int Source { get; }

        public int Dest { get; }
    }


    public class BankUser
    {
        public BankUser(
            string userNameOrNumber,
            string password,
            List<Account> accounts,
            List<DomesticVrpAccountIndexPair> domesticVrpAccountIndexPairs)
        {
            UserNameOrNumber = userNameOrNumber;
            Password = password;
            Accounts = accounts;
            DomesticVrpAccountIndexPairs = domesticVrpAccountIndexPairs;
        }

        public List<Account> Accounts { get; }

        /// <summary>
        ///     List of account index pairs which specify debtor (from) and creditor (to) accounts for domestic VRP payments.
        /// </summary>
        public List<DomesticVrpAccountIndexPair> DomesticVrpAccountIndexPairs { get; }

        public string UserNameOrNumber { get; }

        public string Password { get; }
    }
}
