// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    public class DomesticConsent : IEntity
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("softwareStatementProfileId")]
        public string SoftwareStatementProfileId { get; set; }

        [JsonProperty("issuerUrl")]
        public string IssuerUrl { get; set; }

        [JsonProperty("apiProfileId")]
        public string ApiProfileId { get; set; }

        [JsonProperty("obWriteDomesticConsent")]
        public OBWriteDomesticConsent ObWriteDomesticConsent { get; set; }

        [JsonProperty("tokenEndpointResponse")]
        public TokenEndpointResponse? TokenEndpointResponse { get; set; }

        public string BankId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
