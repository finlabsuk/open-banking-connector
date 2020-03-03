// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent
{
    public class BankClientProfile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("software_statement_profile_id")]
        public string SoftwareStatementProfileId { get; set; }

        [JsonProperty("issuer_url")]
        public string IssuerUrl { get; set; }

        [JsonProperty("x_fapi_financial_id")]
        public string XFapiFinancialId { get; set; }

        [JsonProperty("open_id_configuration")]
        public OpenIdConfiguration OpenIdConfiguration { get; set; }

        // TODO: Add MTLS configuration

        [JsonProperty("open_banking_client_registration_claims")]
        public BankClientRegistrationClaims BankClientRegistrationClaims { get; set; }

        [JsonProperty("open_banking_client_response")]
        public BankClientRegistrationData BankClientRegistrationData { get; set; }
    }
}
