// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankPublicQuery
    {
        string IssuerUrl { get; }
        string XFapiFinancialId { get; }
        string Name { get; }
        string Id { get; }
    }

    public class BankResponse : IBankPublicQuery
    {
        internal BankResponse(string issuerUrl, string xFapiFinancialId, string name, string id)
        {
            IssuerUrl = issuerUrl;
            XFapiFinancialId = xFapiFinancialId;
            Name = name;
            Id = id;
        }

        public string IssuerUrl { get; }

        public string XFapiFinancialId { get; }

        public string Name { get; }

        public string Id { get; }
    }
}
