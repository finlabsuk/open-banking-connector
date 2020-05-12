// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    public class BankClientProfile : IEntity
    {
        public string State { get; set; }

        public string SoftwareStatementProfileId { get; set; }

        public string IssuerUrl { get; set; }

        public string XFapiFinancialId { get; set; }

        public OpenIdConfiguration OpenIdConfiguration { get; set; }

        // TODO: Add MTLS configuration

        public BankClientRegistrationClaims BankClientRegistrationClaims { get; set; }

        public BankClientRegistrationData BankClientRegistrationData { get; set; }
        public string Id { get; set; }
    }
}
