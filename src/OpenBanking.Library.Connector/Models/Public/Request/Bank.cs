// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class Bank
    {
        //Uri?
        public string IssuerUrl { get; set; } = null!;

        public string XFapiFinancialId { get; set; } = null!;

        public string Name { get; set; } = null!;
    }
}
