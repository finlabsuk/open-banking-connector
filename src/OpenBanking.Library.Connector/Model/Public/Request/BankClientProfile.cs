// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Public.Request
{
    [PersistenceEquivalent(typeof(Persistent.BankClientProfile))]
    public class BankClientProfile
    {
        [JsonProperty("softwareStatementProfileId")]
        public string SoftwareStatementProfileId { get; set; }

        [JsonProperty("issuerUrl")]
        public string IssuerUrl { get; set; }

        [JsonProperty("xFapiFinancialId")]
        public string XFapiFinancialId { get; set; }

        [JsonProperty("openIdOverrides")]
        public OpenIdConfigurationOverrides OpenIdConfigurationOverrides { get; set; }

        [JsonProperty("httpMtlsOverrides")]
        public HttpClientMtlsConfigurationOverrides HttpMtlsOverrides { get; set; }

        [JsonProperty("registrationClaimsOverrides")]
        public OpenBankingClientRegistrationClaimsOverrides ClientRegistrationClaimsOverrides { get; set; }

        [JsonProperty("registrationResponseOverrides")]
        public ClientRegistrationResponseOverrides ClientRegistrationDataOverrides { get; set; }
    }
}
