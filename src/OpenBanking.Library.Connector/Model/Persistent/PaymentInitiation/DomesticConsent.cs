// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.PaymentInitiation
{
    public class DomesticConsent
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("softwareStatementProfileId")]
        public string SoftwareStatementProfileId { get; set; }

        [JsonProperty("issuerUrl")]
        public string IssuerUrl { get; set; }

        [JsonProperty("openBankingClientProfileId")]
        public string OpenBankingClientProfileId { get; set; }

        [JsonProperty("obWriteDomesticConsent")]
        public OBWriteDomesticConsent ObWriteDomesticConsent { get; set; }

        [JsonProperty("tokenEndpointResponse")]
        public TokenEndpointResponse TokenEndpointResponse { get; set; }
    }
}
