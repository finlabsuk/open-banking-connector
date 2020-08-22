// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public class BankResponse
    {
        internal BankResponse(Bank bank)
        {
            IssuerUrl = bank.IssuerUrl;
            XFapiFinancialId = bank.XFapiFinancialId;
            Name = bank.Name;
            Id = bank.Id;
        }

        public string IssuerUrl { get; }

        public string XFapiFinancialId { get; }

        public string Name { get; }

        public string Id { get; }
    }
}
